using System.Collections.Generic;
using System.Linq;
using RouletteGame.Interfaces;

namespace RouletteGame.Models
{
    /// <summary>
    /// Ödeme hesaplamalarını yapan sınıf
    /// </summary>
    public class PayoutCalculator
    {
        /// <summary>
        /// Verilen bahisler ve kazanan sayıya göre toplam ödemeyi hesaplar
        /// </summary>
        /// <param name="bets">Bahisler listesi</param>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Toplam ödeme miktarı</returns>
        public decimal CalculateTotalPayout(List<IBet> bets, int winningNumber)
        {
            return bets.Sum(bet => bet.CalculatePayout(winningNumber));
        }

        /// <summary>
        /// Kazanan bahisleri getirir
        /// </summary>
        /// <param name="bets">Bahisler listesi</param>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Kazanan bahisler</returns>
        public List<IBet> GetWinningBets(List<IBet> bets, int winningNumber)
        {
            return bets.Where(bet => bet.IsWinning(winningNumber)).ToList();
        }

        /// <summary>
        /// Kaybeden bahisleri getirir
        /// </summary>
        /// <param name="bets">Bahisler listesi</param>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Kaybeden bahisler</returns>
        public List<IBet> GetLosingBets(List<IBet> bets, int winningNumber)
        {
            return bets.Where(bet => !bet.IsWinning(winningNumber)).ToList();
        }

        /// <summary>
        /// Toplam kaybedilen miktarı hesaplar
        /// </summary>
        /// <param name="bets">Bahisler listesi</param>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Toplam kaybedilen miktar</returns>
        public decimal CalculateTotalLoss(List<IBet> bets, int winningNumber)
        {
            var losingBets = GetLosingBets(bets, winningNumber);
            return losingBets.Sum(bet => bet.BetAmount);
        }

        /// <summary>
        /// Net kar/zarar hesaplar (kazanılan - kaybedilen)
        /// </summary>
        /// <param name="bets">Bahisler listesi</param>
        /// <param name="winningNumber">Kazanan sayı</param>
        /// <returns>Net kar/zarar</returns>
        public decimal CalculateNetProfitLoss(List<IBet> bets, int winningNumber)
        {
            decimal totalPayout = CalculateTotalPayout(bets, winningNumber);
            decimal totalLoss = CalculateTotalLoss(bets, winningNumber);
            return totalPayout - totalLoss;
        }
    }
}

