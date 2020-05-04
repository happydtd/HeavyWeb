using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Heavy.Web.Models;
using Heavy.Web.Filters;

namespace Heavy.Web.Controllers
{
    
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 30 ,Location = ResponseCacheLocation.Client)]
        [LogResourceFilter]
        public IActionResult Index()
        {
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
