const { env } = require('process');

// Detects the backend port automatically, defaulting to your .NET port 7170
const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7170';

const PROXY_CONFIG = [
  {
    context: [
      '/weatherforecast',
      '/api'
    ],
    target,
    secure: false, 
    ws: true,     
    changeOrigin: true, 
    logLevel: 'debug'  
  }
];

module.exports = PROXY_CONFIG;
