using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BowlingApiService.Security
{
    public class SecurityHelper
    {
        private readonly IConfiguration _configuration;

        //Fetches Confinguration from sources
        public SecurityHelper(IConfiguration inConfiguration)
        {
            _configuration = inConfiguration;
        }

        //Creates key for signing
        public SymmetricSecurityKey? GetSecurityKey()
        {
            SymmetricSecurityKey? SIGNING_KEY = null;
            if (_configuration != null)
            {
                string SECRET_KEY = _configuration["SECRET_KEY"];
                SIGNING_KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET_KEY));
            }
            return SIGNING_KEY;
        }
        public bool IsValidUsernameAndPassword(string username, string password)
        {
            string allowedUsername = _configuration["AllowDesktopApp:Username"];
            string allowedPassword = _configuration["AllowDesktopApp:Password"];
            bool credentialsOk = (username.Equals(allowedUsername)) && (password.Equals(allowedPassword));
            return credentialsOk;
        }


    }
}