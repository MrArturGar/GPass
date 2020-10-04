using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GPass
{
    /// <summary>
    /// Логика взаимодействия для JoinWindow.xaml
    /// </summary>
    public partial class JoinWindow : Window
    {
        private string MainPath = AppDomain.CurrentDomain.BaseDirectory;
        public JoinWindow()
        {
            InitializeComponent();
        }

        int a, b, c;

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (IsCaptcha())
            {
                Core core = new Core();
                MainWindow window = new MainWindow();
                window.Show();
                window.LoadItems(fileComboBox.Text, core.GetLoginHash(loginBox.Password), core.GetPasswordHash(passwordBox.Password));
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы не прошли проверку \"Антибот\"!");
            }

            GenerateTextCaptcha();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckDataBases();
            GenerateTextCaptcha();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileComboBox.SelectedIndex == -1)
                buttonStart.IsEnabled = false;
            else
                buttonStart.IsEnabled = true;
        }

        private void CheckDataBases()
        {
            string[] files = Directory.GetFiles(MainPath, "*.gb");

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo info = new FileInfo(files[i]);
                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = info.Name
                };
                fileComboBox.Items.Add(item);
            }

            if (fileComboBox.Items.Count == 1)
                fileComboBox.SelectedIndex = 0;
            else
                fileComboBox.Text = "Select file";

            ComboBox_SelectionChanged(null, null);
        }

        private void buttonNewBase_Click(object sender, RoutedEventArgs e)
        {
            if (IsCaptcha())
            {
                Core core = new Core();
                MainWindow window = new MainWindow();
                window.Show();
                window.NewBase(fileComboBox.Text, core.GetLoginHash(loginBox.Password), core.GetPasswordHash(passwordBox.Password));
                this.Close();
            }
            else
            {
                MessageBox.Show("Вы не прошли проверку \"Антибот\"!");
            }

            GenerateTextCaptcha();
        }

        private void GenerateTextCaptcha()
        {
            Random rnd = new Random();
            a = rnd.Next(-10, 10);
            b = rnd.Next(-10, 10);
            c = rnd.Next(0, 2);
            string d = "";

            switch (c)
            {
                case 0:
                    d = a + "+" + b + "=";
                    break;
                case 1:
                    d = a + "-" + b + "=";
                    break;
                case 2:
                    d = a + "•" + b + "=";
                    break;
            }
            captchaTextBlock.Text = d;
        }

        private bool IsCaptcha()
        {
            int d = 0;
            switch (c)
            {
                case 0:
                    d = a + b;
                    break;
                case 1:
                    d = a - b;
                    break;
                case 2:
                    d = a * b;
                    break;
            }

            return string.Equals(captchaTextBox.Text, d.ToString());
        }
    }
}
