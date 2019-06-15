using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HantnewCMS.Models
{
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }

        public string RealName { get; set; }

        public string Role { get; set; }

        public string Password { get; set; }

        public string CreateTime { get; set; }

        public string LastEditTime { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Birthday { get; set; }

        public string IsValid { get; set; }
    }
}