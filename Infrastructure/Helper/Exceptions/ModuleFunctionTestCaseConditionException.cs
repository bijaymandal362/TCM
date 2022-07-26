using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
    [Serializable]
    public class ModuleFunctionTestCaseConditionException : Exception
    {
        public ModuleFunctionTestCaseConditionException():base("Module or function or testcase condition not satisfied")
        {

        }
    }
}
