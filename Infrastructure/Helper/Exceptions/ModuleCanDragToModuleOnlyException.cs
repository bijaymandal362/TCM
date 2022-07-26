using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ModuleCanDragToModuleOnlyException : Exception
    {
        public ModuleCanDragToModuleOnlyException():base("Module can only be drag and drop to module")
        {

        }
    }
}
