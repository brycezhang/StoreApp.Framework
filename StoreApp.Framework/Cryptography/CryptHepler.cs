using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StoreApp.Framework.Cryptography
{
    public class CryptHepler
    {
        /// <summary>
        /// 获取MD5值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5(string source)
        {

#if WINDOWS_PHONE
            return MD5Helper.GetHashString(source);
#endif

#if WINDOWS_8
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var buff = CryptographicBuffer.ConvertStringToBinary(source, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
#endif
        }

        #region RC4
        public static string RC4EncryptString(string source, byte[] mainKey)
        {
            var by = new byte[4];
            Random Rnd = new Random();
            Rnd.NextBytes(by);
            source = BitConverter.ToString(by).Replace("-", string.Empty) + source;
            //if (source.Length < 5) source = new string((char)32, 5 - source.Length) + source;
            var buffer = Encoding.UTF8.GetBytes(source);
            RC4(buffer, mainKey);
            return BitConverter.ToString(buffer).Replace("-", string.Empty);
        }

        public static string RC4DecryptString(string source, byte[] mainKey)
        {
            var buffer = Enumerable.Range(0, source.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(source.Substring(x, 2), 16)).ToArray();
            RC4(buffer, mainKey);
            return Encoding.UTF8.GetString(buffer, 0, buffer.Length).Substring(8);
        }

        /// <summary>
        /// Rc4原生算法
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="key"></param>
        private static void RC4(byte[] bytes, byte[] key)
        {
            var s = new Byte[256];
            var k = new Byte[256];
            Byte temp;
            int i;

            for (i = 0; i < 256; i++)
            {
                s[i] = (Byte)i;
                k[i] = key[i % key.GetLength(0)];
            }

            int j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
            for (int x = 0; x < bytes.GetLength(0); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                int t = (s[i] + s[j]) % 256;
                bytes[x] ^= s[t];
            }
        }
        #endregion

        #region AES、DES加解密

        public static byte[] Encrypt(byte[] plainText, byte[] key, byte[] iv, CryptType cryptType)
        {
            switch (cryptType)
            {
                case CryptType.AES:
                    return EncryptStringToBytes_Aes(plainText, key, iv);
                default:
                    return EncryptStringToBytes_Des(plainText, key, iv);
            }
        }

        public static byte[] Decrypt(byte[] cipherText, byte[] key, byte[] iv, CryptType cryptType)
        {
            switch (cryptType)
            {
                case CryptType.AES:
                    return DecryptStringFromBytes_Aes(cipherText, key, iv);
                default:
                    return DecryptStringFromBytes_Des(cipherText, key, iv);
            }
        }

#if WINDOWS_PHONE
        private static byte[] EncryptStringToBytes_Aes(byte[] plainText, byte[] key, byte[] iv)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;

            try
            {
                var password = Encoding.UTF8.GetString(key, 0, key.Length);
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, iv);

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                rfc2898.Reset(); //needed for WinRT compatibility
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                //Encrypt Data 
                cryptoStream.Write(plainText, 0, plainText.Length);
                cryptoStream.FlushFinalBlock();

                //Return encrypted data 
                return memoryStream.ToArray();

            }
            catch (Exception eEncrypt)
            {
                return null;
            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();

            }
        }

        private static byte[] DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            AesManaged aes = null;
            MemoryStream memoryStream = null;
            CryptoStream cryptoStream = null;
            byte[] decryptBytes = null;
            try
            {
                var password = Encoding.UTF8.GetString(key, 0, key.Length);
                //Generate a Key based on a Password, Salt and HMACSHA1 pseudo-random number generator 
                Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(password, iv);

                //Create AES algorithm with 256 bit key and 128-bit block size 
                aes = new AesManaged();
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);
                rfc2898.Reset(); //neede to be WinRT compatible
                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);

                //Create Memory and Crypto Streams 
                memoryStream = new MemoryStream();
                cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

                //Decrypt Data 
                cryptoStream.Write(cipherText, 0, cipherText.Length);
                cryptoStream.FlushFinalBlock();

                //Return Decrypted String 
                decryptBytes = memoryStream.ToArray();
            }
            catch (Exception eDecrypt)
            {

            }
            finally
            {
                if (cryptoStream != null)
                    cryptoStream.Close();

                if (memoryStream != null)
                    memoryStream.Close();

                if (aes != null)
                    aes.Clear();
            }
            return decryptBytes;
        }

        private static byte[] EncryptStringToBytes_Des(byte[] plainBytes, byte[] key, byte[] iv)
        {
            if (plainBytes == null || plainBytes.Length <= 0)
                throw new ArgumentNullException("plainBytes");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            var desAlg = new DesManaged();
            desAlg.InitializeKey(key, 0);
            desAlg.SetIV(iv);

            var encrypted = new byte[plainBytes.Length];
            desAlg.EncryptCBC(plainBytes, 0, plainBytes.Length, encrypted, 0);

            return encrypted;
        }

        private static byte[] DecryptStringFromBytes_Des(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            var desAlg = new DesManaged();
            desAlg.InitializeKey(key, 0);
            desAlg.SetIV(iv);

            var plainBytes = new byte[cipherText.Length];
            desAlg.DecryptCBC(cipherText, 0, cipherText.Length, plainBytes, 0);
            return plainBytes;
        }
