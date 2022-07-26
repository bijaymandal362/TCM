using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{

    [Index(nameof(ParentTestPlanId), nameof(TestPlanName),nameof(ProjectId), nameof(TestPlanTypeListItemId), IsUnique = true)]
    public class TestPlan : BaseEntity
    {
        [Key]
        public int TestPlanId { get; set; }
        public int? ParentTestPlanId { get; set; }

        [Required]
        public string TestPlanName { get; set; }

        public int ProjectId { get; set; }
        public int TestPlanTypeListItemId { get; set; }
        public bool IsDeleted { get; set; } 

        [ForeignKey(nameof(TestPlanTypeListItemId))]

        public ListItem TestPlanTypeListItem { get; set; }
        public DateTimeOffset OrderDate { get; set; }

        [AllowNull]
        public string Title { get; set; }

        [AllowNull]
        [MaxLength]
        public string Description { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
