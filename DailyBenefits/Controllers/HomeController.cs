using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using JoyOI.UserCenter.SDK;
using DailyBenefits.Model;

namespace DailyBenefits.Controllers
{
    public class HomeController : BaseController<DailyBenefitsContext>
    {
        private static Random rand = new Random();

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return Redirect("/Home/Login");
            }

            var openId = Guid.Parse(HttpContext.Session.GetString("user"));
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var records = await DB.Records
                .Where(x => x.UserId == openId && x.Time >= firstDayOfMonth)
                .ToListAsync(token);

            ViewBag.SignedDaysJson = Newtonsoft.Json.JsonConvert.SerializeObject(records.Select(x => x.Time.Day));

            return View(records);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromServices] JoyOIUC UC, string username, string password, CancellationToken token)
        {
            var result = await UC.TrustedAuthorizeAsync(username, password);
            if (result.succeeded)
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("user", result.data.open_id.ToString());
                HttpContext.Session.SetString("token", result.data.access_token.ToString());
                return Redirect("/");
            }
            else
            {
                return View("InvalidPwd");
            }
        }

        [HttpPost]
        public IActionResult Sign([FromServices] JoyOIUC UC, CancellationToken token)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                return Content("Failed");
            }

            var openId = Guid.Parse(HttpContext.Session.GetString("user"));
            var today = DateTime.UtcNow.Date;

            lock (rand)
            {
                if (DB.Records.Any(x => x.UserId == openId && x.Time >= today))
                {
                    return Content("Signed");
                }

                var rec = new Record
                {
                    Coins = rand.Next(10, 50),
                    Time = DateTime.UtcNow,
                    UserId = openId
                };

                DB.Records.Add(rec);
                UC.IncreaseExtensionCoinAsync(openId, HttpContext.Session.GetString("token"), "Coin", rec.Coins).Wait();
                DB.SaveChanges();

                return Content(rec.Coins.ToString());
            }
        }
    }
}
