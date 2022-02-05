using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DxHackday.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [CommonExceptionFilter]
    public class BatchController : Controller
    {
        IBatchRequestService _batchRequestService;

        public BatchController(IBatchRequestService batchRequestService, IHttpContextAccessor httpContextAccessor)
        {
            _batchRequestService = batchRequestService;
        }

        [HttpPost]
        public async Task<BatchRequestResponseModel> Post([FromBody] BatchRequestModel model)
        {
            var batchRequestResponseModel = new BatchRequestResponseModel();
            batchRequestResponseModel.Data = await _batchRequestService.Process(model);

            return batchRequestResponseModel;
        }
    }
}
