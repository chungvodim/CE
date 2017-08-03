using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class LoginHelper
    {
        
        public static string EncryptImpersonateKeySetPW(int userID, Guid otpKey)
        {
            return CryptHelper.Encrypt(string.Format("{0}:{1}", userID, otpKey.ToString()));
        }

        public static string EncryptImpersonateKey(int userID)
        {
            return CryptHelper.Encrypt(string.Format("{0}_{1}", userID, DateTime.UtcNow));
        }

        public static string DecryptImpersonateKey(string encryptedKey)
        {
            return CryptHelper.Decrypt(encryptedKey);
        }

        public static KeyValuePair<int, DateTime> ValidateImpersonateKey(string key, int? timeToLive)
        {
            string[] pieces = key.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int userID = int.Parse(pieces[0]);
            DateTime encryptTime = DateTime.Parse(pieces[1]);
            if (timeToLive.HasValue && timeToLive.Value > 0)
            {
                if (DateTime.Now.ToUniversalTime() > encryptTime.AddMinutes(timeToLive.Value))
                {
                    throw new Exception("Token has expired");
                }
            }
            return new KeyValuePair<int, DateTime>(userID, encryptTime);
        }

        public static KeyValuePair<int, Guid> ValidateImpersonateKeySetPW(string key)
        {
            string[] pieces = key.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            int userID = int.Parse(pieces[0]);
            Guid encryptGuid = Guid.Parse(pieces[1]);
            return new KeyValuePair<int, Guid>(userID, encryptGuid);
        }

    }
}
