/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using System.Linq;
using System.Threading.Tasks;
using CMS.Modules.Content.Abstractions.Interface.Dapper;
using CMS.Modules.Content.Abstractions.Interface.Service;
using CMS.Modules.Content.Abstractions.Model;
using Foundation.Application;
using Foundation.DataAccess;
using Foundation.Modal.Result;

namespace CMS.Modules.Content.Service
{
    public class SiteService : DefaultService<Site, ISiteDapper>, ISiteService
    {
        private readonly ISiteDapper _dapper;

        public SiteService(ISiteDapper dapper)
        {
            _dapper = dapper;
        }

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

        protected override ISiteDapper GetDapper()
        {
            return _dapper;
        }
    }
}