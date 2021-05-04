using System;
using System.Collections.Generic;
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
        }


        //Download From URL
        public void DownloadFileFromUrl()
        {
            string url = "https://www.twse.com.tw/statistics/count?url=%2FstaticFiles%2Finspection%2Finspection%2F04%2F003%2F202103_C04003.zip&l1=%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E6%9C%88%E5%A0%B1&l2=%E3%80%90%E5%9C%8B%E5%85%A7%E4%B8%8A%E5%B8%82%E5%85%AC%E5%8F%B8%E7%87%9F%E6%A5%AD%E6%94%B6%E5%85%A5%E5%BD%99%E7%B8%BD%E8%A1%A8%E3%80%91%E6%9C%88%E5%A0%B1";
            //string savePath = @"D:\Demo\FreeImages.jpg";
            WebClient webClient = new WebClient();
            byte[] dataByte = webClient.DownloadData(url);
            byte[] deCompressByte = Extensions.Decompress(dataByte);
            MemoryStream stream = new MemoryStream(deCompressByte);

            Workbook wb = new Workbook(stream);
            //Workbook wb = new Workbook(@"D:\20202103.XLS");
            Worksheet worksheet = wb.Worksheets[0];
            Cells cells = worksheet.Cells;

            List<StockBasic> sList = new List<StockBasic>();
            string typeName = "";
            int startIndex = 10;
            int last_row = worksheet.Cells.GetLastDataRow(1);

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