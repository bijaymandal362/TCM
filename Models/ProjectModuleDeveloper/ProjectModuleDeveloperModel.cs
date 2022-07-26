using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ProjectModuleDeveloper
{
	public class ProjectModuleDeveloperModel
	{
		public int ProjectModuleDeveloperId { get; set; }
		public int ProjectModuleId { get; set; }
		public int ProjectMemberId { get; set; }
		public bool IsDisabled { get; set; }
	}
}
