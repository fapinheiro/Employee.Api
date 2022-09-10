using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Employee.Api.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Employee.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                select EmployeeId, EmployeeName, Department, DateOfJoin, PhotoFileName from dbo.Employee;";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee.Api.Models.Employee emp)
        {
            string query = @"
                insert into dbo.Employee values 
                (
                    '" + emp.EmployeeName + @"',
                    '" + emp.Department + @"',
                    '" + emp.DateOfJoin + @"',
                    '" + emp.PhotoFileName + @"'
                )";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpPut("{EmployeeId}")]
        public JsonResult Put(int EmployeeId, [FromBody] Employee.Api.Models.Employee emp)
        {
            string query = @"
                update dbo.Employee set
                    EmployeeName = '" + emp.EmployeeName + @"',
                    Department = '" + emp.Department + @"',
                    DateOfJoin = '" + emp.DateOfJoin + @"',
                    PhotoFileName = '" + emp.PhotoFileName + @"'
                where 
                    EmployeeId = " + EmployeeId + @"";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{EmployeeId}")]
        public JsonResult Delete(int EmployeeId)
        {
            string query = @"
                delete from dbo.Employee
                where 
                    EmployeeId = " + EmployeeId + @"";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        [Route("photo")]
        [HttpPost]
        public JsonResult SavePhoto()
        {
            // Get file from request
            var httpRequest = Request.Form;
            var postedFile = httpRequest.Files[0];
            string fileName = postedFile.FileName;
            var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

            // Save file locally
            try
            {
                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);
            }
            catch (Exception)
            {
                return new JsonResult("Error saving file " + fileName);
            }

        }
    }
}