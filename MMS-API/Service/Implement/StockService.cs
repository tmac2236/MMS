using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API.Data.Interface.MMS;
using API.Helpers;
using API.Models.MMS;
using Aspose.Cells;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

namespace MMS_API.Service.Implement
{
    public class StockService : IStockService
    {
        private ILogger<StockService> _logger;
        private readonly IStockBasicDAO _stockBasicDAO;
        private readonly IMonthReportDAO _monthReportDAO;
        private readonly IQuarterReportDAO _quarterReportDAO;
        private readonly IServicePoolDAO _servicePoolDAO;
        
        
        //constructor
        public StockService(ILogger<StockService> logger,IStockBasicDAO stockBasicDAO, IMonthReportDAO monthReportDAO, IQuarterReportDAO quarterReportDAO, IServicePoolDAO servicePoolDAO)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stockBasicDAO = stockBasicDAO;
            _monthReportDAO = monthReportDAO;
            _quarterReportDAO = quarterReportDAO;
            _servicePoolDAO = servicePoolDAO;
        }

        //月營收上市 add security monthly by year and month e.g. 202103
        public async Task<string> AddSeStockMonthRevenue(string yearMonth)
        {
            try
            {
                _logger.LogInformation( String.Format(@"****** AddSeStockMonthRevenue  fired!! Parameter: {0} ******", yearMonth) );
                var quarter = Extensions.GetYearQ(yearMonth);
                
                string url = String.Format("https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F04%2F003%2F{0}_C04003.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E6%9C%88%E5%A0%B1&l2=%E3%80%90%E5%9C%8B%E5%85%A7%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E7%87%9F%E6%A5%AD%E6%94%B6%E5%85%A5%E5%BD%99%E7%B8%BD%E8%A1%A8%E3%80%91%E6%9C%88%E5%A0%B1", yearMonth);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(deCompressByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> stockList = new List<StockBasic>();
                List<StockBasic> stockListU = new List<StockBasic>();
                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().AsNoTracking().ToList();
                List<MonthReport> mReportList = new List<MonthReport>();
                List<MonthReport> dbMReportList = _monthReportDAO.FindAll().Where( x => x.YearMonth == yearMonth.Trim() ).ToList();
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
                    string accDiff = cells[i, 7].Value.ToString(); //取得累計增減

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = stockCode;
                    stockModel.Name = codeNName[1];
                    stockModel.Size = 1; //上市
                    stockModel.AccDiffM = accDiff;
                    //if exist in db not add in addList
                    var s = dbStockList.Find(x => x.Id == stockModel.Id);
                    if ( s == null ){
                        stockList.Add(stockModel);
                    }else{
                        s.AccDiffM = accDiff;
                        stockListU.Add(s);
                    }
                     

                    MonthReport mReportModel = new MonthReport();
                    mReportModel.StockId = stockCode;
                    mReportModel.Revenue = monthValu.ToInt();
                    mReportModel.PreRevenue = preMonthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    mReportModel.YearQ = quarter;
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
                }
                if (stockListU.Count > 0)
                {
                    foreach (var model in stockListU)
                    {
                        _stockBasicDAO.Update(model);
                    }
                }   
                await _stockBasicDAO.SaveAll();

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
                _logger.LogError( String.Format("!!!!!!AddSeStockMonthRevenue have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "AddSeStockMonthRevenue";
                sp.SerParam = yearMonth;
                sp.OccTime = DateTime.Now;
                sp.Type = "M";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();
                return yearMonth;
            }
        }
        //月營收上櫃 add security monthly by year and month e.g. 202103
        public async Task<string> AddSe2StockMonthRevenue(string yearMonth)
        {
            try
            {
                _logger.LogInformation( String.Format(@"****** AddSe2StockMonthRevenue  fired!! Parameter: {0} ******", yearMonth) );
                var quarter = Extensions.GetYearQ(yearMonth);

                string url = String.Format("https://www.tpex.org.tw/storage/statistic/sales_revenue/O_{0}.xls", yearMonth);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                //byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(dataByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> stockList = new List<StockBasic>();
                List<StockBasic> stockListU = new List<StockBasic>();
                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().AsNoTracking().ToList();
                List<MonthReport> mReportList = new List<MonthReport>();
                List<MonthReport> dbMReportList = _monthReportDAO.FindAll().Where( x => x.YearMonth == yearMonth.Trim() ).ToList();
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
                    string accDiff = cells[i, 7].Value.ToString(); //取得累計增減
                    string[] codeNName = columA.Split("  ");

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = codeNName[0].ToInt();
                    stockModel.Name = codeNName[1];
                    stockModel.Size = 2; //上櫃
                    stockModel.AccDiffM = accDiff;
                    //if exist in db not add in addList
                    var s = dbStockList.Find(x => x.Id == stockModel.Id);
                    if ( s == null ){
                        stockList.Add(stockModel);
                    }else{
                        s.AccDiffM = accDiff;
                        stockListU.Add(s);
                    }    


                    MonthReport mReportModel = new MonthReport();
                    mReportModel.StockId = codeNName[0].ToInt();
                    mReportModel.Revenue = monthValu.ToInt();
                    mReportModel.PreRevenue = preMonthValu.ToInt();
                    mReportModel.YearMonth = yearMonth.Trim();
                    mReportModel.YearQ = quarter;
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
                    string accDiff = cells[i, 7].Value.ToString(); //取得累計增減

                    string[] codeNName = columA.Split("  ");

                    StockBasic stockModel = new StockBasic();
                    stockModel.Id = codeNName[0].ToInt();
                    stockModel.Name = codeNName[1];
                    stockModel.Size = 2; //上櫃
                    stockModel.AccDiffM = accDiff;
                    //if exist in db not add in addList
                    var s = dbStockList.Find(x => x.Id == stockModel.Id);
                    if ( s == null ){
                        stockList.Add(stockModel);
                    }else{
                        s.AccDiffM = accDiff;
                        stockListU.Add(s);
                    }

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
                }
                if (stockListU.Count > 0)
                {
                    foreach (var model in stockListU)
                    {
                        _stockBasicDAO.Update(model);
                    }
                }
                await _stockBasicDAO.SaveAll();

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
                _logger.LogError( String.Format("!!!!!!AddSe2StockMonthRevenue have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "AddSe2StockMonthRevenue";
                sp.SerParam = yearMonth;
                sp.OccTime = DateTime.Now;
                sp.Type = "M";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();                
                return yearMonth;
            }

        }
        //季報上市 add security quarter year e.g. 2021Q2
        public async Task<string> AddSeStockQEps(string yearQ)
        {
            try
            {
                _logger.LogInformation( String.Format(@"****** AddSeStockQEps  fired!! Parameter: {0} ******", yearQ) );
                string url = String.Format("https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F05%2F001%2F{0}_C05001.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E5%AD%A3%E5%A0%B1&l2=%E3%80%90%E4%B8%8A%E5%B8%82%E8%82%A1%E7%A5%A8%E5%85%AC%E5%8F%B8%E8%B2%A1%E5%8B%99%E8%B3%87%E6%96%99%E7%B0%A1%E5%A0%B1%E3%80%91%E5%AD%A3%E5%A0%B1", yearQ);
                string formatYearQ = yearQ.Trim().Replace("Q", "");
                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(deCompressByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;

                List<StockBasic> dbStockList = _stockBasicDAO.FindAll().AsNoTracking().ToList();
                List<QuarterReport> qReportList = new List<QuarterReport>();
                List<QuarterReport> dbQuarterReportList = _quarterReportDAO.FindAll().Where( x => x.YearQ == formatYearQ ).ToList();
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

                    qReportModel.YearQ = formatYearQ; 
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
                    if (!dbQuarterReportList.Any(x => x.StockId == qReportModel.StockId && x.YearQ == formatYearQ)) qReportList.Add(qReportModel);
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
                _logger.LogError( String.Format("!!!!!!AddSeStockQEps have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "AddSeStockQEps";
                sp.SerParam = yearQ;
                sp.OccTime = DateTime.Now;
                sp.Type = "Q";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();                               
                return yearQ;
            }

        }
        //季報上櫃 add security quarter year e.g. 2021Q2
        public async Task<string> AddSe2StockQEps(string yearQ)
        {
            try
            {
                _logger.LogInformation( String.Format(@"****** AddSe2StockQEps  fired!! Parameter: {0} ******", yearQ) );
                string url = String.Format("https://www.tpex.org.tw/storage/statistic/financial/O_{0}.xls", yearQ);
                string formatYearQ = yearQ.Trim().Replace("Q", "");

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                //byte[] deCompressByte = Extensions.Decompress(dataByte);
                MemoryStream stream = new MemoryStream(dataByte);

                Workbook wb = new Workbook(stream);
                Worksheet worksheet = wb.Worksheets[0];
                Cells cells = worksheet.Cells;


                List<QuarterReport> qReportList = new List<QuarterReport>();
                List<QuarterReport> dbQuarterReportList = _quarterReportDAO.FindAll().Where( x => x.YearQ == formatYearQ ).ToList();
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
                    qReportModel.YearQ = formatYearQ; 
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
                    if (!dbQuarterReportList.Any(x => x.StockId == qReportModel.StockId && x.YearQ == formatYearQ )) qReportList.Add(qReportModel);
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
            catch (Exception ex)
            {
                _logger.LogError( String.Format("!!!!!!AddSe2StockQEps have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "AddSe2StockQEps";
                sp.SerParam = yearQ;
                sp.OccTime = DateTime.Now;
                sp.Type = "Q";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();                               
                return yearQ;
            }                

        }

        //上市公司每日收盤行情 
        //https://www.twse.com.tw/exchangeReport/BWIBBU_d?response=csv&date=20211207&selectType=ALL(殖利率)
        //https://www.twse.com.tw/exchangeReport/MI_INDEX?response=csv&date=20211213&type=ALL
        // date = 20211207
        //"證券代號",   0
        //"證券名稱",   1
        //"成交股數",   2   
        //"成交筆數",   3
        //"成交金額",   4
        //"開盤價",     5
        //"最高價",     6
        //"最低價",     7
        //"收盤價",     8
        //"漲跌(+/-)",  9
        //"漲跌價差",   10
        //"最後揭示買價",   11
        //"最後揭示買量",   12
        //"最後揭示賣價",   13
        //"最後揭示賣量",   14
        //"本益比",         15
        public async Task<string> GetSeDaily(string date)
        {
            try{
                _logger.LogInformation( String.Format(@"****** GetSeDaily(上市)  fired!! Parameter: {0} ******", date) );

                string url = String.Format("https://www.twse.com.tw/exchangeReport/MI_INDEX?response=csv&date={0}&type=ALL", date);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                MemoryStream stream = new MemoryStream(dataByte);

                List<StockBasic> dbStockBasicList = _stockBasicDAO.FindAll().Where( x =>x.Size == 1 ).ToList();

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using(StreamReader reader = new StreamReader(stream,Encoding.GetEncoding("Big5"))){

                    while (!reader.EndOfStream)
                    {
                    var line = reader.ReadLine();
                    var values = line.Split("\","); 
                        values = values.Select( x=> x.Replace("\"", "")).ToArray();
                    var code = values[0].Replace("\"", "").ToInt();    
                    if(code < 1000) continue;
                        StockBasic model = dbStockBasicList.FirstOrDefault(x =>x.Id == code);
                        if( model != null ){
                            model.ClosingPrice = String.Format("{0:0.##}", values[8]).ToDecimal();  //取代小數點後兩位
                            model.Upday = date;
                        } 
            
                    }
                }

                string result = "";

                if (dbStockBasicList.Count > 0)
                {
                    foreach (var model in dbStockBasicList)
                    {
                        _stockBasicDAO.Update(model);
                    }
                    if (!await _stockBasicDAO.SaveAll()) result = date;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError( String.Format("!!!!!!GetSeDaily(上市) have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "GetSeDaily";
                sp.SerParam = date;
                sp.OccTime = DateTime.Now;
                sp.Type = "D";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();                               
                return date;
            }      

        }

        //https://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_result.php?l=zh-tw&o=csv&d=110/12/14
        //代號  0
        //名稱  1
        //收盤  2
        //漲跌  3
        //開盤  4
        //最高  5
        //最低  6
        //均價  7
        //成交股數      8
        //成交金額(元)  9
        //成交筆數      10
        //最後買價      11
        //最後買量(千股) 12
        //最後賣價      13
        //最後賣量(千股) 14
        //發行股數      15
        //次日參考價    16
        //次日漲停價    17
        //次日跌停價    18
        //20211207-->  110/12/14
        public async Task<string> GetSe2Daily(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", 
                                  CultureInfo.InvariantCulture);
            string date2 = dt.AddYears(-1911).ToString("yyy/MM/dd");
            try{
                _logger.LogInformation( String.Format(@"****** GetSe2Daily(上櫃)  fired!! Parameter: {0} ******", date) );

                string url = String.Format("https://www.tpex.org.tw/web/stock/aftertrading/daily_close_quotes/stk_quote_result.php?l=zh-tw&o=csv&d={0}", date2);

                WebClient webClient = new WebClient();
                byte[] dataByte = webClient.DownloadData(url);
                MemoryStream stream = new MemoryStream(dataByte);

                List<StockBasic> dbStockBasicList = _stockBasicDAO.FindAll().Where( x =>x.Size == 2 ).ToList();

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using(StreamReader reader = new StreamReader(stream,Encoding.GetEncoding("Big5"))){

                    while (!reader.EndOfStream)
                    {
                    var line = reader.ReadLine();
                    var values = line.Split("\","); 
                        values = values.Select( x=> x.Replace("\"", "")).ToArray();
                    var code = values[0].Replace("\"", "").ToInt();    
                    if(code < 1000) continue;
                        StockBasic model = dbStockBasicList.FirstOrDefault(x =>x.Id == code);
                        if( model != null ){
                            model.ClosingPrice = String.Format("{0:0.##}", values[2]).ToDecimal();  //取代小數點後兩位
                            model.Upday = date;
                        } 
            
                    }
                }

                string result = "";

                if (dbStockBasicList.Count > 0)
                {
                    foreach (var model in dbStockBasicList)
                    {
                        _stockBasicDAO.Update(model);
                    }
                    if (!await _stockBasicDAO.SaveAll()) result = date2;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError( String.Format("!!!!!!GetSe2Daily(上櫃) have a exception  EMessage:{0}",ex.StackTrace) );
                ServicePool sp = new ServicePool();
                sp.SerName = "GetSe2Daily";
                sp.SerParam = date2;
                sp.OccTime = DateTime.Now;
                sp.Type = "D";
                sp.Emessage = ex.Message ;
                sp.Code = 0 ;
                _servicePoolDAO.Add(sp);
                await _servicePoolDAO.SaveAll();                               
                return date2;
            } 

        }
       public async Task DoUndoTaskByServicePool()
       {
           _logger.LogInformation( String.Format(@"****** DoUndoTaskByServicePool  fired!!******") );
           //find the undo tasks
           var data = await _servicePoolDAO.FindAll(x =>x.Code == 0 ).AsNoTracking().ToListAsync();
            if(data.Count == 0) {
                _logger.LogInformation( String.Format(@"------ No task to process  ------") );
                return;
            }
            foreach (var item in data) {

                _logger.LogInformation( String.Format(@"------ Processing: {0},{1}  ------",item.SerName,item.SerParam) );

                switch (item.SerName)
                {
                    case "AddSeStockMonthRevenue": 
                        await this.AddSeStockMonthRevenue(item.SerParam);
                        item.Code = 1;
                        break;
                    case "AddSe2StockMonthRevenue":
                        await this.AddSe2StockMonthRevenue(item.SerParam);
                        item.Code = 1;              
                        break;
                    case "AddSeStockQEps": 
                        await this.AddSeStockQEps(item.SerParam);
                        item.Code = 1;                                           
                        break;
                    case "AddSe2StockQEps": 
                        await this.AddSe2StockQEps(item.SerParam);
                        item.Code = 1;                                            
                        break;
                    case "GetSeDaily": 
                        await this.GetSeDaily(item.SerParam);
                        item.Code = 1;                                        
                        break;
                    case "GetSe2Daily": 
                        await this.GetSe2Daily(item.SerParam);
                        item.Code = 1;                                        
                        break;                                          
                }
                 _logger.LogInformation( String.Format(@"------ Finished: {0},{1}  ------",item.SerName,item.SerParam) );
                _servicePoolDAO.Update(item);
                await _servicePoolDAO.SaveAll();       
            }  

                
        }

        public async Task<string> CountEstiEps(string date)
        {
            _logger.LogInformation( String.Format(@"****** CountEstiEps  fired!!******") );
            var q = _quarterReportDAO.GetTop4Eps();
            var b = await _stockBasicDAO.FindAll().AsNoTracking().ToListAsync();
            var m = await _monthReportDAO.FindAll().AsNoTracking().ToListAsync();
            
            foreach(var ab in b){
                //use in dev 
                //if(ab.Id)
            }


            string result = "";
            return result;
        }
    }
}