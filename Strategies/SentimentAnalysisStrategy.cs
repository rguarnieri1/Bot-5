using BotCripto.Models;
using BotCripto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotCripto.Strategies;

public class SentimentAnalysisStrategy
{
    private readonly NewsAnalysisService _newsService;
    private readonly decimal _sentimentThresholdBuy;
    private readonly decimal _sentimentThresholdSell;

    public SentimentAnalysisStrategy(
        NewsAnalysisService newsService,
        decimal sentimentThresholdBuy = 0.3m,
        decimal sentimentThresholdSell = -0.2m)
    {
        _newsService = newsService;
        _sentimentThresholdBuy = sentimentThresholdBuy;
        _sentimentThresholdSell = sentimentThresholdSell;
    }

    public async Task<(string Signal, decimal SentimentScore)> AnalyzeAsync(Cryptocurrency crypto)
    {
        try
        {
            var sentimentScore = await _newsService.GetSentimentScoreAsync(crypto.Symbol);

            // Genera segnale basato su sentiment
            string signal = "NEUTRAL";

            if (sentimentScore >= _sentimentThresholdBuy)
            {
                signal = "BUY";
            }
            else if (sentimentScore <= _sentimentThresholdSell)
            {
                signal = "SELL";
            }

            return (signal, sentimentScore);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Errore sentiment analysis {crypto.Symbol}: {ex.Message}");
            return ("NEUTRAL", 0m);
        }
    }

    public async Task<List<Cryptocurrency>> FilterBySentimentAsync(List<Cryptocurrency> cryptos)
    {
        var filteredList = new List<Cryptocurrency>();

        foreach (var crypto in cryptos)
        {
            var (signal, score) = await AnalyzeAsync(crypto);
            if (signal == "BUY" || signal == "SELL")
            {
                filteredList.Add(crypto);
            }
        }

        return filteredList;
    }
}
