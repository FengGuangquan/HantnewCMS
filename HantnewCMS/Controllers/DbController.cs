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
    public class DbController : Controller
    {
        Conn conn = new Conn();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Install(string action)
        {
            string sqlUser =
            "create table [User] (" +
            "ID int identity(1,1) primary key," +
            "UserName varchar(50)," +
            "RealName varchar(50)," +
            "Password varchar(200)," +
            "CreateTime varchar(50)," +
            "LastEditTime varchar(50)," +
            "Phone varchar(50)," +
            "Email varchar(50)," +
            "Birthday varchar(50)," +
            "IsValid bit);";

            string sqlUserRole =
            "create table UserRole (" +
            "ID int identity(1,1) primary key," +
            "UserID int," +
            "RoleID int);";

            string sqlRole =
            "create table Role (" +
            "ID int identity(1,1) primary key," +
            "Name varchar(50)," +
            "Description varchar(200));";

            string sqlArticle =
            "create table Article (" +
            "ID int identity(1,1) primary key," +
            "Title varchar(50)," +
            "Content text," +
            "UserID int," +
            "CreateTime varchar(50)," +
            "LastEditTime varchar(50)," +
            "ArticleClassID int," +
            "AttachmentPath varchar(200)," +
            "TitleImgPath varchar(200)," +
            "IsValid bit);";

            string sqlTableIsValid =
            "create table TableIsValid (" +
            "ID int identity(1,1) primary key," +
            "TableName varchar(50)," +
            "IsValid bit);";

            string sqlArticleClass =
            "create table ArticleClass (" +
            "ID int identity(1,1) primary key," +
            "ClassName varchar(20)," +
            "CreateTime varchar(50)," +
            "LastEditTime varchar(50)," +
            "IsValid bit);";

            string sqlControllerActionRole =
            "create table ControllerActionRole (" +
            "ID int identity(1,1) primary key," +
            "IsAllowed bit," +
            "RoleID int," +
            "Controller varchar(50)," +
            "Action varchar(50));";

            string sqlLoginRecord =
            "create table LoginRecord (" +
            "ID int identity(1,1) primary key," +
            "LoginTime varchar(50)," +
            "UserID int," +
            "UserIP varchar(50));";

            string sqlTask =
            "create table Task (" +
            "ID int identity(1,1) primary key," +
            "Title varchar(50)," +
            "Content text," +
            "PublisherID int," +
            "Developers varchar(200)," +     //参与开发人员
            "BudgetTime int," +              //预算完成时长
            "TurnoverTime varchar(50)," +    //项目交付时间
            "CreateTime varchar(50)," +
            "LastEditTime varchar(50)," +
            "IsValid bit); ";

            string sqlTaskSchedule =
            "create table TaskSchedule (" +
            "ID int identity(1,1) primary key," +
            "Content text," +
            "TaskID int," +
            "PublisherID int," +
            "CreateTime varchar(50)," +
            "LastEditTime varchar(50)," +
            "IsValid bit); ";

            string sqlModule =
            "create table Module (" +
            "ID int identity(1,1) primary key," +
            "ModuleName varchar(50)," +
            "TableName varchar(50)," +
            "IsValid bit); ";

            string sqlCarousel =
            "create table Carousel (" +
            "ID int identity(1,1) primary key," +
            "CarouselPath varchar(200));";

            string sqlInsertUser =
            "insert into [User] VALUES ('" +
            "superadmin','姓名1','123456','2017/12/16 13:03:09','2017/12/16 13:03:09','13000000000','123456@qq.com','1997-12-16',1)," +
            "('admin','姓名2','123456','2017/12/16 13:03:09','2017/12/16 13:03:09','13000000001','123457@qq.com','1997-12-16',1)," +
            "('teacher','姓名3','123456','2017/12/16 13:03:09','2017/12/16 13:03:09','13000000002','123458@qq.com','1997-12-16',1)," +
            "('student','姓名4','123456','2017/12/16 13:03:09','2017/12/16 13:03:09','13000000003','123459@qq.com','1997-12-16',1)";

            string sqlInsertRole =
            "insert into [Role] VALUES ('" +
            "超级管理员','超级管理员')," +
            "('管理员','管理员')," +
            "('教师','教师')," +
            "('学生','学生');";

            string sqlInsertUserRole =
            "insert into [UserRole] VALUES (" +
            "1,1)," +
            "(2,2)," +
            "(3,3)," +
            "(4,4);";

            string sqlInsertPermission =
            "insert into [ControllerActionRole] VALUES " +
            "(1,1,'Admin','Article')," +
            "(1,1,'Admin','Article_Add')," + 
            "(1,1,'Admin','Article_Detail')," +
            "(1,1,'Admin','Article_Edit')," + 
            "(1,1,'Admin','Article_Remove')," +
            "(1,1,'Admin','Class')," + 
            "(1,1,'Admin','Class_Edit')," +
            "(1,1,'Admin','Permission_Add')," + 
            "(1,1,'Admin','PermissionSet')," +
            "(1,1,'Admin','Role')," + 
            "(1,1,'Admin','Role_Remove')," +
            "(1,1,'Admin','Task')," + 
            "(1,1,'Admin','Task_Add')," +
            "(1,1,'Admin','Task_Detail')," + 
            "(1,1,'Admin','Task_Edit')," +
            "(1,1,'Admin','Task_Remove')," + 
            "(1,1,'Admin','TaskSchedule')," +
            "(1,1,'Admin','TaskSchedule_Add')," + 
            "(1,1,'Admin','TaskSchedule_Detail')," +
            "(1,1,'Admin','TaskSchedule_Edit')," + 
            "(1,1,'Admin','TaskSchedule_Remove')," +
            "(1,1,'Admin','UserTable')," + 
            "(1,1,'Admin','UserTable_Detail')," +
            "(1,1,'Admin','UserTable_Edit')," + 
            "(1,1,'Admin','UserTable_Remove')," +
            "(1,1,'Admin','Index')," + 
            "(1,1,'Admin','LoginRecord')," +
            "(1,1,'Admin','UserCenter')," +
            "(1,2,'Admin','Article_Add')," + 
            "(1,2,'Admin','Article_Detail')," +
            "(1,2,'Admin','Article_Edit')," + 
            "(1,2,'Admin','Article_Remove')," +
            "(1,2,'Admin','Class')," + 
            "(1,2,'Admin','Class_Edit')," +
            "(1,2,'Admin','Permission_Add')," + 
            "(1,2,'Admin','PermissionSet')," +
            "(1,2,'Admin','Role')," + 
            "(1,2,'Admin','Role_Remove')," +
            "(1,2,'Admin','Task')," + 
            "(1,2,'Admin','Task_Add')," +
            "(1,2,'Admin','Task_Detail')," + 
            "(1,2,'Admin','Task_Edit')," +
            "(1,2,'Admin','Task_Remove')," + 
            "(1,2,'Admin','TaskSchedule')," +
            "(1,2,'Admin','TaskSchedule_Add')," + 
            "(1,2,'Admin','TaskSchedule_Detail')," +
            "(1,2,'Admin','TaskSchedule_Edit')," + 
            "(1,2,'Admin','TaskSchedule_Remove')," +
            "(1,2,'Admin','UserTable')," + 
            "(1,2,'Admin','UserTable_Detail')," +
            "(1,2,'Admin','UserTable_Edit')," + 
            "(1,2,'Admin','UserTable_Remove')," +
            "(1,2,'Admin','Index')," + 
            "(1,2,'Admin','LoginRecord')," +
            "(1,2,'Admin','UserCenter');";

            string sqlInsertModule =
            "insert into [Module] VALUES ('" +
            "文章','Article','1')," +
            "('投票','Vote','1')," +
            "('问卷','Survey','1')";


            conn.OpenConnection();
            if (action == "Drop tables")
            {
                string sql =
                "declare @sql varchar(8000) " +
                "while (select count(*) from sysobjects where type='U')>0 " +
                "begin SELECT @sql='drop table [' + name+']' FROM sysobjects WHERE (type = 'U') ORDER BY 'drop table [' + name+']' " +
                "exec(@sql) " +
                "end ";
                int drop = conn.Drop(sql);
                if (drop != 0)
                {
                    ViewBag.Result = "Drop tables successfully.";
                }
                else
                {
                    ViewBag.Result = "Failed to drop tables.";
                }
            }
            if (action == "Create tables")
            {
                int result = conn.CreateTable(sqlUser
                + sqlUserRole
                + sqlRole
                + sqlArticle
                + sqlArticleClass
                + sqlControllerActionRole
                + sqlLoginRecord
                + sqlTask
                + sqlTaskSchedule
                + sqlInsertPermission
                + sqlTableIsValid
                + sqlModule
                + sqlCarousel
                );

                if (result != 0)
                {
                    ViewBag.Result = "Create tables succuss.";
                    conn.CloseConnection();
                    return View();
                }
                conn.CloseConnection();
                return View();
            }
            if (action == "Initialized Data")
            {
                int result = conn.InsertData(sqlInsertUser + sqlInsertRole + sqlInsertUserRole + sqlInsertModule);
                if (result != 0)
                {
                    ViewBag.Result = "InsertData success.";
                }
                else
                {
                    ViewBag.Result = "Fail to InsertData.";
                }
            }
            return View();
        }
    }
}