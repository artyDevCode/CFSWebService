using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ModelEntity;

namespace CFSWebService.Json
{
    public class jsonRecord
    {
        public List<ActivityTbl> RecordedData { get; set; }
        public DateTime StartCalendarDate { get; set; }
        public DateTime EndCalendarDate { get; set; }
    }
}