using CLMSAPP.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace CLMSAPP.Controllers
{
    public class VendorController : Controller
    {
        private readonly string _connectionString;

        public VendorController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("dbcs");
        }


        public async Task<IActionResult> VendorProfileEntry()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                ViewBag.user = HttpContext.Session.GetString("Username");

                var vendors = await connection.QueryAsync<AppVendorReg>(
                    "SELECT * FROM App_Vendor_Reg WHERE V_CODE = @UserCode",
                       new { UserCode = ViewBag.user } 
                    );
                ViewBag.list = vendors;
            

                // Get previous details based on session Username
                var prevDetails = await connection.QueryFirstOrDefaultAsync<AppVendorReg>(
                    "SELECT V_NAME, OWNERNAME As OWNER_NAME, ADDRESS, CONTACT_NO AS PHONE_NO FROM App_VendorMaster WHERE V_CODE = @UserCode",
                    new { UserCode = ViewBag.user } // Pass session value as a parameter
                );

                ViewBag.prevDetails = prevDetails; // Pass the first record to ViewBag

                return View();
            }
        }



        [HttpPost]
        public async Task<IActionResult> VendorProfileEntry(AppVendorReg vendor)
        {






            var sql = @"INSERT INTO App_Vendor_Reg
    (
        V_NAME,
        V_CODE, 
        NATURE_OF_WORK,
        ADDRESS,
        EMAIL,
        PHONE_NO,
        OWNER_NAME,
        OWNER_ADHAR_NUMBER,
        PF_CODE_NUMBER,
        ESI_CODE_NUMBER,
        ESI_exempted,
        PF_exempted,
        PF_CHK_AMT,
     
        Location,
        CREATEDON,
        CREATEDBY,
        UPDATEDON,
        UPDATEDBY,
        CC_UPDATEDON,
        CC_UPDATEDBY,
        STATUS
    ) 
    VALUES 
    (
        @V_NAME,
        @V_CODE,
        @NATURE_OF_WORK,
        @ADDRESS,
        @EMAIL,
        @PHONE_NO,
        @OWNER_NAME,
        @OWNER_ADHAR_NUMBER,
        @PF_CODE_NUMBER,
        @ESI_CODE_NUMBER,
        @ESI_exempted,
        @PF_exempted,
        @PF_CHK_AMT,

   
        @Location,
        @CREATEDON,
        @CREATEDBY,
        @UPDATEDON,
        @UPDATEDBY,
        @CC_UPDATEDON,
        @CC_UPDATEDBY,
        @STATUS
    )";

            using (var connection = new SqlConnection(_connectionString))
            {
                vendor.ID = Guid.NewGuid();
                vendor.CREATEDON = DateTime.Now;
                vendor.UPDATEDON = DateTime.Now;
                await connection.ExecuteAsync(sql, vendor);
            }
            ViewData["insert_msg"] = "Record inserted successfully..";
            return RedirectToAction("VendorProfileEntry");
        }

    }
}
