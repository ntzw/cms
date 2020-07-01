using System.Threading.Tasks;
using DataAccess.Interface.Account;
using DataAccess.SqlServer.Account;
using Extension;
using Foundation.Attribute;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Helper;
using Model.Account;

namespace Service.Account
{
    public class AdminService : DefaultService<Administrator, IAdminDapper>
    {
        private IAdminDapper _dapper;

        protected override IAdminDapper GetDapper() => _dapper ??= new AdminDapper();

        private AdminService()
        {
        }

        private static AdminService _interface;
        public static AdminService Interface => _interface ??= new AdminService();

        public Task<PageResponse> Page(IPageRequest req)
        {
            return GetDapper().Page(req);
        }

        /// <summary>
        /// 生成管理员密码
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string PasswordToMd5(string password)
        {
            return Md5Helper.GetMD5_32(password + "123");
        }

        public async Task<Administrator> GetByLoginAdmin()
        {
            var adminLogin = AdminCookieAttribute.GetLoginAdmin();
            if (adminLogin == null || adminLogin.AccountName.IsEmpty()) return null;

            var cacheKey = adminLogin.Id + adminLogin.AccountName;

            var loginAdmin = CacheHelper.Get<Administrator>(cacheKey);
            if (loginAdmin != null) return loginAdmin;

            Administrator admin = await GetById(adminLogin.Id.ToInt());
            CacheHelper.Set(cacheKey, admin);

            return admin;
        }

        public Task<Administrator> GetByAccountName(string accountName)
        {
            return GetDapper().GetByAccountName(accountName);
        }
    }
}