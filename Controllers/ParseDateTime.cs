using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

namespace CFSWebService.Controllers
{
     
    public static class ParseDateTime
    {
        public static DateTime getDate(this DateTime datePartin)
        {
           // DateTime datePart; 
            //try
            //{
               
                return new DateTime(datePartin.Year, datePartin.Month, datePartin.Day);
            //}
            //catch  
            //{
            //    datePart = DateTime.ParseExact(datePartin.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //    return new DateTime(datePart.Year, datePart.Month, datePart.Day);
            //}
           
      //      return datePartin;
        }

        public static DateTime getDate(this string datePartin)
        {
           
            DateTime datePart; // = Convert.ToDateTime(datePartin);
           
            
            //  = DateTime.Parse(datePartin); 
           
            //try
            //{             
                datePart  = DateTime.Parse(datePartin);
                return new DateTime(datePart.Year, datePart.Month, datePart.Day);
            //}
            //catch 
            //{       
            //   // DateTime.TryParseExact(datePartin, "MM/dd/yyyy", null,DateTimeStyles.None, out datePart);
            //    datePart = DateTime.ParseExact(datePartin.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //    return new DateTime(datePart.Year, datePart.Month, datePart.Day);
            //}
           
          
        }


        //public static string ConvertDateStringToJsonDate(this DateTime thisdate)
        //{
        //    string result = string.Empty;
        //    DateTime dt = thisdate; //DateTime.Parse(thisdate);
        //    dt = dt.ToUniversalTime();
        //    TimeSpan ts = dt - DateTime.Parse("1970-01-01");
        //    result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
        //    return result;
        //}

    }
}