using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DxHackday.Controllers
{
    public class BatchRequestModel
    {
        [Required]
        public List<RequestModel> Requests { get; set; }
    }
}
