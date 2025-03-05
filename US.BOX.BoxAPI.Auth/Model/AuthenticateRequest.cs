using System.ComponentModel;

namespace US.BOX.DebtorPortalAPI.Utils
{
    public class AuthenticateRequest
    {

        public required string username { get; set; }

        public required string password { get; set; }

        [DefaultValue("password")]
        public required string grant_type { get; set; }


        [DefaultValue("dp")]
        public required string AppID { get; set; }

        [DefaultValue("System")]
        public required string company { get; set; }
    }
}
