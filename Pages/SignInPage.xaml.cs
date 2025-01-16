using UMAttendanceSystem_Mobile.Services;
using UMAttendanceSystem_Mobile;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace UMAttendanceSystem_Mobile.Pages
{
    public partial class SignInPage : ContentPage
    {
        private readonly DatabaseSignIn _databaseSignIn;
        private readonly AuthService _authService;
        private bool _isSigningIn = false;

        public SignInPage(DatabaseSignIn databaseSignIn, AuthService authService)
        {
            InitializeComponent();
            _databaseSignIn = databaseSignIn;
            _authService = authService;

            Opacity = 0;
            FadeIn();
            CheckAuthenticationState();
        }

        private async void CheckAuthenticationState()
        {
            await _authService.CheckAuthenticationState();
            if (_authService.IsAuthenticated())
            {
                Application.Current.MainPage = new AppShell(_authService);
            }
        }

        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            if (_isSigningIn)
            {
                return;
            }

            _isSigningIn = true;
            signInButton.IsEnabled = false;
            signInActivityIndicator.IsVisible = true;
            signInActivityIndicator.IsRunning = true;
            signInLabel.IsVisible = true;

            try
            {
                string username = usernameEntry?.Text?.Trim() ?? string.Empty;
                string password = passwordEntry?.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Error", "Please enter both username and password.", "OK");
                    return;
                }

                var (isValid, studentName, department, studentNumber) = await _databaseSignIn.CheckCredentialsAsync(username, password);
                if (isValid)
                {
                    string token = GenerateToken(username);

                    // Save additional user information
                    await _authService.SignIn(token, studentName, department, studentNumber);

                    Application.Current.MainPage = new AppShell(_authService);
                }
                else
                {
                    await DisplayAlert("Error", "Invalid username or password.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                _isSigningIn = false;
                signInButton.IsEnabled = true;
                signInActivityIndicator.IsVisible = false; // Hide the activity indicator
                signInActivityIndicator.IsRunning = false; // Stop the activity indicator
                signInLabel.IsVisible = false;
            }
        }

        private string GenerateToken(string username)
        {
            return $"token_for_{username}_{DateTime.Now.Ticks}";
        }

        private async void FadeIn()
        {
            await Task.Delay(100);
            await this.FadeTo(1, 500);
        }
    }
}