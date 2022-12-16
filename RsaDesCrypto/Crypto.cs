using System;
using System.IO;
using System.Security.Cryptography;

namespace RsaDesCrypto;

public class Crypto
{
    public class Des
    {
        public static string GenerateKey()
        {
            byte[] key;
            byte[] iv;

            using (DES des = DES.Create())
            {
                key = des.Key;
                iv = des.IV;
            }

            return Convert.ToBase64String(key) + System.Environment.NewLine + Convert.ToBase64String(iv);
        }

        public static void Encrypt(byte[] data, string path, byte[] key, byte[] iv)
        {
            try
            {
                using (FileStream fStream = File.Open(path, FileMode.Create))
                using (DES des = DES.Create())
                using (ICryptoTransform encryptor = des.CreateEncryptor(key, iv))
                using (var cStream = new CryptoStream(fStream, encryptor, CryptoStreamMode.Write))
                {
                    cStream.Write(data, 0, data.Length);
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                throw;
            }
        }

        public static byte[] Decrypt(string path, byte[] key, byte[] iv)
        {
            try
            {
                using (FileStream fStream = File.OpenRead(path))
                using (DES des = DES.Create())
                using (ICryptoTransform decryptor = des.CreateDecryptor(key, iv))
                using (var cStream = new CryptoStream(fStream, decryptor, CryptoStreamMode.Read))
                using (var memoryStream = new MemoryStream())
                {
                    cStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                throw;
            }
        }
    }
    public class RSA
    {
        public static (string pub, string priv) GenerateKeys(int dwKeySize)
        {
            var csp = new RSACryptoServiceProvider(dwKeySize);

            var privKey = csp.ExportParameters(true);
            var pubKey = csp.ExportParameters(false);

            string pubKeyString;
            string privKeyString;
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            using (var sw = new System.IO.StringWriter())
            {
                xs.Serialize(sw, pubKey);
                pubKeyString = sw.ToString();
            }
            using (var sw = new System.IO.StringWriter())
            {
                xs.Serialize(sw, privKey);
                privKeyString = sw.ToString();
            }
            return (pubKeyString, privKeyString);
        }

        public static byte[] Encrypt(byte[] data, string publicKey)
        {
            RSAParameters pubKey;
            using (var sr = new StringReader(publicKey))
            {
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                pubKey = (RSAParameters)xs.Deserialize(sr);
            }
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);

            var bytesCypherText = csp.Encrypt(data, false);
            return bytesCypherText;
        }

        public static byte[] Decrypt(byte[] data, string privateKey)
        {
            RSAParameters privKey;
            using (var sr = new StringReader(privateKey))
            {
                var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
                privKey = (RSAParameters)xs.Deserialize(sr);
            }
            var csp = new RSACryptoServiceProvider();
            csp.ImportParameters(privKey);

            var decrypted = csp.Decrypt(data, false);
            return (decrypted);
        }
    }
}
