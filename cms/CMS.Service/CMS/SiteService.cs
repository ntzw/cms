/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Linq;
using System.Threading.Tasks;
using CMS.React.Model;
using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Foundation.Modal;
using Foundation.Modal.Result;
using Helper;
using Model.CMS;

namespace Service.CMS
{
    public class SiteService : DefaultService<Site, ISiteDapper>
    {
        private SiteDapper _dapper;

        protected override ISiteDapper GetDapper() => _dapper ??= new SiteDapper();

        private SiteService()
        {
        }

        private static SiteService _interface;
        public static SiteService Interface => _interface ??= new SiteService();

        public Site GetCurrentSite()
        {
            return SessionHelper.Get<Site>("CurrentSite");
        }
        
        public async Task<HandleResult> RemoveOtherDefault(int excludeId)
        {
            return new HandleResult
            {
                IsSuccess = await GetDapper().RemoveOtherDefault(excludeId) > 0
            };
        }

        public async Task<HandleResult> GetSelectData()
        {
            var all = await GetDapper().GetAll();
            return new HandleResult
            {
                IsSuccess = true,
                Data = all.Select(temp => new
                {
                    temp.Name,
                    temp.Num,
                    temp.IsDefault
                }).ToList()
            };
        }

        public Task<Site> GetByHost(string host)
        {
            return GetDapper().GetByHost(host);
        }

        public Task<Site> GetByDefault()
        {
            return GetDapper().GetByDefault();
        }
    }
}