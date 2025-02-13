using CoreVisionDAL.Base;
using CoreVisionDomainModels.AppUser;
using CoreVisionDomainModels.Client;
using CoreVisionDomainModels.Foundation;
using CoreVisionDomainModels.v1.General.ScanCodes;
using Microsoft.EntityFrameworkCore;

namespace CoreVisionDAL.Context
{
    public class ApiDbContext : EfCoreContextRoot
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        #region Log Tables
        public DbSet<ErrorLogRoot> ErrorLogRoots { get; set; }

        #endregion Log Tables
        public DbSet<ApplicationUserDM> ApplicationUsers { get; set; }
        public DbSet<ClientUserDM> ClientUsers { get; set; }
        public DbSet<ClientCompanyDetailDM> ClientCompanyDetails { get; set; }
        public DbSet<ExternalUserDM> ExternalUsers { get; set; }
        public DbSet<ScanCodesFormatDM> ScanCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure unique constraint on Email and UserName
            /*modelBuilder.Entity<ClientUserDM>()
                .HasIndex(u => u.EmailId)
                .IsUnique();

            modelBuilder.Entity<ClientUserDM>()
                .HasIndex(u => u.LoginId)
                .IsUnique();*/

            

            // Seed database with initial data
            DatabaseSeeder<ApiDbContext> seeder = new DatabaseSeeder<ApiDbContext>();
            seeder.SetupDatabaseWithSeedData(modelBuilder);
        }
    }
}
