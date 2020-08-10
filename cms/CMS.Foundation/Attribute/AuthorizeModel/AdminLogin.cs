using System;

namespace Foundation.Attribute.AuthorizeModel
{
    public class AdminLogin
    {
        /// <summary>
        ///     用户Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     用户编号
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        ///     真实姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        ///     角色编号
        /// </summary>
        public string RoleNum { get; set; }

        /// <summary>
        ///     单点登录Token
        /// </summary>
        public string SingleLoginToken { get; set; }

        /// <summary>
        ///     过期时间,默认一天
        /// </summary>
        public DateTime ExpiresUtc { get; set; } = DateTime.Now.AddDays(1);

        /// <summary>
        ///     是否持久化,默认是
        /// </summary>
        public bool IsCookiePersistent { get; set; } = true;
    }
}