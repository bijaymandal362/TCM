﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class FailedToDeleteProjectModuleException : Exception
    {
        
        public FailedToDeleteProjectModuleException(): base("Failed to delete projectmodule")
        {

        }
    }
}
