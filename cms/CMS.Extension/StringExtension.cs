using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Extension
{
    public static class StringExtension
    {
        /// <summary>
        ///     判断路径是不是图片, true 是，false 不是
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsImage(this string input)
        {
            var extesion = new[] {"jpg", "jpeg", "png", "tiff", "gif", "bmp"};
            return extesion.Any(s => input.ToLower().EndsWith("." + s));
        }

        /// <summary>
        ///     合并拼接路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string Combine(this string path, string append)
        {
            if (path.EndsWith("/") && append.StartsWith("/"))
            {
                path = path.TrimEnd('/');
            }

            if (path.EndsWith("\\") && append.StartsWith("\\"))
            {
                path = path.TrimEnd('\\');
            }

            return Path.Combine(path, append);
        }

        /// <summary>
        ///     合并拼接路径,会剔除重复的 \\ 或 //
        /// </summary>
        /// <param name="path"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string CombinePath(this string path, string append)
        {
            return (path + append).Replace("//", "/").Replace("\\\\", "\\");
        }

        /// <summary>
        ///     移除Html
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtml(this string input)
        {
            var htmlstring = input.ToStr();
            if (!string.IsNullOrEmpty(htmlstring))
            {
                htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"–>", "", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"<!–.*", "", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(rdquo);", "”",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(ldquo);", "“",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(emsp);", "   ",
                    RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                htmlstring = HttpUtility.HtmlEncode(htmlstring).Trim();
                return htmlstring;
            }

            return string.Empty;
        }

        /// <summary>
        ///     将字符转为Ascii码
        /// </summary>
        /// <param name="input"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToAscii(this string input, char str)
        {
            return input.Replace(str.ToString(), "&#" + str.Asc() + ";");
        }

        /// <summary>
        ///     将字符转为Ascii数字
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int Asc(this char character)
        {
            var asciiEncoding = new ASCIIEncoding();
            int intAsciiCode = asciiEncoding.GetBytes(character.ToString())[0];
            return intAsciiCode;
        }

        /// <summary>
        ///     判断字符串是否为空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string input)
        {
            return input == null || string.IsNullOrEmpty(input.Trim());
        }

        /// <summary>
        ///     判断字符串不为空
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string input)
        {
            return !IsEmpty(input);
        }

        /// <summary>
        ///     是否24小时制时间（如匹配格式23:59、00:05等）。
        /// </summary>
        /// <param name="input">时间字符串。</param>
        /// <returns></returns>
        public static bool Is24HourSystemTime(this string input)
        {
            var reg = new Regex("(?:0\\d|1\\d|2[0-3]):(?:0\\d|[1-5]\\d)$", RegexOptions.IgnoreCase);
            return reg.IsMatch(input);
        }

        /// <summary>
        ///     [辅助方法]驼峰命名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CamelCase(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = input.ToCharArray()[0].ToString(CultureInfo.InvariantCulture).ToUpper() + input.Substring(1);
                return input;
            }

            return string.Empty;
        }

        public static List<int> ToInList(this string[] inputs)
        {
            List<int> ids = new List<int>();
            foreach (var input in inputs)
            {
                ids.Add(input.ToInt());
            }

            return ids;
        }

        #region 字符串截取

        /// <summary>
        ///     字符串截取函数，截取从左边开始指定的字节数，默认超出部分不显示
        /// </summary>
        /// <param name="text">需要截取的字符串</param>
        /// <param name="cutLength">需要截取的长度，一个中文为2个字符，大小英文为1个字符</param>
        /// <returns>处理后的字符串</returns>
        public static string Cut(this string text, int cutLength)
        {
            return Cut(text, 0, Math.Abs(cutLength), string.Empty);
        }

        /// <summary>
        ///     字符串截取函数，截取从左边开始指定的字节数，默认超出部分不显示
        /// </summary>
        /// <param name="text">需要截取的字符串</param>
        /// <param name="cutLength">需要截取的长度，一个中文为2个字符，大小英文为1个字符</param>
        /// <param name="tailStr">超出部分显示的字符</param>
        /// <returns>处理后的字符串</returns>
        public static string Cut(this string text, int cutLength, string tailStr)
        {
            return Cut(text, 0, cutLength, tailStr);
        }

        /// <summary>
        ///     字符串截取函数，截取从左边开始指定的字节数，默认超出部分不显示
        /// </summary>
        /// <param name="text">需要截取的字符串</param>
        /// <param name="startIndex">截取的起始位置，从0开始</param>
        /// <param name="cutLength">需要截取的长度，一个中文为2个字符，大小英文为1个字符</param>
        /// <param name="tailStr">超出部分显示的字符</param>
        /// <returns>处理后的字符串</returns>
        public static string Cut(this string text, int startIndex, int cutLength, string tailStr)
        {
            var isInts = new List<int>
            {
                0x3002,
                0xff1f,
                0xff01,
                0xff0c,
                0x3001,
                0xff1b,
                0xff1a,
                0x2018,
                0x2019,
                0x201c,
                0x201d,
                0xff08,
                0xff09,
                0x3014,
                0x3015,
                0x3010,
                0x3011,
                0x300c,
                0x300d,
                0x300e,
                0x300f,
                0x2014,
                0x2026,
                0x2013,
                0xff0e,
                0x300a,
                0x300b,
                0x3008,
                0x3009
            };

            var temp = string.Empty;
            text = text.Replace(" ", String.Empty);
            if (Encoding.Default.GetByteCount(text) <= cutLength) //如果长度比需要的长度n小,返回原字符串
            {
                return text;
            }

            var t = 0;
            var q = text.ToCharArray();
            for (var i = startIndex; i <= startIndex + cutLength && i < q.Length; i++)
            {
                if (isInts.Contains(q[i]) || (q[i] >= 0x4E00 && q[i] <= 0x9FA5)) //是否汉字
                {
                    temp += q[i];
                    t += 2;
                }
                else
                {
                    temp += q[i];
                    t += 1;
                }

                if (t >= cutLength)
                {
                    break;
                }
            }

            return (temp + tailStr);
        }

        /// <summary>
        ///     如果字符串超过指定长度则剪切字符串
        /// </summary>
        /// <param name="text">字符串。</param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string SubStr(this string text, int maxLength)
        {
            return Cut(text, maxLength);
        }

        /// <summary>
        ///     转换为时间格式
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToDate(this string input, string format)
        {
            DateTime dt;
            var dateTime = DateTime.TryParse(input, out dt);
            return dt.ToString(format);
        }

        /// <summary>
        ///     截取字符串长度，超出部分用...显示
        /// </summary>
        /// <param name="input"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubString(this string input, int length)
        {
            return Cut(input, length, "....");
        }

        #endregion 字符串截取


        public static string GetUnicode(this string text)
        {
            if (text.IsEmpty()) return text;

            byte[] bytes = Encoding.Unicode.GetBytes(text);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'),
                    bytes[i].ToString("x").PadLeft(2, '0'));
            }

            return stringBuilder.ToString();
        }
    }
}