using CE.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using CE.Dto;

namespace CE.Web.Base
{
    public class ServiceRepository
    {
        public IPrincipal CurrentUser { get; private set; }
        public CE.Enum.UserRole[] CurrentRoles { get; private set; }

        public ServiceRepository(System.Security.Principal.IPrincipal user, CE.Enum.UserRole[] Roles)
        {
            this.CurrentUser = user;
            this.CurrentRoles = Roles;
        }

        public IAdminstrationService AdministrationService
        {
            get
            {
                var service = DependencyResolver.Current.GetService<IAdminstrationService>();
                service.UserID = CurrentUser.Identity.GetUserId<int>();
                service.Roles = CurrentRoles;
                return service;
            }
        }

        public IMasterDataService MasterDataService
        {
            get { return DependencyResolver.Current.GetService<IMasterDataService>(); }
        }
        
        public ILogService LogService
        {
            get { return DependencyResolver.Current.GetService<ILogService>(); }
        }
        
        public IImageService ImageService
        {
            get { return DependencyResolver.Current.GetService<IImageService>(); }
        }
    }
}