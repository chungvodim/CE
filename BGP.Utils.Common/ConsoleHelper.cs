using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BGP.Utils.Common
{
    public static class ConsoleHelper
    {
        public static void WaitCancelKey(CancellationTokenSource tokenSource)
        {
            while (true)
            {
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'c' || ch == 'C')
                {
                    tokenSource.Cancel();
                    Console.WriteLine("\nTask cancellation requested.");
                    break;
                }
            }
            //Console.ReadLine();
        }

        public static void WaitCancelKey()
        {
            Console.WriteLine("Waiting all tasks");
            while (true)
            {
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'c' || ch == 'C')
                {
                    Console.WriteLine("\nTask cancellation requested.");
                    break;
                }
            }
            //Console.ReadLine();
        }
        /// <summary>
        /// Get command date time from arguments
        /// </summary>
        /// <param name="args">default DateTime.UtcNo</param>
        /// <returns></returns>
        public static DateTime GetDateTime(string[] args, string key = "-d")
        {
            var dateTime = DateTime.UtcNow;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if(arg == key)
                    {
                        try
                        {
                            var date = args[i + 1];
                            var time = "00:00:00";
                            if (args.Length > i + 2 && args[i + 2][0] != '-')
                            {
                                time = args[i + 2];
                            }
                            dateTime = DateTime.ParseExact(string.Format("{0} {1}", date, time), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to parse date, please use correct format:{0}", ex.Message);
                        }

                        break;
                    }
                }
            }

            return dateTime;
        }
        /// <summary>
        /// Get command type from arguments
        /// </summary>
        /// <param name="args">default empty</param>
        /// <returns></returns>
        public static string GetType(string[] args, string key = "-t")
        {
            var type = string.Empty;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if(arg == key)
                    {
                        try
                        {
                            type = args[i + 1].ToLower();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("please use correct format -t [type]:{0}", ex.Message);
                        }

                        break;
                    }
                }
            }

            return type;
        }

        public static int GetBranchId(string[] args, string key = "-branchid")
        {
            var branchId = 0;

            if (args != null && args.Length >= 2)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = args[i].ToLower();
                    if (arg == key)
                    {
                        try
                        {
                            branchId = SafeInteger(args[i + 1]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("please use correct format -branchid [BranchID]:{0}", ex.Message);
                        }

                        break;
                    }
                }
            }

            return branchId;
        }

        public static int SafeInteger(object s)
        {
            int result;
            int.TryParse(SafeString(s), out result);
            return result;
        }

        public static string SafeString(object s)
        {
            if (s == null || s is DBNull)
            {
                return "";
            }
            if (s is DateTime && Convert.ToDateTime(s) == DateTime.MinValue)
            {
                return "";
            }
            if (s is Guid)
            {
                return s.ToString();
            }
            return s.GetType().IsEnum ? s.ToString() : Convert.ToString(s).Trim();
        }
    }
}
