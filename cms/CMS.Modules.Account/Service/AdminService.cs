using System.Threading.Tasks;
using CMS.Modules.Account.Abstractions.Interface.Dapper;
using CMS.Modules.Account.Abstractions.Interface.Service;
using CMS.Modules.Account.Abstractions.Model;
using Extension;
using Foundation.Application;
using Foundation.Attribute;
using Foundation.DataAccess;
using Foundation.Modal;
using Foundation.Modal.RequestModal;
using Helper;

namespace Service.Account
{
    public class AdminService : DefaultService<Administrator, IAdminDapper> , IAdminService
    {
        private readonly IAdminDapper _dapper;

        protected override IAdminDapper GetDapper() => _dapper;

        public AdminService(IAdminDapper dapper)
        {
            _dapper = dapper;
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