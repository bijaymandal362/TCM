using Models.ProjectModule;
using Models.TestPlan;

using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.ProjectModule
{
	public static class ProjectModuleExtension
	{
		public static IEnumerable<ProjectModuleModel> Flatten(this IEnumerable<ProjectModuleModel> root)
		{
			foreach (var node in root)
			{
				yield return node;
				if (node.ChildModule != null)  
				{
					foreach (var subNode in node.ChildModule.Flatten())
						yield return subNode;
				}
			}
		}

		public static IEnumerable<TestPlanListModel> Searching(this IEnumerable<TestPlanListModel> roots)
		{
			foreach (var nodes in roots)
			{
				yield return nodes;
				if (nodes.TestPlanChildModule != null)
				{
					foreach (var subNodes in nodes.TestPlanChildModule.Searching())
						yield return subNodes;
				}
			}
		}

	}
}