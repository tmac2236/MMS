using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using API.Data.Interface.CMS;

namespace API.Controllers
{
    public class CMSController : ApiController
    {
        private readonly IConfiguration _config;
        private readonly ICMSCarDAO _cMSCarDAO;
        private readonly ICMSCarManageRecordDAO _cMSCarManageRecordDAO;
        private readonly ICMSCompanyDAO _cMSCompanyDAO;
        private readonly ICMSDepartmentDAO _cMSDepartmentDAO;
        public CMSController(IConfiguration config, ICMSCarDAO cMSCarDAO,
         ICMSCarManageRecordDAO cMSCarManageRecordDAO, ICMSCompanyDAO cMSCompanyDAO, ICMSDepartmentDAO cMSDepartmentDAO)

        {

            _config = config;
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
    }
}