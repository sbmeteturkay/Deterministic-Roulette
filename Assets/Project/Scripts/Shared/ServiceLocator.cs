
namespace RouletteGame.Service
{
    public static class ServiceLocator
    {
        public static ISoundService SoundService { get; set; }

        public static void Initialize(ISoundService soundService)
        {
            SoundService = soundService;
        }
    }
}