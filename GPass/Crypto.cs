using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace GPass
{
    class Crypto
    {
        public byte[] Encrypt(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = EncryptAES(_password, _key, _data);
            return ConversionBytes(_password, buffer, true);
        }
        public string Decrypt(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = ConversionBytes(_password, _data, false);
            buffer = DecryptAES(_password, _key, buffer);

            return ConvertBytesToString(buffer);
        }
        public byte[] EncryptPass(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = EncryptDES(_password, _key, _data);
            return ConversionBytes(_password, buffer, true);
        }
        public string DecryptPass(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = ConversionBytes(_password, _data, false);
            buffer = DecryptDES(_password, _key, buffer);

            return ConvertBytesToString(buffer);
        }

        public byte[] GetSHAHash(string _data)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] sourseBytes = Encoding.UTF8.GetBytes(_data);
                return hash.ComputeHash(sourseBytes);
            }
        }

        public byte[] GetMD5Hash(string _data)
        {
            using (MD5 hash = MD5.Create())
            {
                byte[] sourseBytes = Encoding.UTF8.GetBytes(_data);
                return hash.ComputeHash(sourseBytes);
            }
        }

        private byte[] ConversionBytes(byte[] _key, byte[] _data, bool _encrypt)
        {
            byte[] encryptData = new byte[_data.Length];

            for (int i = 0, j = 0; i < _data.Length; i++, j++)
            {
                j = j >= _key.Length ? 0 : j;

                if (_encrypt)
                    encryptData[i] = (byte)(_data[i] + _key[j]);
                else
                    encryptData[i] = (byte)(_data[i] - _key[j]);

            }

            return encryptData;
        }

        private byte[] EncryptAES(byte[] _key, byte[] _iv, byte[] _data)
        {
            // Check arguments. 
            if (_data == null || _data.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException("Key");

            using (Aes aes = Aes.Create())
            {
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = _key;
                aes.IV = _iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                return PerformCryptography(_data, encryptor);
            }

        }

        private byte[] EncryptDES(byte[] _key, byte[] _iv, byte[] _data)
        {
            // Check arguments. 
            if (_data == null || _data.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (_key == null || _key.Length <= 0)
                throw new ArgumentNullException("Key");
            using (DES des = DES.Create())
            {
                des.Padding = PaddingMode.PKCS7;
                des.Key = _key;
                des.IV = _iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = des.CreateEncryptor(des.Key, des.IV);

                return PerformCryptography(_data, encryptor);
            }
        }
        private byte[] DecryptAES(byte[] _key, byte[] _iv, byte[] _data)
        {
            using (var aes = AesManaged.Create())
            {
                //aes.KeySize = 128;
                //aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;

                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return PerformCryptography(_data, decryptor);
                }

            }
        }
        private byte[] DecryptDES(byte[] _key, byte[] _iv, byte[] _data)
        {
            using (var des = DES.Create())
            {
                //aes.KeySize = 128;
                //aes.BlockSize = 128;
                des.Padding = PaddingMode.PKCS7;

                des.Key = _key;
                des.IV = _iv;

                using (var decryptor = des.CreateDecryptor(des.Key, des.IV))
                {
                    return PerformCryptography(_data, decryptor);
                }

            }
        }
        private byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();

                return ms.ToArray();
            }
        }
        private string ConvertBytesToString(byte[] _byte)
        {
            return Encoding.UTF8.GetString(_byte);
        }
        private byte[] ConvertStringToBytes(string _string)
        {
            return Encoding.UTF8.GetBytes(_string);
        }
    }
}
