using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.Exceptions
{
    public class EntityAlreadyExistsException : AppException
    {
        private static readonly string DEFAULT_CODE = "AlreadyExists";

        public EntityAlreadyExistsException(string code, string message)
            : base(code + DEFAULT_CODE, message)
        {
        }
    }
}
