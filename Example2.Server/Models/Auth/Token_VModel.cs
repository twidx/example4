using Example2.Server.Models._Base;
using Example2.Server.Models.Auth.Data;

namespace Example2.Server.Models.Auth
{
    public class Token_VModel : Base_VModel
    {
        /// <summary>
        /// 登入者資訊
        /// </summary>
        public UserInfo? User { get; set; }

        public string? JwtToken { get; set; }
    }
}
