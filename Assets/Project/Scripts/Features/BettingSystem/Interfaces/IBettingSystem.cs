using System;
using System.Collections.Generic;

namespace RouletteGame.Interfaces
{
    /// <summary>
    /// Bahis sistemi arayüzü
    /// </summary>
    public interface IBettingSystem
    {
        /// <summary>
        /// Aktif bahisler listesi
        /// </summary>
        List<IBet> ActiveBets { get; }
        
        /// <summary>
        /// Toplam bahis miktarı
        /// </summary>
        decimal TotalBetAmount { get; }
        
        /// <summary>
        /// Bahis eklendiğinde tetiklenen olay
        /// </summary>
        event Action<IBet> OnBetAdded;
        
        /// <summary>
        /// Bahis kaldırıldığında tetiklenen olay
        /// </summary>
        event Action<IBet> OnBetRemoved;
        
        /// <summary>
        /// Tüm bahisler temizlendiğinde tetiklenen olay
        /// </summary>
        event Action OnAllBetsCleared;
        
        /// <summary>
        /// Bahis ekler
        /// </summary>
        /// <param name="bet">Eklenecek bahis</param>
        /// <returns>Bahis başarıyla eklendi mi</returns>
        bool AddBet(IBet bet);
        
        /// <summary>
        /// Bahis kaldırır
        /// </summary>
        /// <param name="bet">Kaldırılacak bahis</param>
        /// <returns>Bahis başarıyla kaldırıldı mı</returns>
        bool RemoveBet(IBet bet);
        
        /// <summary>
        /// Tüm bahisleri temizler
        /// </summary>
        void ClearAllBets();
        
    }
}

