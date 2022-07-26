using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class TestRunPlanDetail : BaseEntity
	{
		[Key]
		public int TestRunPlanDetailId { get; set; }

		public int TestRunPlanId { get; set; }
		[ForeignKey(nameof(TestRunPlanId))]
		public TestRunPlan TestRunPlan { get; set; }

		public string TestPlanDetailJson { get; set; }
		public string TestCaseDetailJson { get; set; }

		public string TestCaseStepDetailJson { get; set; }


		

	}
}
