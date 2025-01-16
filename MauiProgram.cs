using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UMAttendanceSystem_Mobile.Services;

namespace UMAttendanceSystem_Mobile
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

            builder.Services.AddSingleton<DatabaseSignIn>(sp => new DatabaseSignIn(App.ConnectionString));
            builder.Services.AddScoped<AuthService>();
            builder.Logging.AddDebug();

            var app = builder.Build();
            return app;
        }
    }
}