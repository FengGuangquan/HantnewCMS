﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class TaskViewModel
    {
        public IEnumerable<Task> Tasks { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}