using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Exceptions
{
    public class InvalidItemStateException : AppException
    {
        private static readonly string DEFAULT_CODE = "InvalidState";

        public InvalidItemStateException(string code, string message)
            :base(code + DEFAULT_CODE, message) 
        {
        }
    }
}
