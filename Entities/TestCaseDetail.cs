using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Index(nameof(ProjectModuleId), IsUnique = true)]
    public class TestCaseDetail : BaseEntity
    {
        [Key]
        public int TestCaseDetailId { get; set; }
      

        [Required]
        public string PreCondition { get; set; }

        [Required]
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
