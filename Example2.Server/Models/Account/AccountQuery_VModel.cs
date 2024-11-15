using Example2.Server.Models._Base;
using Example2.Server.Models.Account.Data;

namespace Example2.Server.Models.Account
{
    public class AccountQuery_VModel : Base_VModel
    {
        public List<AccountItem>? Results { get; set; }
    }
}
