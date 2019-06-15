using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HantnewCMS.Models
{
    public class Article
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string CreateTime { get; set; }

        public string LastEditTime { get; set; }

        public int ArticleClassID { get; set; }

        public string ClassName { get; set; }

        public string AttachmentPath { get; set; }

        public string TitleImgPath { get; set; }

        public Boolean IsValid { get; set; }
    }
}