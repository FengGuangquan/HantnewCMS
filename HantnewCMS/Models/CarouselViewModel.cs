using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class CarouselViewModel
    {
        public IEnumerable<Carousel> Carousels { get; set; }
        public PagingInfo PagingInfo { get; set; }

    }
}