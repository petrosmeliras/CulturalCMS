using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CulturalCMS.Domain.ValueObjects
{
    public record Dimensions(double Length, double Width, double Height, string Unit)
    {
        public override string ToString()
        {
            return $"{Length}x{Width}x{Height} {Unit}";
        }
    }
}
