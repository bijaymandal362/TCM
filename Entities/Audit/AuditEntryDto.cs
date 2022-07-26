using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entities
{
    public class AuditEntryDto
    {
        public AuditEntryDto(EntityEntry entry)
        {
            Entry = entry;
        }
        public EntityEntry Entry { get; }
        public string PersonId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public AuditTypeEnum AuditTypeEnum { get; set; }
        public List<string> ChangedColumns { get; } = new List<string>();
         public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
       public bool HasTemporaryProperties => TemporaryProperties.Any();
        public Audit ToAudit()
        {
            var audit = new Audit();
            audit.PersonId = PersonId;
            audit.Type = AuditTypeEnum.ToString();
            audit.TableName = TableName;
            audit.DateTime = DateTimeOffset.UtcNow;
            audit.PrimaryKey = JsonSerializer.Serialize(KeyValues);
            audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
            audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns);
            return audit;
        }
    }
}