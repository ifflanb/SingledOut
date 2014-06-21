using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

namespace MobileSpace.Helpers
{
	public class SecurityHelper
	{
		/// <summary>
		/// Creates the hash.
		/// </summary>
		/// <returns>The hash.</returns>
		/// <param name="unHashed">Un hashed.</param>
		public string CreateHash(string unHashed)
		{
			var x = new MD5CryptoServiceProvider();
			var data = Encoding.ASCII.GetBytes(unHashed);
			data = x.ComputeHash(data);
			return Encoding.ASCII.GetString(data);
		}
	}
}

