using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class LoginRecord
    {
        public int ID { get; set; }

        public string LoginTime { get; set; }

        public int UserID { get; set; }

        public string UserIP { get; set; }

    }
}