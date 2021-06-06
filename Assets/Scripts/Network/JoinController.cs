using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Json;
using Newtonsoft.Json.Linq;

namespace Network
{
    public class JoinController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> views;
        [SerializeField] private List<InputField> signinInputFields;
        [SerializeField] private List<InputField> signupInputFields;
        [SerializeField] private Button signInButton;
        [SerializeField] private Button signUpButton;
        [SerializeField] private Text failedSignIn;
        [SerializeField] private Text failedSignUp;
        private string memberId;
        private string password;
        private string nickname;
        private string joinInfo;
        private string startInfo;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Return))
            {
                if (views[0].activeSelf)
                {
                    for (int i = 0; i < signinInputFields.Count; i++)
                        if(signinInputFields[i].isFocused)
                        {
                            i++;
                            if (i == signinInputFields.Count) signInButton.Select();
                            else signinInputFields[i].Select();
                            break;
                        }
                }
                else
                {
                    for (int i = 0; i < signupInputFields.Count; i++)
                        if (signupInputFields[i].isFocused)
                        {
                            i++;
                            if (i == signupInputFields.Count) signUpButton.Select();
                            else signupInputFields[i].Select();
                            break;
                        }

                }
            }
        }

        public void ChangeView()
        {
            memberId = "";
            password = "";
            nickname = "";
            joinInfo = "";
            startInfo = "";

            for (int i = 0; i < signinInputFields.Count; i++)
                signinInputFields[i].text = "";

            for (int i = 0; i < signupInputFields.Count; i++)
                signupInputFields[i].text = "";

            foreach (GameObject view in views)
                view.SetActive(!view.activeSelf);

            failedSignIn.gameObject.SetActive(false);
            failedSignUp.gameObject.SetActive(false);
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
                string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/join";
                CreateJoinJson();

                string result = SendHTTP(joinInfo, urlString);
                if (result == "true")
                {
                    ChangeView();
                }
                else
                {
                    failedSignUp.text = "<color=#ff0000>이미 있는 아이디입니다.</color>";
                    failedSignUp.gameObject.SetActive(true);
                }
            }
            else
            {
                failedSignUp.text = "<color=#ff0000>아이디, 비밀번호 조건을 확인해주세요.</color>";
                failedSignUp.gameObject.SetActive(true);
            }
        }

        private bool CheckForm()
        {
            if (memberId.Length < 6 || memberId.Length > 12) return false;
            if (password.Length < 8 || password.Length > 20) return false;
            if (nickname.Length < 2 || nickname.Length > 10) return false;

            {
                string specialCharacterList = "!@#$%^&*()_+-=,./";
                bool pwCondition = false;
                for (int i = 0; i < password.Length; i++)
                {
                    pwCondition = false;
                    if ((password[i] >= 'a' && password[i] <= 'z')
                        || (password[i] >= 'A' && password[i] <= 'Z')
                        || (password[i] >= '0' && password[i] <= '9')
                        || (specialCharacterList.IndexOf(password[i]) != -1))
                        pwCondition = true;

                    if (pwCondition == false) break;
                }
                if (pwCondition) return true;
                else return false;
            }
        }

        private void CreateJoinJson()
        {
            JsonObjectCollection jsonObj = new JsonObjectCollection();
            jsonObj.Add(new JsonStringValue("memberId", memberId));
            jsonObj.Add(new JsonStringValue("password", password));
            jsonObj.Add(new JsonStringValue("nickname", nickname));
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
                Uri url = new Uri(urlString);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.Http.Post;
                request.Timeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(send);
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
                result = "false";
                Debug.Log(e);
            }
            return result;
        }

        public bool ClickStart(out string token, out string pid)
        {
            string urlString = "http://ec2-54-180-153-249.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/login";
            string result = "";
            CreateStartJson();
            JsonObjectCollection jsonObj = new JsonObjectCollection();
            token = "";
            pid = "";

            result = SendHTTP(startInfo, urlString);
            try
            {
                JObject start = JObject.Parse(result);
                token = start["token"].ToString();
                pid = start["memberCode"].ToString();
                string nickname = start["nickname"].ToString();

                NetworkManager.Instance.SetUserInfo(pid, nickname);

                if (result != "")
                {
                    return true;
                }
                else
                {
                    failedSignIn.gameObject.SetActive(true);
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            failedSignIn.gameObject.SetActive(true);
            return false;
        }

        private void CreateStartJson()
        {
            JsonObjectCollection jsonObj = new JsonObjectCollection();
            jsonObj.Add(new JsonStringValue("memberId", memberId));
            jsonObj.Add(new JsonStringValue("password", password));
            startInfo = jsonObj.ToString();
        }
    }
}