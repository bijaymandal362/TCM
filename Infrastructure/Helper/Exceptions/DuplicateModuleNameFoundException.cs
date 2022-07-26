using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{[Serializable]
	public class DuplicateModuleNameFoundException : Exception
	{
		public DuplicateModuleNameFoundException(): base("Duplicate module name found.")
		{

		}
	}
}
