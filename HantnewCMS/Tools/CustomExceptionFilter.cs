using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HantnewCMS.Tools
{
    public class CustomExceptionFilter : HandleErrorAttribute
    {
        public static string errorTitel = "";
        public override void OnException(ExceptionContext filterContext)
        {
            //如果异常未处理
            if (!filterContext.ExceptionHandled)
            {
                Exception innerException = filterContext.Exception;
                //var UserName = filterContext.HttpContext.User.Identity.Name;
                //记录异常,在数据库中创建一个异常信息表，将异常信息写入数据库。

                //ErrorMessage errorMessage = new ErrorMessage();
                //errorMessage.Date = DateTime.Now;
                //errorMessage.Source = innerException.Source;
                //errorMessage.Title = innerException.Message;
                //erroTitel = innerException.Message;
                //errorMessage.MessageContent = innerException.StackTrace;

                //MyDbContext db = new MyDbContext();
                //db.Entry(errorMessage).State = System.Data.Entity.EntityState.Added;
                //db.SaveChanges();
                //filterContext.ExceptionHandled = true;//告诉它已处理异常

                //跳转到异常显示页
                filterContext.Result = new RedirectResult("~/Shared/Error");
            }
        }
    }
}