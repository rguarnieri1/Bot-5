using BotCripto.Models;
using System;

namespace BotCripto.Strategies;

public class HybridSignalStrategy
{
    private readonly decimal _trendWeight;
    private readonly decimal _sentimentWeight;

    public HybridSignalStrategy(decimal trendWeight = 0.6m, decimal sentimentWeight = 0.4m)
    {
        _trendWeight = trendWeight;
        _sentimentWeight = sentimentWeight;
    }

    public (string Signal, string Reason, decimal Confidence) CombineSignals(
        string emaSignal,
        string sentimentSignal,
        decimal sentimentScore,
        string symbol)
    {
        decimal trendConfidence = 0.7m;
        decimal sentimentConfidence = Math.Abs(sentimentScore);
        decimal hybridConfidence = (trendConfidence * _trendWeight) + (sentimentConfidence * _sentimentWeight);

        string finalSignal = "HOLD";
        string reason = "No clear signal";

        // BUY: EMA bullish + Sentiment bullish
        if (emaSignal == "BUY" && (sentimentSignal == "BUY" || sentimentScore >= 0.1m))
        {
            finalSignal = "BUY";
            reason = $"🟢 EMA Bullish + News Bullish (Sentiment: {sentimentScore:F2})";
            hybridConfidence = Math.Min(hybridConfidence * 1.2m, 1m);
        }
        // SELL: EMA bearish + Sentiment bearish
        else if (emaSignal == "SELL" && (sentimentSignal == "SELL" || sentimentScore <= -0.1m))
        {
            finalSignal = "SELL";
            reason = $"🔴 EMA Bearish + News Bearish (Sentiment: {sentimentScore:F2})";
            hybridConfidence = Math.Min(hybridConfidence * 1.2m, 1m);
        }
        // WEAK BUY
        else if (emaSignal == "BUY" && sentimentScore >= -0.2m)
        {
            finalSignal = "BUY";
            reason = $"🟡 EMA Bullish (Sentiment neutral: {sentimentScore:F2})";
            hybridConfidence *= 0.8m;
        }
        // WEAK SELL
        else if (emaSignal == "SELL" && sentimentScore <= 0.2m)
        {
            finalSignal = "SELL";
            reason = $"🟡 EMA Bearish (Sentiment neutral: {sentimentScore:F2})";
            hybridConfidence *= 0.8m;
        }
        // Conflict
        else if ((emaSignal == "BUY" && sentimentSignal == "SELL") || (emaSignal == "SELL" && sentimentSignal == "BUY"))
        {
            finalSignal = "HOLD";
            reason = $"⚠️  Signal Conflict: EMA {emaSignal} vs Sentiment {sentimentSignal}";
            hybridConfidence *= 0.5m;
        }

        Console.WriteLine($"   📊 {symbol}: {reason}");

        return (finalSignal, reason, hybridConfidence);
    }
}
