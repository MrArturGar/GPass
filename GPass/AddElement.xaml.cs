using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace GPass
{
    /// <summary>
    /// Логика взаимодействия для AddElement.xaml
    /// </summary>
    public partial class AddElement : Window
    {
        public AddElement()
        {
            InitializeComponent();
        }

        private void textBoxTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTitle.Text != "")
                buttonAdd.IsEnabled = true;
            else
                buttonAdd.IsEnabled = false;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = GetMainWindowLink();
            XmlElement element = Core.doc.CreateElement("Item");
            element.SetAttribute("Title", textBoxTitle.Text);
            (window as MainWindow).CreateItem(element, textBoxTitle.Text);
            this.Close();

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
    }
}
