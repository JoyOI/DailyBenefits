using Microsoft.EntityFrameworkCore;

namespace DailyBenefits.Model
{
    public class DailyBenefitsContext : DbContext
    {
        public DailyBenefitsContext(DbContextOptions opt) : base(opt)
        {
            this.Database.EnsureCreated();
        }

        public DbSet<Record> Records { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Record>(e => 
            {
                e.HasIndex(x => x.UserId);
                e.HasIndex(x => x.Time);
                e.HasIndex(x => x.Coins);
            });
        }
    }
}
