using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
    [Serializable]
    public class FolderNameAlreadyExistException : Exception
    {
        public FolderNameAlreadyExistException() : base("Folder name already used")
        {

        }
    }
   
}
