using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	[Index(nameof(ProjectId), nameof(PersonId), IsUnique = true)]
	public class ProjectStarred : BaseEntity
	{
		[Key]
		public int ProjectStarredId { get; set; }
		public int ProjectId { get; set; }
		public int PersonId { get; set; }

		[ForeignKey(nameof(ProjectId))]
		public Project Project { get; set; }

		[ForeignKey(nameof(PersonId))]
		public Person Person { get; set; }


	}
}
