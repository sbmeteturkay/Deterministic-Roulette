using System;
using System.Collections.Generic;
using System.Linq;

namespace RouletteGame.Interfaces
{
    public enum RouletteColor
    {
        Red,
        Black,
        Green // 0 ve (Amerikan) 00
    }

    public static class NumberProperties
    {
        // Amerikan ruletinde 00 için -1 kullanalım; kullanıcı tarafında temsil ederken string e çevirilebilir.
        private static readonly HashSet<int> Reds = new() { 1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36 };
        private static readonly HashSet<int> Blacks = new() { 2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35 };

        public static RouletteColor GetColor(int number)
        {
            if (number == 0 || number == -1) // -1 -> "00"
                return RouletteColor.Green;
            if (Reds.Contains(number))
                return RouletteColor.Red;
            if (Blacks.Contains(number))
                return RouletteColor.Black;
            throw new ArgumentOutOfRangeException(nameof(number), $"Geçersiz roulette numarası: {number}");
        }

        public static bool IsLow(int number) => number >= 1 && number <= 18;
        public static bool IsHigh(int number) => number >= 19 && number <= 36;
        public static bool IsEven(int number) => number != 0 && number % 2 == 0;
        public static bool IsOdd(int number) => number % 2 == 1;
    }

    public interface IBet
    {
        int BetAmount { get; }
        IReadOnlyList<int> CoveredNumbers { get; }
        string BetType { get; }
        int PayoutMultiplier { get; }

        bool IsWinning(int winningNumber);
        /// <summary>
        /// Bahis miktarı dahil toplam ödeme (örn: eşleşme varsa betAmount * multiplier)
        /// </summary>
        int CalculatePayout(int winningNumber);
    }

    public abstract class BaseBet : IBet
    {
        public int BetAmount { get; set; }
        public IReadOnlyList<int> CoveredNumbers { get;  set; }
        public string BetType { get;  set; }
        public int PayoutMultiplier { get;  set; }

        protected BaseBet(int betAmount, IEnumerable<int> coveredNumbers, int payoutMultiplier, string betType)
        {
            if (betAmount <= 0) throw new ArgumentException("Bahis miktarı sıfırdan büyük olmalı.", nameof(betAmount));
            BetAmount = betAmount;
            CoveredNumbers = coveredNumbers?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(coveredNumbers));
            PayoutMultiplier = payoutMultiplier;
            BetType = betType;
        }

        public abstract bool IsWinning(int winningNumber);

