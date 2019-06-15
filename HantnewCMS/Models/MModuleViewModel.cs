using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class MModuleViewModel
    {
        public IEnumerable<MModule> MModules { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}