using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class TestCaseStepDetail: BaseEntity
	{
        [Key]
        public int TestCaseStepDetailId { get; set; }

        [Required]
        public int StepNumber { get; set; }

        [Required]
        [MaxLength]
        public string StepDescription { get; set; }

        [AllowNull]
        [MaxLength]
        public string ExpectedResult { get; set; }

        public int TestCaseListItemId { get; set; }

        [ForeignKey(nameof(TestCaseListItemId))]
        public ListItem TestCaseListItem { get; set; }

        public int ProjectModuleId { get; set; }
        [ForeignKey(nameof(ProjectModuleId))]
        public ProjectModule ProjectModule { get; set; }
    }
}
