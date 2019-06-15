using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class ControllerActionRoleViewModel
    {
        public IEnumerable<ControllerActionRole> ControllerActionRoles { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}