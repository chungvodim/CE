using CE.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BGP.Utils.Common;
using System.Threading;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Routing;
using CE.Web.Models;
using System.Threading.Tasks;
using CE.Enum;
using BGP.Utils.Common.AmazonLibrary;
using CE.Web.Base.Attribute;
using Microsoft.AspNet.Identity;
using CE.Web.Base;

namespace CE.Web.Base
{
    [CustomActionFilter]
    [ValidateAntiForgeryTokenWrapper(HttpVerbs.Post)]
    public abstract class BaseController : Controller
    {
        private readonly log4net.ILog __log = log4net.LogManager.GetLogger("BaseController");
        protected bool IsSuperAdmin { get { return User.IsInRole(CE.Enum.UserRole.SuperAdmin.GetDescription()); } }
        protected bool IsVendor { get { return User.IsInRole(CE.Enum.UserRole.Vendor.GetDescription()); } }
        protected bool IsClient { get { return User.IsInRole(CE.Enum.UserRole.Client.GetDescription()); } }
        protected ServiceRepository Services { get; private set; }
        protected int UserID
        {
            get
            {
                int userID = 0;
                if (User != null)
                {
                    userID = User.Identity.GetUserId<int>();
                    if (User.Identity.IsImpersonated())
                    {
                        var impersonatorID = User.Identity.GetImpersonatorID();
                        if (Configuration.isTestMode)
                        {
                            ErrorHelper.Logger.InfoFormat("User {0} has been impersonated by superadmin has ID {1}", User.Identity.Name, impersonatorID);
                        }
                        userID = impersonatorID;
                    }
                }
                return userID;
            }
        }

        public BaseController()
        {

        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            Services = new ServiceRepository(User, GetRoles());
        }


        protected CE.Enum.UserRole[] GetRoles()
        {
            List<CE.Enum.UserRole> roles = new List<CE.Enum.UserRole>();
            if (IsSuperAdmin)
            {
                roles.Add(CE.Enum.UserRole.SuperAdmin);
            }
            if (IsVendor)
            {
                roles.Add(CE.Enum.UserRole.Vendor);
            }
            if (IsClient)
            {
                roles.Add(CE.Enum.UserRole.Client);
            }
            
            return roles.ToArray();
        }

        protected FileContentResult BuildCsvFile<T>(IEnumerable<T> exportModels, bool useResource = false)
        {
            var str = Convertion.SerializeToCsv(exportModels, useResource);
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            bytes = Encoding.UTF8.GetPreamble().Concat(bytes).ToArray();
            return File(bytes, "text/csv", this.ControllerContext.RouteData.Values["controller"].ToString() + "_" + DateTime.Now.ToString("ddMMMyyyy") + ".csv");
        }

        #region Helpers
        

        #endregion
    }
}