        public virtual int CalculatePayout(int winningNumber)
        {
            return IsWinning(winningNumber) ? BetAmount * PayoutMultiplier : 0;
        }
    }
    public static class SignalBetFactory
    {
        /// <summary>
        /// Gelen sinyale göre uygun IBet örneği üretir.
        /// Tanınmayan sinyalde null döner.
        /// Desteklenen sinyaller:
        /// "0".."36" => Straight
        /// "112","212","312" => 1.,2.,3. düzine
        /// "118" => 1-18 (Low), "1936" => 19-36 (High)
        /// "134","235","336" => Column 1,2,3
        /// "even","odd" => Even/Odd
        /// "red","black" => Red/Black
        /// </summary>
        public static IBet CreateBetFromSignal(string signal, int amount)
        {
            if (string.IsNullOrWhiteSpace(signal))
                return null;

            signal = signal.Trim().ToLowerInvariant();

            // Düzine bahisleri
            if (signal == "112")
                return new DozenBet(amount, DozenBet.Dozen.First);
            if (signal == "212")
                return new DozenBet(amount, DozenBet.Dozen.Second);
            if (signal == "312")
                return new DozenBet(amount, DozenBet.Dozen.Third);

            // High / Low
            if (signal == "118")
                return new HighLowBet(amount, HighLowBet.Choice.Low);
            if (signal == "1936")
                return new HighLowBet(amount, HighLowBet.Choice.High);

            // Sütun bahisleri
            if (signal == "134")
                return new ColumnBet(amount, 1);
            if (signal == "235")
                return new ColumnBet(amount, 2);
            if (signal == "336")
                return new ColumnBet(amount, 3);

            // Even / Odd
            if (signal == "even")
                return new EvenOddBet(amount, EvenOddBet.Choice.Even);
            if (signal == "odd")
                return new EvenOddBet(amount, EvenOddBet.Choice.Odd);

            // Red / Black
            if (signal == "red")
                return new RedBlackBet(amount, RedBlackBet.Choice.Red);
            if (signal == "black")
                return new RedBlackBet(amount, RedBlackBet.Choice.Black);

            // Düz sayı (straight)
            if (int.TryParse(signal, out var number) && number >= 0 && number <= 36)
                return new StraightBet(amount, number);

            // Tanınmayan
            return null;
        }
    }
    public class StraightBet : BaseBet
    {
        public StraightBet(int betAmount, int number)
            : base(betAmount, new[] { number }, 35, "Straight") // 35:1 genelde
        { }

        public override bool IsWinning(int winningNumber)
        {
            return CoveredNumbers.Contains(winningNumber);
        }
    }

    public class SplitBet : BaseBet
    {
        public SplitBet(int betAmount, int numberA, int numberB)
            : base(betAmount, new[] { numberA, numberB }, 17, "Split") // 17:1
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class StreetBet : BaseBet
    {
        public StreetBet(int betAmount, IEnumerable<int> threeNumbers)
            : base(betAmount, threeNumbers, 11, "Street") // 11:1
        {
            if (CoveredNumbers.Count != 3) throw new ArgumentException("Street bet 3 sayı içermeli.");
        }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class CornerBet : BaseBet
    {
        public CornerBet(int betAmount, IEnumerable<int> fourNumbers)
            : base(betAmount, fourNumbers, 8, "Corner") // 8:1
        {
            if (CoveredNumbers.Count != 4) throw new ArgumentException("Corner bet 4 sayı içermeli.");
        }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class LineBet : BaseBet // Six Line
    {
        public LineBet(int betAmount, IEnumerable<int> sixNumbers)
            : base(betAmount, sixNumbers, 5, "Line") // 5:1
        {
            if (CoveredNumbers.Count != 6) throw new ArgumentException("Line bet 6 sayı içermeli.");
        }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class DozenBet : BaseBet
    {
        public enum Dozen { First = 1, Second = 2, Third = 3 }
        private static readonly Dictionary<Dozen, int[]> Ranges = new()
        {
            { Dozen.First, Enumerable.Range(1,12).ToArray() },
            { Dozen.Second, Enumerable.Range(13,12).ToArray() },
            { Dozen.Third, Enumerable.Range(25,12).ToArray() }
        };

        public DozenBet(int betAmount, Dozen which)
            : base(betAmount, Ranges[which], 2, "Dozen") // 2:1
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class ColumnBet : BaseBet
    {
        // Rulet tablosundaki 3 sütun: 1,4,7...; 2,5,8...; 3,6,9...
        private static readonly Dictionary<int, int[]> Columns = new()
        {
            { 1, Enumerable.Range(1, 12).Select(i => (i - 1) * 3 + 1).ToArray() }, // first column
            { 2, Enumerable.Range(1, 12).Select(i => (i - 1) * 3 + 2).ToArray() },
            { 3, Enumerable.Range(1, 12).Select(i => (i - 1) * 3 + 3).ToArray() }
        };

        public ColumnBet(int betAmount, int columnIndex)
            : base(betAmount, Columns[columnIndex], 2, "Column") // 2:1
        {
            if (columnIndex < 1 || columnIndex > 3) throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class EvenOddBet : BaseBet
    {
        public enum Choice { Even, Odd }

        public EvenOddBet(int betAmount, Choice choice)
            : base(betAmount,
                  choice == Choice.Even
                      ? Enumerable.Range(1, 36).Where(n => NumberProperties.IsEven(n)).ToArray()
                      : Enumerable.Range(1, 36).Where(n => NumberProperties.IsOdd(n)).ToArray(),
                  1, "EvenOdd") // 1:1
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class RedBlackBet : BaseBet
    {
        public enum Choice { Red, Black }

        public RedBlackBet(int betAmount, Choice choice)
            : base(betAmount,
                  choice == Choice.Red
                      ? Enumerable.Range(1, 36).Where(n => NumberProperties.GetColor(n) == RouletteColor.Red).ToArray()
                      : Enumerable.Range(1, 36).Where(n => NumberProperties.GetColor(n) == RouletteColor.Black).ToArray(),
                  1, "RedBlack") // 1:1
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class HighLowBet : BaseBet
    {
        public enum Choice { High, Low }

        public HighLowBet(int betAmount, Choice choice)
            : base(betAmount,
                  choice == Choice.Low
                      ? Enumerable.Range(1, 18).ToArray()
                      : Enumerable.Range(19, 18).ToArray(),
                  1, "HighLow") // 1:1
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    public class BasketBet : BaseBet
    {
        // Amerikan ruletinde 0, 00(-1), 1,2,3 ortak bahis (5 number)
        public BasketBet(int betAmount)
            : base(betAmount, new[] { 0, -1, 1, 2, 3 }, 6, "Basket") // 6:1 (oyun varyasyonuna göre değişebilir)
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }

    // Ekstra örnek: Snake bet (bazı Amerikan masa varyasyonlarında)
    public class SnakeBet : BaseBet
    {
        private static readonly int[] SnakeNumbers = { 1,5,9,12,14,16,19,23,27,30,32,34 }; // klasik snake
        public SnakeBet(int betAmount)
            : base(betAmount, SnakeNumbers, 2, "Snake") // genelde 2:1 gibi olur (fareli olabilir)
        { }

        public override bool IsWinning(int winningNumber) => CoveredNumbers.Contains(winningNumber);
    }
}
