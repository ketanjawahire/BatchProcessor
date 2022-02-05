using System;
using System.Collections.Generic;

namespace DxHackday.Controllers
{
    public class TempResult
    {
        public int RequestId { get; set; }
        public string StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Object RequestResponse { get; set; }
    }
}
