using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;

namespace API.Data.Interface
{
    public interface IWarehouseDAO
    {
        Task<bool> SaveAll();
        PagedList<F428SampleNoDetail> GetMaterialNoBySampleNoForWarehouse(SF428SampleNoDetail sF428SampleNoDetail);
        Task<List<F428SampleNoDetail>> GetMaterialNoBySampleNoForWarehouse4Excel(SF428SampleNoDetail sF428SampleNoDetail);
        Task<List<StockDetailByMaterialNo>> GetStockDetailByMaterialNo(SF428SampleNoDetail sF428SampleNoDetail);
        
    }
}