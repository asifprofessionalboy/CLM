using CLMSAPP.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol.Plugins;
using System.Numerics;

namespace CLMSAPP.Controllers
{
    public class MasterController : Controller
    {
        private readonly string _connectionString;

        public MasterController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
        }


        public async Task<IActionResult> VendorMaster()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var vendors = await connection.QueryAsync<AppVendorMaster>("SELECT * FROM App_VendorMaster");
                ViewBag.list = vendors;
                return View();
            }
        }



        [HttpPost]
        public async Task<IActionResult> VendorMaster(AppVendorMaster vendor)
        {



            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["error_msg"] = string.Join("<br>", errors); // Display errors

                // Reload Vendor List
                using (var connection = new SqlConnection(_connectionString))
                {
                    var vendors = await connection.QueryAsync<AppVendorMaster>("SELECT * FROM App_VendorMaster");
                    ViewBag.list = vendors;
                }

                return View(vendor);
            }



            var sql = @"INSERT INTO App_VendorMaster (ID, V_NAME, V_CODE, OWNERNAME, ADDRESS, EMAIL, PF_NO, ESI_NO, CONTACT_NO, FROM_DATE, TO_DATE, UPDATEDAT) 
                VALUES (@ID, @V_NAME,@V_CODE, @OWNERNAME, @ADDRESS, @EMAIL, @PF_NO, @ESI_NO, @CONTACT_NO, @FROM_DATE, @TO_DATE, @UPDATEDAT)";

            using (var connection = new SqlConnection(_connectionString))
            {
                vendor.Id = Guid.NewGuid();
                //venndor.CreatedOn = DateTime.Now;
                vendor.FROM_DATE = vendor.FROM_DATE == DateTime.MinValue ? DateTime.Today : vendor.FROM_DATE.Date;
                vendor.TO_DATE = vendor.TO_DATE == DateTime.MinValue ? DateTime.Today : vendor.TO_DATE.Date;
                vendor.UPDATEDAT = DateTime.Now; // Keeping time for updated tracking

                await connection.ExecuteAsync(sql, vendor);
            }
            TempData["insert_msg"] = "Record inserted successfully..";

            return RedirectToAction("VendorMaster");
        }

  
    
    
    
        public async Task<IActionResult> WorkOrderMaster()
        {

            using (var connection = new SqlConnection(_connectionString))
            
            {
                var WoMaster = await connection.QueryAsync<AppWorkOrderMaster>("Select WO_NO,VendorCode,VendorName,StartDate,EndDate,WoStatus from App_WorkOrderMaster");
               ViewBag.list = WoMaster;
                return View();
            }
        
        }



        [HttpPost]
        public async Task<IActionResult> WorkOrderMaster(AppWorkOrderMaster workorder)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["error_msg"] = string.Join("<br>", errors); // Display errors

                // Reload Vendor List
                using (var connection = new SqlConnection(_connectionString))
                {
                    var workoder = await connection.QueryAsync<AppVendorMaster>("SELECT * FROM App_WorkOrderMaster");
                    ViewBag.list = workoder;
                }

                return View(workorder);
            }



            var sql = @"INSERT INTO App_WorkOrderMaster (ID, WO_NO, VendorCode, VendorName,LocationCode,LocationName,DeptCode,DeptName,StartDate,EndDate,WoStatus) 
                VALUES (@ID, @V_NAME,@V_CODE, @OWNERNAME, @ADDRESS, @EMAIL, @PF_NO, @ESI_NO, @CONTACT_NO, @FROM_DATE, @TO_DATE, @UPDATEDAT)";

            using (var connection = new SqlConnection(_connectionString))
            {
                workorder.ID = Guid.NewGuid();
                //venndor.CreatedOn = DateTime.Now;
                workorder.StartDate = workorder.StartDate == DateTime.MinValue ? DateTime.Today : workorder.StartDate.Date;
                workorder.EndDate = workorder.EndDate == DateTime.MinValue ? DateTime.Today : workorder.EndDate.Date;
                //workorder.UPDATEDAT = DateTime.Now; // Keeping time for updated tracking

                await connection.ExecuteAsync(sql, workorder);
            }
            TempData["insert_msg"] = "Record inserted successfully..";

            return RedirectToAction("WorkOrderMaster");
        }




    }
}
