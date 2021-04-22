using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using API.Data.Interface.CMS;
using API.Models.CMS;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Aspose.Cells;
using API.DTOs;
using System.Threading.Tasks;
using CMS_API.DTOs;
using API.Helpers;

namespace API.Controllers
{
    public class CMSController : ApiController
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICMSCarDAO _cMSCarDAO;
        private readonly ICMSCarManageRecordDAO _cMSCarManageRecordDAO;
        private readonly ICMSCompanyDAO _cMSCompanyDAO;
        private readonly ICMSDepartmentDAO _cMSDepartmentDAO;
        public CMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment, ICMSCarDAO cMSCarDAO,
         ICMSCarManageRecordDAO cMSCarManageRecordDAO, ICMSCompanyDAO cMSCompanyDAO, ICMSDepartmentDAO cMSDepartmentDAO)

        {

            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _cMSCarDAO = cMSCarDAO;
            _cMSCarManageRecordDAO = cMSCarManageRecordDAO;
            _cMSCompanyDAO = cMSCompanyDAO;
            _cMSDepartmentDAO = cMSDepartmentDAO;

        }

        [HttpGet("getAllCarList")]
        public IActionResult GetAllCarList()
        {
            try
            {
                var result = _cMSCarDAO.FindAll().ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpGet("getAllCompany")]
        public IActionResult GetAllCompany()
        {
            try
            {
                var result = _cMSCompanyDAO.FindAll().ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpGet("getAllDepartment")]
        public IActionResult GetAllDepartment()
        {
            try
            {
                var result = _cMSDepartmentDAO.FindAll().ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("getTheRecord")]
        public IActionResult GetTheRecord(CarManageRecord carManageRecord)
        {
            try
            {
                var theModel = _cMSCarManageRecordDAO
                .FindSingle(x => x.LicenseNumber == carManageRecord.LicenseNumber && x.SignInDate == carManageRecord.SignInDate);
                return Ok(theModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("addRecord")]
        public async Task<IActionResult> AddRecord(CarManageRecord model)
        {
            try
            {
                //取到秒的Datetime
                //DateTime nowFormat = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                //                                  DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                model.SignInDate = Extensions.GetDateTimeNowInMillionSec();
                _cMSCarManageRecordDAO.Add(model);
                await _cMSCarManageRecordDAO.SaveAll();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("editRecord")]
        public async Task<IActionResult> EditRecord(CarManageRecord model)
        {
            try
            {  
                _cMSCarManageRecordDAO.Update(model);
                await _cMSCarManageRecordDAO.SaveAll();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("signOutRecord")]
        public async Task<IActionResult> SignOutRecord(CarManageRecord model)
        {
            try
            {
                model.SignOutDate = Extensions.GetDateTimeNowInMillionSec();
                _cMSCarManageRecordDAO.Update(model);
                await _cMSCarManageRecordDAO.SaveAll();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }

        [HttpPost("exportReport")]
        public IActionResult ExportReport()
        {
            //var data = await _reporDAO.GetReportDataPass(sReportDataPassDto);
            string rootStr = _webHostEnvironment.ContentRootPath;
            var path = Path.Combine(rootStr, "Resources\\Template\\(Marco)Test.xlsm");
            WorkbookDesigner designer = new WorkbookDesigner();
            designer.Workbook = new Workbook(path);
            Worksheet ws = designer.Workbook.Worksheets[0];
            //designer.SetDataSource("result", data);
            designer.Process();
            MemoryStream stream = new MemoryStream();
            designer.Workbook.Save(stream, SaveFormat.Xlsx);
            byte[] result = stream.ToArray();

            return File(result, "application/xlsx");
        }

        [HttpPost("addSignaturePic")]
        public async Task<IActionResult> AddSignaturePic([FromForm] DriverSinatureDto source)
        {
            try
            {
                string rootdir = Directory.GetCurrentDirectory();
                var localStr = _config.GetSection("AppSettings:ReportPics").Value;
                var pjName = _config.GetSection("AppSettings:ProjectName").Value;
                var pathToSave = rootdir + localStr;
                pathToSave = pathToSave.Replace(pjName + "-API", pjName + "-SPA");
                if (source.File.Length > 0)
                {
                    //檔名含副檔名
                    var formateDate = source.SignInDate.Replace(" ", "-").Replace(":", "-").Replace(".", "-");
                    var fileName = source.LicenseNumber + "_" + formateDate + ".jpg";
                    //新增檔名的全路徑
                    var fullPath = Path.Combine(pathToSave, fileName);
                    if (!Directory.Exists(pathToSave))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(pathToSave);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        source.File.CopyTo(stream);

                        DateTime dt = Convert.ToDateTime(source.SignInDate);
                        var theModel = _cMSCarManageRecordDAO.FindSingle(x => x.LicenseNumber == source.LicenseNumber && x.SignInDate == dt);
                        theModel.DriverSign = fileName;
                        await _cMSCarManageRecordDAO.SaveAll();
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("getCarManageRecordDto")]
        public IActionResult GetCarManageRecordDto([FromQuery] SCarManageRecordDto sCarManageRecordDto)
        {
            try
            {
                if (sCarManageRecordDto.SignInDateS == "" || sCarManageRecordDto.SignInDateS == null) sCarManageRecordDto.SignInDateS = _config.GetSection("LogicSettings:MinDate").Value;
                if (sCarManageRecordDto.SignInDateE == "" || sCarManageRecordDto.SignInDateE == null) sCarManageRecordDto.SignInDateE = _config.GetSection("LogicSettings:MaxDate").Value;

                var result = _cMSCarManageRecordDAO.GetCarManageRecordDto(sCarManageRecordDto);
                Response.AddPagination(result.CurrentPage, result.PageSize,
                result.TotalCount, result.TotalPages);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }


    }
}