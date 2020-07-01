using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Foundation.Attribute.AuthorizeModel;
using Helper;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Foundation.Attribute
{
    public class AdminCookieAttribute : AuthorizeAttribute
    {
        public const string Scheme = "AdminLoginScheme";
        public const string ClaimsIssuer = "";

        public AdminCookieAttribute()
        {
            this.AuthenticationSchemes = Scheme;
        }

        public static async void Login(HttpContext context, AdminLogin administrator)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(Scheme, JwtClaimTypes.Name, JwtClaimTypes.Role);
            claimsIdentity.AddClaims(new[]
            {
                new Claim(JwtClaimTypes.Id, administrator.Id),
                new Claim(JwtClaimTypes.Name, administrator.Name ?? ""),
                new Claim(JwtClaimTypes.Role, administrator.RoleNum ?? ""),
                new Claim(JwtClaimTypes.PreferredUserName, administrator.AccountName ?? ""),
                new Claim(JwtClaimTypes.ReferenceTokenId, administrator.SingleLoginToken ?? ""),
                new Claim(JwtClaimTypes.IdentityProvider, administrator.Num ?? "")
            });

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await context.SignInAsync(Scheme, claimsPrincipal, new AuthenticationProperties
            {
                IsPersistent = administrator.IsCookiePersistent,
                ExpiresUtc = administrator.ExpiresUtc,
                AllowRefresh = administrator.IsCookiePersistent,
            });
        }


        public static async Task<bool> IsLogin(HttpContext context)
        {
            AuthenticateResult result = await context.AuthenticateAsync(Scheme);
            return result?.Principal?.Identity?.IsAuthenticated ?? false;
        }

        public static string GetClaimValue(IEnumerable<Claim> claims, string types)
        {
            try
            {
                Claim claim = claims.First(item => item.Type == types);
                return claim?.Value;
            }
            catch (Exception e)
            {
            }

            return string.Empty;
        }

        public static AdminLogin GetLoginAdmin(IEnumerable<Claim> claims)
        {
            var enumerable = claims.ToList();
            return new AdminLogin
            {
                Id = (GetClaimValue(enumerable, JwtClaimTypes.Id)),
                Name = (GetClaimValue(enumerable, JwtClaimTypes.Name)),
                RoleNum = (GetClaimValue(enumerable, JwtClaimTypes.Role)),
                SingleLoginToken = GetClaimValue(enumerable, JwtClaimTypes.ReferenceTokenId),
                AccountName = GetClaimValue(enumerable, JwtClaimTypes.PreferredUserName),
                Num = GetClaimValue(enumerable, JwtClaimTypes.IdentityProvider)
            };
        }

        public static async Task LoginOut(HttpContext context)
        {
            RemoveLoginSession();
            await context.SignOutAsync(Scheme);
        }

        private const string LoginAdminSessionName = "LoginAdminSessionName";

        public static void RemoveLoginSession()
        {
            SessionHelper.Remove(LoginAdminSessionName);
        }
        
        public static void SetLoginAdmin(AdminLogin administrator)
        {
            SessionHelper.Set(LoginAdminSessionName, administrator);
        }

        public static AdminLogin GetLoginAdmin()
        {
            return SessionHelper.Get<AdminLogin>(LoginAdminSessionName);
        }
    }
}