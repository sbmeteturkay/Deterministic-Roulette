using System;

namespace RouletteGame.Interfaces
{
    /// <summary>
    /// Rulet çarkı model arayüzü
    /// </summary>
    public interface IRouletteWheelModel
    {
        /// <summary>
        /// Çarkın şu anki durumu
        /// </summary>
        WheelState CurrentState { get; }
        
        /// <summary>
        /// Son kazanan sayı
        /// </summary>
        int LastWinningNumber { get; }
        
        /// <summary>
        /// Deterministik mod aktif mi
        /// </summary>
        bool IsDeterministicMode { get; }
        
        /// <summary>
        /// Deterministik modda seçilen sayı
        /// </summary>
        int DeterministicNumber { get; }
        
        /// <summary>
        /// Çark durumu değiştiğinde tetiklenen olay
        /// </summary>
        event Action<WheelState> OnStateChanged;
        
        /// <summary>
        /// Spin tamamlandığında tetiklenen olay
        /// </summary>
        event Action<int> OnSpinCompleted;
        
        /// <summary>
        /// Çarkı döndürür
        /// </summary>
        void Spin();
        
        /// <summary>
        /// Deterministik modu açar/kapatır
        /// </summary>
        /// <param name="enabled">Deterministik mod durumu</param>
        void SetDeterministicMode(bool enabled);
        
        /// <summary>
        /// Deterministik modda kazanacak sayıyı ayarlar
        /// </summary>
        /// <param name="number">Kazanacak sayı</param>
        void SetDeterministicNumber(int number);
        
        /// <summary>
        /// Çarkı durdurur ve sonucu belirler
        /// </summary>
        void StopWheel();
        
        /// <summary>
        /// Çarkı sıfırlar
        /// </summary>
        void ResetWheel();
    }
    
    /// <summary>
    /// Çark durumları
    /// </summary>
    public enum WheelState
    {
        Idle,       // Beklemede
        Spinning,   // Dönüyor
        Stopping,   // Duruyor
        Stopped     // Durdu
    }
}

