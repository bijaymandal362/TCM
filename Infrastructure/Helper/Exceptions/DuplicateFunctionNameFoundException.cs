using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helper.Exceptions
{[Serializable]
	public class DuplicateFunctionNameFoundException  : Exception
	{
		public DuplicateFunctionNameFoundException() : base("Duplicate function name found.")
		{
				
		}
	}
}
