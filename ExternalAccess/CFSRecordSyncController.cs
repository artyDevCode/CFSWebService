using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using CFSWebService.Json;
using CFSWebService.Models;
using System.Data.Entity;
using ModelEntity;
using System.Web.Script.Serialization;
using CFSWebService.ExternalAccess;
using System.Web.Mvc;
using System.Web.Script.Services;

namespace CFSWebService.Controllers
{

    [ApplicationAuthorize]
   // [RequireHttps]

    public class CFSRecordSyncController : ApiController
    {
        // GET: api/CFSRecordSync
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET: api/CFSRecordSync/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST: api/CFSRecordSync
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT: api/CFSRecordSync/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}     
        [ScriptMethod(ResponseFormat=ResponseFormat.Json)]
        public async Task<List<ActivityTbl>> Post([FromBody]jsonRecord RecordedData)
        {

           // List<ActivityTbl> servRecOut = new List<ActivityTbl>();
            List<ActivityTbl> servRecInArray = new List<ActivityTbl>();
            IEnumerable<ActivityTbl> CFSDateRecordOut;
        
          //  ActivityTbl servRecIn;
          //  CFSDateRecord saveDate;
            CFSActivity resultActivity;
            
          //  List<ActivityTbl> RecordedDataOut = new List<ActivityTbl>();

            using (CFSDB cfsDb = new CFSDB())
            {


                foreach (var actSync in RecordedData.RecordedData)
                {
                    resultActivity = new CFSActivity();
                   // saveDate = new CFSDateRecord();
                    

                    var recordedDate = actSync.RecordedDate.getDate();
                  //  RecordedDataOut.Add(actSync);
/*
                    saveDate = await cfsDb.tblCFSDateRecord.Where(r => r.RecordedDate == recordedDate && r.UserName == actSync.UserName 
                         )
                         .FirstOrDefaultAsync();

                    if (saveDate == null)
                    {
                        saveDate = new CFSDateRecord();
                        saveDate.RecordedDate = recordedDate; // actSync.RecordedDate.getDate(); 
                        saveDate.DRDateID = actSync.DRDateID;
                        saveDate.UserName = actSync.UserName;
                        cfsDb.Entry(saveDate).State = EntityState.Added;
                    }
                    else
                    {
                        saveDate.Id = saveDate.Id;                    
                        saveDate.UserName = actSync.UserName;  //this is the only change in the DateRecords database table

                        cfsDb.Entry(saveDate).State = EntityState.Modified;

                    }
 * */
                 //   resultActivity = await cfsDb.tblCFSActivity.Where(a => a.ADateID == actSync.ADateID && a.ActID == actSync.ActID
                 //         && a.UserName == actSync.UserName)
                  //          .FirstOrDefaultAsync();
                    resultActivity = await cfsDb.CFSActivities.Where(a => a.RecordedDate == recordedDate 
                        && a.UserName == actSync.UserName && a.ActID == actSync.ActID 
                       // && a.ADateID == actSync.ADateID
                        ).FirstOrDefaultAsync();

                    if (resultActivity == null)
                    {
                        resultActivity = new CFSActivity();
                        cfsDb.Entry(resultActivity).State = EntityState.Added;
                    }
                    else
                        cfsDb.Entry(resultActivity).State = EntityState.Modified;
                
                    resultActivity.RecordedDate = actSync.RecordedDate;
                    resultActivity.ActID = actSync.ActID;
                    resultActivity.ADateID = actSync.ADateID;                  
                    resultActivity.FromTime = actSync.FromTime;
                    resultActivity.ToTime = actSync.ToTime;
                    resultActivity.ActivityType = actSync.ActivityType;
                    resultActivity.ActivityTitle = actSync.ActivityTitle;
                    resultActivity.UserName = actSync.UserName;
                    resultActivity.ActivityDesc = actSync.ActivityDesc;
                    resultActivity.EnergyLevel = actSync.EnergyLevel;
                    resultActivity.Field1 = actSync.Field1;
                    resultActivity.Field2 = actSync.Field2;
                    resultActivity.SymptomName = actSync.SymptomName;
                    resultActivity.Severity = actSync.Severity;
                    resultActivity.SymptomTitle = actSync.SymptomTitle;
                    resultActivity.SymptomDesc = actSync.SymptomDesc;

                  
                    
                    try
                    {
                       await cfsDb.SaveChangesAsync();
                    }
                    catch (Exception exc2) { }

                    //create temporary table to check difference between records sent by client
                    // servRecInArray.Add(actSync);

                     ActivityTbl servRecIn = new ActivityTbl();
                     servRecIn.RecordedDate = actSync.RecordedDate;
                     servRecIn.ActID = actSync.ActID;
                     servRecIn.ADateID = actSync.ADateID;
                     servRecIn.FromTime = actSync.FromTime;
                     servRecIn.ToTime = actSync.ToTime;
                     servRecIn.ActivityType = actSync.ActivityType;
                     servRecIn.ActivityTitle = actSync.ActivityTitle;
                     servRecIn.UserName = actSync.UserName;
                     servRecIn.ActivityDesc = actSync.ActivityDesc;
                     servRecIn.EnergyLevel = actSync.EnergyLevel;
                     servRecIn.Field1 = actSync.Field1;
                     servRecIn.Field2 = actSync.Field2;
                     servRecIn.SymptomName = actSync.SymptomName;
                     servRecIn.Severity = actSync.Severity;
                     servRecIn.SymptomTitle = actSync.SymptomTitle;
                     servRecIn.SymptomDesc = actSync.SymptomDesc;
                     servRecInArray.Add(servRecIn);                
                }
              
                //var CFSDateRecord = await cfsDb.tblCFSDateRecord
                //    .Where(tp => tp.RecordedDate >= RecordedData.StartCalendarDate && tp.RecordedDate <= RecordedData.EndCalendarDate
                //            && tp.UserName == User.Identity.Name)
                //    .GroupJoin(cfsDb.tblCFSActivity, c => c.DRDateID, o => o.ADateID,
                //        (c, os) => new { c, Activity = os }).ToListAsync();

            //    var CFSDateRecord = await cfsDb.tblCFSDateRecord
            //       .Where(tp => tp.RecordedDate >= RecordedData.StartCalendarDate && tp.RecordedDate <= RecordedData.EndCalendarDate
            //               && tp.UserName == User.Identity.Name)
            //       .GroupJoin(cfsDb.tblCFSActivity, c => c.UserName, o => o.UserName,
            //           (c, os) => new { c, Activity = os.Where(r => r.ADateID == c.DRDateID) }).ToListAsync();

                CFSDateRecordOut = await cfsDb.CFSActivities
                .Where(tp => tp.RecordedDate >= RecordedData.StartCalendarDate && tp.RecordedDate <= RecordedData.EndCalendarDate
                        && tp.UserName == User.Identity.Name)
                        .Select(r => new ActivityTbl
                        {
                            RecordedDate = r.RecordedDate,
                            ActID = r.ActID,
                            ADateID = r.ADateID,
                            FromTime = r.FromTime,
                            ToTime = r.ToTime,
                            ActivityType = r.ActivityType,
                            ActivityTitle = r.ActivityTitle,
                            UserName = r.UserName,
                            ActivityDesc = r.ActivityDesc,
                            EnergyLevel = r.EnergyLevel,
                            Field1 = r.Field1,
                            Field2 = r.Field2,
                            SymptomName = r.SymptomName,
                            Severity = r.Severity,
                            SymptomTitle = r.SymptomTitle,
                            SymptomDesc = r.SymptomDesc
                        }).ToListAsync();
                           
                            //.ToListAsync();
                      
         
            }


            var RecordDiff = CFSDateRecordOut.Except(servRecInArray, new ActivityComparer()).ToList(); //servRecInArray.Except(CFSDateRecordOut).ToList(); 
            
         //   var RecordDiff1 = servRecInArray.Except(CFSDateRecordOut).Union(CFSDateRecordOut.Except(servRecInArray));
           // var aa = Enumerable.SequenceEqual(servRecInArray, CFSDateRecordOut);
           // return new JavaScriptSerializer().Serialize(RecordDiff);            
            return RecordDiff;
        }

