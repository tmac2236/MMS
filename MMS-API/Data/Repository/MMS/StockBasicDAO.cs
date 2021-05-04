
using API.Data.Interface.MMS;
using API.Models.MMS;

namespace API.Data.Repository.MMS
{
    public class StockBasicDAO: MMSCommonDAO<StockBasic>, IStockBasicDAO
    {
        public StockBasicDAO(MMSContext context) : base(context)
        {
        }

    }
}