using Microsoft.Win32;
using System.IO;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
namespace BulkFileRenamer
{
    public partial class MainWindow : Window
    {
        public enum Theme { System = 0, Light = 1, Dark = 2 };

        private const string kRegistryPersonalize = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string kRegistryKeyUseLightTheme = "AppsUseLightTheme";
        private const string kLightThemeUri = "pack://application:,,,/BulkFileRenamer;component/LightTheme.xaml";
        private const string kDarkThemeUri = "pack://application:,,,/BulkFileRenamer;component/DarkTheme.xaml";

        private string _selectedFolder = string.Empty;
        private Theme _theme = Theme.System;

        private static bool IsWindowsInDarkMode()
        {
            try
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1416 // Validate platform compatibility
                using var key = Registry.CurrentUser.OpenSubKey(kRegistryPersonalize);

                if (key != null)
                {
                    object value = key.GetValue(kRegistryKeyUseLightTheme);
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore IDE0079 // Remove unnecessary suppression
                    if (value != null)
                    {
                        int lightThemeEnabled = (int)value;
                        return lightThemeEnabled == 0; // 0 = Dark mode, 1 = Light mode
                    }
                }
            }
            catch
            {
                // If error, assume light mode (safe default)
            }
            return false;
        }

        private void LoadTheme(Theme newTheme, bool isSetting = false)
        {
            switch (newTheme)
            {
                case Theme.Light:
                    ApplyLightMode();
                    
                    break;
                case Theme.Dark:
                    ApplyDarkMode();
                    break;
                default:
                    //Auto detect Windows Dark Mode
                    if (IsWindowsInDarkMode())
                    {
                        ApplyDarkMode();
                    }
                    else
                    {
                        ApplyLightMode();
                    }
                    break;
            }

            if (isSetting)
            {
                ThemeIsSystemCheckBox.IsChecked = newTheme == Theme.System;
                ThemeIsDarkCheckBox.IsChecked =  newTheme == Theme.Dark;
                ThemeIsLightCheckBox.IsChecked = newTheme == Theme.Light;
            }
        }

