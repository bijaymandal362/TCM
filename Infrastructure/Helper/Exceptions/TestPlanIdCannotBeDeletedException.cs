using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Exceptions
{
	[Serializable]
	public class TestPlanIdCannotBeDeletedException : Exception
	{
		public TestPlanIdCannotBeDeletedException() : base("TestPlanId is used in testrun module")
		{

		}
	}
}


