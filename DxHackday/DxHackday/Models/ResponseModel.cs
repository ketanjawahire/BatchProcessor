using System.Collections.Generic;

namespace DxHackday.Controllers
{
    public class ResponseModel
    {
        public int Id { get; set; }
        public string StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public dynamic Body { get; set; }
    }
}
