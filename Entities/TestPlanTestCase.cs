using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Index(nameof(TestPlanId),nameof(ProjectModuleId),IsUnique=true)]
    public class TestPlanTestCase : BaseEntity
    {
        [Key]
        public int TestPlanTestCaseId { get; set; }
        public int TestPlanId { get; set; }
        public int ProjectModuleId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectModuleId))]
        public ProjectModule ProjectModule { get; set; }
        
        
        [ForeignKey(nameof(TestPlanId))]
        public TestPlan TestPlan { get; set; }
    }
}
