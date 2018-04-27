using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace beer.umajkla.web
{
    public abstract class Crypto
    {
        public struct KeyPair
        {
            public string PrivateKey { get; set; }
            public string PublicKey { get; set; }
        }

        public static KeyPair CreateKeyPair()
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);

            string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

            return new KeyPair() { PublicKey = publicKey, PrivateKey = privateKey };
        }

        public static string Encrypt(string publicKey, string data)
        {
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();
            // Encrypt the string to an array of bytes. 
            byte[] encrypted = EncryptStringToBytes(data, myRijndael.Key, myRijndael.IV);

            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(publicKey));
            
            return JsonConvert.SerializeObject(new Tuple<byte[], byte[], byte[]>(rsaProvider.Encrypt(myRijndael.Key, false), rsaProvider.Encrypt(myRijndael.IV, false), encrypted));
        }

        public static string Decrypt(string privateKey, string encryptedString)
        {
            Tuple<byte[], byte[], byte[]> encryptedBytes = JsonConvert.DeserializeObject<Tuple<byte[], byte[], byte[]>>(encryptedString);

            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));

            string plainText = DecryptStringFromBytes(encryptedBytes.Item3, rsaProvider.Decrypt(encryptedBytes.Item1, false), rsaProvider.Decrypt(encryptedBytes.Item2, false));

            return plainText;
        }

        private static Random random = new Random();
        public static string RandomString(int length, bool withAmbiguousCharacters = false)
        {
            string chars = "BCDFGHJKMPQSTVWXY2346789";
            chars = (withAmbiguousCharacters) ? chars + "015AEILNOSUZ" : chars;
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomAPIKey()
        {

            return Regex.Replace(RandomString(25), ".{5}", "$0-").Substring(0, 29);
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

        }
    }
}