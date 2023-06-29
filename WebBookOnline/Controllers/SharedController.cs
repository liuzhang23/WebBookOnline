using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBookOnline.Models;

namespace WebBookOnline.Controllers
{
    public class SharedController : Controller
    {
        private BookReadingDataBaseEntities db = new BookReadingDataBaseEntities();
        // GET: Shared
        public ActionResult Layout()
        {
            ViewBag.listCategories = db.Categories.ToList();
            return View();
        }
    }
}