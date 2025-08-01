public class TradeSignal
{
    public string Symbol { get; set; }
    public bool IsBuy { get; set; } // True for buy, False for sell
    public decimal SignalWeight { get; set; } // Multiplier for signal
    public decimal Price { get; set; } // Price at which the signal is generated
    public decimal RiskLevel { get; set; } // Predefined risk level, e.g., 0.5 for medium risk
    public decimal Confidence { get; set; } // Similar to SignalStrength, could be a percentage
    public int UserId { get; set; } = 1; // Hardcoded for now
    public int PeriodSeconds { get; set; } // Time period for the signal's evaluation
    public DateTime SignalTime { get; set; }
    public decimal StopLoss { get; set; }
    public decimal TakeProfit { get; set; }
    public int Leverage { get; set; }
}