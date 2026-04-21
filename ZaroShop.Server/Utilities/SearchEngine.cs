namespace ZaroShop.Server.Utilities;

public class SearchEngine<T> where T : class
{
    private readonly IEnumerable<T> _items;
    private readonly List<(Func<T, string?> Selector, double Weight)> _searchFields = new();

    public SearchEngine(IEnumerable<T> items)
    {
        _items = items;
    }

    public void AddSearchField(Func<T, string?> selector, double weight)
    {
        _searchFields.Add((selector, weight));
    }

    public IEnumerable<T> Search(string query, int fuzzinessThreshold)
    {
        if (string.IsNullOrWhiteSpace(query)) return Enumerable.Empty<T>();

        var normalizedQuery = query.Trim().ToLower();

        return _items
            .Select(item => new { Item = item, Score = CalculateScore(item, normalizedQuery, fuzzinessThreshold) })
            .Where(result => result.Score > 0)
            .OrderByDescending(result => result.Score)
            .Select(result => result.Item);
    }

    private double CalculateScore(T item, string query, int threshold)
    {
        double totalScore = 0;
        var queryWords = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var (selector, weight) in _searchFields)
        {
            var value = selector(item)?.Trim().ToLower();
            if (string.IsNullOrEmpty(value)) continue;

            // 1. Exact/Contains Match (High Priority)
            if (value == query) { totalScore += 100 * weight; continue; }
            if (value.Contains(query)) { totalScore += 50 * weight; continue; }

            // 2. Word-by-Word Fuzzy Match
            // Split the product name into words (e.g., "Mechanical", "Keyboard")
            var valueWords = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var qWord in queryWords)
            {
                foreach (var vWord in valueWords)
                {
                    // Check if the query word is a fuzzy match for ANY word in the field
                    int distance = GetLevenshteinDistance(qWord, vWord);

                    if (distance <= threshold)
                    {
                        // If word is very short (like "Cat"), threshold should be stricter
                        if (vWord.Length <= 3 && distance > 0) continue;

                        double fuzzyScore = (1.0 - (double)distance / Math.Max(qWord.Length, vWord.Length)) * 40;
                        totalScore += fuzzyScore * weight;
                    }
                }
            }
        }

        return totalScore;
    }

    private int GetLevenshteinDistance(string s, string t)
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
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}