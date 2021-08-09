using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using CFSWebService.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin;




namespace CFSWebService.ExternalAccess
{

    [RoutePrefix("api/Account")]

    public class APIAccountController : ApiController
    {
       // private const string LocalLoginProvider = "Local";
        private ApplicationUserManager usermanager;
        public APIAccountController()
            //: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public APIAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return usermanager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                usermanager = value;
            }
        }

        //private ApplicationRoleManager _roleManager;

        //public ApplicationRoleManager RoleManager
        //{
        //    get
        //    {
        //        return _roleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
        //    }
        //    private set { _roleManager = value; }
        //}

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        //ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; set; } 

        public async Task<ApplicationUser> FindUser(string userName, string password)
        {
            ApplicationUser user = await UserManager.FindAsync(userName, password);

            return user;
        }




        //[HttpPost]
        //[AllowAnonymous]
        //[Route("ExternalLogin")]
        ////  [ValidateAntiForgeryToken]
       
        //public async Task<IHttpActionResult> ExternalLogin(ExternalLogin model)  //(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //   // CFSWebService.Controllers.AccountController ac = new CFSWebService.Controllers.AccountController();
        //   // var aa = await ac.ExternalLogin(model.provider, model.returnUrl);
        //    return Ok(aa);
        //}

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return BadRequest(ModelState);
                }


                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);

                var callbackUrl = Url.Content("~/account/resetpassword?email=") + HttpUtility.UrlEncode(model.Email) + "&code=" + HttpUtility.UrlEncode(code);


                await UserManager.SendEmailAsync(user.Id, "Reset Password",
                    "Please confirm your account by clicking <a href=" + callbackUrl + ">here</a>");

                return Ok("Check your email");
            }


            // If we got this far, something failed 
            return BadRequest(ModelState);
        }

        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok("Loged OUT");
        }
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

       


        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc),
                PostCode = model.PostCode,
                Gender = model.Gender,
                Country = model.Country
            };
            
            var userEmail = await UserManager.FindByEmailAsync(model.Email);
            if (userEmail != null || !(await UserManager.IsEmailConfirmedAsync(userEmail.Id)))
            {
                IdentityResult identityResult1 = await UserManager.DeleteAsync(userEmail);
            }
            IdentityResult identityResult = await UserManager.CreateAsync(user, model.Password);

            IHttpActionResult createResult = GetErrorResult(identityResult);

          
            if (!identityResult.Succeeded)
            {
                return GetErrorResult(identityResult);
            }


            ApplicationUser justCreatedUser = await UserManager.FindByNameAsync(model.Email);


            IdentityResult roleResult = await UserManager.AddToRoleAsync(justCreatedUser.Id, "User");
            IHttpActionResult addRoleResult = GetErrorResult(roleResult);

            if (addRoleResult != null)
            {
                return addRoleResult;
            }

         
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = Url.Link("DefaultAccount", new { controller = "Account/ConfirmEmail", userId = user.Id, code = code });
            //var callbackUrl = new Uri(Url.Link("ConfirmEmail", new { userId = user.Id, code = code }));
         
            await UserManager.SendEmailAsync(user.Id, "Reset Password",
                 "Please confirm your account by clicking <a href=" + callbackUrl + ">here</a>");

          
            return Ok("Registered, check email");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                ModelState.AddModelError("error", "You need to provide your user id and confirmation code");
                return BadRequest(ModelState);
            }


            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                // Redirect(Url.Link("~/home/index", "")); 
                var response = Request.CreateResponse(HttpStatusCode.Moved);
                string fullyQualifiedUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                // response.Headers.Location = new Uri(fullyQualifiedUrl);

                // Redirect(Url.Link("ConfirmEmail", new { controller = "Account", id ="" }));
                Redirect(Url.Link(fullyQualifiedUrl + "/home/index", ""));
            }
            IHttpActionResult errorResult = GetErrorResult(result);
            return errorResult;

          //  return Ok("Activated, Email confirmed");
        }


        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }


        //************************************

          public static class RandomOAuthStateGenerator
          {
              private static System.Security.Cryptography.RandomNumberGenerator _random = new System.Security.Cryptography.RNGCryptoServiceProvider();
              public static string Generate(int strengthInBits)
              {
                  const int bitsPerByte = 8;

                  if (strengthInBits % bitsPerByte != 0)
                  {
                      throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                  }

                  int strengthInBytes = strengthInBits / bitsPerByte;

                  byte[] data = new byte[strengthInBytes];
                  _random.GetBytes(data);
                  return HttpServerUtility.UrlTokenEncode(data);
              }
          }


     

        [AllowAnonymous]
        [Route("ExternalLogins", Name = "ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }


            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = CFSWebService.Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }


            return logins;
        }



        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
       
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {

            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }


            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }


            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }


            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));


            bool hasRegistered = user != null;


            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user, 
                    OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await UserManager.CreateIdentityAsync(user,
                    CookieAuthenticationDefaults.AuthenticationType);
                AuthenticationProperties properties = CFSWebService.Providers.ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }


            return Ok();

        }


    


     

            //************************************

        
        public class ChallengeResult : IHttpActionResult
        {
            public ChallengeResult(string loginProvider, ApiController controller)
            {
                LoginProvider = loginProvider;
                Request = controller.Request;
            }


            public string LoginProvider { get; set; }
            public HttpRequestMessage Request { get; set; }


            public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
            {
                Request.GetOwinContext().Authentication.Challenge(LoginProvider);


                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                response.RequestMessage = Request;
                return Task.FromResult(response);
            }
        }
        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }


            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));


                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }


                return claims;
            }


            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }


                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);


                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }


                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }


                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }
    }
}



