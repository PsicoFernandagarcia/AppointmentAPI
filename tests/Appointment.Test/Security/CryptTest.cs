using Appointment.Domain.Infrastructure;
using Appointment.Infrastructure.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Appointment.Test.Security
{
    public class CryptTest
    {
        public static readonly Crypt crypt = new Crypt(Options.Create(new AuthOptions
        {
            HashValue = "testHashValue"
        }));

        [Fact]
        public void EncryptDecryptTest()
        {
            var originalText = "Hello, World!";
            var encryptedText = crypt.EncryptStringToBytes_Aes(originalText);
            var decryptedText = crypt.DecryptStringFromBytes_Aes(encryptedText);
            Assert.Equal(originalText, decryptedText);
        }
    }
}
