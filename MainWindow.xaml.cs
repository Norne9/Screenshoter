using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Screenshoter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private Settings _settings;
        private DispatcherTimer _timer;
        private int _countdown;

        public MainWindow() {
            Tools.SetProcessDPIAware();
            _settings = Settings.Load();
            InitializeComponent();

            CaptureTimeText.Text = _settings.CaptureTime.ToString();
            SavePathText.Text = _settings.SavePath;
            DeleteDaysText.Text = _settings.DeleteDays.ToString();

            _timer = new DispatcherTimer();
            _timer.Tick += TimerOnTick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e) {
            _countdown += 1;
            TimerProgress.Minimum = 0;
            TimerProgress.Maximum = _settings.CaptureTime;
            TimerProgress.Value = _countdown;
            if (_countdown >= _settings.CaptureTime) {
                _countdown = 0;
                TakeScreenshot();
            }
        }

        private void TakeScreenshot() {
            var screenshot = Tools.TakeScreenshot();
            if (screenshot == null) {
                MessageBox.Show("Не удалось захватить скриншот", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var time = DateTime.Now;
            var path = Path.Combine(_settings.SavePath, time.ToString("dd.MM.yyyy"),
                time.ToString("HH.mm.ss") + ".jpg");
            try {
                Tools.SaveScreenshot(path, screenshot);
            } catch (Exception e) {
                MessageBox.Show($"Не удалось сохранить скриншот\n{e}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try {
                Tools.RemoveFoldersOlder(_settings.SavePath, _settings.DeleteDays);
            } catch (Exception e) {
                MessageBox.Show($"Не удалось удалить старые папки\n{e}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationForTextBox(object sender, TextCompositionEventArgs e) {
            e.Handled = !int.TryParse(e.Text, out var _);
        }

        private void CaptureTimeChanged(object sender, TextChangedEventArgs e) {
            if (!int.TryParse(CaptureTimeText.Text, out var time)) return;
            _settings.CaptureTime = time;
            _settings.Save();
        }

        private void SavePathChanged(object sender, TextChangedEventArgs e) {
            _settings.SavePath = SavePathText.Text;
            _settings.Save();
        }

        private void DeleteDaysChanged(object sender, TextChangedEventArgs e) {
            if (!int.TryParse(DeleteDaysText.Text, out var days)) return;
            _settings.DeleteDays = days;
            _settings.Save();
        }

        private void ChangePathClick(object sender, RoutedEventArgs e) {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
                dialog.SelectedPath = _settings.SavePath;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK) {
                    SavePathText.Text = dialog.SelectedPath;
                }
            }
        }
    }
}