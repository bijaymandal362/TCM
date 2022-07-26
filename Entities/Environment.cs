using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Entities
{
    [Index(nameof(EnvironmentName), nameof(ProjectId), IsUnique = true)]
    public class Environment : BaseEntity
    {
        [Key]
        public int EnvironmentId { get; set; }

        [Required]
        public string EnvironmentName { get; set; }

        [AllowNull]
        public string URL { get; set; }


        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
    }
}
