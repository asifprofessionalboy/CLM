namespace CLMSAPP.Models
{
    public partial class AppVendorReg
    {
        public Guid ID { get; set; }
        public string V_NAME { get; set; } = null!;
        public string V_CODE { get; set; } = null!;
        public string NATURE_OF_WORK { get; set; } = null!;
        public string ADDRESS { get; set; } = null!;
        public string EMAIL { get; set; } = null!;
        public string PHONE_NO { get; set; } = null!;
        public string OWNER_NAME { get; set; } = null!;
        public string OWNER_ADHAR_NUMBER { get; set; } = null!;
        public string PF_CODE_NUMBER { get; set; } = null!;
        public string ESI_CODE_NUMBER { get; set; } = null!;
        public string CREATEDBY { get; set; } = null!;
        public string UPDATEDBY { get; set; } = null!;
        public DateTime? CC_UPDATEDON { get; set; }
        public string? CC_UPDATEDBY { get; set; }
        public string STATUS { get; set; } = null!;
        public string? REMARKS { get; set; }
        public string? PF_exempted { get; set; }
        public string? ESI_exempted { get; set; }
        public string? PF_CHK_AMT { get; set; }

        public string Location { get; set; } = null!;
        public DateTime? CREATEDON { get; set; }
        public DateTime? UPDATEDON { get; set; }
    }

}
