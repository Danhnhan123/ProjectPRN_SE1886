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

        public RegistrationsDAO()
        {
            _context = new PrnProjectContext();
        }

        public List<Household> GetHouseholds()
        {
            return _context.Households
                .Include(h => h.HeadOfHousehold)
                .ToList();
        }

        public HouseholdMember GetHouseholdMemberByUserId(int userId)
        {
            return _context.HouseholdMembers
                .FirstOrDefault(hm => hm.UserId == userId);
        }

        public List<Registration> GetRegistrationsByUserId(int userId)
        {
            return _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Household)
                .ToList();
        }

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
                .ThenInclude(h => h.HeadOfHousehold) 
                .ToList();
        }

        public List<Registration> GetProcessedRegistrations()
        {
            return _context.Registrations
                .Where(r => r.Status == "Approved" || r.Status == "Rejected")
                .Include(r => r.User)
                .Include(r => r.Household)
                .Include(r => r.ApprovedByNavigation)
                .ToList();
        }

        public Registration GetRegistrationById(int registrationId)
        {
            return _context.Registrations
                .FirstOrDefault(r => r.RegistrationId == registrationId);
        }

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

        public void RemoveHouseholdMember(HouseholdMember member)
        {
            try
            {
                _context.HouseholdMembers.Remove(member);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing household member: " + ex.Message);
            }
        }

    }
}