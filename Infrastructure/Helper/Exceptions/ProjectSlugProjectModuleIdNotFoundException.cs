using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
    [Serializable]
    public class ProjectSlugProjectModuleIdNotFoundException : Exception
    {
        public ProjectSlugProjectModuleIdNotFoundException() : base("Projectslug and projectmoduleid not found")
        {

        }
    }
}
