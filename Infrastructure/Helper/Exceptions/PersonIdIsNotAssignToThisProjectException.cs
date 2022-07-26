using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
    [Serializable]
   public class PersonIdIsNotAssignToThisProjectException : Exception
    {
        public PersonIdIsNotAssignToThisProjectException():base("PersonId is not assign to this project")
        {

        }
    }
}
