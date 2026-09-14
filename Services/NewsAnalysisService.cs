using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BotCripto.Services;

public class NewsAnalysisService
{
    private readonly HttpClient _httpClient;
    private readonly string[] _positiveSentimentKeywords;
    private readonly string[] _negativeSentimentKeywords;
    private readonly int _lookbackHours;
    private Dictionary<string, decimal> _sentimentCache = new();
    private DateTime _lastCacheUpdate = DateTime.MinValue;

    public NewsAnalysisService(
        string[] positiveKeywords,
        string[] negativeKeywords,
        int lookbackHours = 24)
    {
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        _positiveSentimentKeywords = positiveKeywords;
        _negativeSentimentKeywords = negativeKeywords;
        _lookbackHours = lookbackHours;
    }

    public async Task<decimal> GetSentimentScoreAsync(string symbol)
    {
        try
        {
            // Controlla cache (valida per 10 minuti)
            if (_sentimentCache.ContainsKey(symbol))
            {
                var cacheAge = (DateTime.UtcNow - _lastCacheUpdate).TotalMinutes;
                if (cacheAge < 10)
                {
                    return _sentimentCache[symbol];
                }
            }

            // Tenta di ottenere notizie da CoinGecko
            var news = await GetCoinGeckoNewsAsync(symbol);
            var score = CalculateSentimentScore(news);

            _sentimentCache[symbol] = score;
            _lastCacheUpdate = DateTime.UtcNow;

            Console.WriteLine($"📰 {symbol}: Sentiment Score = {score:F2} ({GetSentimentDescription(score)})");
            return score;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Errore sentiment {symbol}: {ex.Message}");
            return 0m; // Neutral se errore
        }
    }

    private async Task<List<string>> GetCoinGeckoNewsAsync(string symbol)
    {
        try
        {
            // CoinGecko API - notizie trend sulle criptovalute
            var url = $"https://api.coingecko.com/api/v3/search/trending";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            var content = await response.Content.ReadAsStringAsync();
            dynamic data = JsonConvert.DeserializeObject(content);

            var newsList = new List<string>();

            // Estrai notizie dalle trending coins
            foreach (var coin in data?["coins"] ?? new dynamic[0])
            {
                string coinName = coin?["item"]?["name"]?.ToString() ?? "";
                string coinSymbol = coin?["item"]?["symbol"]?.ToString()?.ToUpper() ?? "";

                if (!string.IsNullOrEmpty(coinName) && (coinSymbol == symbol || coinName.Contains(symbol, StringComparison.OrdinalIgnoreCase)))
                {
                    newsList.Add(coinName);
                }
            }

            return newsList;
        }
        catch
        {
            return new List<string>();
        }
    }

    private decimal CalculateSentimentScore(List<string> newsTexts)
    {
        if (newsTexts.Count == 0)
            return 0m; // Neutral se nessuna notizia

        int positiveCount = 0;
        int negativeCount = 0;

        foreach (var text in newsTexts)
        {
            var lowerText = text.ToLower();

            foreach (var keyword in _positiveSentimentKeywords)
            {
                if (lowerText.Contains(keyword))
                    positiveCount++;
            }

            foreach (var keyword in _negativeSentimentKeywords)
            {
                if (lowerText.Contains(keyword))
                    negativeCount++;
            }
        }

        // Calcola score normalizzato (-1 a +1)
        if (positiveCount + negativeCount == 0)
            return 0m;

        decimal score = (decimal)(positiveCount - negativeCount) / (positiveCount + negativeCount);
        return Math.Clamp(score, -1m, 1m);
    }

    private string GetSentimentDescription(decimal score)
    {
        return score switch
        {
            >= 0.6m => "🟢 Very Bullish",
            >= 0.3m => "🟢 Bullish",
            >= 0.1m => "🟡 Slightly Bullish",
            > -0.1m => "⚪ Neutral",
            > -0.3m => "🔴 Slightly Bearish",
            > -0.6m => "🔴 Bearish",
            _ => "🔴 Very Bearish"
        };
    }

    public void ClearCache()
    {
        _sentimentCache.Clear();
        _lastCacheUpdate = DateTime.MinValue;
    }
}
