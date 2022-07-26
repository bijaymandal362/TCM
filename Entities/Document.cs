using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Document : BaseEntity
    {
        [Key]
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public byte[] File { get; set; }
    }
}
