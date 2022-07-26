using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectMemberIdNotFoundException : Exception
    {
        public ProjectMemberIdNotFoundException():base("ProjectMember id not found")
        {

        }
    }
}
