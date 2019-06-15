using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using HantnewCMS.Models;

namespace HantnewCMS.Controllers
{
    public class FilterController : Controller
    {
        // GET: H
        Conn conn = new Conn();
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeFilter]
        public ActionResult test(string action)
        {
            if (action == "Test")
            {
                return Content(Session["UserName"].ToString());
                //Response.Redirect("../Db/Install");
                //conn.OpenConnection();
                //string sql = "SELECT * FROM [User ORDER BY ID DESC";
                //DataTable dt = conn.List(sql);
                //conn.CloseConnection();
                //ViewBag.Data = dt;
                //return View(ViewBag.Data);
            }
            //return View(ViewBag.Data);
            return View();
        }
    }
}