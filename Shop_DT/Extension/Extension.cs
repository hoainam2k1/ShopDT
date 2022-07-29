using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shop_DT.Extension
{
    public static class Extension
    {
        public static string ToVnd(this double donGia)
        {
            return donGia.ToString("#,##0") + "VNĐ";
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if(!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for(int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if(s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        //public static string ToUrlFriendly(this string url)
        //{
        //    var result = url.ToLower().Trim();
        //    result = Regex.Replace(result, "áàảãạăắằẳẵặấầẩẫậâ", "a");
        //    result = Regex.Replace(result, "éèẻẽẹêếềểễệ", "e");
        //    result = Regex.Replace(result, "óòỏõọôốồổỗộơớờởỡợ", "o");
        //    result = Regex.Replace(result, "úùủũụưứừữửự", "u");
        //    result = Regex.Replace(result, "íìỉĩị", "i");
        //    result = Regex.Replace(result, "ýỳỷỹỵ", "y");
        //    result = Regex.Replace(result, "đ", "d");
        //    result = Regex.Replace(result, "[^a-z0-9-]", "");
        //    result = Regex.Replace(result, "(-)+", "-");

        //    return result;
        //}
    }
}
