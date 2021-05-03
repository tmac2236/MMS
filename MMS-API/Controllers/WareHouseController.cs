using System;
using System.Threading.Tasks;
using API.Data.Interface;
using API.DTOs;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using API.Models.DKS;
using System.Linq;
using API.Data.Interface.MMS;
using API.Data.Interface.DKS;
using Microsoft.AspNetCore.Hosting;

namespace API.Controllers
{
    public class WareHouseController : ApiController
    {
        private readonly IWarehouseDAO _warehouseDao;
        private readonly ISamPartBDAO _samPartBDAO;
        private readonly ICMSCarDAO _cMSCarDAO;
        public WareHouseController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IWarehouseDAO warehouseDao, ISamPartBDAO samPartBDAO, ICMSCarDAO cMSCarDAO)
                 : base(config, webHostEnvironment)
        {
            _warehouseDao = warehouseDao;
            _samPartBDAO = samPartBDAO;
            _cMSCarDAO = cMSCarDAO;
        }
        [HttpGet("getMaterialNoBySampleNoForWarehouse")]
        public IActionResult GetMaterialNoBySampleNoForWarehouse([FromQuery] SF428SampleNoDetail sF428SampleNoDetail)
        {
            try
            {
                //sF428SampleNoDetail.SampleNo ="FW21-SMS-GZ7884-01";
                var model = _cMSCarDAO.FindAll().ToList();
                var result = _warehouseDao.GetMaterialNoBySampleNoForWarehouse(sF428SampleNoDetail);

                Response.AddPagination(result.CurrentPage, result.PageSize,
                result.TotalCount, result.TotalPages);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("getStockDetailByMaterialNo")]
        public async Task<IActionResult> GetStockDetailByMaterialNo(SF428SampleNoDetail sF428SampleNoDetail)
        {
            var data = await _warehouseDao.GetStockDetailByMaterialNo(sF428SampleNoDetail);
            return Ok(data);

        }
        [HttpPost("addStockDetailByMaterialNo")]
        public async Task<IActionResult> AddStockDetailByMaterialNo(SF428SampleNoDetail sF428SampleNoDetail)
        {
            SamPartB model = await _samPartBDAO.FindAll(x => x.SAMPLENO.Trim() == sF428SampleNoDetail.SampleNo.Trim() &&
                                                 x.MATERIANO == sF428SampleNoDetail.MaterialNo.Trim()).FirstOrDefaultAsync();
            if (model != null)
            {
                model.STATUS = sF428SampleNoDetail.Status;
                model.CHKSTOCKNO = sF428SampleNoDetail.ChkStockNo;
                model.CHKUSR = sF428SampleNoDetail.loginUser;
                model.CHKTIME = DateTime.Now;
                _samPartBDAO.Update(model);
            }
            await _samPartBDAO.SaveAll();
            return Ok();

        }
    }
}