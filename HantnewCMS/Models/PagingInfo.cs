using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class PagingInfo
    {
        public int TotalItmes { set; get; }
        public int ItemsPerPage { set; get; }
        public int CurrentPage { set; get; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalItmes / ItemsPerPage);
            }
        }
    }
}