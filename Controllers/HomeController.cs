using System.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mirror.Models;
using Newtonsoft.Json;
using System.Linq;

namespace mirror.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string logFile = @"dumps.txt";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            if(!System.IO.File.Exists(logFile))
                System.IO.File.Create(logFile);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logs()
        {
            var entries = new List<DataModel>();
            foreach (string line in System.IO.File.ReadLines(logFile))
            {
                entries.Add(JsonConvert.DeserializeObject<DataModel>(line));
            }
            return View(new DataViewModel{Entries = entries.OrderByDescending(e =>e.Timestamp).ToList()});

        }
        [HttpPost]
        public async Task<IActionResult> newentry()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var data = new DataModel{
                    Timestamp =  DateTime.Now,
                    Body = await reader.ReadToEndAsync()
                };
                var json = JsonConvert.SerializeObject(data);

                await System.IO.File.AppendAllTextAsync(logFile, json);
            }
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
