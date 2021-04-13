using Microsoft.EntityFrameworkCore;
using API.Models.DKS;
using API.DTOs;

namespace API.Data.Repository
{
    public class DKSContext : DbContext
    {
        //Constructor
        public DKSContext(DbContextOptions<DKSContext> options) : base(options) { }
        //EF(SHCDEV3)
        public DbSet<UserLog> USER_LOG { get; set; }
        public DbSet<SamPartB> SAMPARTB { get; set; }

        //DTO(Stored Procedure
        public DbSet<F428SampleNoDetail> GetMaterialNoBySampleNoForWarehouseView { get; set; }
        public DbSet<StockDetailByMaterialNo> GetStockDetailByMaterialNoView { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLog>().HasKey(x => new { x.LOGINNAME, x.UPDATETIME });
            modelBuilder.Entity<SamPartB>().HasKey(x => new { x.PARTNO, x.SAMPLENO });

            //DTO(Stored Procedure)
            modelBuilder.Entity<F428SampleNoDetail>()
            .HasNoKey();
            modelBuilder.Entity<StockDetailByMaterialNo>()
            .HasNoKey();

        }
    }
}