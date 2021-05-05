using System.Threading.Tasks;

namespace MMS_API.Service.Implement
{
    public interface IStockService
    {
        Task<string> AddSeStockMonthRevenue(string yearMonth);
        Task<string> AddSe2StockMonthRevenue(string yearMonth);
        
    }
}