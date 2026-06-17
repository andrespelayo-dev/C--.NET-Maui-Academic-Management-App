using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Services
{
    public class AuthService
    {
        private const string ValidUsername = "apelayo";
        private const string ValidPassword = "testwgu";

        public bool IsAuthenticated { get; private set; }

        public bool Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            bool isValid =
                username.Trim().Equals(ValidUsername, StringComparison.OrdinalIgnoreCase) &&
                password == ValidPassword;

            IsAuthenticated = isValid;
            return isValid;
        }

        public void Logout()
        {
            IsAuthenticated = false;
        }
    }
}
