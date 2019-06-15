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
    [Description(No = 1, Name = "用户")]
    public class AdminController : Controller
    {
        Conn conn = new Conn();

        public int PageSize = 10;
        //private int page;


        //首页
        [AuthorizeFilter]
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Index()
        {
            conn.OpenConnection();
            string sql_UserCount = "select COUNT(*) from [User]";
            int usercount = conn.Select(sql_UserCount);
            string sql_TaskCount = "select COUNT(*) from [Task]";
            int taskcount = conn.Select(sql_TaskCount);
            string sql_ArticleCount = "select COUNT(*) from [Article]";
            int articlecount = conn.Select(sql_ArticleCount);
            string sql_FinishedTaskCount = "select COUNT(*) from [Task] where IsValid = 'True'";
            int finishedtaskcount = conn.Select(sql_FinishedTaskCount);
            ViewBag.UserCount = usercount;
            ViewBag.TaskCount = taskcount;
            ViewBag.ArticleCount = articlecount;
            ViewBag.FinishedTaskCount = finishedtaskcount;

            string sql = "select top 5 * from [Article] order by CreateTime desc";
            DataSet ds = conn.Ds_List(sql);
            ViewBag.Data = ds.Tables[0];

            conn.CloseConnection();
            return View(ViewBag.Data);


            
        }


        //用户列表
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        //int page = 1;
        public ActionResult UserTable(int page = 1)
        {
            conn.OpenConnection();
            List<User> users = new List<User>();
            string sql = "SELECT [User].ID AS uid, [User].UserName AS uname,[User].RealName,[Role].Name AS rname,[User].CreateTime,[User].LastEditTime,[User].Phone,[User].Email,[User].Birthday,[User].IsValid,Role.* FROM [User] INNER JOIN [UserRole] ON [User].ID = [UserRole].UserID INNER JOIN [Role] ON [UserRole].RoleID = [Role].ID";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                users.Add(new User
                {
                    ID = int.Parse(item["uid"].ToString()),
                    UserName = item["uname"].ToString(),
                    RealName = item["RealName"].ToString(),
                    Role = item["rname"].ToString(),
                    CreateTime = item["CreateTime"].ToString(),
                    LastEditTime = item["LastEditTime"].ToString(),
                    Phone = item["Phone"].ToString(),
                    Email = item["Email"].ToString(),
                    Birthday = item["Birthday"].ToString(),
                    IsValid = item["IsValid"].ToString(),
                });
            }
            conn.CloseConnection();

            UserViewModel UserViewModel = new UserViewModel()
            {
                Users = users.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = users.Count, ItemsPerPage = PageSize }
            };
            return View(UserViewModel);
        }


        //角色列表
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Role(string action_name, FormCollection collection)
        {
            conn.OpenConnection();
            string sql_select = "INSERT INTO [Role] VALUES('" + collection["name"] + "','" + collection["desription"] + "')";

            string sql = "SELECT * FROM [Role] ORDER BY ID DESC";
            if (action_name == "添加")
            {
                if (conn.InsertData(sql_select) != 0)
                {
                    //如果这样返回则提示错误：无法对 null 引用执行运行时绑定
                    ViewBag.Result = "添加成功！";

                    DataTable dt = conn.List(sql);
                    conn.CloseConnection();
                    ViewBag.Data = dt;
                    return View(ViewBag.Data);
                }
                conn.CloseConnection();
                return View();
            }
            else
            {
                conn.OpenConnection();

                DataTable dt = conn.List(sql);
                conn.CloseConnection();
                ViewBag.Data = dt;
                return View(ViewBag.Data);
            }
        }


        //角色删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Role_Remove(int id)
        {
            conn.OpenConnection();
            //需要判断删除角色是否存在用户，如果存在用户，则不允许删除角色。
            string check_sql = "SELECT count(*) FROM [UserRole] WHERE RoleID=" + id;
            int count = conn.Select(check_sql);
            if (count != 0)
            {
                return Content("<script>alert('你不能删除这个角色因为它关联着用户！');</script>");
                //注意can not 不能写成can't
            }
            else
            {
                string sql = "DELETE FROM [Role] WHERE ID =" + id;
                int result = conn.Delete(sql);
                conn.CloseConnection();
                if (result != 0)
                {
                    return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Role", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Role", "Admin") + "'</script>");
                }
            }
        }


        //用户查看
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult UserTable_Detail(int id)
        {
            conn.OpenConnection();
            string sql = "SELECT * FROM [User] WHERE ID= " + id + "";
            DataTable dt = conn.Detail(sql);
            conn.CloseConnection();
            ViewBag.Data = dt;
            return View(ViewBag.Data);
        }


        //用户修改
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult UserTable_Edit(int id, string action_name, FormCollection collection)
        {
            conn.OpenConnection();
            string sql_update1 = "UPDATE [User] SET UserName = '" + collection["username"] + "',RealName='" + collection["realname"] + "',Phone='" + collection["phone"] + "',Email='" + collection["email"] + "',Birthday='" + collection["birthday"] + "',IsValid='" + collection["IsValid"] + "' WHERE ID= " + id + "";
            //注意[User]表没有role字段
            string sql_update2 = "UPDATE [UserRole] SET RoleID='" + collection["rolelist"] + "' WHERE UserID=" + id;

            if (action_name == "修改")
            {
                string realname = collection["realname"];
                string phone = collection["phone"];
                string email = collection["email"];

                //验证真实名字格式
                string realnameStr = @"^[\u4e00-\u9fa5]{0,}$";
                Regex realnameReg = new Regex(realnameStr);
                //验证手机号码格式
                string phoneStr = @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";
                Regex phoneReg = new Regex(phoneStr);
                //验证邮箱地址格式
                string emailStr = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$";
                Regex emailReg = new Regex(emailStr);

                if (!realnameReg.IsMatch(realname.Trim()))
                {
                    return Content("<script>alert('修改失败，真实名字有误！'); window.location.href = '" + Url.Action("UserTable", "Admin") + "'</script>");
                }
                else
                {
                    if (!phoneReg.IsMatch(phone.Trim()))
                    {
                        return Content("<script>alert('修改失败，手机号码有误！'); window.location.href = '" + Url.Action("UserTable", "Admin") + "'</script>");
                    }
                    else
                    {
                        if (!emailReg.IsMatch(email.Trim()))
                        {
                            return Content("<script>alert('修改失败，邮箱有误！'); window.location.href = '" + Url.Action("UserTable", "Admin") + "'</script>");
                        }
                        else
                        {
                            if (conn.UpdateData(sql_update1 + sql_update2) != 0)
                            {
                                conn.CloseConnection();
                                return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("UserTable", "Admin") + "'</script>");
                            }
                            conn.CloseConnection();
                            return View();
                        }
                    }
                }
            }
            else
            {

                conn.OpenConnection();
                List<Role> lists = new List<Role>();
                string se_sql = "SELECT ID, Name FROM [Role]";

                DataSet ds = conn.Ds_List(se_sql);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    lists.Add(new Role
                    {
                        ID = int.Parse(item["ID"].ToString()),
                        Name = item["Name"].ToString()
                    });
                }
                ViewData["RoleList"] = lists;

                string sql = "SELECT u.*, ur.RoleID FROM [User] u LEFT JOIN UserRole ur ON ur.UserID = u.ID WHERE u.ID=" + id + "";
                DataTable dt = conn.List(sql);
                conn.CloseConnection();
                ViewBag.Data = dt;
                return View(ViewBag.Data);
            }
        }


        //用户删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult UserTable_Remove(int id)
        {
            conn.OpenConnection();
            string sql = "DELETE FROM [User] WHERE [ID] =" + id + "DELETE FROM [UserRole] WHERE UserID=" + id;
            int result = conn.Delete(sql);
            conn.CloseConnection();
            if (result != 0)
            {
                return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("UserTable", "Admin") + "'</script>");
            }
            else
            {
                return View();
            }
        }


        //文章
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Article(int page = 1)
        {
            conn.OpenConnection();
            List<Article> articles = new List<Article>();
            string sql = "SELECT [Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid, [Article].AttachmentPath,[ArticleClass].ClassName FROM [Article] INNER JOIN [ArticleClass] ON [Article].ArticleClassID = [ArticleClass].ID INNER JOIN [User] ON [Article].UserID = [User].ID ORDER BY [Article].ID DESC";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                articles.Add(new Article
                {
                    ID = int.Parse(item["article_id"].ToString()),
                    Title = item["Title"].ToString(),
                    Content = item["Content"].ToString(),
                    UserName = item["UserName"].ToString(),
                    ClassName = item["ClassName"].ToString(),
                    CreateTime = item["CreateTime"].ToString(),
                    LastEditTime = item["LastEditTime"].ToString(),
                    AttachmentPath = item["AttachmentPath"].ToString(),
                    IsValid = Boolean.Parse(item["IsValid"].ToString()),
                });
            }

            conn.CloseConnection();
            ArticleViewModel ArticleViewModel = new ArticleViewModel()
            {
                Articles = articles.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = articles.Count, ItemsPerPage = PageSize }
            };
            return View(ArticleViewModel);
        }


        //文章添加
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult Article_Add(FormCollection collection, string action)
        {
            conn.OpenConnection();
            if (action == "添加")
            {
                HttpPostedFileWrapper attachment = (HttpPostedFileWrapper)Request.Files["attachment"];
                string fileName = attachment.FileName.Substring(0, attachment.FileName.LastIndexOf(@".")) + DateTime.Now.Ticks +
                    attachment.FileName.Substring(attachment.FileName.LastIndexOf(@"."));
                attachment.SaveAs(HttpRuntime.AppDomainAppPath.ToString()+ "Upload\\Attachment\\" + fileName);

                string articletitleimg = (string)Request["articletitleimg"];
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(articletitleimg.Split(',')[1]));
                string imgName = Request.Cookies["UserName"].Value + DateTime.Now.Ticks + ".png";
                new Bitmap(stream).Save(HttpRuntime.AppDomainAppPath.ToString() + "Upload\\ArticleTitleImg\\" + imgName);

                if (attachment.ContentLength < 2097152)
                {
                    int uid = int.Parse(Request.Cookies["UID"].Value.ToString());
                    string attachmentPath = "/Upload/Attachment/" + fileName;
                    string ArticleTitleImgPath = "/Upload/ArticleTitleImg/" + imgName;
                    string sql = "INSERT INTO [Article] (Title, Content, UserID, CreateTime, LastEditTime, ArticleClassID, TitleImgPath, AttachmentPath, IsValid) VALUES('" + collection["title"] + "','" + collection["content"] + "'," + uid + ",'" + DateTime.Now + "','" + DateTime.Now + "'," + int.Parse(collection["selectlist"]) + ",'" + ArticleTitleImgPath + "','" + attachmentPath + "',1)";
                    int result = conn.InsertData(sql);
                    if (result > 0)
                    {
                        conn.CloseConnection();
                        return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('添加失败！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('附件大小不能超过2M！'); window.location.href = '" + Url.Action("Article_Add", "Admin") + "'</script>");
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


        //文章查看
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Article_Detail(int id)
        {
            conn.OpenConnection();
            string sql = "SELECT [Article].ID AS article_id,[Article].Title,[Article].Content,[User].UserName,[ArticleClass].ClassName, [Article].AttachmentPath,[Article].CreateTime,[Article].LastEditTime,[Article].IsValid,[Article].TitleImgPath FROM [Article] INNER JOIN [ArticleClass] ON [Article].ArticleClassID = [ArticleClass].ID INNER JOIN [User] ON [Article].UserID = [User].ID WHERE [Article].ID=" + id;
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;
            return View(ViewBag.Data);
        }


        //文章编辑
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult Article_Edit(int id, string action_name, FormCollection collection)
        {
            if (action_name == "修改")
            {
                conn.OpenConnection();
                string sql_id = "SELECT [ArticleClass].ID FROM ArticleClass INNER JOIN [Article] ON [ArticleClass].ID=[Article].ArticleClassID";
                DataTable dt = conn.Detail(sql_id);
                int classid = int.Parse(dt.Rows[0][0].ToString());
                string sql = "UPDATE [Article] SET Title='" + collection["title"] + "',Content='" + collection["content"] + "',LastEditTime='"+DateTime.Now+"',ArticleClassID=" + classid + ",IsValid='" + collection["IsValid"] + "' WHERE ID=" + id;
                if (conn.UpdateData(sql) != 0)
                {
                    return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
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

                sql = "SELECT [Article].*,[User].UserName,[ArticleClass].ClassName FROM [Article] INNER JOIN [User] ON [Article].UserID=[User].ID INNER JOIN [ArticleClass] ON [Article].ArticleClassID=[ArticleClass].ID WHERE [Article].ID= " + id;
                DataTable dt = conn.Detail(sql);
                conn.CloseConnection();
                ViewBag.Data = dt;
                conn.CloseConnection();
                return View(ViewBag.Data);
            }
        }


        //文章删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Article_Remove(int id)
        {
            conn.OpenConnection();
            string sql = "DELETE FROM [Article] WHERE [ID] =" + id;
            int result = conn.Delete(sql);
            conn.CloseConnection();
            if (result != 0)
            {
                return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
            }
            else
            {
                return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Article", "Admin") + "'</script>");
            }

        }


        //分类管理
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Class(int page = 1, string action = "", FormCollection collection = null)
        {
            conn.OpenConnection();
            if (action == "添加")
            {
                string sql1 = "INSERT INTO [ArticleClass] VALUES('" + collection["name"] + "','" + DateTime.Now + "','" + DateTime.Now + "','1')";
                if (conn.InsertData(sql1) != 0)
                {
                    ViewBag.Result = "添加成功";
                }
            }
            List<Class> classes = new List<Class>();
            string sql = "SELECT * FROM [ArticleClass] ORDER BY ID DESC";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                classes.Add(new Class
                {
                    ID = int.Parse(item["ID"].ToString()),
                    ClassName = item["ClassName"].ToString(),
                    CreateTime = item["CreateTime"].ToString(),
                    LastEditTime = item["LastEditTime"].ToString(),
                    IsValid = item["IsValid"].ToString(),
                });
            }
            conn.CloseConnection();
            ClassViewModel ClassViewModel = new ClassViewModel()
            {
                Classes = classes.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = classes.Count, ItemsPerPage = PageSize }
            };
            return View(ClassViewModel);
        }


        //分类删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Class_Remove(int id)
        {
            conn.OpenConnection();
            //需要判断删除分类是否信息，如果存在信息，则不允许删除分类。
            string check_sql = "SELECT count(*) FROM [Article] WHERE ArticleClassID=" + id;
            int count = conn.Select(check_sql);
            if (count == 0)
            {
                string sql = "DELETE FROM [ArticleClass] WHERE [ID] =" + id;
                int result = conn.Delete(sql);
                conn.CloseConnection();
                if (result != 0)
                {
                    return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Class", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Class", "Admin") + "'</script>");
                }
            }
            else
            {
                return Content("<script>alert('你不能删除这个类因为它关联着文章！'); window.location.href = '" + Url.Action("Class", "Admin") + "'</script>");
            }
        }


        //分类修改
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Class_Edit(FormCollection collection, string action, int id)
        {
            conn.OpenConnection();
            if (action == "修改")
            {
                string sql = "UPDATE [ArticleClass] SET ClassName='" + collection["ClassName"] + "',LastEditTime='"+DateTime.Now+"',IsValid='" + collection["IsValid"] + "' WHERE ID=" + id;
                if (conn.UpdateData(sql) != 0)
                {
                    conn.CloseConnection();
                    return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("Class", "Admin") + "'</script>");
                }
                else
                {
                    conn.CloseConnection();
                    return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("Class", "Admin") + "'</script>");
                }
            }
            else
            {
                string sql = "SELECT * FROM [ArticleClass] WHERE ID= " + id;
                DataTable dt = conn.Detail(sql);
                conn.CloseConnection();
                ViewBag.Data = dt;
                conn.CloseConnection();
                return View(ViewBag.Data);
            }
        }


        //项目
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Task(int page = 1, string action = "", FormCollection collection = null)
        {
            if (action == "添加项目")
            {

            }
            else
            {
                conn.OpenConnection();
                List<Task> tasks = new List<Task>();
                //string sql = "SELECT u.*, ur.*,r.* FROM [User] AS u RIGHT OUTER JOIN [UserRole] AS ur ON u.ID = ur.UserID RIGHT OUTER JOIN [Role] AS r ON ur.RoleID = r.ID";
                string sql = "SELECT [Task].*,[User].UserName FROM [Task] INNER JOIN [User] ON [Task].PublisherID = [User].ID ORDER BY ID DESC";
                DataSet ds = conn.Ds_List(sql);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    tasks.Add(new Task
                    {
                        ID = int.Parse(item["ID"].ToString()),
                        Title = item["Title"].ToString(),
                        Content = item["Content"].ToString(),
                        PublisherUserName = item["UserName"].ToString(),
                        Developers = item["Developers"].ToString(),
                        BudgetTime = item["BudgetTime"].ToString(),
                        TurnoverTime = item["TurnoverTime"].ToString(),
                        CreateTime = item["CreateTime"].ToString(),
                        LastEditTime = item["LastEditTime"].ToString(),
                        IsValid = bool.Parse(item["IsValid"].ToString()),
                    });
                }
                conn.CloseConnection();
                TaskViewModel TaskViewModel = new TaskViewModel()
                {
                    Tasks = tasks.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                    PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = tasks.Count, ItemsPerPage = PageSize }
                };
                return View(TaskViewModel);
            }
            return View();
        }


        //项目添加
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult Task_Add(FormCollection collection, string action)
        {
            if (action == "添加")
            {
                int uid = int.Parse(Request.Cookies.Get("UID").Value);
                if (Request.Cookies.Get("UID") != null)
                {
                    conn.OpenConnection();
                    string sql = "INSERT INTO [Task] VALUES('" + collection["title"] + "','" + collection["content"] + "'," + uid + ",'" + collection["developers"] + "','" + collection["budgettime"] + "','" + collection["turnovertime"] + "','" + DateTime.Now + "','" + DateTime.Now + "',1)";
                    if (conn.InsertData(sql) != 0)
                    {
                        conn.CloseConnection();
                        return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('UID为空！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
            }
            else
            {

            }
            return View();
        }


        //项目查看
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Task_Detail(int id)
        {
            conn.OpenConnection();
            string sql = "SELECT [Task].*,[User].UserName FROM [Task] INNER JOIN [User] ON [Task].PublisherID=[User].ID WHERE [Task].ID=" + id;
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;
            return View(ViewBag.Data);
        }


        //项目编辑
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult Task_Edit(int id, string action_name, FormCollection collection)
        {
            conn.OpenConnection();
            if (action_name == "修改")
            {
                string sql = "UPDATE [Task] SET Title='" + collection["title"] + "',Content='" + collection["content"] + "',Developers='" + collection["developers"] + "',BudgetTime='" + collection["budgettime"] + "',TurnoverTime='" + collection["turnovertime"] + "',LastEditTime='"+DateTime.Now+"',IsValid='" + collection["IsValid"] + "' WHERE ID=" + id;
                if (conn.UpdateData(sql) != 0)
                {
                    return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
            }
            else
            {
                string sql = "SELECT [Task].*,[User].UserName FROM [Task] INNER JOIN [User] ON [Task].PublisherID=[User].ID WHERE [Task].ID= " + id;
                DataTable dt = conn.Detail(sql);
                ViewBag.Data = dt;
                conn.CloseConnection();
                return View(ViewBag.Data);
            }
        }


        //项目删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Task_Remove(int id)
        {
            conn.OpenConnection();
            string sql = "DELETE FROM [Task] WHERE [ID]=" + id;
            string sql1 = "DELETE FROM [TaskSchedule] WHERE [TaskSchedule].TaskID=" + id;
            if (conn.Delete(sql + sql1) != 0)
            {
                conn.CloseConnection();
                return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
            }
            return View();
        }


        //项目进度
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult TaskSchedule(int page = 1, string action = "", FormCollection collection = null, int id = 0)
        {
            conn.OpenConnection();
            string sql_1 = "SELECT * FROM [Task] WHERE ID =" + id;
            DataTable dt = conn.Detail(sql_1);
            ViewBag.Data = dt;

            //string sql_id = "SELECT [User].UserName FROM [User] INNER JOIN [TaskSchedule] ON [User].ID = [TaskSchedule].PublisherID";
            //DataTable dts = conn.Detail(sql_id);
            //string PublisherUserName = dts.Rows[0][0].ToString();

            List<TaskSchedule> taskschedules = new List<TaskSchedule>();
            string sql = "SELECT [TaskSchedule].*,[User].UserName FROM [TaskSchedule] INNER JOIN [Task] ON [Task].ID = [TaskSchedule].TaskID INNER JOIN [User] ON [TaskSchedule].PublisherID = [User].ID WHERE [TaskSchedule].TaskID = " + id + " ORDER BY [TaskSchedule].ID DESC ";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                taskschedules.Add(new TaskSchedule
                {
                    ID = int.Parse(item["ID"].ToString()),
                    Content = item["Content"].ToString(),
                    TaskID = int.Parse(item["TaskID"].ToString()),
                    PublisherID = int.Parse(item["PublisherID"].ToString()),
                    PublisherUserName = item["UserName"].ToString(),
                    CreateTime = item["CreateTime"].ToString(),
                    LastEditTime = item["LastEditTime"].ToString(),
                    IsValid = bool.Parse(item["IsValid"].ToString()),
                });
            }
            conn.CloseConnection();
            TaskScheduleViewModel TaskScheduleViewModel = new TaskScheduleViewModel()
            {
                TaskSchedules = taskschedules.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = taskschedules.Count, ItemsPerPage = PageSize }
            };
            return View(TaskScheduleViewModel);

        }


        //项目进度添加
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult TaskSchedule_Add(int id, string action_name, FormCollection collection)
        {
            if (action_name == "添加")
            {
                int uid = int.Parse(Request.Cookies.Get("UID").Value);
                if (Request.Cookies.Get("UID") != null)
                {
                    conn.OpenConnection();
                    string sql = "INSERT INTO [TaskSchedule] VALUES('" + collection["content"] + "'," + id + "," + uid + ",'" + DateTime.Now + "','" + DateTime.Now + "',1)";
                    if (conn.InsertData(sql) != 0)
                    {
                        conn.CloseConnection();
                        return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('添加失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('UID为空！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
            }
            else
            {
                conn.OpenConnection();
                string sql = "SELECT * FROM [Task] WHERE ID =" + id;
                DataTable dt = conn.Detail(sql);
                ViewBag.Data = dt;
                conn.CloseConnection();
                return View(ViewBag.Data);
            }
        }


        //项目进度查看
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult TaskSchedule_Detail(int id)
        {
            conn.OpenConnection();
            string sql = "SELECT [TaskSchedule].*,[User].UserName FROM [TaskSchedule] INNER JOIN [User] ON [TaskSchedule].PublisherID = [User].ID WHERE [TaskSchedule].ID=" + id;
            DataTable dt = conn.Detail(sql);
            ViewBag.Data = dt;
            conn.CloseConnection();
            return View(ViewBag.Data);
        }


        //项目进度编辑
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult TaskSchedule_Edit(int id, string action_name, FormCollection collection)
        {
            conn.OpenConnection();
            if (action_name == "修改")
            {
                string sql = "UPDATE [TaskSchedule] SET Content='" + collection["content"] + "',LastEditTime='" + DateTime.Now + "',IsValid='" + collection["IsValid"] + "' WHERE ID=" + id;
                if (conn.UpdateData(sql) != 0)
                {
                    return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                }
            }
            else
            {
                string sql = "SELECT [TaskSchedule].*,[Task].Title FROM [TaskSchedule] INNER JOIN [Task] ON [Task].ID = [TaskSchedule].TaskID WHERE [TaskSchedule].ID= " + id;
                DataTable dt = conn.Detail(sql);
                ViewBag.Data = dt;
                ViewBag.ID = id;
                conn.CloseConnection();
                return View(ViewBag.Data);
            }

        }


        //项目进度删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult TaskSchedule_Remove(int id)
        {
            conn.OpenConnection();
            string sql = "DELETE FROM [TaskSchedule] WHERE [ID]=" + id;
            if (conn.Delete(sql) != 0)
            {
                conn.CloseConnection();
                return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
            }
            else
            {
                return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
            }
        }


        //角色授权
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult PermissionSet(int page = 1, FormCollection collection = null, int id = 0)
        {
            conn.OpenConnection();
            List<ControllerActionRole> controlleractionroles = new List<ControllerActionRole>();
            string sql = "SELECT [ControllerActionRole].*,[Role].Name FROM [ControllerActionRole] INNER JOIN [Role] ON [Role].ID = [ControllerActionRole].RoleID WHERE [Role].ID=" + id + "";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                controlleractionroles.Add(new ControllerActionRole
                {
                    ID = int.Parse(item["ID"].ToString()),
                    RoleName = item["Name"].ToString(),
                    RoleID = int.Parse(item["RoleID"].ToString()),
                    Controller = item["Controller"].ToString(),
                    Action = item["Action"].ToString(),
                    IsAllowed = item["IsAllowed"].ToString()
                });
            }
            conn.CloseConnection();
            ControllerActionRoleViewModel ControllerActionRoleViewModel = new ControllerActionRoleViewModel()
            {
                ControllerActionRoles = controlleractionroles.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = controlleractionroles.Count, ItemsPerPage = PageSize }
            };
            return View(ControllerActionRoleViewModel);
        }

        public string SetPermission(int id, bool isAllowed)
        {
            conn.OpenConnection();
            string sql = "update [ControllerActionRole] set IsAllowed = '" + (isAllowed ? "True" : "False") + "' where ID = " + id;
            int effectRows = conn.UpdateData(sql);
            conn.CloseConnection();
            return effectRows > 0 ? "1" : "0";
        }

        //角色授权
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult MModule(int page = 1, FormCollection collection = null)
        {
            conn.OpenConnection();
            List<MModule> mmodules = new List<MModule>();
            string sql = "SELECT * FROM [Module]" + "";
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                mmodules.Add(new MModule
                {
                    ID = int.Parse(item["ID"].ToString()),
                    ModuleName = item["ModuleName"].ToString(),
                    IsValid = item["IsValid"].ToString()
                });
            }
            conn.CloseConnection();
            MModuleViewModel ModuleViewModel = new MModuleViewModel()
            {
                MModules = mmodules.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = mmodules.Count, ItemsPerPage = PageSize }
            };
            return View(ModuleViewModel);
        }

        public string SetMModule(int id, bool isvalid)
        {
            conn.OpenConnection();
            string sql = "update [Module] set IsValid = '" + (isvalid ? "True" : "False") + "' where ID = " + id;
            int effectRows = conn.UpdateData(sql);
            conn.CloseConnection();
            return effectRows > 0 ? "1" : "0";
        }

        //模块编辑
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        [ValidateInput(false)]
        public ActionResult MModule_Edit(int id, string action_name, FormCollection collection)
        {
            conn.OpenConnection();
            if (action_name == "修改")
            {
                //string sql = "UPDATE [TaskSchedule] SET Content='" + collection["content"] + "',LastEditTime='" + DateTime.Now + "',IsValid='" + collection["IsValid"] + "' WHERE ID=" + id;
                //if (conn.UpdateData(sql) != 0)
                //{
                //    return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                //}
                //else
                //{
                //    return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("Task", "Admin") + "'</script>");
                //}
                return View();
            }
            else
            {
                //string sql = "SELECT [TaskSchedule].*,[Task].Title FROM [TaskSchedule] INNER JOIN [Task] ON [Task].ID = [TaskSchedule].TaskID WHERE [TaskSchedule].ID= " + id;
                //DataTable dt = conn.Detail(sql);
                //ViewBag.Data = dt;
                //ViewBag.ID = id;
                //conn.CloseConnection();
                //return View(ViewBag.Data);
            }
            return View();

        }


        //角色授权添加
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Permission_Add(string action_name, FormCollection collection)
        {

            conn.OpenConnection();
            if (action_name == "添加")
            {
                string sql = "INSERT INTO [ControllerActionRole] VALUES(" + 1 + ",'" + collection["rolelist"] + "','" + collection["controller"] + "','" + collection["action"] + "')";
                if (conn.InsertData(sql) != 0)
                {
                    return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("PermissionSet", "Admin") + "'</script>");
                }
                else
                {
                    return Content("<script>alert('添加失败！'); window.location.href = '" + Url.Action("PermissionSet", "Admin") + "'</script>");
                }
            }
            else
            {
                List<Role> lists = new List<Role>();
                string se_sql = "SELECT ID,Name FROM [Role]";
                DataSet ds = conn.Ds_List(se_sql);
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    lists.Add(new Role
                    {
                        ID = int.Parse(item["ID"].ToString()),
                        Name = item["Name"].ToString()
                    });
                }
                ViewData["RoleList"] = lists;
                conn.CloseConnection();
            }
            return View();
        }


        //登陆日志
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult LoginRecord(int page = 1)
        {
            conn.OpenConnection();
            string sql = "Select * from [LoginRecord] order by ID desc";
            List<LoginRecord> loginrecords = new List<LoginRecord>();
            DataSet ds = conn.Ds_List(sql);
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                loginrecords.Add(new LoginRecord
                {
                    ID = int.Parse(item["ID"].ToString()),
                    LoginTime = item["LoginTime"].ToString(),
                    UserID = int.Parse(item["UserID"].ToString()),
                    UserIP = item["UserIP"].ToString()
                });
            }
            conn.CloseConnection();
            LoginRecordViewModel LoginRecordViewModel = new LoginRecordViewModel()
            {
                LoginRecords = loginrecords.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo { CurrentPage = page, TotalItmes = loginrecords.Count, ItemsPerPage = PageSize }
            };
            return View(LoginRecordViewModel);
        }


        //用户中心
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult UserCenter(FormCollection collection, string action_name)
        {
            string password = collection["password"];
            string phone = collection["phone"];
            string email = collection["email"];

            //验证密码格式
            string passwordStr = @"^[\w\W]{6,}$";
            Regex passwordReg = new Regex(passwordStr);
            //验证手机号码格式
            string phoneStr = @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";
            Regex phoneReg = new Regex(phoneStr);
            //验证邮箱地址格式
            string emailStr = @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$";
            Regex emailReg = new Regex(emailStr);

            string username = Request.Cookies["UserName"].Value;
            conn.OpenConnection();
            if (action_name == "修改")
            {
                if (!passwordReg.IsMatch(password.Trim()))
                {
                    return Content("<script>alert('修改失败，密码有误！'); window.location.href = '" + Url.Action("UserCenter", "Admin") + "'</script>");
                }
                else
                {
                    if (!phoneReg.IsMatch(phone.Trim()))
                    {
                        return Content("<script>alert('修改失败，手机号码有误！'); window.location.href = '" + Url.Action("UserCenter", "Admin") + "'</script>");
                    }
                    else
                    {
                        if (!emailReg.IsMatch(email.Trim()))
                        {
                            return Content("<script>alert('修改失败，邮箱有误！'); window.location.href = '" + Url.Action("UserCenter", "Admin") + "'</script>");
                        }
                        else
                        {
                            string sql = "UPDATE [User] SET Password='" + collection["password"] + "',Email='" + collection["email"] + "',Phone='" + collection["phone"] + "',Birthday='" + collection["birthday"] + "',LastEditTime='" + DateTime.Now + "' WHERE UserName='" + username + "'";
                            if (conn.UpdateData(sql) != 0)
                            {
                                conn.CloseConnection();
                                return Content("<script>alert('修改成功！'); window.location.href = '" + Url.Action("UserCenter", "Admin") + "'</script>");
                            }
                            else
                            {
                                conn.CloseConnection();
                                return Content("<script>alert('修改失败！'); window.location.href = '" + Url.Action("UserCenter", "Admin") + "'</script>");
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


        //轮播图
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Carousel(FormCollection collection, string action)
        {
            conn.OpenConnection();
            if (action == "确认添加")
            {
                HttpPostedFileWrapper attachment = (HttpPostedFileWrapper)Request.Files["addcarouselinput"];
                string fileName = attachment.FileName.Substring(0, attachment.FileName.LastIndexOf(@".")) + DateTime.Now.Ticks +
                    attachment.FileName.Substring(attachment.FileName.LastIndexOf(@"."));
                attachment.SaveAs(HttpRuntime.AppDomainAppPath.ToString() + "Upload\\Carousel\\" + fileName);

                if (attachment.ContentLength < 2097152)
                {
                    string carouselPath = "/Upload/Carousel/" + fileName;
                    string sql = "INSERT INTO [Carousel] VALUES('" + carouselPath + "')";
                    int result = conn.InsertData(sql);
                    if (result > 0)
                    {
                        conn.CloseConnection();
                        return Content("<script>alert('添加成功！'); window.location.href = '" + Url.Action("Carousel", "Admin") + "'</script>");
                    }
                    else
                    {
                        return Content("<script>alert('添加失败！'); window.location.href = '" + Url.Action("Carousel", "Admin") + "'</script>");
                    }
                }
                else
                {
                    return Content("<script>alert('图片大小不能超过2M！'); window.location.href = '" + Url.Action("Carousel", "Admin") + "'</script>");
                }
            }
            else
            {
                conn.OpenConnection();
                string sql = "select * from [Carousel]";
                DataSet ds = conn.Ds_List(sql);
                ViewBag.Data = ds.Tables[0];
                conn.CloseConnection();
                return View(ViewBag.Data);
            }
        }


        //轮播图删除
        [LoginAuthorizeFilter]
        [UserAuthorizeFilter]
        public ActionResult Carousel_Remove(int id)
        {
            conn.OpenConnection();
            string sql = "DELETE FROM [Carousel] WHERE [ID]=" + id;
            if (conn.Delete(sql) != 0)
            {
                conn.CloseConnection();
                return Content("<script>alert('删除成功！'); window.location.href = '" + Url.Action("Carousel", "Admin") + "'</script>");
            }
            else
            {
                return Content("<script>alert('删除失败！'); window.location.href = '" + Url.Action("Carousel", "Admin") + "'</script>");
            }
        }



    }
}