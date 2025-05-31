using System.ComponentModel.DataAnnotations;

namespace CLMSAPP.Models
{
    public partial class AppVendorMaster
    {
  

        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vendor Name is required")]
        public string V_NAME { get; set; } = null!;

        [Required(ErrorMessage = "Vendor Code is required")]
        public string V_CODE { get; set; } = null!;

        [Required(ErrorMessage = "Owner Name is required")]
        public string OWNERNAME { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public string ADDRESS { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EMAIL { get; set; } = null!;
        public string? PF_NO { get; set; } 
        public string? ESI_NO { get; set; } 

        [Required(ErrorMessage = "Contact No is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Contact Number")]
        public string CONTACT_NO { get; set; } = null!;
        public DateTime FROM_DATE { get; set; }
        public DateTime TO_DATE { get; set; }
        public DateTime UPDATEDAT { get; set; }
    }
}
