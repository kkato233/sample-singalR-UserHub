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
using WebApplication1.Utility;

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
            string userGroup = this.HttpContext.User.Identity.Name;

            if (userGroup != null)
            {
                UserHubSignalR personSignal = new UserHubSignalR(Request, userGroup);

                // 遅い処理をバックグラウンドで実行させる
                Queue.QueueBackgroundWorkItem(async token =>
                {
                    await personSignal.SendMessageAsync("開始しました");

                    await Task.Delay(1000);
                    // ユーザに非同期で通知
                    await personSignal.SendMessageAsync(".. A ..");

                    await Task.Delay(1000);
                    await personSignal.SendMessageAsync(".. B ..");

                    await Task.Delay(1500);

                    // ユーザに非同期で通知
                    await personSignal.SendMessageAsync("完了しました");

                    // 完了通知
                    await personSignal.SendClientFinishMessageAsync();
                });
            }
            

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
