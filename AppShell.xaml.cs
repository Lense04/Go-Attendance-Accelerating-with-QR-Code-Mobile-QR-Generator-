using UMAttendanceSystem_Mobile.Services;
using UMAttendanceSystem_Mobile.Pages;
using Microsoft.Maui.Controls;

namespace UMAttendanceSystem_Mobile
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;

        public AppShell(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnLogoutMenuItemClicked(object sender, EventArgs e)
        {
            await _authService.Logout();
            Application.Current.MainPage = new NavigationPage(new SignInPage(new DatabaseSignIn(App.ConnectionString), _authService));
        }
    }
}