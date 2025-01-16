using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using QRCoder;

namespace UMAttendanceSystem_Mobile
{
    public partial class MainPage : ContentPage
    {
        private System.Timers.Timer _timer;
        private int _totalTime = 300;
        private int _remainingTime;
        private AuthService _authService;

        public MainPage()
        {
            InitializeComponent();
            _authService = new AuthService();
            InitializeTimer();
            GenerateQR();
        }

        private void InitializeTimer()
        {
            DateTime now = DateTime.Now;
            DateTime nextInterval = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
            nextInterval = nextInterval.AddMinutes(5 - (now.Minute % 5));
            _remainingTime = (int)(nextInterval - now).TotalSeconds;

            if (_timer == null)
            {
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.AutoReset = true;
                _timer.Start();
            }
        }

        private void OnTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _remainingTime--;

            if (_remainingTime <= 0)
            {
                _remainingTime = 0;
                _timer.Stop();
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    GenerateQR();
                    ResetTimer();
                });
            }
            else
            {
                MainThread.InvokeOnMainThreadAsync(UpdateTimerUI);
            }
        }

        private void ResetTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            InitializeTimer();
        }

        private void UpdateTimerUI()
        {
            int minutes = _remainingTime / 60;
            int seconds = _remainingTime % 60;

            TimerLabel.Text = $"{minutes}:{seconds:D2}";
            TimerBar.Progress = (double)_remainingTime / _totalTime;

            TimerBar.ProgressColor = _remainingTime switch
            {
                > 20 => Colors.Green,
                > 5 => Colors.Yellow,
                _ => Colors.Red
            };
        }

        private void GenerateQRCode(string data)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.L);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            QRCodeImage.Source = ImageSource.FromStream(() => new MemoryStream(qrCodeImage));
        }

        private async void GenerateQR()
        {
            string studentNumber = await _authService.GetStudentNumber();
            string studentName = await _authService.GetStudentName();
            string department = await _authService.GetDepartment();

            if (!string.IsNullOrEmpty(studentNumber) && !string.IsNullOrEmpty(studentName) && !string.IsNullOrEmpty(department))
            {
                string roundedTimestamp = GetRoundedTimestamp();
                string dataToEncode = $"{studentNumber}/{studentName}/{department}/{roundedTimestamp}";
                GenerateQRCode(dataToEncode);
            }
            else
            {
                await DisplayAlert("Error", "Student information is not available.", "OK");
            }
        }

        private string GetRoundedTimestamp()
        {
            DateTime now = DateTime.Now;
            int minutes = now.Minute;
            int roundedMinutes = (minutes / 5) * 5;
            DateTime roundedTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, roundedMinutes, 0);
            return roundedTime.ToString("yyyyMMddHHmmss");
        }
    }
}