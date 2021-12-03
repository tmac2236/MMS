using System.Threading.Tasks;

namespace MMS_API.Service.Implement
{
    public interface IStockService
    {
        Task<string> AddSeStockMonthRevenue(string yearMonth);
        Task<string> AddSe2StockMonthRevenue(string yearMonth);
        Task<string> AddSeStockQEps(string yearQ);
        Task<string> AddSe2StockQEps(string yearQ);
        
        
    }
}