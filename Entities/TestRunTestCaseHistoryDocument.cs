using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    [Index(nameof(TestRunTestCaseHistoryId), nameof(DocumentId), IsUnique = true)]
    public class TestRunTestCaseHistoryDocument : BaseEntity
    {
        [Key]
        public int TestRunTestCaseHistoryDocumentId { get; set; }
        public int TestRunTestCaseHistoryId { get; set; }
        public int? DocumentId { get; set; }
        public string Comment { get; set; }

        [ForeignKey(nameof(TestRunTestCaseHistoryId))]
        public TestRunTestCaseHistory ProjectModule { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document ProjectMember { get; set; }
    }
}
