using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{
	[Serializable]
	public class ModuleContainsFunctionException : Exception
	{
		public ModuleContainsFunctionException() : base("Module cannnot be deleted,  it contains Function")
		{

		}
	}
}






