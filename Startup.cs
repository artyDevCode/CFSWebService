using CFSWebService;
using CFSWebService.ExternalAccess;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Host.SystemWeb;
using Owin;
using System;
using System.Web.Http;
using CFSWebService.Providers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(CFSWebService.Startup))]
namespace CFSWebService
{
    public partial class Startup
    {
      
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureAuth(app);
            ConfigureOAuth(app);          

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

        }

        public static OAuthAuthorizationServerOptions OAuthServerOptions { get; private set; }
        public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }
        public static string PublicClientId { get; private set; }
        public void ConfigureOAuth(IAppBuilder app)
        {
            PublicClientId = "self";

            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory) //SimpleAuthorizationServerProvider()
            };
            app.UseOAuthBearerTokens(OAuthServerOptions);
         
            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

         }
      }

     
}
/*
  for (int b = 1; b < 30; b++)
            {

                for(int a=1; a < 30; a++)
                {
                   context.CFSActivities.AddOrUpdate(p => p.ActID,
                   new ModelEntity.CFSActivity
                   {
                    RecordedDate = DateTime.Parse("2015-05-" + b + " 00:00:00"), 
                    UserName = "tbkawa@hotmail.com",
                    ADateID = 0, 
                    ActID = a, 
                    FromTime = "7:57 PM", 
                    ToTime = "7:58 PM", 
                    ActivityType = "Play", 
                    ActivityDesc = "Desc" + a, 
                    ActivityTitle = "Title" + a, 
                    EnergyLevel = 20, 
                    SymptomName = "Headaches=Primary--Loss of muscular strength=Primary--Muscle and-or joint pain=Primary--Un-refreshing sleep=Primary--Sore Throat=Primary--Severe Fatigue=Primary--Seizures=Neurological and cardiac--Fainting=Neurological and cardiac--Dizziness and-or vertigo=Neurological and cardiac--Irregular heartbeat=Neurological and cardiac--Light headedness=Neurological and cardiac--Shortness of Breath=Neurological and cardiac--Chest Pains=Neurological and cardiac--Abdominal Pain=Gastrointestinal--Frequent bloating=Gastrointestinal--Frequent constipation=Gastrointestinal--Food cravings=Gastrointestinal--Frequent diarrhoea=Gastrointestinal--Irritable bowl=Gastrointestinal--Reflux=Gastrointestinal--Vomiting=Gastrointestinal--Appetite Changes=Gastrointestinal--Difficulty making decisions=Cognitive--Difficulty with numbers=Cognitive--Hand writing difficulty=Cognitive--Impairment of speech=Cognitive--Memory loss or brain fog=Cognitive--Poor judgement=Cognitive--Reading difficulty=Cognitive--Slowed Speech=Cognitive--Blurred Vision=Sensory--Dryness of the mouth and eyes=Sensory--Eye pain - spasms=Sensory--Numbness or tingling sensations=Sensory--Ringing in the ears=Sensory--Sensitive to Light=Sensory--Sensitive to Noise=Sensory--Chronic cough=Flu like--Chills and night sweat=Flu like--Low grade fever=Flu like--Nausea=Flu like--Recurrent infections and-or flu=Flu like--Rash=Flu like--Tender or swollen lymph nodes=Flu like",
                    Severity = "Mild", 
                    SymptomTitle = "", 
                    SymptomDesc = "Symptom desc" + a, 
                    Field1 = "", 
                    Field2 =""
                   });  

                }
         }
 
 */