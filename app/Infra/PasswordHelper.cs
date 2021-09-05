using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace app.Infra
{
    public static class PasswordHelper
    {

        
        public static (string, string) HashPassword(this string Password)
        {
            string SecurityStamp = Guid.NewGuid().ToString();
            return Password.HashPassword(SecurityStamp);
        }
        public static (string, string) HashPassword(this string Password, string SecurityStamp)
        {
            string hash;
            using (Rfc2898DeriveBytes h = new Rfc2898DeriveBytes(Password, Encoding.UTF8.GetBytes(SecurityStamp), 10))
            {
                hash = Convert.ToBase64String(h.GetBytes(20));
            }
            return (hash, SecurityStamp);
        }

        public static bool CheckValidPasswprd(this string Password, string SecurityStamp, string passwordHash)
        {
            var (hash, _) = Password.HashPassword(SecurityStamp);
            if (hash == passwordHash)
                return true;
            else return false;

        }
    }

    public class AESEncrytDecryt
    {
        private string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }

        private byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
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

        public string DecryptStringAES(string cipherText)
        {
            var keybytes = Encoding.UTF8.GetBytes("1080808080808080");
            var iv = Encoding.UTF8.GetBytes("1080808080808080");

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        public string EncriptStringAES(string plainText)
        {
            var keybytes = Encoding.UTF8.GetBytes("1080808080808080");
            var iv = Encoding.UTF8.GetBytes("1080808080808080");

            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(plainText);
            byte[] bytesEncrypted = EncryptStringToBytes(plainText, keybytes, iv);
            string result = Convert.ToBase64String(bytesEncrypted);
            return result;
        }
    }
}
