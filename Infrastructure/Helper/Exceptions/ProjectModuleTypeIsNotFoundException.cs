using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectModuleTypeIsNotFoundException : Exception
    {
        public ProjectModuleTypeIsNotFoundException(): base("ProjectModule type is not found")
        {

        }
    }
}
