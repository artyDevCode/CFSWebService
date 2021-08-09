using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CFSWebService.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using ModelEntity;
using System.Web.Script.Serialization;
using CFSWebService.ExternalAccess;
using System.Web.Mvc;


namespace CFSWebService.Controllers
{
    [ApplicationAuthorize]
  //  [RequireHttps]
   // [Authorize]
    public class CFSRecordController : ApiController
    {


        // GET: api/CFSRecord
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/CFSRecord/10-10-2014
        public async Task<List<ActivityTbl>> Get(string RecordedDate)
        {
            using (CFSDB cfsDb = new CFSDB())
            {

              //  System.Security.Claims.ClaimsPrincipal principal = Request.GetRequestContext().Principal as System.Security.Claims.ClaimsPrincipal;
              //  var UserName = principal.Identity.Name;
                var username = User.Identity.Name;
                ActivityTbl servRec;
                List<ActivityTbl> servRecOut = new List<ActivityTbl>();
              //  DateTime recordedDate = DateTime.Parse(RecordedDate); //.ConvertDateStringToJsonDate();
                DateTime recordedDate = RecordedDate.getDate();

               // var CFSDateRecord = await cfsDb.tblCFSDateRecord
               //     .Where(tp => tp.RecordedDate == recordedDate && tp.UserName == User.Identity.Name)
               //     .GroupJoin(cfsDb.tblCFSActivity, c => c.UserName, o => o.UserName,
               //       (c, os) => new { c, Activity = os.Where(r => r.ADateID == c.DRDateID) }).ToListAsync();
                //.GroupJoin(cfsDb.tblCFSActivity, c => c.DRDateID, o => o.ADateID,
                // (c, os) => new { c, Activity = os }).ToListAsync();
                var CFSDateRecord = await cfsDb.CFSActivities
                     .Where( tp => tp.RecordedDate == recordedDate && tp.UserName == User.Identity.Name).ToListAsync();
                foreach (var a in CFSDateRecord)
                {
                    //foreach (var d in a.Activity)
                    //{
                        servRec = new ActivityTbl();
                        servRec.RecordedDate = a.RecordedDate; // a.c.RecordedDate.ToString();
                        servRec.UserName = a.UserName;
                        servRec.ActID = a.ActID;
                        servRec.ADateID = a.ADateID;
                        servRec.FromTime = a.FromTime;
                        servRec.ToTime = a.ToTime;
                        servRec.ActivityType = a.ActivityType;
                        servRec.ActivityDesc = a.ActivityDesc;
                        servRec.ActivityTitle = a.ActivityTitle;
                        servRec.EnergyLevel = a.EnergyLevel;
                        servRec.Field1 = a.Field1;
                        servRec.Field2 = a.Field2;                    
                        servRec.SymptomName = a.SymptomName;
                        servRec.Severity = a.Severity;
                        servRec.SymptomTitle = a.SymptomTitle;
                        servRec.SymptomDesc = a.SymptomDesc;
                        servRecOut.Add(servRec);
                   // };

                };


              //  string json = new JavaScriptSerializer().Serialize(servRecOut);


                return servRecOut;
            }
        }

     
        public async Task<string> Post([FromBody]ActivityTbl RecordedData)
        {
            
            CFSActivity Activity = new CFSActivity();
            CFSDateRecord saveDate = new CFSDateRecord();
            using (CFSDB cfsDb = new CFSDB())
            {
                CFSActivity resultActivity = null;


                var recordedDate = RecordedData.RecordedDate.getDate(); // DateTime.Parse(RecordedData.RecordedDate);
                //saveDate = await cfsDb.tblCFSDateRecord.Where(r => r.RecordedDate == recordedDate &&
                //                r.DRDateID == RecordedData.ADateID
                //                 && r.UserName == RecordedData.UserName).FirstOrDefaultAsync();


                //if (saveDate == null)
                //{
                //    saveDate = new CFSDateRecord();
                //    saveDate.RecordedDate = recordedDate; // RecordedData.RecordedDate.getDate(); 
                //    saveDate.DRDateID = RecordedData.DRDateID;
                //    saveDate.UserName = RecordedData.UserName;
                //    cfsDb.Entry(saveDate).State = EntityState.Added;
                //}
                //else
                //{
                //    saveDate.Id = saveDate.Id;
                //    saveDate.UserName = RecordedData.UserName;
                //    cfsDb.Entry(saveDate).State = EntityState.Modified;
                //}

                //resultActivity = await cfsDb.tblCFSActivity.Where(a => a.ADateID == saveDate.DRDateID && a.ActID == RecordedData.ActID
                //                       && a.UserName == RecordedData.UserName)
                //        .FirstOrDefaultAsync();
                resultActivity = await cfsDb.CFSActivities.Where(a => a.ActID == RecordedData.ActID 
                                  //  && a.ADateID == RecordedData.ADateID
                                    && a.UserName == RecordedData.UserName && a.RecordedDate == RecordedData.RecordedDate) 
                     .FirstOrDefaultAsync();

                if (resultActivity == null)
                {
                    resultActivity = new CFSActivity();
                    cfsDb.Entry(resultActivity).State = EntityState.Added;
                }
                else
                    cfsDb.Entry(resultActivity).State = EntityState.Modified;
                resultActivity.RecordedDate = RecordedData.RecordedDate;
                resultActivity.ActID = RecordedData.ActID;
                resultActivity.ADateID = RecordedData.ADateID;
                resultActivity.FromTime = RecordedData.FromTime;
                resultActivity.ToTime = RecordedData.ToTime;
                resultActivity.ActivityType = RecordedData.ActivityType;
                resultActivity.ActivityTitle = RecordedData.ActivityTitle;
                resultActivity.UserName = RecordedData.UserName;
                resultActivity.ActivityDesc = RecordedData.ActivityDesc;
                resultActivity.EnergyLevel = RecordedData.EnergyLevel;
                resultActivity.Field1 = RecordedData.Field1;
                resultActivity.Field2 = RecordedData.Field2;              
                resultActivity.SymptomName = RecordedData.SymptomName;
                resultActivity.Severity = RecordedData.Severity;
                resultActivity.SymptomTitle = RecordedData.SymptomTitle;
                resultActivity.SymptomDesc = RecordedData.SymptomDesc;


                //try
                //{
                //    cfsDb.Entry(resultActivity).State = resultActivity != null ? EntityState.Modified : EntityState.Added;
                //}
                //catch (Exception exc) { }
                try
                {
                    await cfsDb.SaveChangesAsync();
                    return "Saved";
                }
                catch { return "NOT Saved"; }
            }
            
         
            
        }

        // PUT: api/CFSRecord/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CFSRecord/5
        public void Delete(int id)
        {
        }
    }
}
