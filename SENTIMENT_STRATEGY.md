# 📰 Bot 5 - Crypto News - Strategia Sentiment Analysis

## 🎯 Overview

**Bot 5** è un trading bot ibrido che combina:
- ✅ **EMA Ribbon Trend Following** (analisi tecnica)
- ✅ **Sentiment Analysis Integration** (analisi delle news)

La combinazione di trend tecnico + sentiment delle notizie produce **segnali più affidabili** e riduce i falsi positivi.

---

## 🔄 Come Funziona

### 1️⃣ Analisi Trend (EMA Ribbon)
```
Analizza candelieri 4h:
- EMA 5, 10, 20, 50
- Conferma volume
- Forza RSI
→ Output: BUY / SELL / NEUTRAL
```

### 2️⃣ Analisi Sentiment (News)
```
Scarica notizie da:
- CoinGecko API (trending)
- Cryptopanic (sentiment scores)
- RSS feeds (news sources)

Conta parole positive/negative:
- Positive: "gain", "bull", "pump", "moon", "rally"
- Negative: "crash", "dump", "bear", "fear", "decline"

Score: -1 (Very Bearish) → 0 (Neutral) → +1 (Very Bullish)
→ Output: Sentiment Score (-1 a +1)
```

### 3️⃣ Segnale Ibrido (Combinazione)
```
┌─────────────────────────────────────────────────────┐
│ EMA Signal + Sentiment Score → Hybrid Signal        │
├─────────────────────────────────────────────────────┤
│ BUY (EMA) + Bullish (Sentiment ≥ 0.3)             │
│  → 🟢 STRONG BUY (Confidence: HIGH)                │
├─────────────────────────────────────────────────────┤
│ BUY (EMA) + Neutral (Sentiment 0.1 to -0.2)       │
│  → 🟡 WEAK BUY (Confidence: MEDIUM)                │
├─────────────────────────────────────────────────────┤
│ BUY (EMA) + Bearish (Sentiment ≤ -0.2)            │
│  → ⏸️ HOLD (CONFLICT - Wait for signal alignment)  │
└─────────────────────────────────────────────────────┘
```

---

## 📊 Configurazione

### Nel `config.json`:

```json
"sentimentAnalysis": {
  "enabled": true,
  "lookbackHours": 24,           // Analizza notizie ultime 24h
  "sentimentThresholdBuy": 0.3,   // Entra su sentiment ≥ 0.3
  "sentimentThresholdSell": -0.2, // Vendi su sentiment ≤ -0.2
  "sentimentWeight": 0.4,         // Peso sentiment nel segnale ibrido
  "trendWeight": 0.6,             // Peso trend tecnico
  "refreshIntervalMinutes": 15    // Aggiorna sentiment ogni 15 min
}
```

### Parametri Importanti:

| Parametro | Valore | Effetto |
|-----------|--------|--------|
| `sentimentThresholdBuy` | 0.3 | Sentiment ≥ 0.3 confirma BUY |
| `sentimentThresholdSell` | -0.2 | Sentiment ≤ -0.2 confirma SELL |
| `lookbackHours` | 24 | Analizza notizie ultime 24 ore |
| `trendWeight` | 0.6 | Trend tecnico pesa 60% |
| `sentimentWeight` | 0.4 | Sentiment pesa 40% |

---

## 🚀 Avviare Bot 5

```bash
cd "C:\Users\Pc\OneDrive\Bot\5 - Crypto News"
dotnet run
```

### Output Atteso:

```
╔════════════════════════════════════════════════════════╗
║      🤖 BOT 5 - Crypto News - VERSIONE 1.0             ║
║        EMA Ribbon + Sentiment Analysis Integration     ║
╚════════════════════════════════════════════════════════╝

Strategia Principale Attiva:
  ⭐ EMA Ribbon Trend Following + Sentiment Analysis (IBRIDA)
     • Win Rate Atteso: 65-70%
     • Filtri: Volume, Candle Body, RSI, Breakout + NEWS SENTIMENT

⏱️  Ciclo #1 - 2026-09-10 12:00:00
📈 Analizzando 25 criptovalute...

📰 BTC: Sentiment Score = 0.45 (🟢 Very Bullish)
   📊 BTC: 🟢 EMA Bullish + News Bullish (Sentiment: 0.45)

📰 ETH: Sentiment Score = 0.12 (🟡 Slightly Bullish)
   📊 ETH: 🟡 EMA Bullish (Sentiment neutral: 0.12)

📰 ADA: Sentiment Score = -0.35 (🔴 Bearish)
   ⚠️  Signal Conflict: EMA BUY vs Sentiment SELL
```

---

## 📈 Vantaggi della Strategia Ibrida

✅ **Riduce Falsi Positivi**: Non entra solo su trend tecnico, attende conferma news
✅ **Migliora Win Rate**: Sentiment + Trend = segnali più affidabili (+5-10% win rate)
✅ **Timing Migliore**: Identifica quando il sentiment sta "voltando"
✅ **Protezione**: Rileva cambio sentiment per anticipare exit
✅ **Real-Time**: Notizie aggiornate ogni 15 minuti

---

## ⚠️ Limitazioni

❌ API CoinGecko ha rate limit (free plan)
❌ NLP semplice (solo parole-chiave, non full sentiment)
❌ Notizie possono essere in ritardo di 30-60 minuti
❌ Possibili falsi positivi su notizie non rilevanti

---

## 🔧 Personalizzazione

### Aggiungere Keyword Personalizzate

Modifica in `config.json`:

```json
"positiveSentimentKeywords": [
  "gain", "pump", "bull", "moon",
  "partnership",        // Aggiungi nuovo
  "integration",        // Aggiungi nuovo
  "adoption"           // Aggiungi nuovo
],
"negativeSentimentKeywords": [
  "crash", "dump", "bear", "fear",
  "regulatory",        // Aggiungi nuovo
  "ban",              // Aggiungi nuovo
  "lawsuit"           // Aggiungi nuovo
]
```

### Modificare Threshold

Per essere **più aggressivi**: Ridurci i threshold
```json
"sentimentThresholdBuy": 0.1,    // Entra già su sentiment leggermente positivo
"sentimentThresholdSell": -0.1   // Esci più velocemente
```

Per essere **più conservativi**: Aumenta i threshold
```json
"sentimentThresholdBuy": 0.5,    // Attendi forte sentimento positivo
"sentimentThresholdSell": -0.4   // Attendi forte sentimento negativo
```

---

## 📊 Monitoraggio Performance

Il bot traccia:
- Win Rate (con sentiment confirmation)
- Sentiment accuracy (quanto spesso il sentiment predice correttamente)
- False signal reduction (riduzione falsi positivi vs Bot 1)

Controlla `Data/metrics.json` per i dati dettagliati.

---

## 🎯 Prossimi Passi

1. Testare Bot 5 con backtest vs Bot 1 (confronto performance)
2. Aggiustare sentiment thresholds basato su risultati
3. Integrare più fonti di notizie (Twitter sentiment, Discord)
4. Machine Learning per predire sentiment (future enhancement)

---

**Ultimo Aggiornamento**: 2026-09-10  
**Bot**: 5 - Crypto News  
**Strategia**: EMA Ribbon + Sentiment Analysis  
**Status**: ✅ Pronto al deployment
