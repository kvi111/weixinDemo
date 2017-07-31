using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace weixinDemo
{
    public class Utils
    {
        //private static Gson gson = new GsonBuilder().disableHtmlEscaping().create();

        //    private Utils()
        //    {
        //    }

        public static Dictionary<String, Object> createMap(Object[] values)
        {
            Dictionary<String, Object> map = new Dictionary<String, Object>(values.Length / 2);
            for (int i = 0; i < values.Length; i++)
            {
                map.Add(values[i].ToString(), values[i+1]);
                ++i;
            }
            return map;
        }

        public static String emptyOr(String str1, String str2)
        {
            if (isBlank(str1))
            {
                return str2;
            }
            return str1;
        }

        //    public static void sleep(long timeout)
        //    {
        //        try
        //        {
        //            TimeUnit.MILLISECONDS.sleep(timeout);
        //        }
        //        catch (Exception e)
        //        {

        //        }
        //    }

        public static String match(String p, String str)
        {
            MatchCollection mc = Regex.Matches(p, str);
            if (mc.Count > 0)
            {
                return mc[0].Groups[1].Value;
            }
            return "";
        }

        //    public static void closeQuietly(Closeable closeable)
        //    {
        //        try
        //        {
        //            closeable.close();
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }
        //    }

        public static bool isBlank(String str)
        {
            return null == str || "" == str.Trim();
        }

        //    public static boolean isNotBlank(String str)
        //    {
        //        return !isBlank(str) && !"null".equalsIgnoreCase(str);
        //    }

        /// <summary>
        /// 得到指定位数的随机数
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static String getRandomNumber(int size)
        {
            //String num = "";
            //for (int i = 0; i < size; i++)
            //{
            //    Random rnd = new Random();
            //    double a = rnd.NextDouble() * 9;
            //    a = Math.Ceiling(a);
            //    int randomNum = (int)a;
            //    num += (int)a;
            //}
            //return num;
            Random rnd = new Random();
            int min = (int)Math.Pow(10, size - 1);
            int max = (int)Math.Pow(10, size) - 1;
            return rnd.Next(min, max).ToString();
        }

        /**
         * obj转json
         */
        public static String toJson(Object o)
        {
            try
            {
                return JsonConvert.SerializeObject(o);
                //return gson.toJson(o);
            }
            catch
            {
                //WriteLog("Json序列化失败", e);
            }
            return "";
        }

        //    /**
        //     * json转obj
        //     */
        //    public static <T> T fromJson(String json, Class<T> classOfT)
        //    {
        //        try
        //        {
        //            return gson.fromJson(json, classOfT);
        //        }
        //        catch (Exception e)
        //        {
        //            WriteLog("Json反序列化失败", e);
        //        }
        //        return null;
        //    }

        //    public static String utf8ToUnicode(String inStr)
        //    {
        //        char[] myBuffer = inStr.toCharArray();
        //        StringBuffer sb = new StringBuffer();
        //        for (int i = 0; i < inStr.length(); i++)
        //        {
        //            Character.UnicodeBlock ub = Character.UnicodeBlock.of(myBuffer[i]);
        //            if (ub == Character.UnicodeBlock.BASIC_LATIN)
        //            {
        //                //英文及数字等
        //                sb.append(myBuffer[i]);
        //            }
        //            else if (ub == Character.UnicodeBlock.HALFWIDTH_AND_FULLWIDTH_FORMS)
        //            {
        //                //全角半角字符
        //                int j = (int)myBuffer[i] - 65248;
        //                sb.append((char)j);
        //            }
        //            else
        //            {
        //                //汉字
        //                short s = (short)myBuffer[i];
        //                String hexS = Integer.toHexString(s);
        //                String unicode = "\\u" + hexS;
        //                sb.append(unicode.toLowerCase());
        //            }
        //        }
        //        return sb.toString();
        //    }

        public static String unicodeToUtf8(String string1)
        {
            try
            {
                if (string1 == null) return "";

                if (string1.IndexOf("\\u") == -1)
                {
                    return string1;
                }
                //byte[] utf8 = string1.ToString( .getBytes("UTF-8");
                //// Convert from UTF-8 to Unicode
                //return new String(utf8, "UTF-8");

                Encoding unicode = Encoding.Unicode;
                Encoding utf8 = Encoding.UTF8;

                byte[] unicodeBytes = unicode.GetBytes(string1);
                byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicodeBytes);
                return utf8.GetString(utf8Bytes);
            }
            catch
            {

            }
            return string1;
        }

        public static String getCookie(List<String> cookies)
        {
            StringBuilder sBuffer = new StringBuilder();
            foreach (String value in cookies)
            {
                if (value == null)
                {
                    continue;
                }
                String cookie = value.Substring(0, value.IndexOf(";") + 1);
                sBuffer.Append(cookie);
            }
            return sBuffer.ToString();
        }

        private static DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long currentTimeMillis()
        {
            return (long)((DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
        }
        public static string PostWebRequest(string postUrl, string paramData)
        {
            string ret = string.Empty;
            try
            {
                byte[] responseData = PostWebRequestBytes(postUrl, paramData);
                ret = Encoding.UTF8.GetString(responseData);//解码
            }
            catch
            {
             
            }
            return ret;
        }
        public static byte[] PostWebRequestBytes(string postUrl, string paramData)
        {
            byte[] responseData = new byte[0];
            try
            {
                byte[] postData = Encoding.UTF8.GetBytes(paramData);
                WebClient webClient = new WebClient();
                responseData = webClient.UploadData(postUrl, "POST", postData);//得到返回字符流
            }
            catch
            {
            
            }
            return responseData;
        }

        //public static byte[] PostWebRequestBytes1(string postUrl, string paramData)
        //{
        //    byte[] responseData = new byte[0];
        //    try
        //    {
        //        byte[] postData = Encoding.UTF8.GetBytes(paramData);
        //        HttpWebRequest webClient = (HttpWebRequest)HttpWebRequest.Create(postUrl);
        //        webClient.Headers.Add("Cookie", "");
        //        responseData = webClient.UploadData(postUrl, "POST", postData);//得到返回字符流
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //    }
        //    return responseData;
        //}

        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
    }
}
