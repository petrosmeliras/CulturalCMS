using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Application.Interfaces
{
    public interface IEncryptionUtil
    {
        string Encrypt(string plainText);
        bool IsValidPassword(string plainText, string cipherText);
    }
}
