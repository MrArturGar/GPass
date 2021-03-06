﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Input;

namespace GPass
{
    class Crypto
    {
        public byte[] Encrypt(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = EncryptAES(_password, _key, _data);
            buffer = EncryptDES(buffer);
            return ConversionBytes(_password, buffer, true);
        }
        public string Decrypt(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = ConversionBytes(_password, _data, false);
            buffer = DecryptDES(buffer);
            return DecryptAES(_password, _key, buffer);
        }
        public byte[] EncryptPass(byte[] _key, byte[] _password, string _data)
        {
            byte[] buffer = EncryptAES(_password, _key, ConvertStringToBytes(_data));
            return ConversionBytes(GetSHA512Hash(_key), buffer, true);
        }
        public string DecryptPass(byte[] _key, byte[] _password, byte[] _data)
        {
            byte[] buffer = ConversionBytes(GetSHA512Hash(_key), _data, false);
            return DecryptAES(_password, _key, buffer);
        }

        public byte[] GetSHA256Hash(string _data)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] sourseBytes = Encoding.UTF8.GetBytes(_data);
                return hash.ComputeHash(sourseBytes);
            }
        }

        private byte[] GetSHA512Hash(byte[] _data)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return hash.ComputeHash(_data);
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

        private byte[] ConversionBytes(byte[] _mask, byte[] _data, bool _encrypt)
        {
            byte[] encryptData = new byte[_data.Length];

            for (int i = 0, j = 0; i < _data.Length; i++, j++)
            {
                j = j >= _mask.Length ? 0 : j;

                if (_encrypt)
                    encryptData[i] = (byte)(_data[i] + _mask[j]);
                else
                    encryptData[i] = (byte)(_data[i] - _mask[j]);

            }

            return encryptData;
        }
        private byte[] EncryptDES(byte[] _data)
        {
            using (DES des = DES.Create())
            {
                des.Key = new byte[] { 0x8f, 0x52, 0xa5, 0xbd, 0xcc, 0x5d, 0x89, 0x99};
                des.IV = new byte[] { 0x21, 0x10, 0x19, 0x98, 0x23, 0x03, 0x19, 0x98 };

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = des.CreateEncryptor(des.Key, des.IV);

                return PerformCryptography(_data, encryptor);
            }
        }
        private byte[] DecryptDES(byte[] _data)
        {
            using (var des = DES.Create())
            {
                des.Key = new byte[] { 0x8f, 0x52, 0xa5, 0xbd, 0xcc, 0x5d, 0x89, 0x99 };
                des.IV = new byte[] { 0x21, 0x10, 0x19, 0x98, 0x23, 0x03, 0x19, 0x98 };

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
            return Encoding.Default.GetString(_byte);
        }
        private byte[] ConvertStringToBytes(string _string)
        {
            return Encoding.Default.GetBytes(_string);
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
                //aes.Padding = PaddingMode.PKCS7;
                aes.Key = _key;
                aes.IV = _iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                return PerformCryptography(_data, encryptor);
            }

        }

        private string DecryptAES(byte[] _key, byte[] _iv, byte[] _data)
        {
            using (var aes = Aes.Create())
            {
                //aes.KeySize = 128;
                //aes.BlockSize = 128;
                //aes.Padding = PaddingMode.PKCS7;

                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return ConvertBytesToString(PerformCryptography(_data, decryptor));
                }

            }
        }

    }
}
