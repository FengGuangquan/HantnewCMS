using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class TaskSchedule
    {
        public int ID { get; set; }

        public string Content { get; set; }

        public string TaskTitle { get; set; }

        public int TaskID { get; set; }

        public int PublisherID { get; set; }

        public string PublisherUserName { get; set; }

        public string CreateTime { get; set; }

        public string LastEditTime { get; set; }

        public Boolean IsValid { get; set; }
    }
}