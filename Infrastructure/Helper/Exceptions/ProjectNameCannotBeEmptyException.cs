using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ProjectNameCannotBeEmptyException : Exception
    {
        public ProjectNameCannotBeEmptyException() : base("ProjectName cannot be empty")
        {

        }
    }
}
