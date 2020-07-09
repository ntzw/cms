/**
 * author：created by zw on 2020-07-08 15:54:54
 * email：ntzw.geek@gmail.com
 */

using DataAccess.Interface.CMS;
using DataAccess.SqlServer.CMS;
using Model.CMS;

namespace Service.CMS
{
    public class ModelFieldService : DefaultService<ModelField, IModelFieldDapper>
    {
        private ModelFieldDapper _dapper;

        protected override IModelFieldDapper GetDapper() => _dapper ??= new ModelFieldDapper();

        private ModelFieldService()
        {
        }

        private static ModelFieldService _interface;
        public static ModelFieldService Interface => _interface ??= new ModelFieldService();
    }
}