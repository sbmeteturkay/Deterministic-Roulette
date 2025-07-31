using System.Collections.Generic;

namespace RouletteGame.Interfaces
{
    /// <summary>
    /// Rulet türlerinin (Avrupa/Amerikan) ortak arayüzü
    /// </summary>
    public interface IRouletteType
    {
        /// <summary>
        /// Rulet türü adı
        /// </summary>
        string TypeName { get; }
        
        /// <summary>
        /// Mevcut sayılar listesi (0, 00 dahil)
        /// </summary>
        List<int> AvailableNumbers { get; }
        
        /// <summary>
        /// Sıfır sayıları (0 ve varsa 00)
        /// </summary>
        List<int> ZeroNumbers { get; }
        
        /// <summary>
        /// Kırmızı sayılar
        /// </summary>
        List<int> RedNumbers { get; }
        
        /// <summary>
        /// Siyah sayılar
        /// </summary>
        List<int> BlackNumbers { get; }
        
        /// <summary>
        /// Verilen sayının rengini döndürür
        /// </summary>
        /// <param name="number">Sayı</param>
        /// <returns>Renk (Red, Black, Green)</returns>
        string GetNumberColor(int number);
        
        /// <summary>
        /// Verilen sayının çift mi tek mi olduğunu kontrol eder
        /// </summary>
        /// <param name="number">Sayı</param>
        /// <returns>True ise çift, False ise tek</returns>
        bool IsEven(int number);
        
        /// <summary>
        /// Verilen sayının yüksek (19-36) mi düşük (1-18) mü olduğunu kontrol eder
        /// </summary>
        /// <param name="number">Sayı</param>
        /// <returns>True ise yüksek, False ise düşük</returns>
        bool IsHigh(int number);
    }
}