        // DELETE: api/CFSRecordSync/5
        public void Delete(int id)
        {
        }


    }
    class ActivityComparer : IEqualityComparer<ActivityTbl>
    {
        // Products are equal if their names and product numbers are equal. 
        public bool Equals(ActivityTbl x, ActivityTbl r)
        {

            // Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, r)) return true;

            // Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(r, null))
                return false;

            // Check whether the products' properties are equal. 

            return x.RecordedDate == r.RecordedDate &&
                    x.ActID == r.ActID &&
                    x.ADateID == r.ADateID &&
                    x.FromTime == r.FromTime &&
                    x.ToTime == r.ToTime &&
                    x.ActivityType == r.ActivityType &&
                    x.ActivityTitle == r.ActivityTitle &&
                    x.UserName == r.UserName &&
                    x.ActivityDesc == r.ActivityDesc &&
                    x.EnergyLevel == r.EnergyLevel &&
                    x.Field1 == r.Field1 &&
                    x.Field2 == r.Field2 &&
                    x.SymptomName == r.SymptomName &&
                    x.Severity == r.Severity &&
                    x.SymptomTitle == r.SymptomTitle &&
                    x.SymptomDesc == r.SymptomDesc;


           // return x.Code == y.Code && x.Name == y.Name;
        }

        // If Equals() returns true for a pair of objects, 
        // GetHashCode must return the same value for these objects. 

        public int GetHashCode(ActivityTbl r)
        {
            // Check whether the object is null. 
            if (Object.ReferenceEquals(r, null)) return 0;


            // Get the hash code for the Name field if it is not null. 
          //  int hashProductName = product.Name == null ? 0 : product.Name.GetHashCode();

            // Get the hash code for the Code field. 
           // int hashProductCode = product.Code.GetHashCode();

            // Calculate the hash code for the product. 
           // return hashProductName ^ hashProductCode;
            int hashRecordedDate = r.RecordedDate.GetHashCode();
            int hashActID = r.ActID = r.ActID == 0 ? 0 : r.ActID.GetHashCode();
            int hashADateID = r.ADateID = r.ADateID == 0 ? 0 : r.ADateID.GetHashCode();
            int hashFromTime = r.FromTime.GetHashCode();
            int hashToTime = r.ToTime.GetHashCode();
            int hashActivityType = r.ActivityType.GetHashCode();
            int hashActivityTitle = r.ActivityTitle.GetHashCode();
            int hashUserName = r.UserName.GetHashCode();
            int hashActivityDesc = r.ActivityDesc.GetHashCode();
            int hashEnergyLevel = r.EnergyLevel.GetHashCode();
            int hashField1 = r.Field1.GetHashCode();
            int hashField2 = r.Field2.GetHashCode();
            int hashSymptomName = r.SymptomName.GetHashCode();
            int hashSeverity = r.Severity.GetHashCode();
            int hashSymptomTitle = r.SymptomTitle.GetHashCode();
            int hashSymptomDesc = r.SymptomDesc.GetHashCode();


            return hashRecordedDate ^
                   hashActID ^
                   hashADateID ^
                   hashFromTime ^
                   hashToTime ^
                   hashActivityType ^
                   hashActivityTitle ^
                   hashUserName ^
                   hashActivityDesc ^
                   hashEnergyLevel ^
                   hashField1 ^
                   hashField2 ^
                   hashSymptomName ^
                   hashSeverity ^
                   hashSymptomTitle ^
                   hashSymptomDesc;

        }

    }

}
