using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using API.Models.MMS;
using Aspose.Cells;
using HtmlAgilityPack;


namespace API.Helpers
{
    public class Test
    {
        public Test()
        {
            //StartCrawlerAsync();
            //DownloadFileFromUrl();
        }


        //Download From URL
        public void DownloadFileFromUrl()
        {
            string yearMonth = "202103";
            string url = String.Format("https://www.tpex.org.tw/storage/statistic/sales_revenue/O_{0}.xls", yearMonth);

            WebClient webClient = new WebClient();
            byte[] dataByte = webClient.DownloadData(url);
            //byte[] deCompressByte = Extensions.Decompress(dataByte);
            MemoryStream stream = new MemoryStream(dataByte);

            Workbook wb = new Workbook(stream);
            Worksheet worksheet = wb.Worksheets[0];
            Cells cells = worksheet.Cells;

            List<StockBasic> sList = new List<StockBasic>();
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

                monthValu.ToInt();
                StockBasic model = new StockBasic();
                model.Id = codeNName[0].ToInt();
                model.Name = codeNName[1];
                model.Type = typeName;
                model.Size = 2; //上櫃
                sList.Add(model);
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

                monthValu.ToInt();
                StockBasic model = new StockBasic();
                model.Id = codeNName[0].ToInt();
                model.Name = codeNName[1];
                model.Type = typeName;
                model.Size = 2; //上櫃
                sList.Add(model);
            }
        }
        //Crawler
        public async Task StartCrawlerAsync()
        {
            var url = "https://www.twse.com.tw/zh/page/trading/exchange/STOCK_DAY.html";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var divs = htmlDocument.DocumentNode.Descendants("Div").ToList();

        }
        public void GetPic()
        {
            string rootdir = Directory.GetCurrentDirectory();
            string folderPath = rootdir + "\\Resources\\ArticlePics";
            DirectoryInfo d = new DirectoryInfo(folderPath);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.jpg"); //Getting Text files
            List<string> fileNames = new List<string>();
            foreach (FileInfo file in Files)
            {
                fileNames.Add(file.Name);
            }
        }
    }
}