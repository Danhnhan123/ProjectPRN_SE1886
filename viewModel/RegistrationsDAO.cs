using Microsoft.EntityFrameworkCore;
using ProjectPRN_SE1886.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectPRN_SE1886
{
    public class RegistrationsDAO
    {
        private readonly PrnProjectContext _context;

        // Constructor nhận DbContext thông qua dependency injection hoặc khởi tạo trực tiếp
        public RegistrationsDAO()
        {
            _context = new PrnProjectContext();
        }

        // Lấy danh sách tất cả các hộ gia đình
        public List<Household> GetHouseholds()
        {
            return _context.Households
                .Include(h => h.HeadOfHousehold) // Bao gồm thông tin HeadOfHousehold nếu cần
                .ToList();
        }

        // Lấy thông tin thành viên hộ gia đình theo UserId
        public HouseholdMember GetHouseholdMemberByUserId(int userId)
        {
            return _context.HouseholdMembers
                .FirstOrDefault(hm => hm.UserId == userId);
        }

        // Lấy danh sách các đăng ký theo UserId
        public List<Registration> GetRegistrationsByUserId(int userId)
        {
            return _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Household) // Bao gồm thông tin hộ gia đình nếu cần
                .ToList();
        }

        // Lấy danh sách các Head of Household (nếu vẫn cần cho các chức năng khác)
        public List<User> GetHeadOfHouseholds()
        {
            var headIds = _context.Households
                .Where(h => h.HeadOfHouseholdId.HasValue)
                .Select(h => h.HeadOfHouseholdId.Value)
                .Distinct()
                .ToList();

            return _context.Users
                .Where(u => headIds.Contains(u.UserId))
                .ToList();
        }

        // Thêm một đăng ký mới vào cơ sở dữ liệu
        public void AddRegistration(Registration registration)
        {
            try
            {
                _context.Registrations.Add(registration);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding registration: " + ex.Message);
            }
        }
        public List<Registration> GetPendingRegistrations()
        {
            return _context.Registrations
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .Include(r => r.Household)
                .ToList();
        }

        // Lấy các đăng ký đã xử lý (Approved hoặc Rejected)
        public List<Registration> GetProcessedRegistrations()
        {
            return _context.Registrations
                .Where(r => r.Status == "Approved" || r.Status == "Rejected")
                .Include(r => r.User)
                .Include(r => r.Household)
                .Include(r => r.ApprovedByNavigation)
                .ToList();
        }

        // Lấy đăng ký theo ID
        public Registration GetRegistrationById(int registrationId)
        {
            return _context.Registrations
                .FirstOrDefault(r => r.RegistrationId == registrationId);
        }

        // Cập nhật đăng ký
        public void UpdateRegistration(Registration registration)
        {
            try
            {
                _context.Registrations.Update(registration);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating registration: " + ex.Message);
            }
        }

        // Thêm thành viên hộ gia đình
        public void AddHouseholdMember(HouseholdMember member)
        {
            try
            {
                _context.HouseholdMembers.Add(member);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding household member: " + ex.Message);
            }
        }

        // Cập nhật thành viên hộ gia đình
        public void UpdateHouseholdMember(HouseholdMember member)
        {
            try
            {
                _context.HouseholdMembers.Update(member);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating household member: " + ex.Message);
            }
        }
    }

}