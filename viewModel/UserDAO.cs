using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPRN_SE1886.Models;

namespace ProjectPRN_SE1886.viewModel
{
    class UserDAO
    {
        public static List<User> GetUsers()
        {
            PrnProjectContext context = new PrnProjectContext();
            return context.Users.ToList();
        }
    }
}
