using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Xml;

namespace GPass
{
    /// <summary>
    /// Логика взаимодействия для LineElement.xaml
    /// </summary>
    [Serializable]
    public partial class LineElement : UserControl
    {
        private int Index = -1;
        public LineElement(int _indexTitle)
        {
            InitializeComponent();
            Index = _indexTitle;
            textBoxName.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(textBox_CopyName), true);
            textBoxData.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(textBox_CopyData), true);
        }

        public bool encrypt = false;
        public bool multiline = false;
        private string forEncryptText = "••••••••••••••••";
        private bool editable = false;
        public void SetData(string _name, string _data)
        {
            textBoxName.Text = _name;
            textBoxData.Text = _data;
            LoadSetting();
            ButtonEdit_Click(null, null);
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            Window window = GetMainWindowLink();
            if (string.Equals(ButtonEdit.Content, "✎"))
            {
                editable = true;
                textBoxName.IsReadOnly = false;
                textBoxData.IsReadOnly = false;
                textBoxName.Background = Brushes.White;
                textBoxData.Background = Brushes.White;
                ButtonEdit.Content = "✓";

                if (encrypt)
                {
                    if (textBoxData.Tag != null)
                        textBoxData.Text = (window as MainWindow).GetPass((string)textBoxData.Tag);
                    //textBoxData.Text = (window as MainWindow).GetPass(Encoding.UTF8.GetBytes((string)textBoxData.Tag));
                    else
                        textBoxData.Text = "";
                }
            }
            else
            {
                editable = false;
                textBoxName.IsReadOnly = true;
                textBoxData.IsReadOnly = true;
                textBoxName.Background = null;
                textBoxData.Background = null;
                ButtonEdit.Content = "✎";

                if (encrypt)
                {
                    textBoxData.Tag = ((window as MainWindow).SetPass(textBoxData.Text));
                    //textBoxData.Tag = Encoding.UTF8.GetString((window as MainWindow).SetPass(Encoding.UTF8.GetBytes(textBoxData.Text)));
                    textBoxData.Text = forEncryptText;
                }
            SaveXML();
            }
        }

        public void AddElement()
        {
            ButtonEdit_Click(null, null);
        }

        private void SaveXML()
        {
            var window = GetMainWindowLink();
            XmlElement line = Core.doc.CreateElement("Line");
            line.SetAttribute("Id", Index.ToString());

            XmlElement setting = Core.doc.CreateElement("Setting");
            setting.SetAttribute("Encrypt", encrypt.ToString());
            setting.SetAttribute("Multiline", multiline.ToString());
            line.AppendChild(setting);

            XmlElement name = Core.doc.CreateElement("Name");
            name.SetAttribute("Text", textBoxName.Text);
            line.AppendChild(name);

            XmlElement data = Core.doc.CreateElement("Data");
            if (encrypt)
                data.InnerText = (string) textBoxData.Tag;
            else
                data.InnerText = textBoxData.Text;
            line.AppendChild(data);
            (window as MainWindow).SaveLineInItem(line, Index);
        }

        public void LoadXML(XmlElement _root)
        {
            foreach(XmlElement element in _root.ChildNodes)
            {
                switch (element.Name)
                {
                    case "Setting":
                        {
                            encrypt = Convert.ToBoolean(element.GetAttribute("Encrypt"));
                            multiline = Convert.ToBoolean(element.GetAttribute("Multiline"));
                            LoadSetting();
                            break;
                        }
                    case "Name":
                        {
                            textBoxName.Text = element.GetAttribute("Text");
                            break;
                        }
                    case "Data":
                        {
                            if (encrypt)
                                textBoxData.Tag = element.InnerText;
                            else
                                textBoxData.Text = element.InnerText;
                            break;
                        }
                }
            }
        }

        private void LoadSetting()
        {
            if (encrypt)
            {
                textBoxData.Text = forEncryptText;
            }

            if (multiline)
            {
                textBoxData.AcceptsReturn = true;
                textBoxData.AcceptsTab = true;
                textBoxData.TextWrapping = TextWrapping.Wrap;
                textBoxData.Width = 430;
                textBoxData.Height = 100;
                textBoxName.Width += 200;
            }

        }
        private Window GetMainWindowLink()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    return window;
                }
            }
            return null;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var window = GetMainWindowLink();
            (window as MainWindow).SaveLineInItem(null, Index);
            (window as MainWindow).wrapPanel.Children.Remove(this);
        }

        private void textBox_CopyName(object sender, MouseButtonEventArgs e)
        {
            string buffer = textBoxName.Text;
            Clipboard.SetText(buffer);
            Window window = GetMainWindowLink();
            (window as MainWindow).SetLabelStatus("Скопировано");
        }

        private void textBox_CopyData(object sender, MouseButtonEventArgs e)
        {
            if (!editable)
            {
                Window window = GetMainWindowLink();
                string buffer = "";
                if (encrypt)
                {
                    //byte[] pass = Encoding.UTF8.GetBytes((string)textBoxData.Tag);
                    //byte[] pass =Convert.FromBase64String((string)textBoxData.Tag);
                    //buffer = (window as MainWindow).GetPass(pass);
                    buffer = (window as MainWindow).GetPass((string)textBoxData.Tag);
                }
                else
                    buffer = textBoxData.Text;

                Clipboard.SetText(buffer);
                (window as MainWindow).SetLabelStatus("Скопировано");
            }
        }
    }
}
