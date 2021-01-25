using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IBackgroundTaskQueue Queue { get; }
        public HomeController(
            IBackgroundTaskQueue queue,
            ILogger<HomeController> logger)
        {
            _logger = logger;
            Queue = queue;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost,ActionName("Index")]
        public IActionResult IndexPost()
        {
            // 遅い処理をバックグラウンドで実行させる
            Queue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(1000);
                Debug.WriteLine("A");
                await Task.Delay(1000);
                Debug.WriteLine("B");
            });

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
