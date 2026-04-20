using System;
using System.Collections.Generic;
using System.Linq;

namespace ZaroShop.Core.Utilities;

public class SearchEngine<T> where T : class
{
    private readonly List<T> _items;
    private readonly Dictionary<Func<T, string?>, double> _searchFields;

    public SearchEngine(IEnumerable<T> items)
    {
        _items = items.ToList();
        _searchFields = new Dictionary<Func<T, string?>, double>();
    }

    // Add a field to search in with a specific weight (e.g., Name = 1.0, SKU = 0.5)
    public void AddSearchField(Func<T, string?> fieldSelector, double weight)
    {
        _searchFields[fieldSelector] = weight;
    }

    public IEnumerable<T> Search(string query, int fuzzinessThreshold = 2)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<T>();

        var scoredResults = new List<(T Item, double Score)>();
        string normalizedQuery = query.Trim().ToLower();

        foreach (var item in _items)
        {
            double totalScore = 0;

            foreach (var field in _searchFields)
            {
                string? fieldValue = field.Key(item)?.ToLower();
                if (string.IsNullOrEmpty(fieldValue)) continue;

                double fieldWeight = field.Value;

                // 1. Exact or Contains Match (High Priority)
                if (fieldValue.Contains(normalizedQuery))
                {
                    totalScore += 10 * fieldWeight;
                }
                else
                {
                    // 2. Fuzzy Matching (Levenshtein Distance)
                    int distance = ComputeLevenshteinDistance(normalizedQuery, fieldValue);
                    if (distance <= fuzzinessThreshold)
                    {
                        // Score is inversely proportional to distance
                        totalScore += (5.0 / (distance + 1)) * fieldWeight;
                    }
                }
            }

            if (totalScore > 0)
            {
                scoredResults.Add((item, totalScore));
            }
        }

        return scoredResults
            .OrderByDescending(x => x.Score)
            .Select(x => x.Item);
    }

    // Core C# implementation of Levenshtein Distance Algorithm
    private int ComputeLevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 0; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}