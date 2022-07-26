using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectSlugNotFoundException : Exception
    {
        public ProjectSlugNotFoundException():base("ProjectSlug not found")
        {

        }
    }
}
