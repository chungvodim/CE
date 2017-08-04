using System.Security.Claims;
using System.Security.Principal;
using CE.Entity.Main;
using BGP.Utils.Common;
using CE.Enum;
using System.Linq;
using System;

namespace CE.Web
{
    public static class IdentityExtensions
    {
        public static string GetGivenName(this IIdentity identity)
        {
            if (identity == null)
                return null;

            return (identity as ClaimsIdentity).FirstOrNull(ClaimTypes.GivenName);
        }

        public static string GetFirstName(this IIdentity identity)
        {
            return (identity as ClaimsIdentity)?.FirstOrNull(CustomClaimTypes.FirstName);
        }

        public static string GetLastName(this IIdentity identity)
        {
            return (identity as ClaimsIdentity)?.FirstOrNull(CustomClaimTypes.LastName);
        }

        public static bool IsImpersonated(this IIdentity identity)
        {
            var claimsIdentity = (identity as ClaimsIdentity);

            var claim = claimsIdentity.FindFirst(CustomClaimTypes.IsImpersonated);

            if (claim == null)
            {
                return false;
            }

            var isImpersonated = Convert.ToBoolean(claim.Value);
            if (Configuration.isTestMode)
            {
                if (isImpersonated)
                {
                    ErrorHelper.Logger.InfoFormat("User {0} has been impersonated", claimsIdentity.GetGivenName());
                }
            }
            return isImpersonated;
        }

        public static int GetImpersonatorID(this IIdentity identity)
        {
            var claimsIdentity = (identity as ClaimsIdentity);

            var claim = claimsIdentity.FindFirst(CustomClaimTypes.ImpersonatorID);

            if (claim == null)
            {
                return 0;
            }

            return Convert.ToInt32(claim.Value);
        }

        public static bool? GetConsent(this IIdentity identity)
        {
            //return (identity as ClaimsIdentity)?.FirstOrDefault<bool>(CustomClaimTypes.ProtoolConsent);
            return true;
        }

        private static string FirstOrNull(this ClaimsIdentity identity, string claimType)
        {
            var val = identity.FindFirst(claimType);

            return val == null ? null : val.Value;
        }

        private static T FirstOrDefault<T>(this ClaimsIdentity identity, string claimType) where T : struct
        {
            var val = identity.FindFirst(claimType);

            return val == null ? default(T) : val.Value.To<T>();
        }

        private static T? FirstOrNull<T>(this ClaimsIdentity identity, string claimType) where T : struct
        {
            var val = identity.FindFirst(claimType);

            return val == null ? null : val.Value.ToNullable<T>();
        }
    }
}