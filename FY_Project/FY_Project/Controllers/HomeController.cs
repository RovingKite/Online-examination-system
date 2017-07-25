using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FY_Project.Models;

namespace FY_Project.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        FYPDBEntities obj = new FYPDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(User u)
        {
            User us = new User();
            if(ModelState.IsValid)
            {
                try
                {
                    us = obj.Users.First(a => a.User_Id.Equals(u.User_Id) && a.Password.Equals(u.Password));
                    if(us.Role.Equals("Teacher"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Name"] = us.First_Name +" "+ us.Last_Name;
                        return RedirectToAction("Index","Teacher");
                    }
                    else if(us.Role.Equals("Student"))
                    {
                        Session["User_Id"] = us.User_Id;
                        Session["User_Name"] = us.First_Name + " " + us.Last_Name;
                        return RedirectToAction("Index", "Student");
                    }
                }
                catch
                {
                    ViewBag.Message = "User not found";
                    return View();
                }
            }
            else
            {
                return View();
            }
            return View();
        }
    }
}
