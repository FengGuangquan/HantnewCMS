using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using HantnewCMS.Models;
using System.Reflection;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

namespace HantnewCMS.Controllers
{
    public class ShowController : Controller
    {
        Conn conn = new Conn();
        public int PageSize = 10;

        //首页
        public ActionResult Index()
        {
            conn.OpenConnection();
            string sql = "select top 3 * from [Article] order by CreateTime desc";
            DataSet ds = conn.Ds_List(sql);
            ViewBag.Data = ds.Tables[0];

            string sql1 = "select * from [Carousel]";
            DataSet ds1 = conn.Ds_List(sql1);
            ViewBag.Data1 = ds1.Tables[0];

            conn.CloseConnection();
            return View(ViewBag.Data);
        }


        //文章
        public ActionResult Article()
        {
            conn.OpenConnection();
            string sql = "SELECT " +
                "[Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[ArticleClass].ClassName,[Article].AttachmentPath,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].TitleImgPath " +
                "FROM [Article] " +
                "INNER JOIN [ArticleClass] " +
                "ON [Article].ArticleClassID = [ArticleClass].ID " +
                "INNER JOIN [User] " +
                "ON [Article].UserID = [User].ID " +
                "ORDER BY CreateTime DESC";
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;
            conn.CloseConnection();
            return View(ViewBag.Data);

            //conn.OpenConnection();
            //List<Article> articles = new List<Article>();
            //string sql1 = "SELECT " +
            //    "[Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].AttachmentPath,[ArticleClass].ClassName,[Article].TitleImgPath " +
            //    "FROM [Article] " +
            //    "INNER JOIN [ArticleClass] ON [Article].ArticleClassID = [ArticleClass].ID " +
            //    "INNER JOIN [User] ON [Article].UserID = [User].ID " +
            //    "ORDER BY [Article].ID DESC";

            //DataSet ds1 = conn.Ds_List(sql1);
            //foreach (DataRow item in ds1.Tables[0].Rows)
            //{
            //    articles.Add(new Article
            //    {
            //        ID = int.Parse(item["article_id"].ToString()),
            //        Title = item["Title"].ToString(),
            //        Content = item["Content"].ToString(),
            //        UserName = item["UserName"].ToString(),
            //        ClassName = item["ClassName"].ToString(),
            //        CreateTime = item["CreateTime"].ToString(),
            //        LastEditTime = item["LastEditTime"].ToString(),
            //        AttachmentPath = item["AttachmentPath"].ToString(),
            //        TitleImgPath = item["TitleImgPath"].ToString(),
            //        IsValid = Boolean.Parse(item["IsValid"].ToString()),
            //    });
            //}
            //conn.CloseConnection();
            //ArticleViewModel ArticleViewModel = new ArticleViewModel()
            //{
            //    Articles = articles.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
            //    PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = articles.Count, ItemsPerPage = PageSize }
            //};
            //return View(ArticleViewModel);
        }


        //文章查看
        public ActionResult Article_Detail(int id)
        {
            conn.OpenConnection();
            string sql = "SELECT " +
                "[Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[ArticleClass].ClassName,[Article].AttachmentPath,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].TitleImgPath " +
                "FROM [Article] " +
                "INNER JOIN [ArticleClass] " +
                "ON [Article].ArticleClassID = [ArticleClass].ID " +
                "INNER JOIN [User] " +
                "ON [Article].UserID = [User].ID " +
                "WHERE [Article].ID=" + id;
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;

            string sql1 = "select UserID from [Article] where ID = " + id;
            DataTable dt1 = conn.Detail(sql1);
            string sql2 = "select MAX(CreateTime) AS CreateTime from [Article] where UserID = " + dt1.Rows[0]["UserID"];
            DataTable dt2 = conn.Detail(sql2);
            string maxCreateTime = dt2.Rows[0]["CreateTime"].ToString();
            ViewData["maxCreateTime"] = maxCreateTime;

            string sql3 = "select top 3 * from [Article] where UserID = " + dt1.Rows[0]["UserID"] + "order by CreateTime desc";
            DataSet ds3 = conn.Ds_List(sql3);
            ViewBag.Data3 = ds3.Tables[0];

            conn.CloseConnection();
            return View(ViewBag.Data);
        }


