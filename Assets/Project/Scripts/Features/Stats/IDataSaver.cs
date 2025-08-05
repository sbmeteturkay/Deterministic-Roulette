namespace RouletteGame.Interfaces
{
    /// <summary>
    /// Veri kaydetme ve yükleme işlemleri için arayüz
    /// </summary>
    public interface IDataSaver
    {
        /// <summary>
        /// Veriyi belirtilen dosya yoluna kaydeder
        /// </summary>
        /// <param name="data">Kaydedilecek veri</param>
        /// <param name="filePath">Dosya yolu</param>
        void Save<T>(T data, string filePath);
        
        /// <summary>
        /// Belirtilen dosya yolundan veriyi yükler
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Yüklenen veri</returns>
        T Load<T>(string filePath);
        
        /// <summary>
        /// Belirtilen dosya yolunda dosya var mı kontrol eder
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <returns>Dosya varlık durumu</returns>
        bool FileExists(string filePath);
        
        /// <summary>
        /// Belirtilen dosya yolundaki dosyayı siler
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        void DeleteFile(string filePath);
    }
}

