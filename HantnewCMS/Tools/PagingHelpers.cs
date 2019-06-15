using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using HantnewCMS.Models;

namespace System.Web.Mvc
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            result.Append("<div><ul style='display:inline-block'>");

            if (pagingInfo.CurrentPage > 1)
            {
                //建立首页分页链接
                result.Append("<li style='display:inline-block;'>");
                result.Append("<a href='");
                result.Append(pageUrl(1));
                result.Append("'>首页&nbsp;</a>");
                result.Append("</li>");
                //建立上一页分页链接
                result.Append("<li style='display:inline-block;'>");
                result.Append("<a href='");
                result.Append(pageUrl(pagingInfo.CurrentPage - 1));
                result.Append("'>上一页&nbsp;</a>");
                result.Append("</li>");
            }

            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                if (i <= 2 || i >= pagingInfo.CurrentPage - 3 && i <= pagingInfo.CurrentPage + 3 || i >= pagingInfo.TotalPages - 1)
                {
                    createPageLink(pagingInfo, pageUrl, result, i);
                }
                else if (result[result.Length - 1] != '.')
                {
                    result.Append("...");
                }
            }

            if (pagingInfo.CurrentPage < pagingInfo.TotalPages)
            {
                //建立上一页分页链接
                result.Append("<li style='display:inline-block;'>");
                result.Append("<a href='");
                result.Append(pageUrl(pagingInfo.CurrentPage + 1));
                result.Append("'>下一页&nbsp;</a>");
                result.Append("</li>");
                //建立首页分页链接
                result.Append("<li style='display:inline-block;'>");
                result.Append("<a href='");
                result.Append(pageUrl(pagingInfo.TotalPages));
                result.Append("'>末页&nbsp;</a>");
                result.Append("</li>");

            }
            result.Append("</ul>");

            //建立 当前页/总页数
            result.Append("<ul style='float:right;'>当前在第");
            result.Append(pagingInfo.CurrentPage);
            result.Append(" / ");
            result.Append(pagingInfo.TotalPages);
            result.Append("页");
            result.Append("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

            //建立 总数
            result.Append("总共");
            result.Append(pagingInfo.TotalItmes);
            result.Append("条</ul></div>");

            return MvcHtmlString.Create(result.ToString());
        }

        private static void createPageLink(PagingInfo pagingInfo, Func<int, string> pageUrl, StringBuilder result, int i)
        {
            if (i == pagingInfo.CurrentPage)
            {
                result.Append("<li class='current' style='display:inline-block;'>");
            }
            else
            {
                result.Append("<li style='display:inline-block;'>");
            }
            result.Append("<a href='");
            result.Append(pageUrl(i));
            result.Append("'>");
            result.Append(i);
            result.Append("&nbsp;</a>");
            result.Append("</li>");
        }
    }
}