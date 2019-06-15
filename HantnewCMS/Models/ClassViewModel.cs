using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class ClassViewModel
    {
        public IEnumerable<Class> Classes { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}