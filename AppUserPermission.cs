namespace CLMSAPP.Models
{
    public class AppUserPermission
    {
        public class UserPermissionModel
        {
            public string VCODE { get; set; }
            public string VNAME { get; set; }
            public bool Read { get; set; }
            public bool Create { get; set; }
            public bool Modify { get; set; }
            public bool Delete { get; set; }
            public bool All { get; set; }
        }
    }
}
