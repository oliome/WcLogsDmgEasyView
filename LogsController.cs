using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using WcLogsDmgEasyView.Models;

namespace WcLogsDmgEasyView.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<LogsController> _logger;
        private readonly IConfiguration _configuration;

        public LogsController(ILogger<LogsController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

        }
        static Browser Browser;

        static async Task StartBrowser()
        {

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args = new string[]
                {
                    "--headless",
                    "--no-sandbox",
                    "--disable-gpu",
                    "--no-zygote",
                    "--window-size=1920,1080"
                },
                DefaultViewport = null,
                ExecutablePath = Environment.CurrentDirectory + "\\ChromiumBrowser\\Win64-706915\\chrome-win\\chrome.exe"


            };
            Browser = await Puppeteer.LaunchAsync(launchOptions);

            if (!Browser.IsConnected || Browser == null) { 
                Browser = await Puppeteer.LaunchAsync(launchOptions);
                Console.WriteLine("Browser launched");
              
            }
        }

        [HttpGet]
        [Route("GetFromLogAsync/{report}")]
        public async Task<IActionResult> Get(string report)
        {
            var apiKey = _configuration["ApiKey"];
            List<JToken> friendlies = new List<JToken>();
            List<JToken> bosses = new List<JToken>();
            var listOfPlayers = new List<Player>();
            var listOfBosses = new List<Boss>();

            using (var client = new HttpClient())
            {

                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.BaseAddress = new Uri("https://classic.warcraftlogs.com/v1/report/fights/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var endpoint = report + apiKey;
                    HttpResponseMessage response = await client.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        var resp = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                        friendlies = resp["friendlies"].Children().ToList();
                        bosses = resp["fights"].Children().ToList();
                        _logger.LogTrace(response.ReasonPhrase);
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError(response.ReasonPhrase);
                        return BadRequest(response.ToString());
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return BadRequest(e.Message);
                }

            }

            foreach (var friend in friendlies)
            {
                friend.Last.Remove();
                listOfPlayers.Add(friend.ToObject<Player>());
            }

            foreach (var boss in bosses)
            {
                if (boss.Value<double>("boss") != 0)
                {
                    listOfBosses.Add(new Boss
                    {
                        Name = boss.Value<string>("name"),
                        Id = boss.Value<int>("id")
                    });
                }

            }

            var listOfNpcs = listOfPlayers.Where(x => x.Type == "NPC" || x.Type == "Pet");
            await StartBrowser();
            return Ok(listOfPlayers.OrderBy(x => x.Type).Except(listOfNpcs));


        }
        [HttpGet]
        [Route("GenerateImage/{reportId}/{id}")]
        public async Task<IActionResult> GetImageById(string id, string reportId)
        {
            var address =
                    ($"https://classic.warcraftlogs.com/reports/{reportId}#boss=-3&difficulty=0&type=damage-done&source={id}");
            await StartBrowser();
            var result = await ProcessRequest(address);
                
            await Browser.DisposeAsync();
            await Browser.CloseAsync();
            return Ok(result);


        }

        public async Task<byte[]> ProcessRequest(string address)
        {

            byte[] image = new byte[] { };


            await using var page = await Browser.NewPageAsync();
            try
            {
                await page.GoToAsync(address);
                await page.WaitForSelectorAsync("#view-tabs-and-table-container");
                await page.ClickAsync("button[type='button'][mode='primary']");
            }
            catch (Exception e)
            {
                if (e.GetType().ToString() == "PuppeteerSharp.NavigationException" || e.GetType().ToString() == "PuppeteerSharp.WaitTaskTimeoutException")
                {
                    await page.CloseAsync();
                    await page.DisposeAsync();
                    return null;
                }
                Console.WriteLine(e);
            }
            var element = await page.QuerySelectorAsync("#view-tabs-and-table-container");
            image = await element.ScreenshotDataAsync();

            await page.CloseAsync();
            await page.DisposeAsync();

            return image;
        }
    }
      
}

