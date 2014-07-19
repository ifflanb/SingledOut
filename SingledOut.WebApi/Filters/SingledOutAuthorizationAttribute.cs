using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Ninject;
using SingledOut.Repository;
using SingledOut.SearchParameters;

namespace SingledOut.WebApi.Filters
{
    public class SingledOutAuthorizationAttribute : AuthorizationFilterAttribute
    {
        [Inject]
        public UserRepository UserRepository { get; set; }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Get injection dependncies
            IKernel kernel = new StandardKernel();
            kernel.Inject(this);

            var authHeader = actionContext.Request.Headers.Authorization;

            if (authHeader != null)
            {
                int userId;
                if (int.TryParse(authHeader.Parameter, out userId))
                {
                    var user = UserRepository.GetUser(userId);
                    if (user != null)
                    {
                        Guid token;
                        if (Guid.TryParse(authHeader.Scheme, out token))
                        {
                            if (user.AuthToken == token)
                            {
                                return;
                            }
                        }
                    }
                }

                if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                    !String.IsNullOrWhiteSpace(authHeader.Parameter))
                    {
                        var credArray = GetCredentials(authHeader);
                        var userName = credArray[0];
                        var password = credArray[1];

                        if (UserRepository.LoginUser(userName, password))
                        {
                            var currentPrincipal = new GenericPrincipal(new GenericIdentity(userName), null);
                            Thread.CurrentPrincipal = currentPrincipal;
                            HttpContext.Current.User = currentPrincipal;

                            return;
                        }
                    }
                }

            HandleUnauthorizedRequest(actionContext);
        }

        /// <summary>
        /// Gets the Credentials from the auth header.
        /// </summary>
        /// <param name="authHeader"></param>
        /// <returns></returns>
        private string[] GetCredentials(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
        {
            //Base 64 encoded string
            var rawCred = authHeader.Parameter;
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var cred = encoding.GetString(Convert.FromBase64String(rawCred));
            var credArray = cred.Split(':');

            return credArray;
        }
        
        /// <summary>
        /// Handles the unauthorized requests.
        /// </summary>
        /// <param name="actionContext"></param>
        private void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);

            actionContext.Response.Headers.Add("WWW-Authenticate",
            "Basic Scheme='sinlgedOut' location='http://localhost:8323/account/login'");
        }
    }
}