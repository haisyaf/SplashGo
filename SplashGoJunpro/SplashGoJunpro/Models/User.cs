using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplashGoJunpro.Models
{
    public class User : Tourist
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLoggedIn { get; private set; }

        public virtual bool Login(string email, string password)
        {
            if (Email == email && Password == password)
            {
                IsLoggedIn = true;
                return true;
            }
            return false;
        }

        public void Logout()
        {
            IsLoggedIn = false;
        }

        public bool GetLoginStatus()
        {
            return IsLoggedIn;
        }
    }
}
