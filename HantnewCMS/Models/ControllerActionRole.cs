using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class ControllerActionRole
    {
        public int ID { get; set; }

        public string IsAllowed { get; set; }

        public int RoleID { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string RoleName { get; set; }
    }
}