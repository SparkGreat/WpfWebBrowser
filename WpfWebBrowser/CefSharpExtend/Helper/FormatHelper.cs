using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WpfWebBrowser.CefSharpExtend.Helper
{
    class FormatHelper
    {

        /// <summary>
        /// 对 URL 字符串进行编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();

            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// 对 URL 字符串进行解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

    }
}
