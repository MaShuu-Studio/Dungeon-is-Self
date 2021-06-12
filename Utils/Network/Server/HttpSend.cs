using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace Server
{
    public static class HttpSend
    {
        public static string SerializeObject(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static void SendPost(string result, string urlString)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(result);
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static string RequestJoinClient(string token)
        {
            string urlString = "http://ec2-13-209-42-66.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/authorization";
            int nStartTime = 0;
            string result = "";
            string strMsg = string.Empty;
            nStartTime = Environment.TickCount;

            HttpWebRequest request = null;
            HttpWebResponse response = null;

            try
            {
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;

                request.Headers.Add("Authorization", "Bearer " + token);
                //request.Headers["Authentication"] = "Bearer " + token;
                request.Timeout = 5000;

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();

                JObject start = JObject.Parse(result);
                result = start["memberCode"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = "false";
            }
            return result;
        }

    }
}
