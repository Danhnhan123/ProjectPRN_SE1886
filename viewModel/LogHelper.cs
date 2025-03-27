using ProjectPRN_SE1886.Models;
using System;

namespace ProjectPRN_SE1886.DAO
{
    public static class LogHelper
    {
        public static void WriteLog(int userId, string action)
        {
            using var context = new PrnProjectContext();
            var log = new Log
            {
                UserId = userId,
                Action = action,
                Timestamp = DateTime.Now
            };
            context.Logs.Add(log);
            context.SaveChanges();
        }
    }
}
