using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectNameCannotBeAdminException : Exception
    {
        public ProjectNameCannotBeAdminException() : base("ProjectName cannot be 'Admin'")
        {
        }
    }
}
