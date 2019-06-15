using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HantnewCMS.Models
{
    public class Task
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public string Content { get; set; }

        public int PublisherID { get; set; }

        public string PublisherUserName { get; set; }

        public string Developers { get; set; }

        public string BudgetTime { get; set; }

        public string TurnoverTime { get; set; }

        public string CreateTime { get; set; }

        public string LastEditTime { get; set; }

        public Boolean IsValid { get; set; }
    }
}