        //发布文章
        [ValidateInput(false)]
        public ActionResult Article_Add(FormCollection collection, string action)
        {
            if (action == "添加")
            {
                HttpPostedFileWrapper attachment = (HttpPostedFileWrapper)Request.Files["attachment"];
                string fileName = attachment.FileName.Substring(0, attachment.FileName.LastIndexOf(@".")) + DateTime.Now.Ticks +
                    attachment.FileName.Substring(attachment.FileName.LastIndexOf(@"."));
                attachment.SaveAs(HttpRuntime.AppDomainAppPath.ToString() + "Upload\\Attachment\\" + fileName);

                string articletitleimg = (string)Request["articletitleimg"];
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(articletitleimg.Split(',')[1]));
                string imgName = Request.Cookies["UserName"].Value + DateTime.Now.Ticks + ".png";
                new Bitmap(stream).Save(HttpRuntime.AppDomainAppPath.ToString() + "Upload\\ArticleTitleImg\\" + imgName);

                if (attachment.ContentLength < 2097152)
                {
                    conn.OpenConnection();
                    int uid = int.Parse(Request.Cookies["UID"].Value.ToString());
                    string attachmentPath = "/Upload/Attachment/" + fileName;
                    string ArticleTitleImgPath = "/Upload/ArticleTitleImg/" + imgName;
                    string sql = "INSERT INTO [Article] (Title, Content, UserID, CreateTime, LastEditTime, ArticleClassID, TitleImgPath, AttachmentPath, IsValid) VALUES('" + collection["title"] + "','" + collection["content"] + "'," + uid + ",'" + DateTime.Now + "','" + DateTime.Now + "'," + int.Parse(collection["selectlist"]) + ",'" + ArticleTitleImgPath + "','" + attachmentPath + "',1)";
                    int result = conn.InsertData(sql);
                    if (result > 0)
                    {
                        conn.CloseConnection();
                        return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("Article", "Show") + "'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('添加失败！'); window.location.href = '" + Url.Action("Article", "Show") + "'</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('附件大小不能超过2M！'); window.location.href = '" + Url.Action("Article_Add", "Show") + "'</script>");
                }
            }
            else
            {
                conn.OpenConnection();
                List<ArticleClass> lists = new List<ArticleClass>();
                string sql = "SELECT * FROM [ArticleClass] ORDER BY ID DESC";
                DataSet ds = conn.Ds_List(sql);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    lists.Add(new ArticleClass
                    {
                        ID = int.Parse(item["ID"].ToString()),
                        ClassName = item["ClassName"].ToString(),
                    });
                }
                ViewData["articleList"] = lists;
            }
            return View();
        }


        //我的文章
        public ActionResult Article_My()
        {
            conn.OpenConnection();
            string sql = "SELECT " +
                "[Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[ArticleClass].ClassName,[Article].AttachmentPath,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].TitleImgPath " +
                "FROM [Article] " +
                "INNER JOIN [ArticleClass] " +
                "ON [Article].ArticleClassID = [ArticleClass].ID " +
                "INNER JOIN [User] " +
                "ON [Article].UserID = [User].ID " +
                "WHERE [Article].UserID=" + Request.Cookies["UID"].Value +
                " ORDER BY CreateTime DESC";
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;
            conn.CloseConnection();
            return View(ViewBag.Data);

            //List<Article> articles = new List<Article>();
            //string sql1 = "SELECT " +
            //    "[Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].AttachmentPath,[ArticleClass].ClassName " +
            //    "FROM [Article] " +
            //    "INNER JOIN [ArticleClass] ON [Article].ArticleClassID = [ArticleClass].ID " +
            //    "INNER JOIN [User] ON [Article].UserID = [User].ID " +
            //    "WHERE UserID = '" + Request.Cookies["UID"].Value + "'" +
            //    "ORDER BY [Article].ID DESC";

            //DataSet ds1 = conn.Ds_List(sql1);
            //foreach (DataRow item in ds1.Tables[0].Rows)
            //{
            //    articles.Add(new Article
            //    {
            //        ID = int.Parse(item["article_id"].ToString()),
            //        Title = item["Title"].ToString(),
            //        Content = item["Content"].ToString(),
            //        UserName = item["UserName"].ToString(),
            //        ClassName = item["ClassName"].ToString(),
            //        CreateTime = item["CreateTime"].ToString(),
            //        LastEditTime = item["LastEditTime"].ToString(),
            //        AttachmentPath = item["AttachmentPath"].ToString(),
            //        IsValid = Boolean.Parse(item["IsValid"].ToString()),
            //    });
            //}
            //conn.CloseConnection();
            //ArticleViewModel ArticleViewModel = new ArticleViewModel()
            //{
            //    Articles = articles.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
            //    PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = articles.Count, ItemsPerPage = PageSize }
            //};
            //return View(ArticleViewModel);
        }


        //投票
        public ActionResult Vote()
        {
            return View();
        }


        //问卷调查
        public ActionResult Investigation()
        {
            return View();
        }


        //联系我们
        public ActionResult ContactUs()
        {
            return View();
        }


        //个人中心
        public ActionResult PersonCenter(FormCollection collection, string action_name)
        {
            string password = collection["password"];
            string realname = collection["realname"];
            string phone = collection["phone"];
            string email = collection["email"];

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

            if (Request.Cookies["UserName"] == null || Request.Cookies["UID"] == null)
            {
                return Content("<script>alert('对不起，您还未登陆，请登录'); window.location.href = '" + Url.Action("Login", "Log") + "'</script>");
            }
            else
            {
                string username = Request.Cookies["UserName"].Value;
                string id = Request.Cookies["UID"].Value;

                conn.OpenConnection();
                if (action_name == "修改")
                {
                    if (!passwordReg.IsMatch(password.Trim()))
                    {
                        return Content("<script>alert('修改失败，密码必须6位或以上！'); window.location.href = '" + Url.Action("PersonCenter", "Show") + "'</script>");
                    }
                    else
                    {
                        if (!realnameReg.IsMatch(realname.Trim()))
                        {
                            return Content("<script>alert('修改失败，真实名字必须为中文！'); window.location.href = '" + Url.Action("PersonCenter", "Show") + "'</script>");
                        }
                        else
                        {
                            if (!phoneReg.IsMatch(phone.Trim()))
                            {
                                return Content("<script>alert('修改失败，手机号码有误！'); window.location.href = '" + Url.Action("PersonCenter", "Show") + "'</script>");
                            }
                            else
                            {
                                if (!emailReg.IsMatch(email.Trim()))
                                {
                                    return Content("<script>alert('修改失败，邮箱有误！'); window.location.href = '" + Url.Action("PersonCenter", "Show") + "'</script>");
                                }
                                else
                                {
                                    string sql = "UPDATE [User] SET Password='" + collection["password"] + "',Realname='" + collection["realname"] + "',Email='" + collection["email"] + "',Phone='" + collection["phone"] + "',Birthday='" + collection["birthday"] + "',LastEditTime='" + DateTime.Now + "' WHERE ID='" + id + "'";
                                    if (conn.UpdateData(sql) != 0)
                                    {
                                        conn.CloseConnection();
                                        return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("PersonCenter", "Show") + "'</script>");
                                    }
                                    else
                                    {
                                        conn.CloseConnection();
                                        return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("UserCenter", "Show") + "'</script>");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    string sql1 = "SELECT [User].* FROM [User] WHERE UserName = '" + username + "'";
                    DataTable dt = conn.Detail(sql1);
                    ViewBag.Data = dt;
                    conn.CloseConnection();
                    return View(ViewBag.Data);
                }
            }
        }


    }
}