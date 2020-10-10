using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace GPass
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //TestMethod();
        }

        //void TestMethod()
        //{
        //    Core core = new Core();
        //    string file = "testFile.gb";
        //    byte[] login = core.GetLoginHash("Это логин");
        //    byte[] password = core.GetPasswordHash("Это пароль!!!123");
        //    string[] data = new string[] { "Это данные для шифрования. Ну что, посмотрим.","Hello world!!", "Hello мир 12321412" };
        //    core.SaveFile(file, login, password, data);
        //    data = core.OpenFile(file,login, password);
        //}

        private string FileName = "New file";
        private byte[] Login;
        private byte[] Password;

        public void LoadItems(string _fileName, byte[] _login, byte[] _password)
        {
            Core core = new Core();
            try
            {
                Login = _login;
                Password = _password;
                fileNameTextBox.Text = _fileName;
                XmlElement root = core.ParseFile(_fileName, _login, _password);
                foreach (XmlElement element in root.ChildNodes)
                {
                    CreateItem(element, element.GetAttribute("Title"));
                }
            }
            catch (Exception ex)
            {
                core.AddLog(ex.ToString());
                MessageBox.Show("Не удалось открыть базу.", "Ошибка");
                Environment.Exit(0);
            }
        }

        private void SetNewFileName(string _fileName)
        {
            if (_fileName == "New file")
            {
                string name = "NewBase";
                int index = 1;

                while (File.Exists(name + index.ToString() + ".gb"))
                {
                    index++;
                }
                fileNameTextBox.Text = name + index.ToString() + ".gb";
                FileName = fileNameTextBox.Text;
            }
            else
            {
                fileNameTextBox.Text = _fileName;
                FileName = fileNameTextBox.Text;
            }
        }

        public void CreateItem(XmlElement _element, string _title)
        {
            ListBoxItem item = new ListBoxItem()
            {
                Content = _title,
                Background = Brushes.Gray,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Tag = _element
            };
            listBox.Items.Add(item);
        }
        public void NewBase(string _fileName, byte[] _login, byte[] _password)
        {
            Core core = new Core();
            try
            {
                SetNewFileName(_fileName);
                Login = _login;
                Password = _password;
            }
            catch (Exception ex)
            {
                core.AddLog(ex.ToString());
            }
        }

        private void listBox_Loaded(object sender, RoutedEventArgs e)
        {
            listBox_SelectionChanged(null, null);
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            wrapPanel.Children.Clear();
            if (listBox.SelectedIndex != -1)
            {
                menuDelete.IsEnabled = true;
                menuAddElement.IsEnabled = true;
                LoadLines((ListBoxItem)listBox.SelectedItem);
            }
            else
            {
                menuDelete.IsEnabled = false;
                menuAddElement.IsEnabled = false;
            }
        }

        private void LoadLines(ListBoxItem item)
        {
            XmlElement root = (XmlElement)item.Tag;
            foreach (XmlElement line in root.ChildNodes)
            {
                LineElement control = new LineElement(Convert.ToInt32(line.GetAttribute("Id")));
                control.LoadXML(line);
                wrapPanel.Children.Add(control);
            }
        }

        private void CreateNewLine(string _setting)
        {
            LineElement element = new LineElement(-1);
            switch (_setting)
            {
                case "Encrypt":
                    element.encrypt = true;
                    break;
                case "Multiline":
                    element.multiline = true;
                    break;
            }
            element.SetData("", "");
            wrapPanel.Children.Add(element);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Regex.IsMatch(FileName, @"^\w*.gb$"))
            {
                if (Regex.IsMatch(FileName, @"^\w*$"))
                { FileName += ".gb"; }
                else
                { 
                    SetNewFileName("New file"); 
                }
            }




                Core core = new Core();
            try
            {
                if (listBox.Items.Count != 0)
                    SaveItems();
            }
            catch (Exception ex)
            {
                core.AddLog(ex.ToString());
                MessageBox.Show("Не удалось сохранить базу.", "Ошибка");
            }

        }

        private void menuAddTitle_Click(object sender, RoutedEventArgs e)
        {
            AddElement element = new AddElement();
            element.ShowDialog();
        }

        private void menuAddElement_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            CreateNewLine((string) item.Tag);
        }

        public void SaveLineInItem(XmlElement _element, int _index)
        {
            ListBoxItem item = (ListBoxItem)listBox.Items[listBox.SelectedIndex];
            XmlElement XMLItem = (XmlElement)item.Tag;
            XmlNodeList lines = XMLItem.GetElementsByTagName("Line");

            if (_index != -1)
                for (int i = 0; i < lines.Count; i++)
                {
                    XmlElement line = (XmlElement)lines[i];
                    if (line.GetAttribute("Id") == _index.ToString())
                    {
                        if (_element == null)
                        {
                            XMLItem.RemoveChild((XmlElement)lines[i]);
                        }
                        else
                            XMLItem.ReplaceChild(_element, (XmlElement)lines[i]);
                        break;
                    }
                }
            else
            {
                _element.SetAttribute("Id", lines.Count.ToString());
                XMLItem.AppendChild(_element);
            }
        }

        private void SaveItems()
        {
            Core core = new Core();
            XmlElement root = Core.doc.CreateElement("DataBase");

            foreach (ListBoxItem item in listBox.Items)
            {
                XmlElement element = (XmlElement)item.Tag;
                root.AppendChild(element);
            }

                core.GenerateFile(FileName, Login, Password, root);
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)listBox.SelectedItem;
            var diag = MessageBox.Show($"Вы уверены что хотите удалить данные \"{item.Content}\"?", "Внимание!", MessageBoxButton.YesNo);

            if (diag == MessageBoxResult.Yes)
                listBox.Items.Remove(listBox.SelectedItem);
        }

        DispatcherTimer timer;
        public void SetLabelStatus(string _text)
        {
            textBlockStatus.Text = "Статус: " + _text;
            textBlockStatus.Background = Brushes.Cyan;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += ClearStatusByTimer;
            timer.Start();
        }
        private void ClearStatusByTimer(object sender, EventArgs e)
        {
            textBlockStatus.Text = "Статус:";
            textBlockStatus.Background = null;
        }

        public string GetPass(string _data)
        {
            Core core = new Core();
            byte[] dataBytes = Convert.FromBase64String(_data);
            return core.DencryptPass(dataBytes, Login, Password);
        }
        public string SetPass(string _data)
        {
            Core core = new Core();
            return Convert.ToBase64String(core.EncryptPass(_data, Login, Password));
        }

        private void fileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FileName = fileNameTextBox.Text;
        }
    }
}
