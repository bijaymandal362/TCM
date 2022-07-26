using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{

	[Serializable]
	public class ModuleContainsNestedModuleException : Exception
	{
		public ModuleContainsNestedModuleException() : base("Module cannot be deleted, it contains  nested module")
		{

		}
	}
}

