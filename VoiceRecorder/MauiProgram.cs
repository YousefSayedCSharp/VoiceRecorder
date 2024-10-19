#if ANDROID || IOS
using VoiceRecorder.Platforms.Service;
#endif

namespace VoiceRecorder
{
    public static class MauiProgram
    {
        public static string ySRecorderFolder()
        {
            string dir =  "/storage/emulated/0/ySRecorder/";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("AudioIconFonts.ttf", "AudioIconFonts");
                });

#if ANDROID || IOS
            builder.Services.AddTransient<IAudioPlayerService, AudioPlayerService>();
            builder.Services.AddTransient<IRecordAudioService, RecordAudioService>();
#endif

            builder.Services.AddTransient<HomeView>();
            builder.Services.AddTransient<AppShell>();
            //builder.ConfigureSyncfusionListView();
            return builder.Build();
        }
    }
}