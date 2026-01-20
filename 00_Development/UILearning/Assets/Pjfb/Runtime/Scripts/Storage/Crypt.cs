using System;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.IO.Compression;

namespace Pjfb.Storage {

    public class CryptoStreamWrapper : CryptoStream
    {
        private readonly Aes aes;
    
        public CryptoStreamWrapper(Stream stream, ICryptoTransform transform, CryptoStreamMode mode, Aes aes) : base(stream, transform, mode) 
        {
            this.aes = aes;
        }
    
        protected override void Dispose(bool disposing) 
        {
            // finalizeでDisposeが呼ばれた(disposing=false)場合、順番によって参照に不整合が生じる可能性があるためチェック(順番が定められていない)
            if (disposing == true)
            {
                aes.Dispose();
            }
            base.Dispose(disposing);
        }
    }
    
    public static class Crypt {
        
        // AES設定値
        private const int aesKeySize = 256;
        private const int aesBlockSize = 128;

        /// <summary>
        /// 暗号化
        /// </summary>
        static public byte[] Encrypt(string plainText, string IV, string key) {
            var keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            var IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            return Encrypt( plainText, IVByte, keyByte );
        }

        /// <summary>
        /// 暗号化
        /// </summary>
        static public byte[] Encrypt(byte[] byteArray, string IV, string key) {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            return Encrypt( byteArray, IVByte, keyByte );
        }

        /// <summary>
        /// 暗号化
        /// </summary>
        static public byte[] Encrypt(string plainText, byte[] IV, byte[] key) {
            if ( string.IsNullOrEmpty(plainText) ) {
                throw new System.ArgumentNullException("plainText is empty");
            }

            if (key == null || key.Length <= 0) {
                throw new System.ArgumentNullException("key is empty");
            }

            if (IV == null || IV.Length <= 0) {
                throw new System.ArgumentNullException("IV is empty");
            }

            byte[] encrypted = null;
            using (Aes aes = Aes.Create()) {
                aes.KeySize = aesKeySize;
                aes.BlockSize = aesBlockSize;
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
        
        /// <summary>
        /// 暗号化
        /// </summary>
        static public byte[] Encrypt(byte[] byteArray, byte[] IV, byte[] key) {
            if ( byteArray == null || byteArray.Length <= 0 ) {
                throw new System.ArgumentNullException("byteArray is empty");
            }

            if (key == null || key.Length <= 0) {
                throw new System.ArgumentNullException("key is empty");
            }

            if (IV == null || IV.Length <= 0) {
                throw new System.ArgumentNullException("IV is empty");
            }

            byte[] encrypted = null;
            using (Aes aes = Aes.Create()) {
                aes.KeySize = aesKeySize;
                aes.BlockSize = aesBlockSize;
                aes.Key = key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt)) {
                            swEncrypt.Write(byteArray);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }
        
        /// <summary>
        /// 複合
        /// </summary>
        static public string Decrypt(byte[] encryptData, string IV, string key ) {
            var keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            var IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            return Decrypt( encryptData, IVByte, keyByte );
        }

        /// <summary>
        /// 複合
        /// </summary>
        static public string Decrypt(byte[] encryptData, byte[] IV, byte[] key) {
            if (encryptData == null || encryptData.Length <= 0) {
                throw new System.ArgumentNullException("encryptData is empty");
            }

            if (key == null || key.Length <= 0) {
                throw new System.ArgumentNullException("key is empty");
            }

            if (IV == null || IV.Length <= 0) {
                throw new System.ArgumentNullException("IV is empty");
            }

            string plaintext = null;
            using (Aes aes = Aes.Create()) {
                aes.KeySize = aesKeySize;
                aes.BlockSize = aesBlockSize;
                aes.Key = key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptData)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        
        /// <summary>
        /// 複合
        /// </summary>
        public static byte[] DecryptToBytes(byte[] encryptData, string IV, string key ) {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            return DecryptToBytes( encryptData, IVByte, keyByte );
        }
        
        /// <summary>
        /// 複合
        /// </summary>
        public static byte[] DecryptToBytes(byte[] encryptData, byte[] IV, byte[] key) {
            if (encryptData == null || encryptData.Length <= 0) {
                throw new System.ArgumentNullException("encryptData is empty");
            }

            if (key == null || key.Length <= 0) {
                throw new System.ArgumentNullException("key is empty");
            }

            if (IV == null || IV.Length <= 0) {
                throw new System.ArgumentNullException("IV is empty");
            }

            byte[] plainBytes = null;
            using (Aes aes = Aes.Create()) {
                aes.KeySize = aesKeySize;
                aes.BlockSize = aesBlockSize;
                aes.Key = key;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptData)) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (BinaryReader srDecrypt = new BinaryReader(csDecrypt)) {
                            plainBytes = srDecrypt.ReadBytes(encryptData.Length);
                        }
                    }
                }
            }

            return plainBytes;
        }

        /// <summary>
        /// CryptoStream取得（書き込み用）
        /// 注：必ずusing等で解放すること
        /// </summary>
        static public CryptoStreamWrapper GetCryptoStreamWrite(string filePath, string IV, string key) {
            if (string.IsNullOrEmpty(filePath)) {
                throw new System.ArgumentNullException("filePath is empty");
            }

            if (string.IsNullOrEmpty(key)) {
                throw new System.ArgumentNullException("key is empty");
            }

            if (string.IsNullOrEmpty(IV)) {
                throw new System.ArgumentNullException("IV is empty");
            }
            
            byte[] KeyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            
            Aes aes = Aes.Create();
            aes.KeySize = aesKeySize;
            aes.BlockSize = aesBlockSize;
            aes.Key = KeyByte;
            aes.IV = IVByte;

            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            CryptoStreamWrapper cryptoStream = new CryptoStreamWrapper(fileStream, aes.CreateEncryptor(), CryptoStreamMode.Write, aes);
            return cryptoStream;
        }
        
        /// <summary>
        /// CryptoStream読み込み
        /// 注：必ずusing等で解放すること
        /// </summary>
        static public CryptoStreamWrapper ReadCryptoStream(string path, string IV, string key) 
        {
            byte[] keyByte = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] IVByte = System.Text.Encoding.UTF8.GetBytes(IV);
            
            try
            {
                Aes aes = Aes.Create();
                aes.KeySize = aesKeySize;
                aes.BlockSize = aesBlockSize;
                aes.Key = keyByte;
                aes.IV = IVByte;

                if(!File.Exists(path)) 
                {
                    throw new System.IO.FileNotFoundException(path);
                } 
                FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                CryptoStreamWrapper cryptoStream = new CryptoStreamWrapper(fileStream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read, aes);
                return cryptoStream;
            } 
            catch( System.Exception e )
            {
                CruFramework.Logger.LogError("Read file Error : " + e.Message);
                throw e;
            }
        } 
        
        /// <summary>
        /// ハッシュか
        /// </summary>
        static public byte[] Sha256Hash(byte[] data ) {
            using (SHA256 sha256 = SHA256.Create()){
                var hash = sha256.ComputeHash(data);
                return hash;
            }
        }

   }
}