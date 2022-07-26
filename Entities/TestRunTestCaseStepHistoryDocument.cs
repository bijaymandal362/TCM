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
    [Index(nameof(TestRunTestCaseStepHistoryId), nameof(DocumentId), IsUnique = true)]
    public class TestRunTestCaseStepHistoryDocument : BaseEntity
    {
        [Key]
        public int TestRunTestCaseStepHistoryDocumentId { get; set; }
        public int TestRunTestCaseStepHistoryId { get; set; }
        public int? DocumentId { get; set; }
        public string Comment { get; set; }

        [ForeignKey(nameof(TestRunTestCaseStepHistoryId))]
        public TestRunTestCaseStepHistory ProjectModule { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document ProjectMember { get; set; }
    }
}
