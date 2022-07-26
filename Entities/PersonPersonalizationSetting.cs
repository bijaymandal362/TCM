using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    [Index(nameof(PersonId), IsUnique = true)]
    public class PersonPersonalizationSetting : BaseEntity
    {
        [Key]
        public int PersonPersonalizationId { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        public int ThemeListItemId { get; set; }
        public string TimeZone { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

        [ForeignKey(nameof(ThemeListItemId))]
        public ListItem ListItem { get; set; }
    }
}
