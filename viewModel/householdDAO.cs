using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectPRN_SE1886.Models;


class householdDAO
{
    public static List<Household> GetHouseholds()
    {
        PrnProjectContext context = new PrnProjectContext();
        return context.Households.Include(x => x.HeadOfHousehold)  // Thêm Include này
                             .Include(x => x.HouseholdMembers)
                             .ToList();
    }
}

