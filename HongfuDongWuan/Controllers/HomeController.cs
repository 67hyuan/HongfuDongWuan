using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HongfuDongWuan.Models;

namespace HongfuDongWuan.Controllers
{
    public class HomeController : Controller
    {
        const string username = "******";
        const string password = "******";

        public ActionResult Index(LoginModel lm)
        {
            if (lm.UserName != null && lm.Password !=null)
            {
                if (!lm.UserName.Equals(username) && !lm.Password.Equals(password))
                {
                    ViewBag.NotValidUser = "用户名或密码不正确，请重新输入！";

                    return View("Index");
                }
                else
                {
                    return RedirectToAction("Customer", "Test");
                }
            }
            
            return View();
        }

        public ActionResult OpenFile()
        {
            using (var client = new System.Net.WebClient()) //this is to open new webclient with specifice file
            {
                var dir = Server.MapPath("\\File\\东莞大数据比赛模型.pdf");
                return File(dir, "application/pdf");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}