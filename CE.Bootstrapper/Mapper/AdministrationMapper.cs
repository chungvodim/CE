using AutoMapper;
using CE.Dto;
using CE.Entity.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Bootstrapper.Mapper
{
    internal class AdministrationMapper
    {
        public static void Configure(IProfileExpression config)
        {
            config.CreateMap<Role, RoleDto>()
                .IgnoreAll()
                .ForMember(des => des.RoleID, mo => mo.MapFrom(src => src.Id))
                .ForMember(des => des.RoleName, mo => mo.MapFrom(src => src.Name))
                .ForMember(des => des.Description, mo => mo.MapFrom(src => src.Description));

            config.CreateMap<RoleDto, Role>()
                .IgnoreAll()
                .ForMember(des => des.Id, mo => mo.MapFrom(src => src.RoleID))
                .ForMember(des => des.Name, mo => mo.MapFrom(src => src.RoleName))
                .ForMember(des => des.Description, mo => mo.MapFrom(src => src.Description));

            #region User mapping
            config.CreateMap<User, UserDto>()
                .IgnoreAll()
                .ForMember(des => des.UserID, mo => mo.MapFrom(src => src.Id))
                .ForMember(des => des.UserName, mo => mo.MapFrom(src => src.UserName))
                .ForMember(des => des.Email, mo => mo.MapFrom(src => src.Email))
                .ForMember(des => des.FirstName, mo => mo.MapFrom(src => src.FirstName))
                .ForMember(des => des.LastName, mo => mo.MapFrom(src => src.LastName))
                .ForMember(des => des.DateOfBirth, mo => mo.MapFrom(src => src.DateOfBirth))
                .ForMember(des => des.CreatedDate, mo => mo.MapFrom(src => src.CreatedDate))
                .ForMember(des => des.UpdatedDate, mo => mo.MapFrom(src => src.UpdatedDate))
                .ForMember(des => des.Gender, mo => mo.MapFrom(src => src.Gender))
                .ForMember(des => des.PostalCode, mo => mo.MapFrom(src => src.PostalCode))
                .ForMember(des => des.City, mo => mo.MapFrom(src => src.City))
                .ForMember(des => des.PhoneNumber, mo => mo.MapFrom(src => src.PhoneNumber))
                .ForMember(des => des.UserStatus, mo => mo.MapFrom(src => src.UserStatus))
                .ForMember(des => des.CreatedUserID, mo => mo.MapFrom(src => src.CreatedUserID))
                .ForMember(des => des.UpdatedUserId, mo => mo.MapFrom(src => src.UpdatedUserId))
                .ForMember(des => des.RoleIds, mo => mo.MapFrom(src => src.Roles.Select(x => x.RoleId).ToList()))
                .ForMember(des => des.Roles, mo => mo.MapFrom(src => src.Roles.Select(x => x.Role.Name).ToList()))
                .ForMember(des => des.PrimaryRole, mo => mo.MapFrom(src => src.Roles.Select(x => x.Role.Name).FirstOrDefault()));

            config.CreateMap<UserDto, User>()
                .IgnoreAll()
                .ForMember(des => des.Id, mo => mo.MapFrom(src => src.UserID))
                .ForMember(des => des.UserName, mo => mo.MapFrom(src => src.UserName))
                .ForMember(des => des.Email, mo => mo.MapFrom(src => src.Email))
                .ForMember(des => des.PasswordHash, mo => mo.MapFrom(src => src.PasswordHash))
                .ForMember(des => des.SecurityStamp, mo => mo.MapFrom(src => src.SecurityStamp))
                .ForMember(des => des.FirstName, mo => mo.MapFrom(src => src.FirstName))
                .ForMember(des => des.LastName, mo => mo.MapFrom(src => src.LastName))
                .ForMember(des => des.DateOfBirth, mo => mo.MapFrom(src => src.DateOfBirth))
                .ForMember(des => des.CreatedDate, mo => mo.MapFrom(src => src.CreatedDate))
                .ForMember(des => des.UpdatedDate, mo => mo.MapFrom(src => src.UpdatedDate))
                .ForMember(des => des.Gender, mo => mo.MapFrom(src => src.Gender))
                .ForMember(des => des.PostalCode, mo => mo.MapFrom(src => src.PostalCode))
                .ForMember(des => des.City, mo => mo.MapFrom(src => src.City))
                .ForMember(des => des.PhoneNumber, mo => mo.MapFrom(src => src.PhoneNumber))
                .ForMember(des => des.UserStatus, mo => mo.MapFrom(src => src.UserStatus))
                .ForMember(des => des.CreatedUserID, mo => mo.MapFrom(src => src.CreatedUserID))
                .ForMember(des => des.UpdatedUserId, mo => mo.MapFrom(src => src.UpdatedUserId));

            config.CreateMap<UserRole, UserRoleDto>()
                .IgnoreAll()
                .ForMember(des => des.UserID, mo => mo.MapFrom(src => src.UserId))
                .ForMember(des => des.RoleID, mo => mo.MapFrom(src => src.RoleId));

            config.CreateMap<UserRoleDto, UserRole>()
                .IgnoreAll()
                .ForMember(des => des.UserId, mo => mo.MapFrom(src => src.UserID))
                .ForMember(des => des.RoleId, mo => mo.MapFrom(src => src.RoleID));

            #endregion
        }
    }
}
