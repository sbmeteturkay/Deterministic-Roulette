using System.Collections.Generic;

namespace RouletteGame.Interfaces
{
    /// <summary>
    /// Rulet bahis türlerinin ortak arayüzü
    /// </summary>
    public interface IBet
    {
        /// <summary>
        /// Bahis miktarı
        /// </summary>
        float BetAmount { get; }
        
        /// <summary>
        /// Bahis yerleştirilen sayılar/pozisyonlar
        /// </summary>
        List<int> CoveredNumbers { get; }
        
        /// <summary>
        /// Bahis türü adı
        /// </summary>
        string BetType { get; }
        
        /// <summary>
        /// Kazanma çarpanı
        /// </summary>
        float PayoutMultiplier { get; }
        
        /// <summary>
        /// Verilen sayının bu bahiste kazandırıp kazandırmadığını kontrol eder
        /// </summary>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Kazanma durumu</returns>
        bool IsWinning(int winningNumber);
        
        /// <summary>
        /// Kazanma durumuna göre ödeme miktarını hesaplar
        /// </summary>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Ödeme miktarı (0 ise kaybetmiş)</returns>
        float CalculatePayout(int winningNumber);
    }
    public class StraightBet: IBet
    {
        public StraightBet(float betAmount, List<int> coveredNumbers, float payoutMultiplier, string betType)
        {
            BetAmount = betAmount;
            CoveredNumbers = coveredNumbers;
            PayoutMultiplier = payoutMultiplier;
            BetType = betType;
        }

        public float BetAmount { get; }
        public List<int> CoveredNumbers { get; }
        public string BetType { get; }
        public float PayoutMultiplier { get; }
        public bool IsWinning(int winningNumber)
        {
            return CoveredNumbers.Contains(winningNumber);
        }

        public float CalculatePayout(int winningNumber)
        {
            return IsWinning(winningNumber)?PayoutMultiplier:0;
        }
    }
}

