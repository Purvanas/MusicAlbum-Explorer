using Microsoft.Extensions.Logging;
using MusicAlbum_Explorer.Views;

namespace MusicAlbum_Explorer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // register pages for routing
            builder.Services.AddSingleton<ArtistListPage>();
            builder.Services.AddTransient<ArtistDetailsPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
