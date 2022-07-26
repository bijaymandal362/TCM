using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AuditDataContext : DbContext
    {
        public AuditDataContext( DbContextOptions<AuditDataContext> options) : base(options)
        {
        }

        public DbSet<Audit> Audit{get;set;} 
    }
}