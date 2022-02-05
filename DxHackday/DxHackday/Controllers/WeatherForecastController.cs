using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DxHackday.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 2).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet]
        public WeatherForecast Get()
        {
            using (var client = new HttpClient()) 
            {
                var temp = client.GetAsync("https://api.adbox.pro/v1.0/businessProfiles/3810/mediaPlans/259/lineitems/insights?grouping=Entity&includeToday=false", HttpCompletionOption.ResponseContentRead).GetAwaiter().GetResult();
            }

            var rng = new Random();
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            };
        }

        [HttpGet("{requestId}")]
        public object Get([FromRoute]string requestId)
        {
            return new { abc = string.Join("_", requestId.ToString(), DateTime.Now) };
        }

        [HttpPost]
        public object Post([FromBody] TestRequestModel request)
        {
            return request;
        }

        [HttpPut]
        public object Put([FromBody] TestRequestModel request)
        {
            return request;
        }

        [HttpDelete("{requestId}")]
        public object Delete([FromRoute] string requestId)
        {
            return new { abc = string.Join("_", requestId.ToString(), DateTime.Now) };
        }
    }
    public class TestRequestModel
    {
        public string Test1 { get; set; }
        public string Test2 { get; set; }
        public string Test3 { get; set; }
    }
}
