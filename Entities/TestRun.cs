using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class TestRun : BaseEntity
	{
		[Key]
		public int TestRunId { get; set; }

		[Required]
		public string Title { get; set; }
		public string Description { get; set; }


		public int ProjectId { get; set; }
		[ForeignKey(nameof(ProjectId))]
		public Project Project { get; set; }

		public int? EnvironmentId { get; set; }
		[ForeignKey(nameof(EnvironmentId))]
		public Environment Environment { get; set; }

		public int? DefaultAssigneeProjectMemberId { get; set; }
		[ForeignKey(nameof(DefaultAssigneeProjectMemberId))]
		public ProjectMember ProjectMember { get; set; }
	}
}
