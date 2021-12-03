using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Data.Interface.MMS;
using API.Helpers;
using API.Models.MMS;
using Aspose.Cells;

namespace MMS_API.Service.Implement
{
    public class StockService : IStockService
    {
        private readonly IStockBasicDAO _stockBasicDAO;
        private readonly IMonthReportDAO _monthReportDAO;
        private readonly IQuarterReportDAO _quarterReportDAO;
        
        //constructor
        public StockService(IStockBasicDAO stockBasicDAO, IMonthReportDAO monthReportDAO, IQuarterReportDAO quarterReportDAO)
        {
            _stockBasicDAO = stockBasicDAO;
            _monthReportDAO = monthReportDAO;
            _quarterReportDAO = quarterReportDAO;
        }

        //月營收上市 add security monthly by year and month e.g. 202103
        public async Task<string> AddSeStockMonthRevenue(string yearMonth)
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
                    long preMonthValu = cells[i, 4].Value.ToLong(); //get 去年本月 val

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
                    mReportModel.PreRevenue = preMonthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    mReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim())) mReportList.Add(mReportModel);

                }
                string result = "";
                if (stockList.Count > 0)
                {
                    foreach (var model in stockList)
                    {
                        _stockBasicDAO.Add(model);
                    }
                    if (!await _stockBasicDAO.SaveAll()) result = yearMonth;
                }
                if (mReportList.Count > 0)
                {
                    foreach (var model in mReportList)
                    {
                        _monthReportDAO.Add(model);
                    }
                    if (!await _monthReportDAO.SaveAll()) result = yearMonth;
                }

                return result;
            }
            catch (Exception ex)
            {
                return yearMonth;
            }
        }
        //月營收上櫃 add security monthly by year and month e.g. 202103
        public async Task<string> AddSe2StockMonthRevenue(string yearMonth)
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

                    long monthValu = cells[i, 3].Value.ToLong(); //get 本月 val
                    long preMonthValu = cells[i, 5].Value.ToLong(); //get 去年本月 val
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
                    mReportModel.PreRevenue = preMonthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    mReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim())) mReportList.Add(mReportModel);
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

                    long monthValu = cells2[i, 3].Value.ToLong(); //get 本月 val
                    long preMonthValu = cells2[i, 5].Value.ToLong(); //get 去年本月 val

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
                    mReportModel.PreRevenue = preMonthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    mReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbMReportList.Any(x => x.StockId == mReportModel.StockId && x.YearMonth == yearMonth.Trim())) mReportList.Add(mReportModel);
                }

                string result = "";
                if (stockList.Count > 0)
                {
                    foreach (var model in stockList)
                    {
                        _stockBasicDAO.Add(model);
                    }
                    if (!await _stockBasicDAO.SaveAll()) result = yearMonth;
                }
                if (mReportList.Count > 0)
                {
                    foreach (var model in mReportList)
                    {
                        _monthReportDAO.Add(model);
                    }
                    if (!await _monthReportDAO.SaveAll()) result = yearMonth;
                }
                return result;
            }
            catch (Exception ex)
            {
                return yearMonth;
            }

        }
        //季報上市 add security quarter year e.g. 2021Q2
        public async Task<string> AddSeStockQEps(string yearQ)
        {
            try
            {
                string url = String.Format("https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F05%2F001%2F{0}_C05001.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E5%AD%A3%E5%A0%B1&l2=%E3%80%90%E4%B8%8A%E5%B8%82%E8%82%A1%E7%A5%A8%E5%85%AC%E5%8F%B8%E8%B2%A1%E5%8B%99%E8%B3%87%E6%96%99%E7%B0%A1%E5%A0%B1%E3%80%91%E5%AD%A3%E5%A0%B1", yearQ);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(deCompressByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().ToList();
                List<QuarterReport> qReportList = new List<QuarterReport>();
                List<QuarterReport> dbQuarterReportList = _quarterReportDAO.FindAll().ToList();
                string typeName = "";
                int startIndex = 9; //index從9開始
                int last_row = cells.MaxDataRow;

                //to save each value to List, 10 is 水泥工業類
                for (int i = startIndex; i <= last_row; i++)
                {
                    var oneCol = cells[i, 0].Value;
                    if(oneCol == null) continue;
                    string columA = oneCol.ToString().Replace(" ", "");    //因為如果是空白不能Trim
                    if (columA.ToInt() < 100)    //篩選掉空白或類別(01水泥類)
                    {
                        typeName = columA;
                        continue;
                    }
                    QuarterReport qReportModel = new QuarterReport();

                    qReportModel.StockId = oneCol.ToInt();

                    qReportModel.YearQ = yearQ.Trim().Replace("Q", ""); 
                    qReportModel.Eps = cells[i, 13].Value.ToDecimal();  //本季累計EPS
                    qReportModel.PreEps = cells[i, 14].Value.ToDecimal();   //去年本季累計EPS

                    //如果本季為Q1不用扣除上季累計EPS
                    if(qReportModel.YearQ[4] == '1'){
                        qReportModel.TheEps = qReportModel.Eps;
                    }else{
                        var lastQ = qReportModel.YearQ.ToInt() - 1;
                        var lastModel = dbQuarterReportList.FirstOrDefault(x => x.StockId == qReportModel.StockId && x.YearQ == lastQ.ToString());
                        if(lastModel != null){
                            qReportModel.TheEps = ( qReportModel.Eps - lastModel.Eps);
                        }
                    }
                    qReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbQuarterReportList.Any(x => x.StockId == qReportModel.StockId && x.YearQ == yearQ.Trim())) qReportList.Add(qReportModel);
                }

                string result = "";

                if (qReportList.Count > 0)
                {
                    foreach (var model in qReportList)
                    {
                        _quarterReportDAO.Add(model);
                    }
                    if (!await _quarterReportDAO.SaveAll()) result = yearQ;
                }
                return result;
            }
            catch (Exception ex)
            {
                return yearQ;
            }

        }
        //季報上櫃 add security quarter year e.g. 2021Q2
        public async Task<string> AddSe2StockQEps(string yearQ)
        {

                string url = String.Format("https://www.tpex.org.tw/storage/statistic/financial/O_{0}.xls", yearQ);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                //byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(dataByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;


                List<QuarterReport> qReportList = new List<QuarterReport>();
                List<QuarterReport> dbQuarterReportList = _quarterReportDAO.FindAll().ToList();
                string typeName = "";
                int startIndex = 9;
                int last_row = cells.MaxDataRow - 5;

                //to save each value to List, 10 is 水泥工業類
                for (int i = startIndex; i <= last_row; i++)
                {
                    var oneCol = cells[i, 0].Value;
                    if(oneCol == null) continue;
                    string columA = oneCol.ToString().Replace(" ", "");    //因為如果是空白不能Trim
                    if (columA.ToInt() < 100)    //篩選掉空白或類別(01水泥類)
                    {
                        typeName = columA;
                        continue;
                    }

                    QuarterReport qReportModel = new QuarterReport();
                    qReportModel.StockId = oneCol.ToInt();
                    qReportModel.YearQ = yearQ.Trim().Replace("Q", ""); 
                    qReportModel.Eps = cells[i, 13].Value.ToDecimal();  //本季累計EPS
                    qReportModel.PreEps = cells[i, 14].Value.ToDecimal();   //去年本季累計EPS
                    
                    //如果本季為Q1不用扣除上季累計EPS
                    if(qReportModel.YearQ[4] == '1'){
                        qReportModel.TheEps = qReportModel.Eps;
                    }else{
                        var lastQ = qReportModel.YearQ.ToInt() - 1;
                        var lastModel = dbQuarterReportList.FirstOrDefault(x => x.StockId == qReportModel.StockId && x.YearQ == lastQ.ToString());
                        if(lastModel != null){
                            qReportModel.TheEps = ( qReportModel.Eps - lastModel.Eps);
                        }
                    }
                    qReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbQuarterReportList.Any(x => x.StockId == qReportModel.StockId && x.YearQ == yearQ.Trim())) qReportList.Add(qReportModel);
                }
                //第二頁
                Worksheet worksheet2 = wb.Worksheets[1];
                Cells cells2 = worksheet2.Cells;
                int last_row2 = cells2.MaxDataRow - 5;
                for (int i = startIndex; i <= last_row2; i++)
                {
                    var oneCol = cells2[i, 0].Value;
                    if(oneCol == null) continue;
                    string columA = oneCol.ToString().Replace(" ", "");    //因為如果是空白不能Trim
                    if (columA.ToInt() < 100)    //篩選掉空白或類別(01水泥類)
                    {
                        typeName = columA;
                        continue;
                    }
                    
                    QuarterReport qReportModel = new QuarterReport();
                    qReportModel.StockId = oneCol.ToInt();
                    qReportModel.YearQ = yearQ.Trim().Replace("Q", ""); 
                    qReportModel.Eps = cells2[i, 13].Value.ToDecimal();  //本季累計EPS
                    qReportModel.PreEps = cells2[i, 14].Value.ToDecimal();   //去年本季累計EPS
                    
                    //如果本季為Q1不用扣除上季累計EPS
                    if(qReportModel.YearQ[4] == '1'){
                        qReportModel.TheEps = qReportModel.Eps;
                    }else{
                        var lastQ = qReportModel.YearQ.ToInt() - 1;
                        var lastModel = dbQuarterReportList.FirstOrDefault(x => x.StockId == qReportModel.StockId && x.YearQ == lastQ.ToString());
                        if(lastModel != null){
                            qReportModel.TheEps = ( qReportModel.Eps - lastModel.Eps);
                        }
                    }
                    qReportModel.UpdateTime = DateTime.Now;
                    //if exist in db not add in addList
                    if (!dbQuarterReportList.Any(x => x.StockId == qReportModel.StockId && x.YearQ == yearQ.Trim())) qReportList.Add(qReportModel);
                }

                string result = "";

                if (qReportList.Count > 0)
                {
                    foreach (var model in qReportList)
                    {
                        _quarterReportDAO.Add(model);
                    }
                    if (!await _quarterReportDAO.SaveAll()) result = yearQ;
                }
                return result;

        }        
    }
}