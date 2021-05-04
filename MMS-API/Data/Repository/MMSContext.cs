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
        //DTO(Stored Procedure


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //EF
            modelBuilder.Entity<StockBasic>().HasKey(x => new { x.Id });

            //DTO(Stored Procedure)

        }
    }
}