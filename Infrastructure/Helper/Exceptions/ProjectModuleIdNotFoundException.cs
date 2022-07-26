using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectModuleIdNotFoundException : Exception
    {
        public ProjectModuleIdNotFoundException() : base("ProjectModuleId is not found")
        {
                
        }
    }
}
