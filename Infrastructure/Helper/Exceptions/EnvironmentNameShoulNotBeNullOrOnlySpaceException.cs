using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class EnvironmentNameShoulNotBeNullOrOnlySpaceException :Exception
    {
        public EnvironmentNameShoulNotBeNullOrOnlySpaceException():base("EnvironmentName should not empty or only space")
        {

        }
    }
}
