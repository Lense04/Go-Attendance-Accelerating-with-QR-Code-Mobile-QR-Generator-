using Microsoft.Maui.Controls;
using UMAttendanceSystem_Mobile.Pages;
using UMAttendanceSystem_Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UMAttendanceSystem_Mobile
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        private readonly DatabaseSignIn _databaseSignIn;

        public static string ConnectionString { get; } = "Server = 34.126.135.52,1433;Database=finalproject;User Id = sqlserver;Password=admin;TrustServerCertificate=True;";

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            
            _authService = serviceProvider.GetRequiredService<AuthService>();
            _databaseSignIn = serviceProvider.GetRequiredService<DatabaseSignIn>();

           
            MainPage = CreateMainPage();
        }

        private Page CreateMainPage()
        {
            var isAuthenticated = _authService.IsAuthenticated();

            if (isAuthenticated)
            {
                return new AppShell(_authService);
            }
            else
            {
                return new SignInPage(_databaseSignIn, _authService);
            }
        }
    }
}