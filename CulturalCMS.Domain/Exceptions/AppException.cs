using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Exceptions
{
    public abstract class AppException : Exception
    {
        public string Code { get; set; }

        public AppException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
