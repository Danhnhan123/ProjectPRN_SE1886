using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectPRN_SE1886.Models;

namespace ProjectPRN_SE1886
{
    class DAO
    {
        // UserDAO.cs
        public class UserDAO
        {
            private readonly PrnProjectContext _context;

            public UserDAO()
            {
                _context = new PrnProjectContext();
            }

            public User Login(string email, string password)
            {
                return _context.Users
                    .FirstOrDefault(u => u.Email == email && u.Password == password);
            }

            public static List<User> GetAllUsers()
            {
                PrnProjectContext _context = new PrnProjectContext();
                return _context.Users.ToList();
            }

            public static List<User> GetUserByRole(String role, List<User> u)
            {
                List<User> users = new List<User>();
                foreach (var item in u)
                {
                    if (item.Role.Equals(role))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static List<User> GetUserByEmail(string email, List<User> u)
            {
                List<User> users = new List<User>();
                foreach (var item in u)
                {
                    if (item.Email.Contains(email))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static List<User> GetUserByName(string ename, List<User> u)
            {
                List<User> users = new List<User>();
                foreach (var item in u)
                {
                    if (item.FullName.Contains(ename))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static List<User> GetUserByAddress(string address, List<User> u)
            {
                List<User> users = new List<User>();
                foreach (var item in u)
                {
                    if (item.Address.Contains(address))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static void AddUser(User user)
            {
                PrnProjectContext _context = new PrnProjectContext();
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            public static void UpdateUser(User user)
            {
                PrnProjectContext _context = new PrnProjectContext();
                _context.Users.Update(user);
                _context.SaveChanges();
            }

            public static void DeleteUser(int userId)
            {
                PrnProjectContext _context = new PrnProjectContext();
                var user = _context.Users.Find(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
            }
        }

        // HouseholdDAO.cs
        public class HouseholdDAO
        {

            
            public static List<Household> GetAllHouseholds()
            {
                PrnProjectContext _context = new PrnProjectContext();
                return _context.Households.Include(x => x.HeadOfHousehold)  // Thêm Include này
                             .Include(x => x.HouseholdMembers)
                             .ToList();
            }

            public static void AddHousehold(Household household)
            {
                PrnProjectContext _context = new PrnProjectContext();
                _context.Households.Add(household);
                _context.SaveChanges();
            }

            public static void UpdateHousehold(Household household)
            {
                PrnProjectContext _context = new PrnProjectContext();
                _context.Households.Update(household);
                _context.SaveChanges();
            }

            public void DeleteHousehold(int householdId)
            {
                PrnProjectContext _context = new PrnProjectContext();
                var household = _context.Households.Find(householdId);
                if (household != null)
                {
                    _context.Households.Remove(household);
                    _context.SaveChanges();
                }
            }

            public static List<Household> GetHouseholdByName(int ename, List<Household> u)
            {
                List<Household> users = new List<Household>();
                foreach (var item in u)
                {
                    if (item.HeadOfHouseholdId==ename)
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static List<Household> GetUserByAddress(string address, List<Household> u)
            {
                List<Household> users = new List<Household>();
                foreach (var item in u)
                {
                    if (item.Address.Contains(address))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }

            public static List<Household> GetUserByDate(DateOnly d, List<Household> u)
            {
                List<Household> users = new List<Household>();
                foreach (var item in u)
                {
                    if (item.CreatedDate.Equals(d))
                    {
                        users.Add(item);
                    }
                }
                return users;
            }
        }
        public class LogDAO
        {
            private readonly PrnProjectContext _context;

            public LogDAO()
            {
                _context = new PrnProjectContext();
            }

            public List<Log> GetAllLogs()
            {
                return _context.Logs.Include(l => l.User).OrderByDescending(l => l.Timestamp).ToList();
            }




            // 📌 Lọc logs theo UserID, Action, khoảng thời gian
            public List<Log> SearchLogs(int? userId, string action, DateTime? startDate, DateTime? endDate)
            {
                var query = _context.Logs.AsQueryable();

                if (userId.HasValue)
                {
                    query = query.Where(l => l.UserId == userId.Value);
                }

                if (!string.IsNullOrEmpty(action) && action != "Tất cả")
                {
                    query = query.Where(l => l.Action == action);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(l => l.Timestamp >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(l => l.Timestamp <= endDate.Value);
                }

                return query.OrderByDescending(l => l.Timestamp).ToList();
            }

            public void DeleteAllLogs()
            {
                _context.Logs.RemoveRange(_context.Logs);
                _context.SaveChanges();
            }

            public void AddLog(int userId, string action)
            {
                var newLog = new Log
                {
                    UserId = userId,
                    Action = action,
                    Timestamp = DateTime.Now
                };
                _context.Logs.Add(newLog);
                _context.SaveChanges();
            }
        }
    }
}
