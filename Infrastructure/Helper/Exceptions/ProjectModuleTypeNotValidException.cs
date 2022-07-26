using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
    [Serializable]
    public class ProjectModuleTypeNotValidException : Exception
    {
        public ProjectModuleTypeNotValidException() : base("ProjectModuleType is not valid")
        {

        }

    }
}
