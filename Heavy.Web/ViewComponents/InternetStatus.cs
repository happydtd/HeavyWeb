using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Heavy.Web.ViewComponents
{
    public class InternetStatus :ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://www.baidu.com");
            if(response.StatusCode == HttpStatusCode.OK)
            {
                return View(true);
            }

            return View(false);
        }
    }
}
