using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.Routing;

namespace HantnewCMS.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class UserAuthorizeFilterAttribute : ActionFilterAttribute
    {
        // OnActionExecuting 在执行操作方法之前由 ASP.NET MVC 框架调用。
        // OnActionExecuted 在执行操作方法后由 ASP.NET MVC 框架调用。
        // OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        // OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。

        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        Conn conn = new Conn();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionDescriptor = filterContext.ActionDescriptor;
            var controllerDescriptor = actionDescriptor.ControllerDescriptor;
            var controller = controllerDescriptor.ControllerName;
            var action = actionDescriptor.ActionName;

            string username = System.Web.HttpContext.Current.Request.Cookies.Get("UserName").Value;
            conn.OpenConnection();
            string sql = "SELECT * FROM UserRole WHERE UserID IN (SELECT ID FROM [User] WHERE UserName= '" + username + "')";
            DataTable dt = conn.Detail(sql);
            int roleid = int.Parse(dt.Rows[0]["RoleID"].ToString());

            sql = "SELECT COUNT(*) FROM [ControllerActionRole] WHERE Controller='" + controller + "' AND Action='" + action + "' AND IsAllowed='0' AND RoleID="+ roleid;
            int count = conn.Select(sql);
            if (count == 1)
            {
                filterContext.Result = new ContentResult { Content = @"<script>alert('抱歉,非超级管理员无权操作！');history.go(-1);</script>" };
            }
        }
        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        ///  OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        /// <summary>
        /// OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }
    }
}