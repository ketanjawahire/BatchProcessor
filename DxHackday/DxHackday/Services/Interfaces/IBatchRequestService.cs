using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DxHackday.Controllers
{
    public interface IBatchRequestService
    {
        Task<JObject> Process(BatchRequestModel model);
    }
}
