using System;

namespace Entities
{
    public abstract class BaseEntity
    {
        public int InsertPersonId { get; set; }
        public DateTimeOffset InsertDate { get; set; }

        public int UpdatePersonId {get;set;}

        public DateTimeOffset UpdateDate {get;set;}
    }
}