using Example2.Database.Context;
using Example2.Server.Models.Auth.Data;
using Example2.Server.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Example2.Server.Controllers
{
    /// <summary>
    /// 驗證
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class AuthController(WebDbContext db, IConfiguration conf) : Controller
    {
        private readonly WebDbContext db = db;
        private readonly IConfiguration conf = conf;

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public Login_VModel Login([FromBody] Login_PModel model)
        {
            var vm = new Login_VModel { Success = false };

            try
            {
                if (string.IsNullOrEmpty(model.AccountNo)) throw new Exception("帳號不可空白！");
                if (string.IsNullOrEmpty(model.Password)) throw new Exception("密碼不可空白！");

                var user = db.Account
                    .Where(x => x.AccountNo == model.AccountNo && x.Password == model.Password)
                    .Select(x => new UserInfo { AccountNo = x.AccountNo, Name = x.Name })
                    .FirstOrDefault() ?? throw new Exception("帳號或密碼錯誤！");

                vm.JwtToken = GenerateToken(user.AccountNo, 24);
                vm.User = user;
                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// Token驗證
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public Token_VModel Token()
        {
            var vm = new Token_VModel { Success = false };

            try
            {
                var accountNo = User.Claims.FirstOrDefault(p => p.Type == "user")?.Value;

                vm.User = db.Account.Where(x => x.AccountNo == accountNo)
                    .Select(x => new UserInfo { AccountNo = x.AccountNo, Name = x.Name })
                    .FirstOrDefault() ?? throw new Exception("Token驗證失敗！");

                vm.Success = true;
            }
            catch (Exception ex)
            {
                vm.Message = ex.Message;
            }

            return vm;
        }

        /// <summary>
        /// 產生Jwt Token
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        protected string GenerateToken(string accountNo, int expireHours)
        {
            var issuer = conf.GetValue<string>("JwtSettings:Issuer");
            var signKey = conf.GetValue<string>("JwtSettings:SignKey") ?? "";

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>
            {
                // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！
                //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
                //claims.Add(new Claim(JwtRegisteredClaimNames.Sub, token)); // User.Identity.Name
                //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
                //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
                //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
                //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
                new("user", accountNo) // JWT ID
            };

            // 網路上常看到的這個 NameId 設定是多餘的
            //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

            // 這個 Claim 也以直接被 JwtRegisteredClaimNames.Sub 取代，所以也是多餘的
            //claims.Add(new Claim(ClaimTypes.Name, userInfo.Name));

            // 你可以自行擴充 "roles" 加入登入者該有的角色
            //claims.Add(new Claim("roles", userInfo.ROLES_JSON ?? ""));
            //claims.Add(new Claim("roles", "Users"));

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.UtcNow.AddHours(expireHours),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
