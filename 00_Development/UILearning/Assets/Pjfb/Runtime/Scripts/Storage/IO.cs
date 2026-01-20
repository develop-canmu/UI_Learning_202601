using System;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Text;

namespace Pjfb.Storage {
    public static class IO {
        
        /// <summary>
        /// PersistentPathにデータ書き込み
        /// </summary>
        static public void WriteFileToPersistent<T>( string fileName, T data ) {
            var path = AppendPersistentPath(fileName);
            Write(path, data);
        }

        static public void WriteEncryptFileToPersistent<T>( string fileName, T data, string IV, string key ) {
            var path = AppendPersistentPath(fileName);
            WriteEncrypt(path, data, IV, key);
        }

        /// <summary>
        /// データ書き込み
        /// </summary>
        static public void Write<T>( string path, T data) {
            try {
                var json = JsonUtility.ToJson(data);
                WriteText( path, json );
            } catch(System.Exception e) {
                CruFramework.Logger.LogError("Write file Error : " + e.Message);
                throw e;
            }
            
        }

        static public void WriteEncrypt<T>( string path, T data, string IV, string key) {
            try {
                var json = JsonUtility.ToJson(data);
                var bytes = Crypt.Encrypt( json, IV, key );
                WriteBytes( path, bytes );
            } catch(System.Exception e) {
                CruFramework.Logger.LogError("Write encrypt file Error : " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// テキスト書き込み
        /// </summary>
        static public void WriteText( string path, string str) {
            try{
                File.WriteAllText(path, str);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Write file Error : " + e.Message);
                throw e;
            }
        }

        static public void WriteTextEncrypt( string path, string str, string IV, string key) {
            try {
                var bytes = Crypt.Encrypt( str, IV, key );
                WriteBytes( path, bytes );
            } catch(System.Exception e) {
                CruFramework.Logger.LogError("Write text encrypt file Error : " + e.Message);
                throw e;
            }
        }

        static public void WriteBytes( string path, byte[] bytes) {
            try{
                File.WriteAllBytes(path, bytes);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Write file Error : " + e.Message);
                throw e;
            }
        }
        
        static public void WriteByteArrayEncrypt( string path, byte[] bytes, string IV, string key) {
            try {
                byte[] encryptBytes = Crypt.Encrypt( bytes, IV, key );
                WriteBytes( path, encryptBytes );
            } catch(System.Exception e) {
                CruFramework.Logger.LogError("Write text encrypt file Error : " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// PersistentPathからデータ読み込み
        /// </summary>
        static public T ReadFileFromPersistent<T>( string fileName ) where T : new() {
            var path = AppendPersistentPath(fileName);
            return Read<T>( path );
        }

        static public T ReadEncryptFileFromPersistent<T>( string fileName, string IV, string key ) where T : new() {
            var path = AppendPersistentPath(fileName);
            return ReadEncrypt<T>( path, IV, key );
        }


        /// <summary>
        /// データ読み込み
        /// </summary>
        static public T Read<T>( string path)  where T : new() {
            try {
                var json = ReadText( path );
                if( string.IsNullOrEmpty(json) ) {
                    throw new System.Exception("file is empty");
                }
                return JsonUtility.FromJson<T>(json);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read file Error : " + e.Message);
                throw e;
            }
        }

        static public T ReadEncrypt<T>( string path, string IV, string key)  where T : new() {
            try {
                var data = ReadBytes( path );
                if( data == null ) {
                    throw new System.Exception("file is empty");
                }
                var json = Crypt.Decrypt(data, IV, key);
                return JsonUtility.FromJson<T>(json);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read encrypt file Error : " + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// テキスト読み込み
        /// </summary>
        static public string ReadText( string path ) {
            try{
                if( !File.Exists(path) ) {
                    throw new System.IO.FileNotFoundException(path);
                } 
                return File.ReadAllText(path);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read file Error : " + e.Message);
                throw e;
            }
        }

        static public string ReadTextEncrypt( string path, string IV, string key ) {
            try {
                var data = ReadBytes( path );
                if( data == null ) {
                    throw new System.Exception("file is empty");
                }
                var str = Crypt.Decrypt(data, IV, key);
                return str;
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read encrypt file Error : " + e.Message);
                throw e;
            }
        }

        static public byte[] ReadByteArrayEncrypt( string path, string IV, string key ) {
            try {
                byte[] data = ReadBytes( path );
                if( data == null ) {
                    throw new System.Exception("file is empty");
                }
                byte[] decryptByteArray = Crypt.DecryptToBytes(data, IV, key);
                return decryptByteArray;
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read encrypt file Error : " + e.Message);
                throw e;
            }
        }
        
        static public byte[] ReadBytes( string path ) {
            try{
                if( !File.Exists(path) ) {
                    throw new System.IO.FileNotFoundException(path);
                } 
                return File.ReadAllBytes(path);
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("Read file Error : " + e.Message);
                throw e;
            }
        }
        
        static public void CreateDirectory( string path ){
            
            try{
                if( ExistsDirectory(path) ) {
                    return;
                }

                Directory.CreateDirectory( path ); 
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("create directory Error : " + e.Message);
                throw e;
            }

        }

        /// <summary>
        /// ファイル削除
        /// </summary>
        static public void DeletePersistentFile( string fileName ) {
            var path = AppendPersistentPath(fileName);
            Delete(path);
        }

        static public void DeleteTemporaryFile( string fileName ) {
            var path = AppendTemporaryPath(fileName);
            Delete(path);
        }

       
        static public void Delete( string path ) {
            try{
                if( ExistsFile(path) ) {
                    File.Delete(path);
                }
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("delete file Error : " + e.Message);
                throw e;
            }
        }

        static public void DeleteDirectory( string path ) {
            try{
                if( ExistsDirectory(path) ) {
                    Directory.Delete(path, true);
                }
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("delete directory Error : " + e.Message);
                throw e;
            }
        }


        /// <summary>
        /// ファイルが存在しているか
        /// </summary>
        static public bool ExistsFile( string path ) {
            return System.IO.File.Exists(path);
        }

        static public bool ExistsDirectory( string path ) {
            return System.IO.Directory.Exists(path);
        }

        static public bool ExistsFileInPersistent( string fileName ) {
            var path = AppendPersistentPath(fileName);
            return System.IO.File.Exists(path);
        }


        static public string AppendPersistentPath( string path ) {
            return Path.Combine(Application.persistentDataPath, path);
        }

        static public string AppendTemporaryPath( string path ) {
            return Path.Combine(Application.temporaryCachePath, path);
        }
     

        /// <summary>
        /// ハッシュ化したファイル名取得
        /// </summary>
        static public string CreateHashFileName( string baseFileName ){
            var strBytes = System.Text.Encoding.UTF8.GetBytes(baseFileName);
            var hash =  Crypt.Sha256Hash(strBytes);
            var hashStr = System.BitConverter.ToString(hash).ToLower().Replace("-","");
            return hashStr.Substring(0,16);
        }


        /// <summary>
        /// GZipの解凍
        /// </summary>
        static public byte[] DecompressGZip( byte[] bytes ) {
            try{
               

                using (var inStream = new MemoryStream(bytes))
                using (var outStream = new MemoryStream())
                using (var stream = new GZipStream(inStream, System.IO.Compression.CompressionMode.Decompress)) {
                    var buf = new byte[1024];
                    int num;
                    while ((num = stream.Read(buf, 0, buf.Length)) > 0) {
                        outStream.Write(buf, 0, num);
                    }

                    var output = outStream.ToArray();
                    inStream.Close();
                    outStream.Close();
                    return output;
                }
                
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("DecompressGZip Error : " + e.Message);
                throw e;
            }
        }
        
        /// <summary>
        /// GZipの解凍
        /// </summary>
        static public string DecompressGZipToString( byte[] bytes ) {
            try{
                using MemoryStream memoryStream = new MemoryStream(bytes);
                using GZipStream stream = new GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress); 
                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                return reader.ReadToEnd();
                
            } catch( System.Exception e ){
                CruFramework.Logger.LogError("DecompressGZip Error : " + e.Message);
                throw e;
            }
        }

    }
}