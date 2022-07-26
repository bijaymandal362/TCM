using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class DownloadFailedException : Exception
    {
        public DownloadFailedException(): base("Download Failed")
        {

        }
    }
}
