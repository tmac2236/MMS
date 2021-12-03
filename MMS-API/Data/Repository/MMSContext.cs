using Microsoft.EntityFrameworkCore;
using API.Models.MMS;

namespace API.Data.Repository
{
    public class MMSContext : DbContext
    {
        //Constructor
        public MMSContext(DbContextOptions<MMSContext> options) : base(options) { }
        //EF(SHCDEV3)
        public DbSet<StockBasic> StockBasic { get; set; }
        public DbSet<MonthReport> MonthReport { get; set; }
        public DbSet<QuarterReport> QuarterReport { get; set; }
        

        //DTO(Stored Procedure


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EF
            modelBuilder.Entity<StockBasic>().HasKey(x => new { x.Id });
            modelBuilder.Entity<MonthReport>().HasKey(x => new { x.StockId, x.YearMonth });
            modelBuilder.Entity<QuarterReport>().HasKey(x => new { x.StockId, x.YearQ });
            //DTO(Stored Procedure)

        }
    }
}