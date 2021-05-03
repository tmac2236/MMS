using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Models.MMS;
using MMS_API.DTOs;

namespace API.Data.Repository
{
    public class CMSContext : DbContext
    {
        //Constructor
        public CMSContext(DbContextOptions<CMSContext> options) : base(options) { }
        //EF(SHCDEV3)
        public DbSet<Car> CMSCar { get; set; }
        public DbSet<CarManageRecord> CMSCarManageRecord { get; set; }
        public DbSet<Company> CMSCompany { get; set; }
        public DbSet<Department> CMSDepartment { get; set; }
        
        //DTO(Stored Procedure
        public DbSet<CarManageRecordDto> GetCarManageRecordDto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EF
            modelBuilder.Entity<Car>().HasKey(x => new { x.Id });
            modelBuilder.Entity<CarManageRecord>().HasKey(x => new { x.LicenseNumber, x.SignInDate });
            modelBuilder.Entity<Company>().HasKey(x => new { x.Id });
            modelBuilder.Entity<Department>().HasKey(x => new { x.Id });

            //DTO(Stored Procedure)
            modelBuilder.Entity<CarManageRecordDto>()
            .HasNoKey();

        }
    }
}