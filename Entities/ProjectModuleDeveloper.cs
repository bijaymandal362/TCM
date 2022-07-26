using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Index(nameof(ProjectModuleId), nameof(ProjectMemberId), IsUnique = true)]
    public class ProjectModuleDeveloper:BaseEntity
    {
        [Key]
        public int ProjectModuleDeveloperId { get; set; }
        public int ProjectModuleId { get; set; }
        public int ProjectMemberId { get; set; }
        public bool IsDisabled { get; set; } 
      
        [ForeignKey(nameof(ProjectModuleId))]
        public ProjectModule ProjectModule { get; set; } 
        
        [ForeignKey(nameof(ProjectMemberId))]
        public ProjectMember ProjectMember { get; set; }
    }
}
