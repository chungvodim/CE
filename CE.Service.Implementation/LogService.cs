using CE.Dto;
using CE.Entity.Main;
using CE.Repository.Log;
using BGP.Utils.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CE.Enum;
using CE.Repository.Main;
using CE.Repository.Mongo;
using CE.Entity.Log;

namespace CE.Service.Implementation
{
    public class LogService : BaseService, ILogService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(LogService));

        public LogService(BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository> mainService,
            BGP.Utils.Service.EntityFramework.BaseService<LogEntityFrameworkRepository> logService,
            BGP.Utils.Service.Mongo.BaseService<MongoRepository> dataService)
            : base(mainService, logService, dataService, _logger)
        {
            
        }

        public async Task AddUserLoginLogs(UserLoginLogDto userLoginLog)
        {
            await ProcessingAsync(_logService, async (svc) =>
            {
                await svc.CreateAsync<UserLoginLog, UserLoginLogDto>(userLoginLog);
            });
        }

        public async Task AddUserLogs(UserLogDto userLogDto)
        {
            await ProcessingAsync(_logService, async (svc) =>
            {
                await svc.CreateAsync<UserLog, UserLogDto>(userLogDto);
            });
        }

        #region OneTimePassword

        public async Task AddOneTimePassword(OneTimePasswordDto oneTimePasswordDto)
        {
            await ProcessingAsync(_logService, async (svc) =>
            {
                await svc.CreateAsync<OneTimePassword, OneTimePasswordDto>(oneTimePasswordDto);
            });
        }

        public OneTimePasswordDto GetOneTimePasswordByID(int otpID)
        {
            OneTimePasswordDto model = null;

            Processing(_logService, (svc) =>
            {
                model = svc.Find<OneTimePassword, OneTimePasswordDto>(x => x.OneTimePasswordID == otpID);
            });

            return model;
        }

        public async Task<OneTimePasswordDto> GetOneTimePasswordByUserID(int userID)
        {
            OneTimePasswordDto model = null;

            await ProcessingAsync(_logService, async (svc) =>
            {
                model = await svc.FindAsync<OneTimePassword, OneTimePasswordDto>(x => x.UserID == userID && x.DateFirstLogin.Year == 1900);
            });

            return model;
        }

        public async Task<OneTimePasswordDto> GetOneTimePasswordByOTPKey(int userID, Guid otpKey)
        {
            OneTimePasswordDto model = null;

            await ProcessingAsync(_logService, async (svc) =>
            {
                model = await svc.FindAsync<OneTimePassword, OneTimePasswordDto>(x => x.UserID == userID && x.OTPKey == otpKey );
            });

            return model;
        }

        public void UpdateOneTimePassword(int otpID, OneTimePasswordDto otp)
        {
            _logService.Update<OneTimePassword, OneTimePasswordDto>(otpID, otp);
        }

        public void UpdateOTPDateFirstLogin(int otpID)
        {
            OneTimePasswordDto model = this.GetOneTimePasswordByID(otpID);
            if (model != null)
            {
                model.DateFirstLogin = DateTime.UtcNow;
                _logService.Update<OneTimePassword, OneTimePasswordDto>(otpID, model);
            }            
        }

        #endregion

    }
}
