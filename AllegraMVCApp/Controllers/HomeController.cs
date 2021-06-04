using AllegraApp.Models;
using AllegraMVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AllegraApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppSettings _appSettings;

        public HomeController(ILogger<HomeController> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddUser(UserModel user)
        {
            var userResult = (UserCreateResponse)null;

            try
            {
                using (var httpClient = new HttpClient())
                {

                    httpClient.BaseAddress = _appSettings.BaseUrl;

                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("api/exercise/new-user", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            userResult = JsonConvert.DeserializeObject<UserCreateResponse>(apiResponse);
                        }
                        else
                        {
                            return RedirectToAction("CustomErrorView", JsonConvert.DeserializeObject<ServiceApiResponse>(apiResponse));
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("An error has occured.");
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }

            return View();
        }

        public IActionResult AddExercise()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddExercise(ExerciseModel exercise)
        {
            var exerciseResult = (ExerciseCreateResponse)null;

            try
            {
                using (var httpClient = new HttpClient())
                {

                    httpClient.BaseAddress = _appSettings.BaseUrl;

                    StringContent content = new StringContent(JsonConvert.SerializeObject(exercise), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("api/exercise/add", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        
                        if (response.IsSuccessStatusCode)
                        {
                            exerciseResult = JsonConvert.DeserializeObject<ExerciseCreateResponse>(apiResponse);
                        }
                        else
                        {
                            return RedirectToAction("CustomErrorView", JsonConvert.DeserializeObject<ServiceApiResponse>(apiResponse));
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation("An error has occured.");
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult CustomErrorView(ServiceApiResponse serviceApiResponse)
        {
            return View(serviceApiResponse);
        }
    }
}
