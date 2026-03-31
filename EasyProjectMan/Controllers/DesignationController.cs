using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectManagement.Manager;
using ProjectManagement.Models;
using ProjectManagement.Utility;

namespace ProjectManagement.Controllers {

    public class DesignationController : Controller {
        private DesignationManager designationManager;
        private UserAccessManager userAccess;

        public DesignationController() {
            designationManager = new DesignationManager();
            userAccess = new UserAccessManager();
        }

        // save designation
        [HttpGet]
        public IActionResult Save() {
            var authData = HttpContext.Session.GetString("userInfo");

            if (!string.IsNullOrWhiteSpace(authData) && authData != "") {
                User user = JsonConvert.DeserializeObject<User>(authData);

                if (userAccess.HasAccess(user.Id, -30, user.DesignationId)) {
                    return View();
                }
                else {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");

                return RedirectToAction("Login", "LogIn");
            }
        }

        [HttpPost]
        public IActionResult Save(Designation designation) {
            var authData = HttpContext.Session.GetString("userInfo");

            if (!string.IsNullOrWhiteSpace(authData) && authData != "") {
                User user = JsonConvert.DeserializeObject<User>(authData);

                if (userAccess.HasAccess(user.Id, -30, user.DesignationId)) {
                    if (ModelState.IsValid) {
                        designation.State = 1;
                        ViewData["Message"] = designationManager.Save(designation);

                        ModelState.Clear();
                        return View();
                    }
                    else {
                        ViewData["Message"] = Alert.AlertGenerate("Failed", "Falied", "Fill up all fields correctly");
                        return View(designation);
                    }
                }
                else {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");

                return RedirectToAction("Login", "LogIn");
            }
        }

        // view all designation
        [HttpGet]
        public IActionResult ViewAll() {
            var authData = HttpContext.Session.GetString("userInfo");

            if (!string.IsNullOrWhiteSpace(authData) && authData != "") {
                User user = JsonConvert.DeserializeObject<User>(authData);

                if (userAccess.HasAccess(user.Id, -30, user.DesignationId)) {
                    ViewBag.Designations = designationManager.GetAll();
                    return View();
                }
                else {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");

                return RedirectToAction("Login", "LogIn");
            }
        }

        // edit designation
        [HttpGet]
        public IActionResult Edit(int id) {
            var authData = HttpContext.Session.GetString("userInfo");

            if (!string.IsNullOrWhiteSpace(authData) && authData != "") {
                User user = JsonConvert.DeserializeObject<User>(authData);

                if (userAccess.HasAccess(user.Id, -30, user.DesignationId)) {
                    if (designationManager.IsDesignationExists(id)) {
                        Designation designation = designationManager.GetById(id);
                        return View(designation);
                    }
                    else {
                        return NotFound("404 Not Found");
                    }
                }
                else {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");

                return RedirectToAction("Login", "LogIn");
            }
        }

        [HttpPost]
        public IActionResult Edit(Designation designation) {
            var authData = HttpContext.Session.GetString("userInfo");

            if (!string.IsNullOrWhiteSpace(authData) && authData != "") {
                User user = JsonConvert.DeserializeObject<User>(authData);

                if (userAccess.HasAccess(user.Id, -30, user.DesignationId)) {
                    if (designationManager.IsDesignationExists(designation.Id)) {
                        if (ModelState.IsValid) {
                            designation.State = 1;
                            string updated = designationManager.Update(designation);

                            if (updated.Equals("1")) {
                                return RedirectToAction("ViewAll", "Designation");
                            }
                            else {
                                ViewData["Message"] = updated;
                                return View(designation);
                            }
                        }
                        else {
                            ViewData["Message"] = Alert.AlertGenerate("Failed", "Failed", "Fill up all fields correctly"); ;
                            return View(designation);
                        }
                    }
                    else {
                        return NotFound("404 Not Found");
                    }
                }
                else {
                    return RedirectToAction("AccessDenied", "Home");
                }
            }
            else {
                HttpContext.Session.SetString("userInfo", "");

                return RedirectToAction("Login", "LogIn");
            }
        }
    }
}