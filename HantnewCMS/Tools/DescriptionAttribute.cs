using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS
{
    public class DescriptionAttribute : Attribute
    {
        public string Name
        {
            set;
            get;
        }
        public int No
        {
            set;
            get;
        }
    }
}