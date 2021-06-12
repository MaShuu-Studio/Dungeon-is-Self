using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Json;
using UnityEngine;
using UnityEngine.UI;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
                NextSelect();
        }

        public void NextSelect()
        {
            if (views[0].activeSelf)
            {
                for (int i = 0; i < signinInputFields.Count; i++)
                    if (signinInputFields[i].isFocused)
                    {
                        i++;
                        if (i != signinInputFields.Count) signinInputFields[i].Select();
                        else signInButton.Select();
                        break;
                    }
            }
            else
            {
                for (int i = 0; i < signupInputFields.Count; i++)
                    if (signupInputFields[i].isFocused)
                    {
                        i++;
                        if (i != signupInputFields.Count) signupInputFields[i].Select();
                        else signUpButton.Select();
                        break;
                    }

            }
        }
        public void NextSelect(GameObject obj)
        {
            bool inputReturn = Input.GetButtonDown("Submit");

            if (views[0].activeSelf)
            {
                for (int i = 0; i < signinInputFields.Count; i++)
                    if (signinInputFields[i].gameObject == obj)
                    {
                        i++;
                        if (i != signinInputFields.Count) signinInputFields[i].Select();
                        else if(inputReturn) signInButton.onClick.Invoke();
                        break;
                    }
            }
            else
            {
                for (int i = 0; i < signupInputFields.Count; i++)
                    if (signupInputFields[i].gameObject == obj)
                    {
                        i++;
                        if (i != signupInputFields.Count) signupInputFields[i].Select();
                        else if (inputReturn) signUpButton.onClick.Invoke();
                        break;
                    }
            }
        }

        public void ChangeView()
        {
            memberId = "";
            password = "";
            nickname = "";

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

        public void SignUp()
        {
            if (CheckForm())
            {
                string urlString = "http://ec2-13-209-42-66.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/join";

                JsonObjectCollection jsonObj = new JsonObjectCollection();
                jsonObj.Add(new JsonStringValue("memberId", memberId));
                jsonObj.Add(new JsonStringValue("password", password));
                jsonObj.Add(new JsonStringValue("nickname", nickname));
                string joinInfo = jsonObj.ToString();

                string result = HTTPRequestController.Instance.SendHTTPPost(joinInfo, urlString);
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


        public bool SignIn(out string token, out string pid)
        {
            string urlString = "http://ec2-13-209-42-66.ap-northeast-2.compute.amazonaws.com:8080/api/dgiself/member/login";
            string result = "";

            JsonObjectCollection jsonObj = new JsonObjectCollection();
            jsonObj.Add(new JsonStringValue("memberId", memberId));
            jsonObj.Add(new JsonStringValue("password", password));
            string startInfo = jsonObj.ToString();

            token = "";
            pid = "";

            result = HTTPRequestController.Instance.SendHTTPPost(startInfo, urlString);
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
    }
}