using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Exceptions
{
    public class ServerException : AppException
    {
        public ServerException(string code, string message)
            : base(code, message)
        {
        }


    }
}