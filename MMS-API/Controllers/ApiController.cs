using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using API.Filters;
using Aspose.Cells;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ApiActionFilter))]
    [TypeFilter(typeof(ApiExceptionFilter))]
    public class ApiController : ControllerBase
    {
        protected readonly IConfiguration _config;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected ILogger<ApiController> _logger;
        //constructor
        public ApiController(IConfiguration config, IWebHostEnvironment webHostEnvironment,ILogger<ApiController> logger)
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        protected byte[] CommonExportReport(object data, string templateName)
        {

            string rootStr = _webHostEnvironment.ContentRootPath;
            var path = Path.Combine(rootStr, "Resources\\Template\\" + templateName);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];
            designer.SetDataSource("result", data);
            designer.Process();
            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            return stream.ToArray(); ;
        }
        protected byte[] CommonExportReportTabs(List<object> dataList, string templateName)
        {

            string rootStr = _webHostEnvironment.ContentRootPath;
            var path = Path.Combine(rootStr, "Resources\\Template\\" + templateName);
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            int index = 0;
            foreach (object data in dataList)
            {
                Worksheet ws = designer.Workbook.Worksheets[index];
                designer.SetDataSource("result", data);
                index++;
            }
            designer.Process();
            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);

            return stream.ToArray(); ;
        }
        //儲存檔案到Server
        //file:檔案 
        //settingNam: root資料夾名稱
        //fileName: 檔案名稱
        protected async Task<Boolean> SaveFiletoServer(IFormFile file, string settingNam, string fileName)
        {
            Boolean isSuccess = false;
            try
            {
                string rootdir = Directory.GetCurrentDirectory();
                var localStr = _config.GetSection("AppSettings:" + settingNam).Value;
                var pjName = _config.GetSection("AppSettings:ProjectName").Value;
                var pathToSave = rootdir + localStr;
                pathToSave = pathToSave.Replace(pjName + "-API", pjName + "-SPA");
                if (file.Length > 0)
                {

                    //新增檔名的全路徑
                    var fullPath = Path.Combine(pathToSave, fileName);
                    if (!Directory.Exists(pathToSave))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathToSave);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                return isSuccess;
            }

            return isSuccess;
        }
    }
}