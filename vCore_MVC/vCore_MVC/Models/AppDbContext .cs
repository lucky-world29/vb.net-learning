using Microsoft.EntityFrameworkCore;

namespace vCore_MVC.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //public DbSet<User> CoreUsers { get; set; }
        public DbSet<Rot> Rots { get; set; }
        public DbSet<CoreCommodity> CoreCommodities { get; set; }
        public DbSet<CoreSurchargeMaster> CoreSurchargeMasters { get; set; }
        public DbSet<CoreRole> CoreRoles { get; set; }
        public DbSet<CoreServiceType> CoreServiceTypes { get; set; }
        public DbSet<CoreTenantOrganisation> CoreTenantOrganisations { get; set; }
        public DbSet<CoreUser> CoreUsers { get; set; }
        public DbSet<CoreBranch> CoreBranches { get; set; }
        public DbSet<CoreBranchSubUnit> CoreBranchSubUnits { get; set; }

        // =========================================
        // TRIGGER CONFIGURATION
        // =========================================
        protected override void OnModelCreating(
    ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================
            // CoreBranch Triggers
            // =====================================
            modelBuilder.Entity<CoreBranch>()
                .ToTable(tb =>
                {
                    tb.HasTrigger("Add_CoreBranchLog");

                    tb.HasTrigger("update_CoreBranchLog");
                });

            // =====================================
            // CoreBranchSubUnits Triggers
            // =====================================
            modelBuilder.Entity<CoreBranchSubUnit>()
                .ToTable(tb =>
                {
                    tb.HasTrigger("Add_CoreBranchSubUnitsLog");

                    tb.HasTrigger("update_CoreBranchSubUnitsLog");
                });
        }
    }
}