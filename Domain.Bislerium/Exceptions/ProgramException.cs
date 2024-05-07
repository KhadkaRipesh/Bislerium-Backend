using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Bislerium.Exceptions
{
    public class ProgramException: Exception
    {
        public ProgramException(String message): base(message) { }
    }
}
