using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using HantnewCMS.Models;

namespace HantnewCMS.Controllers
{
    public class LogController : Controller
    {
        Conn conn = new Conn();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        public ActionResult Register(FormCollection collection, string action)
        {
            string username = collection["username"];
            string password = collection["password"];
            string realname = collection["realname"];
            string phone = collection["phone"];
            string email = collection["email"];

            //验证用户名格式
            string usernameStr = @"^[a-zA-Z][a-zA-Z0-9_]{4,15}$";
            Regex usernameReg = new Regex(usernameStr);
            //验证密码格式
            string passwordStr = @"^[\w\W]{6,}$";
            Regex passwordReg = new Regex(passwordStr);
            //验证真实名字格式
            string realnameStr = @"^[\u4e00-\u9fa5]{0,}$";
            Regex realnameReg = new Regex(realnameStr);
            //验证手机号码格式
            string phoneStr = @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";
            Regex phoneReg = new Regex(phoneStr);
            //验证邮箱地址格式
            string emailStr = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$";
            Regex emailReg = new Regex(emailStr);

            if (action == "注册") {
                conn.OpenConnection();
                string sql_reg = "INSERT INTO [User](UserName,RealName,Password,CreateTime,LastEditTime,Phone,Email,Birthday,IsValid) VALUES('" + collection["username"] + "','" + collection["realname"] + "','" + collection["password"] + "','" + DateTime.Now + "','" + DateTime.Now + "','" + collection["phone"] + "','" + collection["email"] + "','" + collection["birthday"] + "','1')";
                string sql = sql_reg;
                
                string sql_check = "select Count(*) from [User] where UserName = '" + username + "'";
                int count = conn.Select(sql_check);

                if (count > 0) {
                    ViewBag.Result = "该用户名已注册";
                }
                else
                {
                    if (!usernameReg.IsMatch(username.Trim()))
                    {
                        ViewBag.Result = "注册失败，用户名有误 （Tips:字母开头，允许5-16字节，允许字母数字下划线）";
                    }
                    else
                    {
                        if (!passwordReg.IsMatch(password.Trim()))
                        {
                            ViewBag.Result = "注册失败，密码必须6位或以上";
                        }
                        else
                        {
                            if (!realnameReg.IsMatch(realname.Trim()))
                            {
                                ViewBag.Result = "注册失败，真实名字有误";
                            }
                            else
                            {
                                if (!phoneReg.IsMatch(phone.Trim()))
                                {
                                    ViewBag.Result = "注册失败，手机号码有误";
                                }
                                else
                                {
                                    if (!emailReg.IsMatch(email.Trim()))
                                    {
                                        ViewBag.Result = "注册失败，邮箱有误";
                                    }
                                    else
                                    {
                                        int result = conn.InsertData(sql);
                                        //分配默认角色
                                        string sql_uid = "SELECT * FROM [User] WHERE UserName='" + username + "'";
                                        DataSet ds = conn.Ds_List(sql_uid);
                                        foreach (DataRow item in ds.Tables[0].Rows)
                                        {
                                            //查询新增用户ID
                                            int ID = int.Parse(item["ID"].ToString());

                                            //给新增用户分配默认角色
                                            string user_role = "INSERT INTO [UserRole] (UserID,RoleID) VALUES(" + ID + ",4)";
                                            int result1 = conn.InsertData(user_role);
                                            if (result > 0)
                                            {
                                                conn.CloseConnection();
                                                return Content("<script>alert('注册成功！');window.location.href='Login'</script>");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return View();
        }

        
        public ActionResult Login(FormCollection collection, string action)
        {
            if (action == "立即登陆")
            {
                conn.OpenConnection();
                string username = collection["username"];
                string password = collection["password"];
                string sql = "SELECT COUNT(*) FROM [User] WHERE UserName='" + username + "'AND Password='" + password + "'AND IsValid='True'";
                int count = conn.Select(sql);
                if (count == 0)
                {
                    ViewBag.Result = "用户不存在或密码错误!";
                    conn.CloseConnection();
                    return View();
                }
                else
                {
                    Response.Cookies["UserName"].Value = username;
                    //根据username获得uid
                    string sql_uid = "SELECT * FROM [User] WHERE UserName='" + username + "'";
                    DataSet ds = conn.Ds_List(sql_uid);
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        Response.Cookies["UID"].Value = item["ID"].ToString();
                    }
                    //根据UID获得[Role].Name
                    string sql_name = 
                        "SELECT r.Name "+
                        "FROM [Role] r " +
                        "INNER JOIN [UserRole] ur" +
                        "   ON ur.RoleID = r.ID " +
                        "INNER JOIN [User] u " +
                        "   ON u.ID = ur.UserID " +
                        "WHERE ur.UserID='" + Response.Cookies["UID"].Value + "'";
                    DataSet ds1 = conn.Ds_List(sql_name);
                    foreach (DataRow item in ds1.Tables[0].Rows)
                    {
                        Response.Cookies["Name"].Value = item["Name"].ToString();
                    }

                    if (Request.Cookies["UserName"] != null)
                    {
                        //Response.Redirect("../Admin/Index");
                        string ip = HttpContext.Request.UserHostAddress;
                        string add_loginrecord = "Insert into [LoginRecord] values('" + DateTime.Now + "','" + int.Parse(Response.Cookies["UID"].Value) + "','" + ip + "')";
                        conn.InsertData(add_loginrecord);

                        if(Response.Cookies["Name"].Value == "超级管理员" || Response.Cookies["Name"].Value == "管理员")
                        {
                            return RedirectToAction("Index", "Admin");
                            //return Content();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Show");
                        }

                        
                    }
                }

            }
            else
            {
                //ViewBag.Result = "未点击注册!";
                return View();
            }
            return View();
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["UserName"] != null)
            {
                Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("Login");
                return View();
            }
            return View();
        }
    }
}