using System;
using System.Collections.Generic;
using System.Linq;

public class RouletteSignalParser
{
    public class SignalResult
    {
        public string BetType { get; set; }
        public string Category { get; set; }
        public List<int> CoveredNumbers { get; set; }
        public int Multiplier { get; set; }
        public string RawSignal { get; set; }
        public int? Number { get; set; }
    }

    private class SignalDefinition
    {
        public string BetType;
        public string Category;
        public List<int> CoveredNumbers;
        public int Multiplier;
        public int? Number;

        public SignalDefinition(string betType, string category, List<int> coveredNumbers, int multiplier, int? number)
        {
            BetType = betType;
            Category = category;
            CoveredNumbers = coveredNumbers;
            Multiplier = multiplier;
            Number = number;
        }
    }

    private static readonly SignalDefinition Unknown = new SignalDefinition(
        "Unknown",
        "Unknown",
        new List<int>(),
        0,
        null
    );

    private static readonly Dictionary<string, SignalDefinition> signalDefinitions = new Dictionary<string, SignalDefinition>(StringComparer.OrdinalIgnoreCase)
    {
        ["112"] = new SignalDefinition("1st Dozen", "Dozen", Enumerable.Range(1,12).ToList(), 2, null),
        ["212"] = new SignalDefinition("2nd Dozen", "Dozen", Enumerable.Range(13,12).ToList(), 2, null),
        ["312"] = new SignalDefinition("3rd Dozen", "Dozen", Enumerable.Range(25,12).ToList(), 2, null),

        ["118"] = new SignalDefinition("1 to 18", "Half", Enumerable.Range(1,18).ToList(), 1, null),
        ["1936"] = new SignalDefinition("19 to 36", "Half", Enumerable.Range(19,18).ToList(), 1, null),

        ["134"] = new SignalDefinition("1st Column", "Column", new List<int>{1,4,7,10,13,16,19,22,25,28,31,34}, 1, null),
        ["235"] = new SignalDefinition("2nd Column", "Column", new List<int>{2,5,8,11,14,17,20,23,26,29,32,35}, 1, null),
        ["336"] = new SignalDefinition("3rd Column", "Column", new List<int>{3,6,9,12,15,18,21,24,27,30,33,36}, 1, null),

        ["red"] = new SignalDefinition("Red", "Color", new List<int>{1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36}, 1, null),
        ["black"] = new SignalDefinition("Black", "Color", new List<int>{2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35}, 1, null),

        ["odd"] = new SignalDefinition("Odd", "OddEven", Enumerable.Range(1,35).Where(n => n % 2 == 1).ToList(), 1, null),
        ["even"] = new SignalDefinition("Even", "OddEven", Enumerable.Range(2,35).Where(n => n % 2 == 0).ToList(), 1, null),
    };

    public static SignalResult ParseSignal(string signal)
    {
        if (signal == null) signal = "";
        signal = signal.Trim();

        SignalDefinition def;

        if (int.TryParse(signal, out int num) && num >= 0 && num <= 36)
        {
            def = new SignalDefinition(
                "Straight " + num,
                "Straight",
                num == 0 ? new List<int> { 0 } : new List<int> { num },
                35,
                num
            );
        }
        else if (!signalDefinitions.TryGetValue(signal, out def))
        {
            def = Unknown;
        }

        return new SignalResult
        {
            BetType = def.BetType,
            Category = def.Category,
            CoveredNumbers = def.CoveredNumbers,
            Multiplier = def.Multiplier,
            RawSignal = signal,
            Number = def.Category == "Straight" ? def.Number : (int?)null
        };
    }
}
