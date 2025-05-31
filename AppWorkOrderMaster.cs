using System.Xml;

namespace CLMSAPP.Models
{
    public class AppWorkOrderMaster
    {
        public Guid ID { get; set; }
        public string WO_NO { get; set; } = null!;
        public string VendorCode { get; set; } = null!;
        public string VendorName { get; set; } = null!;
        public string LocationCode { get; set; } = null!;
        public string LocationName { get; set; } = null!;
        public string DeptCode { get; set; } = null!;
        public string DeptName { get; set; } = null!;
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string CreatedOn { get; set; } = null!;
        public string WoStatus { get; set; } = null!;
    }
}
