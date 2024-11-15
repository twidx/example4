namespace Example2.Server.Models.Account
{
    /// <summary>
    /// 查詢
    /// </summary>
    public class AccountQuery_PModel
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string? Account { get; set; }

        /// <summary>
        /// 頁數
        /// </summary>
        public int Page { get; set; } = 1;
    }
}
