using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using API.Data.Interface.CMS;
using API.Models.CMS;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using API.DTOs;
using System.Threading.Tasks;
using CMS_API.DTOs;
using API.Helpers;
using System.Collections.Generic;

namespace API.Controllers
{
    public class CMSController : ApiController
    {
        private readonly ICMSCarDAO _cMSCarDAO;
        private readonly ICMSCarManageRecordDAO _cMSCarManageRecordDAO;
        private readonly ICMSCompanyDAO _cMSCompanyDAO;
        private readonly ICMSDepartmentDAO _cMSDepartmentDAO;
        public CMSController(IConfiguration config, IWebHostEnvironment webHostEnvironment, ICMSCarDAO cMSCarDAO,
         ICMSCarManageRecordDAO cMSCarManageRecordDAO, ICMSCompanyDAO cMSCompanyDAO, ICMSDepartmentDAO cMSDepartmentDAO)
         : base(config, webHostEnvironment)
        {
            _cMSCarDAO = cMSCarDAO;
            _cMSCarManageRecordDAO = cMSCarManageRecordDAO;
            _cMSCompanyDAO = cMSCompanyDAO;
            _cMSDepartmentDAO = cMSDepartmentDAO;

        }
        [HttpPost("addOrUpdateCompanyList")]
        public async Task<IActionResult> addOrUpdateCompanyList(List<Company> companyList)
        {
            try
            {
                var updateList = companyList.Where(x => x.Id != 0).ToList();
                var addList = companyList.Where(x => x.Id == 0).ToList();
                updateList.ForEach(m =>
                {
                    _cMSCompanyDAO.Update(m);
                });
                await _cMSCompanyDAO.SaveAll();
                //取出最後一個Id
                int lastId = _cMSCompanyDAO.FindAll().OrderByDescending(m => m.Id).FirstOrDefault().Id;
                addList.ForEach(m =>
                {
                    lastId++;
                    m.Id = lastId;
                    _cMSCompanyDAO.Add(m);
                });

                await _cMSCompanyDAO.SaveAll();
                
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("addOrUpdateCarList")]
        public async Task<IActionResult> addOrUpdateCarList(List<Car> carList)
        {
            try
            {
                var updateList = carList.Where(x => x.Id != 0).ToList();
                var addList = carList.Where(x => x.Id == 0).ToList();
                updateList.ForEach(m =>
                {
                    _cMSCarDAO.Update(m);
                });
                await _cMSCarDAO.SaveAll();
                //取出最後一個Id
                int lastId = _cMSCarDAO.FindAll().OrderByDescending(m => m.Id).FirstOrDefault().Id;
                addList.ForEach(m =>
                {
                    lastId++;
                    m.Id = lastId;
                    _cMSCarDAO.Add(m);
                });

                await _cMSCarDAO.SaveAll();
                
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
        }
        [HttpPost("addOrUpdateDepartmentList")]
        public async Task<IActionResult> addOrUpdateDepartmentList(List<Department> departmentList)
        {
            try
            {
                var updateList = departmentList.Where(x => x.Id != 0).ToList();
                var addList = departmentList.Where(x => x.Id == 0).ToList();
                updateList.ForEach(m =>
                {
                    _cMSDepartmentDAO.Update(m);
                });
                await _cMSDepartmentDAO.SaveAll();
                //取出最後一個Id
                int lastId = _cMSDepartmentDAO.FindAll().OrderByDescending(m => m.Id).FirstOrDefault().Id;
                addList.ForEach(m =>
                {
                    lastId++;
                    m.Id = lastId;
                    _cMSDepartmentDAO.Add(m);
                });

                await _cMSDepartmentDAO.SaveAll();
                
                return Ok(true);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}.");
            }
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
        // Get the record by liciense and the last date of SignInDate
        [HttpPost("getLastRecord")]
        public IActionResult getLastRecord(CarManageRecord carManageRecord)
        {
            try
            {
                var theModel = _cMSCarManageRecordDAO
                    .FindAll(x => x.LicenseNumber == carManageRecord.LicenseNumber)
                     .OrderByDescending(x => x.SignInDate).Take(1).ToList().FirstOrDefault();
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
        public async Task<IActionResult> ExportReport(SCarManageRecordDto sCarManageRecordDto)
        {

            if (sCarManageRecordDto.SignInDateS == "" || sCarManageRecordDto.SignInDateS == null) sCarManageRecordDto.SignInDateS = _config.GetSection("LogicSettings:MinDate").Value;
            if (sCarManageRecordDto.SignInDateE == "" || sCarManageRecordDto.SignInDateE == null) sCarManageRecordDto.SignInDateE = _config.GetSection("LogicSettings:MaxDate").Value;


            var data = await _cMSCarManageRecordDAO.GetCarManageRecordDto(sCarManageRecordDto);

            byte[] result = CommonExportReport(data, "TempCarRecord.xlsx");

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
        public async Task<IActionResult> GetCarManageRecordDto([FromQuery] SCarManageRecordDto sCarManageRecordDto)
        {
            try
            {
                if (sCarManageRecordDto.SignInDateS == "" || sCarManageRecordDto.SignInDateS == null) sCarManageRecordDto.SignInDateS = _config.GetSection("LogicSettings:MinDate").Value;
                if (sCarManageRecordDto.SignInDateE == "" || sCarManageRecordDto.SignInDateE == null) sCarManageRecordDto.SignInDateE = _config.GetSection("LogicSettings:MaxDate").Value;

                var data = await _cMSCarManageRecordDAO.GetCarManageRecordDto(sCarManageRecordDto);
                PagedList<CarManageRecordDto> result = PagedList<CarManageRecordDto>.Create(data, sCarManageRecordDto.PageNumber, sCarManageRecordDto.PageSize, sCarManageRecordDto.IsPaging);
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