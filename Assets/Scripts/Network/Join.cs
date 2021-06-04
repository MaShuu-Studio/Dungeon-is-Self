using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Data;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Json;
using Newtonsoft.Json.Linq;

namespace Network
{
    public class Join : MonoBehaviour
    {
        /*
        private static Join instance;
        public static Join Instance
        {
            get {
                var obj = FindObjectOfType<Join>();
                instance = obj;
                return instance;
            }
        }

        void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        */
        public string memberId { get; private set; }
        public string password { get; private set; }
        public string nickname { get; private set; }
        public string joinInfo { get; private set; }
        public string startInfo { get; private set; }
        public string token { get; private set; }

        public AuthenticationHeaderValue authorization { get; private set; }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InputID(string myID)
        {
            memberId = myID;
        }

        public void InputPW(string myPW)
        {
            password = myPW;
        }

        public void InputNN(string myNN)
        {
            nickname = myNN;
        }

        public void ClickJoin()
        {
            if (CheckForm())
            {
                string respath = @"D:\Yujun\study\41\Dudream\Dungeon-is-Self\response.json";
                string reqpath = @"D:\Yujun\study\41\Dudream\Dungeon-is-Self\test.json";
                string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/join";
                CreateJoinJson(reqpath);
                //Debug.Log(SendHTTP(joinInfo));
                JsonObjectCollection jsonObj = new JsonObjectCollection();
                jsonObj.Add(new JsonStringValue("joinInfo", SendHTTP(joinInfo, urlString)));
                File.WriteAllText(respath, jsonObj.ToString());
            }
        }

        private bool CheckForm()
        {
            if (memberId.Length < 6 && memberId.Length > 16) return false;
            if (password.Length < 7 && password.Length > 20) return false;
            else
            {
                bool checkC = false, checkN = false;
                for (int i = 0; i < password.Length; i++)
                {
                    if ((password[i] >= 65 && password[i] <= 90) || (password[i] >= 97 && password[i] <= 122)) checkC = true;
                    if (password[i] >= 48 && password[i] <= 57) checkN = true;
                }
                if (checkC && checkN) return true;
                else return false;
            }
        }

        private void CreateJoinJson(string path)
        {
            JsonObjectCollection jsonObj = new JsonObjectCollection();
            jsonObj.Add(new JsonStringValue("memberId", memberId));
            jsonObj.Add(new JsonStringValue("password", password));
            jsonObj.Add(new JsonStringValue("nickname", nickname));
            File.WriteAllText(path, jsonObj.ToString());
            joinInfo = jsonObj.ToString();
        }

        private string SendHTTP(string send, string urlString)
        {
            int nStartTime = 0;
            string result = "";
            string strMsg = string.Empty;
            nStartTime = Environment.TickCount;
            //string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/join";

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                Debug.Log("Send");
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(send);
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Debug.Log("Send");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                Debug.Log("Send");

                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();
                Debug.Log("Send");
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        private string SendHTTP2(/*string send, */string urlString, string token)
        {
            int nStartTime = 0;
            string result = "";
            string strMsg = string.Empty;
            nStartTime = Environment.TickCount;

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            
            try
            {
                Debug.Log("Send");
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Get;
                
                request.Headers.Add("Authorization", "Bearer " + token);
                //request.Headers["Authentication"] = "Bearer " + token;
                request.Timeout = 5000;

                /*Debug.Log("Send");
                byte[] data = Encoding.UTF8.GetBytes(send);
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                Debug.Log("Send");
                Stream dataStream = request.GetRequestStream();
                Debug.Log("1");
                dataStream.Write(data, 0, data.Length);
                Debug.Log("2");
                dataStream.Close();
                Debug.Log("3");*/

                Debug.Log("Send");
                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                result = streamReader.ReadToEnd();

                streamReader.Close();
                responseStream.Close();
                response.Close();
                Debug.Log("Send");
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                return result;
            }
            return result;
        }

        public bool ClickStart()
        {
            string respath = @"D:\Yujun\study\41\Dudream\Dungeon-is-Self\StartResponse.json";
            string reqpath = @"D:\Yujun\study\41\Dudream\Dungeon-is-Self\Starttest.json";
            string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/login";
            string url = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/authorization";
            string result = "";
            string result2 = "";
            CreateStartJson(reqpath);
            JsonObjectCollection jsonObj = new JsonObjectCollection();

            result = SendHTTP(startInfo, urlString);
            JObject start = JObject.Parse(result);
            token = start["token"].ToString();
            jsonObj.Add(new JsonStringValue("startInfo", result));

            
            
            result2 = SendHTTP2(/*memberId, */url, token);
            jsonObj.Add(new JsonStringValue("token", result2));

            File.WriteAllText(respath, jsonObj.ToString());

            //File.WriteAllText(path, token);
            if(result != "") return true;
            else return false;
        }

        private void CreateStartJson(string path)
        {
            JsonObjectCollection jsonObj = new JsonObjectCollection();
            jsonObj.Add(new JsonStringValue("memberId", memberId));
            jsonObj.Add(new JsonStringValue("password", password));
            File.WriteAllText(path, jsonObj.ToString());
            startInfo = jsonObj.ToString();
        }
    }
}