using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace RapidPay.Application.Model
{
    public static class Encryptor
    {
        private static readonly string _key = "mBNx7L2mlNKwjIKzN4e/8HkvLtR0ITENprF/tZPdjCY=";
        private static readonly string _iv = "klZOXCR2k00heuFm6fjRVQ==";
        public static string HashPassword(string passwordText)
        {
            var sha512 = SHA512.Create();
            byte[] hash = sha512.ComputeHash(ToByteArray(passwordText));
            string result = System.BitConverter.ToString(hash);
            result = result.Replace("-", "");
            return result;
        }
        public static string Encrypt(string plainText)
        {
            string result;
            using (Aes aesEncryption = Aes.Create())
            {
                // Check arguments.
                if (plainText == null || plainText.Length <= 0)
                    throw new ArgumentNullException("plainText");
                
                byte[] encrypted;

                // Create an Aes object
                // with the specified key and IV.
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_key);
                    aesAlg.IV = Convert.FromBase64String(_iv);

                    // Create an encryptor to perform the stream transform.
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

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

                result = Convert.ToBase64String(encrypted);
            }

            // Return the encrypted bytes from the memory stream.
            return result;
        }

        public static string Decrypt(string value)
        {
            string result;
            using (Aes aesEncryption = Aes.Create())
            {
                byte[] cipherText = Convert.FromBase64String(value);

                // Check arguments.
                if (cipherText == null || cipherText.Length <= 0)
                    throw new ArgumentNullException("cipherText");
                
                // Declare the string used to hold
                // the decrypted text.
                string plaintext;

                // Create an Aes object
                // with the specified key and IV.
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Convert.FromBase64String(_key);
                    aesAlg.IV = Convert.FromBase64String(_iv);

                    // Create a decryptor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

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

                result = plaintext;
            }
                
            return result;
        }

        private static byte[] ToByteArray(object value)
        {
            byte[] result = new byte[] { };
            string val = value as string;
            if (val != null)
                result = new System.Text.UnicodeEncoding().GetBytes(val);
            return result;
        }
    }
}
