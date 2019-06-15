using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class LoginRecordViewModel
    {
        public IEnumerable<LoginRecord> LoginRecords { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}