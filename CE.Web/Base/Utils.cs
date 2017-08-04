using BGP.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace CE.Web.Base
{
    public static class Utils
    {
        public static int Revision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;
        /// <summary>
        /// Convert an enumeration to a Mvc.SelectList for use in dropdowns. Note that the enumeration values must all be added to a resource file.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="enumObj">The enum obj.</param>
        /// <param name="sortAlphabetically">If set to <c>true</c> [the list is sorted alphabetically].</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>(this TEnum enumObj, bool sortAlphabetically = true)
        {
            IList<SelectListItem> values =
                        (from TEnum e in System.Enum.GetValues(typeof(TEnum))
                         select new SelectListItem
                         {
                             Text = e.ToString(),
                             Value = Convert.ToInt32(e).ToString()
                         }).ToList();

            if (sortAlphabetically)
                values = values.OrderBy(v => v.Text).ToList();

            return new SelectList(values, "Value", "Text", enumObj);
        }

        /// <summary>
        /// Get Errors From ModelState
        /// </summary>
        /// <param name="modelStates"></param>
        /// <returns></returns>
        public static string GetErrorsFromModelState(ModelStateDictionary modelStates)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ModelState modelState in modelStates.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    sb.Append(error.ErrorMessage + "<br>");
                }
            }
            return sb.ToString();
        }

        public static string ToLocaleNumberString(this object number, string locale = "fr-BE", string format = "{0:n0}")
        {
            try
            {
                return string.Format(System.Globalization.CultureInfo.GetCultureInfo(locale), format, number);
            }
            catch (Exception ex)
            {
                BGP.Utils.Common.ErrorHelper.Logger.Error(ex.Message,ex);
                return "";
            }
        }

        public static bool EnableGoogleTagManager(HttpSessionStateBase session,IPrincipal user)
        {
            return (!user.Identity.IsImpersonated() && !user.IsInRole(CE.Enum.UserRole.SuperAdmin.GetDescription()));
        }

        public static string GetClientIpAddress(HttpRequestBase request)
        {
            try
            {
                var userHostAddress = request.UserHostAddress;

                // Attempt to parse.  If it fails, we catch below and return "0.0.0.0"
                // Could use TryParse instead, but I wanted to catch all exceptions
                IPAddress.Parse(userHostAddress);

                var xForwardedFor = request.ServerVariables["X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(xForwardedFor))
                    return userHostAddress;

                // Get a list of public ip addresses in the X_FORWARDED_FOR variable
                var publicForwardingIps = xForwardedFor.Split(',').Where(ip => !IsPrivateIpAddress(ip)).ToList();

                // If we found any, return the last one, otherwise return the user host address
                return publicForwardingIps.Any() ? publicForwardingIps.Last() : userHostAddress;
            }
            catch (Exception)
            {
                // Always return all zeroes for any failure (my calling code expects it)
                return "0.0.0.0";
            }
        }

        public static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }
    }
}