//**************************************  Client Code for calling external login



//using System;
//using System.Drawing;
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using System.Collections.Generic;

//namespace ExternalProviderAuthentication.iOS
//{
//    public partial class ExternalProviderAuthentication_iOSViewController : UIViewController
//    {
//        private readonly AuthenticationServices _services = new AuthenticationServices("http://externalproviderdemo.azurewebsites.net/");
//        private ExternalLoginViewModel _selectedProvider = null;

//        public ExternalProviderAuthentication_iOSViewController (IntPtr handle) : base (handle)
//        {
//        }

//        public override void DidReceiveMemoryWarning ()
//        {
//            // Releases the view if it doesn't have a superview.
//            base.DidReceiveMemoryWarning ();
			
//            // Release any cached data, images, etc that aren't in use.
//        }

//        #region View lifecycle

//        public override async void ViewDidLoad ()
//        {
//            base.ViewDidLoad ();
			
//            List<ExternalLoginViewModel> externalLoginProviders = new List<ExternalLoginViewModel>(await _services.GetExternalLoginProviders ());
//            InvokeOnMainThread (() => {
//                float y = 50;
//                int index = 0;
//                foreach(ExternalLoginViewModel model in externalLoginProviders)
//                {
//                    int localIndex = index;
//                    UIButton button = UIButton.FromType(UIButtonType.RoundedRect);
//                    button.SetTitle(model.Name, UIControlState.Normal);
//                    button.Tag = index;
//                    RectangleF frame = button.Frame;
//                    frame.Y = y;
//                    frame.Width = View.Frame.Width;
//                    frame.Height = 27;
//                    button.Frame = frame;
//                    View.AddSubview(button);
//                    button.TouchUpInside += (object sender, EventArgs e) => {
//                        _selectedProvider = externalLoginProviders [localIndex];
//                        PerformSegue ("presentBrowser", this);
//                    };

//                    index++;
//                    y += frame.Height + 7;
//                }
//            });
//        }

//        public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
//        {
//            base.PrepareForSegue (segue, sender);
//            if (segue.Identifier == "presentBrowser") {
//                BrowserLoginViewController controller = (BrowserLoginViewController)segue.DestinationViewController;
//                controller.ExternalLoginProvider = _selectedProvider;
//                controller.Services = _services;
//            }
//        }

//        public override void ViewWillAppear (bool animated)
//        {
//            base.ViewWillAppear (animated);
//        }

//        public override void ViewDidAppear (bool animated)
//        {
//            base.ViewDidAppear (animated);
//        }

//        public override void ViewWillDisappear (bool animated)
//        {
//            base.ViewWillDisappear (animated);
//        }

//        public override void ViewDidDisappear (bool animated)
//        {
//            base.ViewDidDisappear (animated);
//        }

//        #endregion
//    }
//}

//// This file has been autogenerated from a class added in the UI designer.
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Net;
//using System.IO;
//using System.Security;
//using Newtonsoft.Json;
//using System.Text;

//namespace ExternalProviderAuthentication.iOS
//{
//    public class AuthenticationServices
//    {
//        private readonly string _baseUri;

//        public AuthenticationServices (string baseUri)
//        {
//            _baseUri = baseUri;
//        }

//        public string BaseUri { get { return _baseUri; } }

//        public string AccessToken { get; set; }

//        public async Task<IEnumerable<ExternalLoginViewModel>> GetExternalLoginProviders()
//        {
//            string uri = String.Format("{0}/api/Account/ExternalLogins?returnUrl=%2F&generateState=true", _baseUri);
//            HttpWebRequest request = new HttpWebRequest(new Uri(uri));
//            request.Method = "GET";
//            try
//            {
//                WebResponse response = await request.GetResponseAsync();
//                HttpWebResponse httpResponse = (HttpWebResponse) response;
//                string result;

//                using (Stream responseStream = httpResponse.GetResponseStream())
//                {
//                    result = new StreamReader(responseStream).ReadToEnd();
//                }

