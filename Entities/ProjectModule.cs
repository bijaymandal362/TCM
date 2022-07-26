using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{
    [Index(nameof(ProjectId), nameof(ModuleName), nameof(ParentProjectModuleId), nameof(ProjectModuleListItemId), IsUnique = true)]
    public class ProjectModule:BaseEntity
    {
        [Key]
        public int ProjectModuleId { get; set; }

        public int? ParentProjectModuleId { get; set; }
        public int ProjectId { get; set; }

        [Required]
        public string ModuleName { get; set; }
        public int ProjectModuleListItemId { get; set; }

        [AllowNull]
        [StringLength(500)]

        public string Description { get; set; }
        public DateTimeOffset OrderDate { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; } 
        
        [ForeignKey(nameof(ProjectModuleListItemId))]
        public ListItem ProjectModuleListItem { get; set; }
    }
}
