using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Data;
using System.Web;
using System.Data.SqlClient;

namespace DigitalNagrik.Models
{
    public class BasicAuthentication : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authHeader = actionContext.Request.Headers.Authorization;
            if (authHeader != null)
            {
                var authenticationScheme = actionContext.Request.Headers.Authorization.Scheme;
                if (authenticationScheme.ToUpper() == "Basic".ToUpper())
                {
                    var authenticationToken = actionContext.Request.Headers.Authorization.Parameter;
                    var decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
                    var usernamePasswordArray = decodedAuthenticationToken.Split(':');
                    var AuthUserID = AESCryptography.DecryptAES(HttpUtility.UrlDecode(usernamePasswordArray[0]));
                    var AuthPassword = AESCryptography.DecryptAES(HttpUtility.UrlDecode(usernamePasswordArray[1]));
                    if (isValid(AuthUserID, AuthPassword))
                    {
                        var principal = new GenericPrincipal(new GenericIdentity(AuthUserID), null);
                        Thread.CurrentPrincipal = principal;
                        return;
                    }
                }
            }
            HandleUnathorized(actionContext);
        }
        private static void HandleUnathorized(HttpActionContext actionContext)
        {
            var response = new Generic_Responce();
            response.status_code = 401;
            response.Message = "Unauthorized Request";
            response.developer_message = "Invalid Basic Authentication";

            actionContext.Response = actionContext.Request.CreateResponse((HttpStatusCode)response.status_code, response);
            actionContext.Response.Headers.Add("WWW-Authenticate", "Basic Scheme='Data' location = 'http://localhost:");
        }

        private bool isValid(string AuthUserID, string AuthPassword)
        {
            if (AuthUserID == "NaasMobileApp" & AuthPassword == "9kO9E3C7F05F1324")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}