#endif
#if WINDOWS_8
        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            IBuffer buffer = fileHeadKey.AsBuffer();// 原文

            // 根据算法名称实例化一个对称算法提供程序
            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);
            
            // 创建一个随机密钥 key
            //IBuffer key = CryptographicBuffer.DecodeFromHexString(deviceId);// key
            
            // 根据 key 生成 CryptographicKey 对象
            CryptographicKey cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key.AsBuffer());
            
            // 如果是 CBC 模式则随机生成一个向量
            IBuffer biv = CryptographicBuffer.CreateFromByteArray(iv);
            
            IBuffer encrypted = CryptographicEngine.Encrypt(cryptoKey, buffer, biv);
            
            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            
            var buffer = cipherText.AsBuffer();
            // 根据算法名称实例化一个对称算法提供程序
            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbc);

            // 创建一个随机密钥 key
            //IBuffer key = CryptographicBuffer.DecodeFromHexString(deviceId);

            // 根据 key 生成 CryptographicKey 对象
            CryptographicKey cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key.AsBuffer());

            // 如果是 CBC 模式则随机生成一个向量
            IBuffer biv = CryptographicBuffer.CreateFromByteArray(iv);

            IBuffer encrypted = CryptographicEngine.Decrypt(cryptoKey, buffer, biv);
            return encrypted;
        }

        private static byte[] EncryptStringToBytes_Des(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(fileHeadKey);

            // 根据算法名称实例化一个对称算法提供程序
            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.DesCbc);

            // 创建一个随机密钥 key
            IBuffer key = CryptographicBuffer.ConvertStringToBinary(serverHeadKey, BinaryStringEncoding.Utf8);

            // 根据 key 生成 CryptographicKey 对象
            CryptographicKey cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key);

            // 如果是 CBC 模式则随机生成一个向量
            IBuffer iv = CryptographicBuffer.CreateFromByteArray(MainIv);

            IBuffer decrypted = CryptographicEngine.Decrypt(cryptoKey, buffer, iv);

            return Convert.FromBase64String(Encoding.UTF8.GetString(decrypted.ToArray(), 0, (int)decrypted.Length));
        }

        private static string DecryptStringFromBytes_Des(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("key");

            IBuffer buffer = CryptographicBuffer.DecodeFromBase64String(cipherText);

            // 根据算法名称实例化一个对称算法提供程序
            SymmetricKeyAlgorithmProvider symmetricAlgorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.DesCbc);
            
            // 创建一个随机密钥 key
            //IBuffer key = CryptographicBuffer.ConvertStringToBinary(serverHeadKey, BinaryStringEncoding.Utf8);
            
            // 根据 key 生成 CryptographicKey 对象
            CryptographicKey cryptoKey = symmetricAlgorithm.CreateSymmetricKey(key.AsBuffer());
            
            // 如果是 CBC 模式则随机生成一个向量
            IBuffer biv = CryptographicBuffer.CreateFromByteArray(iv);
            
            IBuffer decrypted = CryptographicEngine.Decrypt(cryptoKey, buffer, biv);
            
            return Encoding.UTF8.GetString(decrypted.ToArray(), 0, (int)decrypted.Length);
        }
#endif
        #endregion
    }

    public enum CryptType
    {
        AES,
        DES
    }
}
