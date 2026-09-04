using CulturalCMS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Infrastructure.Security
{
    public class EncryptionUtil : IEncryptionUtil
    {
        public string Encrypt(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public bool IsValidPassword(string plainText, string cipherText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, cipherText);
        }
    }
}
