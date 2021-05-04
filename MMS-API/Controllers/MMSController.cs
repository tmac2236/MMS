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

        public MMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment, IStockBasicDAO stockBasicDAO)
         : base(config, webHostEnvironment)
        {
            _stockBasicDAO = stockBasicDAO;

        }
        [HttpGet("getStockCodeAndName")]
        public async Task<IActionResult> GetStockCodeAndName()
        {
            try
            {
                string url = "https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F04%2F003%2F202103_C04003.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E6%9C%88%E5%A0%B1&l2=%E3%80%90%E5%9C%8B%E5%85%A7%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E7%87%9F%E6%A5%AD%E6%94%B6%E5%85%A5%E5%BD%99%E7%B8%BD%E8%A1%A8%E3%80%91%E6%9C%88%E5%A0%B1";

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(deCompressByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> sList = new List<StockBasic>();
                string typeName = "";
                int startIndex = 10;
                int last_row = worksheet.Cells.GetLastDataRow(1) - 2; //最後列不要(總額/平均)

                //to save each value to List, 10 is 水泥工業類
                for (int i = startIndex; i <= last_row; i++)
                {
                    long monthValu = cells[i, 2].Value.ToLong(); //get 本月 val
                    if (monthValu == 0)
                    {
                        continue;
                    }
                    string columA = cells[i, 0].Value.ToString().Trim();    //取得類別
                    if (columA.Last() == '類')
                    {
                        typeName = columA;
                        continue;
                    }
                    string[] codeNName = cells[i, 0].Value.ToString().Trim().Split("  ");

                    monthValu.ToInt();
                    StockBasic model = new StockBasic();
                    model.Id = codeNName[0].ToInt();
                    model.Name = codeNName[1];
                    model.Type = typeName;
                    sList.Add(model);
                }
                foreach (var model in sList)
                {
                    _stockBasicDAO.Add(model);
                }
                bool result = await _stockBasicDAO.SaveAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }





    }
}