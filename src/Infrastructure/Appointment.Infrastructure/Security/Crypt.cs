using Appointment.Domain.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Appointment.Infrastructure.Security
{
    public interface ICrypt
    {
        string DecryptStringFromBytes_Aes(string password);
        string EncryptStringToBytes_Aes(string plainText);
    }
    public class Crypt : ICrypt
    {
        private readonly AuthOptions _authConfig;
        public Crypt(IOptions<AuthOptions> authOptions)
            => (_authConfig)
                = (authOptions.Value);

        public string DecryptStringFromBytes_Aes(string password)
        {
            var cipherBytes = Convert.FromBase64String(password.Trim());
            using Aes encryptor = Aes.Create();
            var salt = cipherBytes.Take(16).ToArray();
            var iv = cipherBytes.Skip(16).Take(16).ToArray();
            var encrypted = cipherBytes.Skip(32).ToArray();
            var pdb = new Rfc2898DeriveBytes(_authConfig.HashValue, salt, 100, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.Padding = PaddingMode.PKCS7;
            encryptor.Mode = CipherMode.CBC;
            encryptor.IV = iv;
            using var ms = new MemoryStream(encrypted);
            using var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cs, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public string EncryptStringToBytes_Aes(string plainText)
        {
            using Aes encryptor = Aes.Create();
            var salt = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            encryptor.Padding = PaddingMode.PKCS7;
            encryptor.Mode = CipherMode.CBC;
            encryptor.GenerateIV();
            var pdb = new Rfc2898DeriveBytes(_authConfig.HashValue, salt, 100, HashAlgorithmName.SHA256);
            encryptor.Key = pdb.GetBytes(32);
            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);
            ms.Write(encryptor.IV, 0, encryptor.IV.Length);
            using var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            using var writer = new StreamWriter(cs, Encoding.UTF8);
            writer.Write(plainText);
            writer.Flush();
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
