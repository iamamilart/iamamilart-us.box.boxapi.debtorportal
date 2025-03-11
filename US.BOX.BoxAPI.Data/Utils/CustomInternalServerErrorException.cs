using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace US.BOX.BoxAPI.Data.Utils
{
    public class CustomInternalServerErrorException : Exception
    {
        public CustomInternalServerErrorException(string message) : base(message)
        {
        }
    }
}
