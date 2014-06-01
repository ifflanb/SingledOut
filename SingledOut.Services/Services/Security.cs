using System.Text;
using SingledOut.Services.Interfaces;

namespace SingledOut.Services.Services
{
    public class Security : ISecurity
    {
        public string CreateHash(string unHashed)
        {
          var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
          var data = Encoding.ASCII.GetBytes(unHashed);
          data = x.ComputeHash(data);
          return Encoding.ASCII.GetString(data);
        }

        public bool MatchHash(string hashValue, string unhashedValue)
        {
            var hashedValue = CreateHash(unhashedValue);

            if (hashValue == hashedValue)
            {
                return true;
            }
            return false;
        }
    }
}
