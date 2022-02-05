using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;

namespace DxHackday.Controllers
{
    public class RequestModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpMethod Method { get; set; }

        [Required]
        public string Url { get; set; }
        
        public dynamic Data { get; set; }
        
        public int? DependsOn { get; set; }
    }
}
