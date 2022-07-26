using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{
    [Index(nameof(ProjectName), IsUnique = true)]
    [Index(nameof(ProjectSlug), IsUnique = true)]
    public class Project : BaseEntity
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        public string ProjectName { get; set; }

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ProjectSlug { get; set; }


        [Required]
        public int ProjectMarketListItemId { get; set; }

        [ForeignKey(nameof(ProjectMarketListItemId))]
        public ListItem ProjectMarketListItem { get; set; }

        [AllowNull]
        public string ProjectDescription { get; set; }
    }
}
