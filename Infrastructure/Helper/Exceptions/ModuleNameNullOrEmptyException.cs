using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ModuleNameNullOrEmptyException : Exception
    {
        public ModuleNameNullOrEmptyException() : base("TestCaseRepository Name cannot be empty")
        {

        }
    }
}