//                List<ExternalLoginViewModel> models = JsonConvert.DeserializeObject<List<ExternalLoginViewModel>>(result);
//                return models;
//            }
//            catch (SecurityException)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException("Unable to get login providers", ex);
//            }
//        }

//        public async Task RegisterExternal(
//            string username)
//        {
//            string uri = String.Format("{0}/api/Account/RegisterExternal", BaseUri);

//            RegisterExternalBindingModel model = new RegisterExternalBindingModel
//            {
//                UserName = username
//            };
//            HttpWebRequest request = new HttpWebRequest(new Uri(uri));

//            request.ContentType = "application/json";
//            request.Accept = "application/json";
//            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken));
//            request.Method = "POST";

//            string postJson = JsonConvert.SerializeObject(model);
//            byte[] bytes = Encoding.UTF8.GetBytes(postJson);
//            using (Stream requestStream = await request.GetRequestStreamAsync())
//            {
//                requestStream.Write(bytes, 0, bytes.Length);
//            }

//            try
//            {
//                WebResponse response = await request.GetResponseAsync();
//                HttpWebResponse httpResponse = (HttpWebResponse)response;
//                string result;

//                using (Stream responseStream = httpResponse.GetResponseStream())
//                {
//                    result = new StreamReader(responseStream).ReadToEnd();
//                    Console.WriteLine(result);
//                }
//            }
//            catch (SecurityException)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw new InvalidOperationException("Unable to register user", ex);
//            }
//        }
//    }
//}




//using System;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//using System.Linq;
//using System.Net;
//using System.IO;
//using System.Threading.Tasks;

//namespace ExternalProviderAuthentication.iOS
//{
//    public partial class BrowserLoginViewController : UIViewController
//    {
//        public BrowserLoginViewController (IntPtr handle) : base (handle)
//        {
//        }

//        public ExternalLoginViewModel ExternalLoginProvider {
//            get;
//            set;
//        }

//        public AuthenticationServices Services {
//            get;
//            set;
//        }

//        public override void ViewDidLoad ()
//        {
//            base.ViewDidLoad ();
//            AttemptLogin ();
//            _webBrowser.LoadFinished += (sender, e) => {
//                Console.WriteLine(_webBrowser.Request.Url);
//                ParseUrlForAccessToken(_webBrowser.Request.Url.ToString());
//            };
//        }
			
//        private async void ParseUrlForAccessToken(string url)
//        {
//            const string fieldName = "access_token=";
//            int accessTokenIndex = url.IndexOf(fieldName, StringComparison.Ordinal);
//            if (accessTokenIndex > -1) {

//                int ampersandTokenIndex = url.IndexOf("&", accessTokenIndex, StringComparison.Ordinal);
//                string tokenField = url.Substring (accessTokenIndex, ampersandTokenIndex - accessTokenIndex);
//                string token = tokenField.Substring (fieldName.Length);
//                Console.WriteLine (token);
//                Services.AccessToken = token;
//                if (!IsLocalUser)
//                {
//                    await Services.RegisterExternal("ausername"); // collect the username from the UI and put it here
//                    InvokeOnMainThread(() => AttemptLogin());
//                }
//                else
//                {
//                    await TestAuthorization ();
//                }
//            }
//        }

//        private void AttemptLogin()
//        {
//            string url = String.Format ("{0}/{1}", Services.BaseUri, ExternalLoginProvider.Url);
//            _webBrowser.LoadRequest (NSUrlRequest.FromUrl (new NSUrl (url)));
//        }

//        private bool IsLocalUser
//        {
//            get
//            {
//                NSHttpCookieStorage cookieJar = NSHttpCookieStorage.SharedStorage;
//                return cookieJar.Cookies.Any(cookie => cookie.Name == ".AspNet.Cookies");
//            }
//        }

//        private async Task TestAuthorization()
//        {
//            string uri = String.Format("{0}/api/Values/1", Services.BaseUri);

//            HttpWebRequest request = new HttpWebRequest(new Uri(uri));

//            request.Headers.Add("Authorization", String.Format("Bearer {0}", Services.AccessToken));
//            request.Method = "GET";

//            try
//            {
//                WebResponse response = await request.GetResponseAsync();
//                HttpWebResponse httpResponse = (HttpWebResponse)response;
//                string result;

//                using (Stream responseStream = httpResponse.GetResponseStream())
//                {
//                    result = new StreamReader(responseStream).ReadToEnd();
//                    Console.WriteLine(result);
//                    UIAlertView alert = new UIAlertView ("Success", result, null, "OK", null);
//                    alert.Show ();
//                }
//            }
//            catch (Exception ex)
//            {
//                UIAlertView alert = new UIAlertView ("Failure", ex.GetType().Name, null, "OK", null);
//                alert.Show ();
//            }
//        }
//    }
