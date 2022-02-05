using DxHackday.Helpers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DxHackday.Controllers
{
    public class BatchRequestService : IBatchRequestService
    {
        HttpClient httpClient;
        HttpContext _httpContext;

        public BatchRequestService(IHttpContextAccessor httpContextAccessor)
        {
            httpClient = new HttpClient();
            _httpContext = httpContextAccessor.HttpContext;
        }

        public void Validate(BatchRequestModel model)
        {
            int maxRequestLimit = 30;
            
            //Check if request exceeding max limit
            if(!(model.Requests.Count > 0 && model.Requests.Count <= maxRequestLimit))
            {
                throw new InvalidRequestLengthException(model.Requests.Count);
            }

            //Requests should not contain batch request itself
            if(model.Requests.Where(e=> e.Url.Contains("/batch")).Count() > 0)
            {
                throw new BatchInBatchException();
            }
            
            //There has to be atleast one parent request
            if(model.Requests.Count() <= model.Requests.Where(e=> e.DependsOn.HasValue).Count())
            {
                throw new InvalidParentRequestsException();
            }

            //Check if all requests are marked with distinct id
            if (model.Requests.Select(e => e.Id).Distinct().Count() != model.Requests.Count)
            {
                throw new DuplicateRequestIdsException();
            }

            //All requests marked as dependsOn should exist in the payload
            var invalidParentRequests = (from b in model.Requests.Where(e => e.DependsOn.HasValue).Select(e => e.DependsOn).Distinct()
                       join a in model.Requests.Select(e => (int?)e.Id) on b equals a
                       into ab
                       from c in ab.DefaultIfEmpty(null)
                       where c is null
                       select b.Value).ToList();
            if(invalidParentRequests.Count > 0)
            {
                throw new InvalidRequestIdsException(invalidParentRequests.ToArray());
            }

            //TODO : check for circular dependency
        }

        public async Task<JObject> Process(BatchRequestModel batchModel)
        {
            Validate(batchModel);

            var finalResult = new ConcurrentBag<TempResult>();

            var tasks = batchModel.Requests
                 .Where(e => e.DependsOn is null)
                 .AsParallel()
                 .Select(async baseRequest =>
                 {
                     var baseRequestResponse = await GetHttpRequstTask(baseRequest).ConfigureAwait(false);

                     var parsedResponseMsg = await ParseResponseContent(baseRequestResponse);

                     var baseRequestResult = new TempResult()
                     {
                         RequestId = baseRequest.Id,
                         Headers = baseRequestResponse.Headers.ToDictionary(e => e.Key, a => string.Join(";", a.Value)),
                         StatusCode = baseRequestResponse.StatusCode.ToString(),
                         RequestResponse = parsedResponseMsg
                     };

                     finalResult.Add(baseRequestResult);

                     var dependentRequests = batchModel.Requests.Where(e => e.DependsOn.HasValue && e.DependsOn.Value == baseRequest.Id);

                     if (dependentRequests.Count() > 0)
                     {
                         //If parent request is failed(other than 201 OK status code) then we dont have to process dependent requests.
                         if (baseRequestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                         {
                             var dependentRequestResults = dependentRequests.Select(async f =>
                             {
                                 var replacedObject = ReplaceMacros(f, baseRequestResult);

                                 var test = await GetHttpRequstTask(replacedObject).ConfigureAwait(false);

                                 var parsedResponseMsg1 = await ParseResponseContent(test);

                                 var temp1 = new TempResult()
                                 {
                                     RequestId = replacedObject.Id,
                                     StatusCode = test.StatusCode.ToString(),
                                     Headers = test.Headers.ToDictionary(e => e.Key, a => string.Join(";", a.Value)),
                                     RequestResponse = parsedResponseMsg1
                                 };

                                 return temp1;
                             });

                             var result = await Task.WhenAll(dependentRequestResults).ConfigureAwait(false);
                             Array.ForEach(result, e => finalResult.Add(e));
                         }
                         else
                         {
                             dependentRequests.ToList().ForEach(f =>
                             {
                                 var temp1 = new TempResult()
                                 {
                                     RequestId = f.Id,
                                     StatusCode = "Skipped",
                                     Headers = null,
                                     RequestResponse = "Skipped execution due to Parent request failure. Check parent request for exception details."
                                 };

                                 finalResult.Add(temp1);
                             });
                         }
                     }

                     return await Task.FromResult(1).ConfigureAwait(false);
                 });


            var asd = await Task.WhenAll(tasks);

            var finalJObject = new JObject();
            finalResult.OrderBy(e => e.RequestId).ToList().ForEach(e => finalJObject.Add(e.RequestId.ToString(), JToken.FromObject(e)));

            return finalJObject;
        }

        public async Task<dynamic> ParseResponseContent(HttpResponseMessage responseMessage)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
                return content;
            if (string.IsNullOrWhiteSpace(content))
                return content;

            var token = JToken.Parse(content);
            if (token.Type == JTokenType.Object)
            {
                return token.ToObject<dynamic>();
            }
            else if (token.Type == JTokenType.Array)
            {
                return token.ToArray();
            }
            else
            {
                return token.ToString();
            }
        }

        public RequestModel ReplaceMacros(RequestModel requestModel, TempResult baseRequestResult)
        {
            var macroCollection = _httpContext.Request.Headers.ToDictionary(e => $"{{{{baseRequest_headers_{e.Key}}}}}", e => string.Join(";", e.Value));

            var serializedRequestModel = JsonConvert.SerializeObject(requestModel);
            
            //Get all macros from current request.
            var macros = MacroHelper.GetAllMacros(serializedRequestModel);

            //Nothing to process here. return
            if (macros.Count() == 0)
                return requestModel;

            var refObject = JObject.FromObject(baseRequestResult.RequestResponse);

            foreach (var macro in macros.Where(e=> !e.Contains("baseRequest")))
            {
                var token = refObject.SelectToken(macro.Replace($"{{{{request{baseRequestResult.RequestId}_body_", string.Empty).Replace("}}", string.Empty));

                if (token != null)
                    macroCollection.Add(macro, token.ToString());
            }

            var macroReplacedString = MacroHelper.ReplaceMacros(serializedRequestModel, macroCollection);

            //return updated object
            return JsonConvert.DeserializeObject<RequestModel>(macroReplacedString);
        }

        public async Task<HttpResponseMessage> GetHttpRequstTask(RequestModel requestModel)
        {
            var msg = new HttpRequestMessage(new System.Net.Http.HttpMethod(requestModel.Method.ToString()), requestModel.Url);
            _httpContext.Request.Headers.ToList().ForEach(e => msg.Headers.TryAddWithoutValidation(e.Key, string.Join(";", e.Value)));

            msg.Headers.Host = msg.RequestUri.Host;

            if (requestModel.Method == HttpMethod.POST || requestModel.Method == HttpMethod.PUT || requestModel.Method == HttpMethod.PATCH)
            {
                //TODO : handle various content types.
                msg.Content = new StringContent(JsonConvert.SerializeObject(requestModel.Data), System.Text.Encoding.UTF8);
                msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            return await httpClient.SendAsync(msg);
        }

        ~BatchRequestService()
        {
            httpClient.Dispose();
        }
    }
}
