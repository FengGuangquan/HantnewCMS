using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HantnewCMS.Models
{
    public class PageinatedList<T> : List<T>
    {
        private int pageIndex;//当前页码-1
        private int pageSize;//每页大小（数据在条数）
        private int totalCount;//（记录总条数）
        private int totalPages;//（总页数）


        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex
        {
            set { pageIndex = value; }
            get { return pageIndex; }
        }


        /// <summary>
        /// 每页个数
        /// </summary>
        public int PageSize
        {
            set { pageSize = value; }
            get { return pageSize; }
        }
        /// <summary>
        /// 数据总数
        /// </summary>
        public int TotalCount
        {
            set { totalCount = value; }
            get { return totalCount; }
        }
        /// <summary>
        /// 可分页数
        /// </summary>
        public int TotalPages
        {
            set { totalPages = value; }
            get { return totalPages; }
        }

        public PageinatedList(List<T> list, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = list.Count;
            this.TotalPages = TotalCount / PageSize; //总页数=记录总数/每页大小
            this.AddRange(list.Skip(pageIndex * PageSize).Take(PageSize));//跳过m（当前页码*每页条数）条，取n（每页条数）条记录进行显示

        }
    }
}