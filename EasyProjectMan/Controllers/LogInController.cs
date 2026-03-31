using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagement.Manager;
using ProjectManagement.Models;
using ProjectManagement.Models.ViewModels;
using ProjectManagement.Utility;
using System.Collections.Generic;

namespace ProjectManagement.Controllers {

    public class LogInController : Controller {
        private UserAuthenticationManager authManager;

        public LogInController() {
            authManager = new UserAuthenticationManager();
        }

        [HttpGet]
        public IActionResult Login() {
            HttpContext.Session.SetString("userInfo", "");

            ViewData["NoOfProjects"] = authManager.NumberOfProjects();
            ViewData["NoOfTasks"] = authManager.NumberOfTasks();
            ViewData["NoOfNewTasks"] = authManager.NewTasks();
            ViewData["NoOfNewComments"] = authManager.NumberOfNewComments();

            List<int> projects = authManager.GetProjectNumberByMonth();

            ViewData["Jan"] = projects[0];
            ViewData["Feb"] = projects[1];
            ViewData["Mar"] = projects[2];
            ViewData["Apr"] = projects[3];
            ViewData["May"] = projects[4];
            ViewData["Jun"] = projects[5];
            ViewData["Jul"] = projects[6];
            ViewData["Aug"] = projects[7];
            ViewData["Sept"] = projects[8];
            ViewData["Oct"] = projects[9];
            ViewData["Nov"] = projects[10];
            ViewData["Dec"] = projects[11];

            List<int> projectCount = authManager.ThisAndLastTwoYearProjectNumber();
            ViewData["ThisYear"] = projectCount[0];
            ViewData["LastYear"] = projectCount[1];
            ViewData["PrevLastYear"] = projectCount[2];
            return View();
        }

        [HttpPost]
        public IActionResult Login(AuthenticationViewModel model) {
            if (ModelState.IsValid) {
                User user = authManager.CheckAuthentication(model);

                if (user != null) {
                    var userInfo = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString("userInfo", userInfo);

                    return RedirectToAction("Index", "Home");
                }
                else {
                    HttpContext.Session.SetString("userInfo", "");

                    ViewData["Message"] = Alert.AlertGenerate("Falied", "Log in Falied", "Enter Valid Email or Password");
                    return View();
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");
                ViewData["Message"] = Alert.AlertGenerate("Falied", "Log in Falied", "Fill up all fields correctly");
                return View();
            }
        }
    }
}