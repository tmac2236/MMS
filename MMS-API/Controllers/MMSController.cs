using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using API.Data.Interface.MMS;
using API.Models.MMS;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using API.DTOs;
using System.Threading.Tasks;
using MMS_API.DTOs;
using API.Helpers;
using System.Collections.Generic;
using Aspose.Cells;
using System.Net;
using MMS_API.Service.Implement;

namespace API.Controllers
{
    public class MMSController : ApiController
    {
        private readonly IStockService _stockService;

        public MMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IStockService stockService
         )
         : base(config, webHostEnvironment)
        {
            _stockService = stockService;
        }

        //上市公司  e.g yearMonth = 202103
        [HttpGet("addSeStockMonthRevenue")]
        public async Task<IActionResult> AddSeStockMonthRevenue(string startDate, string endDate)
        {
            try
            {
                List<string> errorYearMonth = new List<string>();
                var dateList = Extensions.GetBetweenYearMonths(startDate,endDate);
                foreach(var date in dateList){
                    errorYearMonth.Add(await _stockService.AddSeStockMonthRevenue(date));
                }
                return Ok(errorYearMonth);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }

        //上櫃公司  e.g yearMonth = 202103
        [HttpGet("addSe2StockMonthRevenue")]
        public async Task<IActionResult> AddSe2StockMonthRevenue(string startDate, string endDate)
        {
            try
            {
                List<string> errorYearMonth = new List<string>();
                var dateList = Extensions.GetBetweenYearMonths(startDate,endDate);
                foreach(var date in dateList){
                    errorYearMonth.Add(await _stockService.AddSe2StockMonthRevenue(date));
                }
                return Ok(errorYearMonth);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }




    }
}