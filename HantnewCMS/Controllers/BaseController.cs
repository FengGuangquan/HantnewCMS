using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using HantnewCMS.Models;

namespace HantnewCMS.Controllers
{
    public class BaseController : Controller
    {
        Conn conn = new Conn();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string username = System.Web.HttpContext.Current.Request.Cookies.Get("UserName").Value;
            conn.OpenConnection();
            string sql = "SELECT * FROM UserRole WHERE UserID IN (SELECT ID FROM [User] WHERE UserName= '" + username + "')";

            DataTable dt = conn.Detail(sql);
            string get_roleid = dt.Rows[0][1].ToString();
            conn.CloseConnection();


            //fcinfo = new filterContextInfo(filterContext);
            //fcinfo.actionName;//获取域名
            //fcinfo.controllerName;获取 controllerName 名称

            //bool isstate = true;
            //islogin = false;
            if (get_roleid != "")//如果满足
            {

                //filterContext.Result = new ContentResult { Content = @"您的角色ID是:" + get_roleid };
                //filterContext.Result = new HttpUnauthorizedResult("../Admin/Index");//直接URL输入的页面地址跳转到登陆页  
                //filterContext.Result = new RedirectResult("../Admin/Index");//也可以跳到别的站点
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "../Admin", action = "Index" }));
                filterContext.HttpContext.Response.Redirect("../Admin/Index");//重定向
            }
            else
            {
                filterContext.Result = new ContentResult { Content = @"<script>alert('空！');</script>" };
                //filterContext.Result = new ContentResult { Content = @"抱歉,你不具有当前操作的权限！" };
            }
        }

        // GET: Base
        protected override void OnActionExecuted(ActionExecutedContext filterContext)//protected 只能被子类访问  
        {
            base.OnActionExecuted(filterContext);
            if (Session["UserName"] == null)
            {
                filterContext.Result = Redirect("../Log/Login");//  没有返回值， 所以不是return   是filterContexr.Result    
            }
        }
    }
}