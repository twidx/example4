namespace Example2.Server.Models.Auth.Data
{
    public class UserInfo
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string AccountNo { get; set; } = null!;

        /// <summary>
        /// 姓名
        /// </summary>
        public string? Name { get; set; }
    }
}
