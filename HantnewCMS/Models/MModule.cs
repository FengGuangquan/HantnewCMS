using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HantnewCMS.Models
{
    public class MModule
    {
        public int ID { get; set; }

        public string ModuleName { get; set; }

        public string TableName { get; set; }

        public string IsValid { get; set; }
    }
}