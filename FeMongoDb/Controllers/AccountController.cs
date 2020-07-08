using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FeMongoDb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FeMongoDb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Success()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account objUser)
        {
            var result = await IsAuthenticated(objUser);
            if (result is null)
            {
                //var name = HttpContext.Request.Query["RequestPath"];
                //ViewBag.RequestPath = objUser.RequestPath;
                ViewBag.Text = "Username hoặc mật khẩu không đúng";
                return View();
            }



            //var iddmuser = Convert.ToInt32(result.Iddmuser);
            //var resultRoleAwait = await _iUserRepo.GetUserRoleByIddmuser(iddmuser);
            //var resultRole = resultRoleAwait.Select(m=>m.Role).ToList();
            //string resultRolestr = string.Join(",", resultRole);


            ////tạo claim (có thể tạo nhiều claim khác nhau)
            //var longClaim = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, result.Username),
            //    new Claim(ClaimTypes.Name, result.Iddmuser.ToString(CultureInfo.InvariantCulture)),
            //    new Claim(ClaimTypes.Name, result.Tenuser),
            //    new Claim(ClaimTypes.Role, resultRolestr),
                
            //};

            ////tạo identity
            //var longIdentity = new ClaimsIdentity(longClaim, "Long Identity");
            ////var licenseIdentity = new ClaimsIdentity(licenseClaim, "license Identity");

            ////tạo principle, add nhiều claim và mảng
            //var principle = new ClaimsPrincipal(new []
            //{
            //    longIdentity
            //});

            //// sign-in
            //await HttpContext.SignInAsync(
            //    scheme: "DemoSecurityScheme",
            //    principal: principle,
            //    properties: new AuthenticationProperties
            //    {
            //        //IsPersistent = true, // for 'remember me' feature
            //        //ExpiresUtc = DateTime.UtcNow.AddMinutes(1)
            //    });
            

            //return Redirect(objUser.RequestPath ?? "/");
            ViewBag.Account = result;
            return View("Success");
        }

        private async Task<List<Account>> IsAuthenticated(Account objUser)
        {
            var url = _configuration.GetSection("baseUrl:ApiMongo").Value;
            List<Account> content = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                
                var responseTask = await client.PostAsJsonAsync("/api/Account/Login", objUser);
                
                var result = responseTask;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = await result.Content.ReadAsAsync<List<Account>>();
                    
                    if (readTask is null)
                    {
                        return null;
                    }

                    content = readTask.ToList();
                }
                else 
                {
                    content = null;
                }
            }

            return content;
        }
    }
}