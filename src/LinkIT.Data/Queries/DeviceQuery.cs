using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIT.Data.Queries
{
    public class DeviceQuery
    {
        public Guid? Id { get; set; }

        public string Tag { get; set; }

        public string Owner { get; set; }

        public string Brand { get; set; }

        public string Type { get; set; }
    }
}