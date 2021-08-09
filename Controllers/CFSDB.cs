using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ModelEntity;


namespace CFSWebService.Controllers
{
    public class CFSDB : DbContext
    {
        public CFSDB()      : base("name=CFSDB")
        {
        }

        public DbSet<CFSActivity> CFSActivities { get; set; }

    }
   
}