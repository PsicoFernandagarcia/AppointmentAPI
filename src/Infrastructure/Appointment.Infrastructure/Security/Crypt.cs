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
    }
}
