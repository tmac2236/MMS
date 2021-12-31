using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using API.Helpers;
using System.Collections.Generic;
using Aspose.Cells;
using System.Net;
using MMS_API.Service.Implement;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class MMSController : ApiController
    {
        private readonly IStockService _stockService;

        public MMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment, ILogger<MMSController> logger, IStockService stockService
         )
         : base(config, webHostEnvironment,logger)
        {
            _stockService = stockService;
        }
        //月營收
        //上市公司  e.g yearMonth = 202103
        [HttpGet("addSeStockMonthRevenue")]
        public async Task<IActionResult> AddSeStockMonthRevenue(string startDate, string endDate)
        {
            List<string> errorYearMonth = new List<string>();
            var dateList = Extensions.GetBetweenYearMonths(startDate,endDate);
            foreach(var date in dateList){
                errorYearMonth.Add(await _stockService.AddSeStockMonthRevenue(date));
            }
            return Ok(errorYearMonth);
        }
        //月營收
        //上櫃公司  e.g yearMonth = 202103
        [HttpGet("addSe2StockMonthRevenue")]
        public async Task<IActionResult> AddSe2StockMonthRevenue(string startDate, string endDate)
        {
            List<string> errorYearMonth = new List<string>();
            var dateList = Extensions.GetBetweenYearMonths(startDate,endDate);
            foreach(var date in dateList){
                errorYearMonth.Add(await _stockService.AddSe2StockMonthRevenue(date));
            }
            return Ok(errorYearMonth);
        }
        //季報
        //上市公司  e.g yearQ = 2021Q2
        [HttpGet("addSeStockQEps")]
        public async Task<IActionResult> AddSeStockQEps(string yearQ)
        {

            await _stockService.AddSeStockQEps(yearQ);
            return Ok();

        }        
        //季報
        //上櫃公司  e.g yearQ = 2021Q2
        [HttpGet("addSe2StockQEps")]
        public async Task<IActionResult> AddSe2StockQEps(string yearQ)
        {

            await _stockService.AddSe2StockQEps(yearQ);
            return Ok();

        } 
        //上市公司每日收盤行情 
        [HttpGet("getSeDaily")]
        public async Task<IActionResult> GetSeDaily(string date)
        {

            await _stockService.GetSeDaily(date);
            return Ok();

        }  
        //上櫃公司每日收盤行情 
        [HttpGet("getSe2Daily")]
        public async Task<IActionResult> GetSe2Daily(string date)
        {

            await _stockService.GetSe2Daily(date);
            return Ok();

        }
        [HttpGet("getTop4Eps")]
        public IActionResult GetTop4Eps()
        {
            _stockService.CountEstiEps("");
            return Ok();
        }          

    }
}