        private void ApplyThemeUri(string themeUri)
        {
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(themeUri, UriKind.Absolute) });
        }

        private void ApplyLightMode()
        {
            ApplyThemeUri(kLightThemeUri);
        }

        private void ApplyDarkMode()
        {
            ApplyThemeUri(kDarkThemeUri);
        }


        public MainWindow()
        {
            InitializeComponent();
            LoadTheme((Theme)Properties.Settings.Default.Theme, true);

            // Load the saved setting
            IgnoreRenameWarningCheckBox.IsChecked = Properties.Settings.Default.IgnoreRenameWarning;

            // Add event handler for the checkbox to save changes immediately
            IgnoreRenameWarningCheckBox.Checked += IgnoreRenameWarningCheckBox_CheckedChanged;
            IgnoreRenameWarningCheckBox.Unchecked += IgnoreRenameWarningCheckBox_CheckedChanged;

            ThemeIsSystemCheckBox.Checked += ThemeIsSystemCheckBox_Checked;
            ThemeIsDarkCheckBox.Checked += ThemeIsDarkCheckBox_Checked;
            ThemeIsLightCheckBox.Checked += ThemeIsLightCheckBox_Checked;

            var folderIcon = IconHelper.GetFolderIcon();
            if (folderIcon == null)
            {
                BrowseButton.Content = "Select Folder";
            }
            else
            {
                var icon = new System.Windows.Controls.Image
                {
                    Source = folderIcon,
                    Width = 25,
                    Height = 25
                };
                BrowseButton.Content = icon;
            }

            // Allow drag-and-drop
            this.AllowDrop = true;
            this.Drop += Window_Drop;

            // New! Update preview automatically on text changes
            FolderPathTextBox.TextChanged += Input_Changed;
            BaseNameTextBox.TextChanged += Input_Changed;
            SuffixTextBox.TextChanged += Input_Changed;
            SkipHiddenCheckBox.Checked += Input_Changed;
            SkipHiddenCheckBox.Unchecked += Input_Changed;
            LeadingZerosCheckBox.Checked += Input_Changed;
            LeadingZerosCheckBox.Unchecked += Input_Changed;
            KeepExtensions.Checked += Input_Changed;
            KeepExtensions.Unchecked += Input_Changed;
        }

        // Event handler for checkbox changes
        private void IgnoreRenameWarningCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Save the setting immediately when changed
            Properties.Settings.Default.IgnoreRenameWarning = IgnoreRenameWarningCheckBox.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void ThemeIsSystemCheckBox_Checked(object sender, EventArgs e)
        {
            SetTheme(Theme.System);
        }
        private void ThemeIsDarkCheckBox_Checked(object sender, EventArgs e)
        {
            SetTheme(Theme.Dark);
        }
        private void ThemeIsLightCheckBox_Checked(object sender, EventArgs e)
        {
            SetTheme(Theme.Light);
        }

        private void SetTheme(Theme newTheme)
        {
            // Save the setting immediately when changed
            Properties.Settings.Default.Theme = (int)newTheme;
            Properties.Settings.Default.Save();

            _theme = newTheme;
            switch (_theme)
            {
                case Theme.Light:
                    ThemeIsSystemCheckBox.IsChecked = false;
                    ThemeIsDarkCheckBox.IsChecked = false;
                    break;
                case Theme.Dark:
                    ThemeIsSystemCheckBox.IsChecked = false;
                    ThemeIsLightCheckBox.IsChecked = false;
                    break;
                default:
                    ThemeIsDarkCheckBox.IsChecked = false;
                    ThemeIsLightCheckBox.IsChecked = false;
                    break;
            }
            LoadTheme(_theme);
        }

        private void Input_Changed(object sender, EventArgs e)
        {
            GeneratePreview();
        }
        private void KeepExtensions_Unchecked(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void KeepExtensions_Checked(object sender, EventArgs e)
        {
            GeneratePreview();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Select Folder",
                ValidateNames = false
            };

            if (dialog.ShowDialog() == true)
            {
                string directoryPath = Path.GetDirectoryName(dialog.FileName);
                _selectedFolder = string.IsNullOrEmpty(directoryPath) ? string.Empty : directoryPath;
                FolderPathTextBox.Text = _selectedFolder;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string folder = FolderPathTextBox.Text.Trim();
            string baseName = BaseNameTextBox.Text;
            string suffix = SuffixTextBox.Text;

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show("Please select a valid folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Skip the warning if the checkbox is checked
            if (IgnoreRenameWarningCheckBox.IsChecked == false)
            {
                var warningMessage = MessageBox.Show("Renaming is irreversible change, do you really want to continue ?", "Warning renaming is irreversible, continue ?", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                if (warningMessage != MessageBoxResult.OK)
                    return;
            }

            var files = new DirectoryInfo(folder).GetFiles()
                        .Where(f => !string.IsNullOrEmpty(f.Name))
                        .ToList();

            if (SkipHiddenCheckBox.IsChecked == true)
            {
                files = [.. files.Where(f => (f.Attributes & FileAttributes.Hidden) == 0 && (f.Attributes & FileAttributes.System) == 0)];
            }

            int total = files.Count;
            int counter = 0;
            int padLength = LeadingZerosCheckBox.IsChecked == true ? total.ToString().Length : 0;

            foreach (var file in files)
            {
                counter++;
                string ext = KeepExtensions.IsChecked == true ? file.Extension : string.Empty;
                string number = LeadingZerosCheckBox.IsChecked == true
                                ? counter.ToString().PadLeft(padLength, '0')
                                : counter.ToString();

                string newName = $"{baseName}{number}{suffix}{ext}";
                string newPath = Path.Combine(folder, newName);

                try
                {
                    file.MoveTo(newPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming {file.Name}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            StatusTextBlock.Text = $"Done! Renamed {counter} files.";
            Invoke(() => { StatusTextBlock.Text = string.Empty; }, 5f);
            MessageBox.Show(StatusTextBlock.Text, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

            GeneratePreview();
        }

        public static async void Invoke(Action action, float delaySeconds = 0f)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (delaySeconds != 0f)
                await Task.Delay((int)(Math.Abs(delaySeconds) * 1000));

            if (Application.Current.Dispatcher.CheckAccess())
                action();
            else
                Application.Current.Dispatcher.Invoke(action);
        }

        public void ClearStatusTextBlock()
        {
            StatusTextBlock.Text = string.Empty;
        }

        private void GeneratePreview()
        {
            if (PreviewListBox != null && PreviewListBox.Items != null && PreviewListBox.Items.Count > 0)
                PreviewListBox.Items.Clear();

            string folder = FolderPathTextBox.Text.Trim();
            string baseName = BaseNameTextBox.Text;
            string suffix = SuffixTextBox.Text;

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            var files = new DirectoryInfo(folder).GetFiles()
                        .Where(f => !string.IsNullOrEmpty(f.Name))
                        .ToList();

            if (SkipHiddenCheckBox.IsChecked == true)
            {
                files = [.. files.Where(f => (f.Attributes & FileAttributes.Hidden) == 0 && (f.Attributes & FileAttributes.System) == 0)];
            }

            int total = files.Count;
            int padLength = LeadingZerosCheckBox.IsChecked == true ? total.ToString().Length : 0;
            int startingAt = 1;
            foreach (var file in files.Select((value, index) => new { value, index }))
            {
                int index = file.index + startingAt; // Adjust index to start from 1 ?????- should it be configrable for user -??????
                string ext = KeepExtensions.IsChecked == true ? file.value.Extension : string.Empty;
                string number = LeadingZerosCheckBox.IsChecked == true
                                ? index.ToString().PadLeft(padLength, '0')
                                : index.ToString();

                string newName = $"{baseName}{number}{suffix}{ext}";

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                PreviewListBox.Items.Add($"{file.value.Name} ➔ {newName}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        public void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] dropped = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                if (Directory.Exists(dropped[0]))
                {
                    _selectedFolder = dropped[0];
                    FolderPathTextBox.Text = _selectedFolder;
                    GeneratePreview();
                }
            }
        }
    }
}
