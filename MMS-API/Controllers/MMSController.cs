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

namespace API.Controllers
{
    public class MMSController : ApiController
    {
        private readonly IStockBasicDAO _stockBasicDAO;
        private readonly IMonthReportDAO _monthReportDAO;


        public MMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment,
         IStockBasicDAO stockBasicDAO, IMonthReportDAO monthReportDAO)
         : base(config, webHostEnvironment)
        {
            _stockBasicDAO = stockBasicDAO;
            _monthReportDAO = monthReportDAO;
        }

        //上市公司  e.g yearMonth = 202103
        [HttpGet("addSeStockMonthRevenue")]
        public async Task<IActionResult> AddSeStockMonthRevenue(string yearMonth)
        {
            try
            {
                string url = String.Format("https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F04%2F003%2F{0}_C04003.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E6%9C%88%E5%A0%B1&l2=%E3%80%90%E5%9C%8B%E5%85%A7%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E7%87%9F%E6%A5%AD%E6%94%B6%E5%85%A5%E5%BD%99%E7%B8%BD%E8%A1%A8%E3%80%91%E6%9C%88%E5%A0%B1", yearMonth);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(deCompressByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> stockList = new List<StockBasic>();
                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().ToList();
                List<MonthReport> mReportList = new List<MonthReport>();
                List<MonthReport> dbMReportList = _monthReportDAO.FindAll().ToList();
                string typeName = "";
                int startIndex = 10;
                int last_row = worksheet.Cells.GetLastDataRow(1) - 2; //最後列不要(總額/平均)

                //to save each value to List, 10 is 水泥工業類
                for (int i = startIndex; i <= last_row; i++)
                {
                    string columA = cells[i, 0].Value.ToString().Trim();    //取得類別
                    int stockCode = columA.Split("  ")[0].ToInt();
                    if (stockCode == 0) //篩選掉英文類別
                    {
                        continue;
                    }
                    if (stockCode < 100) //篩選掉中文類別
                    {
                        typeName = columA;
                        continue;
                    }
                    string[] codeNName = columA.Split("  ");
                    long monthValu = cells[i, 2].Value.ToLong(); //get 本月 val

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = stockCode;
                    stockModel.Name = codeNName[1];
                    stockModel.Type = typeName;
                    stockModel.Size = 1; //上市
                    //if exist in db not add in addList
                    if (!dbStockList.Any(x => x.Id == stockModel.Id)) stockList.Add(stockModel);

                    MonthReport mReportModel = new MonthReport();
                    mReportModel.StockId = stockCode;
                    mReportModel.Revenue = monthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim() )) mReportList.Add(mReportModel);

                }
                bool result = true;
                if (stockList.Count > 0)
                {
                    foreach (var model in stockList)
                    {
                        _stockBasicDAO.Add(model);
                    }
                    result = await _stockBasicDAO.SaveAll();
                }
                if (mReportList.Count > 0)
                {
                    foreach (var model in mReportList)
                    {
                        _monthReportDAO.Add(model);
                    }
                    result = await _monthReportDAO.SaveAll();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }

        //上櫃公司  e.g yearMonth = 202103
        [HttpGet("addSe2StockMonthRevenue")]
        public async Task<IActionResult> AddSe2StockMonthRevenue(string yearMonth)
        {
            try
            {
                string url = String.Format("https://www.tpex.org.tw/storage/statistic/sales_revenue/O_{0}.xls", yearMonth);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                //byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(dataByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> stockList = new List<StockBasic>();
                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().ToList();
                List<MonthReport> mReportList = new List<MonthReport>();
                List<MonthReport> dbMReportList = _monthReportDAO.FindAll().ToList();
                string typeName = "";
                int startIndex = 10;
                int last_row = cells.MaxDataRow - 5;

                //to save each value to List, 10 is 水泥工業類
                for (int i = startIndex; i <= last_row; i++)
                {
                    string columA = cells[i, 0].Value.ToString().Trim().Replace(" ", "  ");    //取得類別
                    if (columA.Split("  ")[0].ToInt() < 100)    //篩選掉中文類別
                    {
                        typeName = columA;
                        continue;
                    }

                    long monthValu = cells[i, 2].Value.ToLong(); //get 本月 val

                    string[] codeNName = columA.Split("  ");

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = codeNName[0].ToInt();
                    stockModel.Name = codeNName[1];
                    stockModel.Type = typeName;
                    stockModel.Size = 2; //上櫃
                    //if exist in db not add in addList
                    if (!dbStockList.Any(x => x.Id == stockModel.Id)) stockList.Add(stockModel);

                    MonthReport mReportModel = new MonthReport();
                    mReportModel.StockId = codeNName[0].ToInt();
                    mReportModel.Revenue = monthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim() )) mReportList.Add(mReportModel);
                }
                //第二頁
                Worksheet worksheet2 = wb.Worksheets[1];
                Cells cells2 = worksheet2.Cells;
                int last_row2 = cells2.MaxDataRow - 5;
                for (int i = startIndex; i <= last_row2; i++)
                {
                    string columA = cells2[i, 0].Value.ToString().Trim().Replace(" ", "  ");    //取得類別
                    if (columA.Split("  ")[0].ToInt() < 100)    //篩選掉中文類別
                    {
                        typeName = columA;
                        continue;
                    }

                    long monthValu = cells2[i, 2].Value.ToLong(); //get 本月 val

                    string[] codeNName = columA.Split("  ");

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = codeNName[0].ToInt();
                    stockModel.Name = codeNName[1];
                    stockModel.Type = typeName;
                    stockModel.Size = 2; //上櫃
                    //if exist in db not add in addList
                    if (!dbStockList.Any(x => x.Id == stockModel.Id)) stockList.Add(stockModel);

                    MonthReport mReportModel = new MonthReport();
                    mReportModel.StockId = codeNName[0].ToInt();
                    mReportModel.Revenue = monthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim() )) mReportList.Add(mReportModel);
                }

                bool result = true;
                if (stockList.Count > 0)
                {
                    foreach (var model in stockList)
                    {
                        _stockBasicDAO.Add(model);
                    }
                    result = await _stockBasicDAO.SaveAll();
                }
                if (mReportList.Count > 0)
                {
                    foreach (var model in mReportList)
                    {
                        _monthReportDAO.Add(model);
                    }
                    result = await _monthReportDAO.SaveAll();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }




    }
}