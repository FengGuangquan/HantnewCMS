using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HantnewCMS.Models
{
    public class Class
    {
        public int ID { get; set; }
        public string ClassName { get; set; }

        public string CreateTime { get; set; }

        public string LastEditTime { get; set; }

        public string IsValid { get; set; }
    }
}