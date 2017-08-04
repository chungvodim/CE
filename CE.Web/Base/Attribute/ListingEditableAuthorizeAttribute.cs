using CE.Repository.Main;
using BGP.Utils.Common;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace CE.Web.Base.Attribute
{
    public class ListingAuthorizeAttribute : AuthorizeAttribute
    {
        public string ObjectIdName { get; set; }
        public bool _Readable = true;
        public bool Readable
        {
            get
            {
                return _Readable;
            }
            set
            {
                this._Readable = value;
            }
        }
        public bool Editable { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorized = base.AuthorizeCore(httpContext);
            if (!authorized)
            {
                // The user is not authenticated
                return false;
            }

            var user = httpContext.User;

            var rd = httpContext.Request.RequestContext.RouteData;
            var form = httpContext.Request.Form;
            if (!string.IsNullOrWhiteSpace(ObjectIdName))
            {
                string listingID = null;

                if (httpContext.Request.HttpMethod == "GET")
                {
                    listingID = rd.Values[ObjectIdName] as string;
                }
                else if(httpContext.Request.HttpMethod == "POST" && form.HasKeys())
                {
                    listingID = form[ObjectIdName];
                }

                if (string.IsNullOrWhiteSpace(listingID))
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(listingID) && !IsOwnerOfListing(user, listingID))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsOwnerOfListing(IPrincipal user, string listingID)
        {
            using (var context = new MainContext())
            {
                return true;
            }
        }
    }
}