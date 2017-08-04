using CE.Dto;
using CE.Entity.Main;
using System.Threading.Tasks;
using System.Collections.Generic;
using log4net;
using CE.Repository.Main;
using System.Linq;
using EntityFramework.Audit;
using System;
using CE.Entity.Log;
using System.Net.Http;
using CE.Enum;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BGP.Utils.Common;
using Microsoft.AspNet.Identity;
using UserRole = CE.Entity.Main.UserRole;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Data.Entity;
using CE.Repository.Log;

namespace CE.Service.Implementation
{
    public class AdministrationService : BaseService, IAdminstrationService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AdministrationService));
        public int UserID { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public CE.Enum.UserRole[] Roles { get; set; }
        public AdministrationService(BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository> mainService,
            BGP.Utils.Service.EntityFramework.BaseService<LogEntityFrameworkRepository> logService) : base(mainService, logService, _logger)
        {
        }



        public virtual async Task<int> AddRole(RoleDto newRole)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                await svc.CreateAsync<Role, RoleDto>(newRole);

                return newRole.RoleID;
            });
        }

        public virtual async Task<List<RoleDto>> GetAllRoles()
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                return await svc.AllAsync<Role, RoleDto>();
            });
        }

        public virtual async Task<UserDto> GetUserById(int userId)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                return await svc.FindByIdAsync<User, UserDto>(userId);
            });
        }

        public async Task<List<UserDto>> GetUserByIds(List<int> userIds)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                return await svc.FilterAsync<User, UserDto>(x => userIds.Contains(x.UserID));
            });
        }

        private async Task AddUserLog()
        {
            // log
            await ProcessingAsync(_logService, async (lsvc) =>
            {
                var userLogDto = new UserLogDto
                {
                    CreatedByID = UserID,
                    ChangeLog = this.GetLastLog()
                };

                await _logService.CreateAsync<UserLog, UserLogDto>(userLogDto);
            });
        }

        public virtual async Task<int> AddUserAsync(UserDto newUser)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                // Create new user
                await svc.CreateAsync<User, UserDto>(newUser);

                // log
                await AddUserLog();

                // Add roles to user
                await svc.CreateManyAsync<UserRole, UserRoleDto>(newUser.RoleIds.Select(x => new UserRoleDto { RoleID = x, UserID = newUser.UserID }));

                // log
                await AddUserLog();

                return newUser.UserID;
            });
        }

        public virtual async Task CreateManyRoleAsync(UserDto newUser)
        {
            await ProcessingAsync(_mainService, async (svc) =>
            {
                // Add roles to user
                await svc.CreateManyAsync<UserRole, UserRoleDto>(newUser.RoleIds.Select(x => new UserRoleDto { RoleID = x, UserID = newUser.UserID }));

                // log
                await AddUserLog();
            });
        }

        public virtual async Task UpdateUser(int userId, UserDto user)
        {
            await ProcessingAsync(_mainService, async (svc) =>
            {
                await svc.UpdateAsync<User, UserDto>(userId, user);

                // log
                await AddUserLog();
            });
        }

        public virtual async Task UpdateUser(int userId, UserDto newUser, params Expression<Func<User, Object>>[] propertiesToUpdate)
        {
            await AdvanceProcessingAsync(_mainService, async (svc) =>
            {
                await svc.UpdateSelectFieldsAsync<User, UserDto>(userId, newUser, propertiesToUpdate);
                // log
                await AddUserLog();
            });
            
        }
        
        public async Task<List<UserRoleDto>> GetRolesByUserID(int userID)
        {
            return await ProcessingAsync(_mainService, async (svc) =>
            {
                return await svc.FilterAsync<UserRole, UserRoleDto>(x => x.UserID == userID);
            });
        }

        public IQueryable<UserDto> QueryUsers()
        {
            return _mainService.Query<User, UserDto>(x => true);
        }

        public string GetLastLog()
        {
            return _mainService.GetLastLog();
        }

    }
}
