using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Xml;

namespace GPass
{
    class Core
    {
        private string MainPath = AppDomain.CurrentDomain.BaseDirectory;
        private string LogFile = "GTemp.log";
        public static XmlDocument doc = new XmlDocument();

        private string OpenFile(string _fileName, byte[] _login, byte[] _password)
        {
            byte[] file = File.ReadAllBytes(MainPath + _fileName);
            Crypto cr = new Crypto();
            return cr.Decrypt(_login, _password, file);
        }
        private void SaveFile(string _fileName, byte[] _login, byte[] _password, string _data)
        {
            Crypto cr = new Crypto();
            byte[] buffer = Encoding.Default.GetBytes(_data);
            buffer = cr.Encrypt(_login, _password, buffer);
            byte[] fileOld = File.ReadAllBytes(MainPath + _fileName);

            if (!buffer.SequenceEqual(fileOld))
            {
                var diagResult = MessageBox.Show("Сохранить базу?", "Внимание!", MessageBoxButton.YesNo);
                if (diagResult == MessageBoxResult.Yes)
                    File.WriteAllBytes(MainPath + _fileName, buffer);
            }
        }

        public byte[] GetPasswordHash(string _password)
        {
            Crypto cr = new Crypto();
            return cr.GetSHA256Hash(_password);
        }
        public byte[] GetLoginHash(string _login)
        {
            Crypto cr = new Crypto();
            return cr.GetMD5Hash(_login);
        }

        public XmlElement ParseFile(string _fileName, byte[] _login, byte[] _password)
        {
            string buffer = OpenFile(_fileName, _login, _password);
            doc.LoadXml(buffer);

            return doc.DocumentElement;
        }
        public bool GenerateFile(string _filename, byte[] _login, byte[] _password, XmlElement _element)
        {
            try
            {
                if (!doc.HasChildNodes)
                {
                    //    XmlElement el = (XmlElement) doc.ChildNodes[0];
                    //    doc.ChildNodes[0] = (XmlNode)_element;
                    //}
                    //else
                    //{
                    doc.AppendChild(_element);
                }
                else
                {
                    doc.ReplaceChild(_element, doc.FirstChild);
                }

                StringWriter data = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(data);
                doc.WriteTo(xw);
                SaveFile(_filename, _login, _password, data.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void AddLog(string _data)
        {
            File.AppendAllText(MainPath + LogFile, DateTime.Now + ": " + _data + Environment.NewLine);
        }

        public string DencryptPass(byte[] _data, byte[] _login, byte[] _password)
        {
            Crypto cr = new Crypto();
            return cr.DecryptPass(cr.GetMD5Hash(Encoding.UTF8.GetString(_password)), cr.GetSHA256Hash(Encoding.UTF8.GetString(_login)), _data);
        }
        public byte[] EncryptPass(string _data, byte[] _login, byte[] _password)
        {
            Crypto cr = new Crypto();
            return cr.EncryptPass(cr.GetMD5Hash(Encoding.UTF8.GetString(_password)), cr.GetSHA256Hash(Encoding.UTF8.GetString(_login)), _data);
        }
    }
}
