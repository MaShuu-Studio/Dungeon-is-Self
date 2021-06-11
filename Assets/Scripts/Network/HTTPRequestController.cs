using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Json;
using Newtonsoft.Json.Linq;

public class HTTPRequestController : MonoBehaviour
{
    private static HTTPRequestController instance;
    public static HTTPRequestController Instance
    {
        get
        {
            var obj = FindObjectOfType<HTTPRequestController>();
            instance = obj;
            return instance;
        }
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public string SendHTTPPost(string send, string urlString)
    {
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

    public string SendHTTPGet(string urlString)
    {
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
            request.Timeout = 5000;

            response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            result = streamReader.ReadToEnd();

            streamReader.Close();
            responseStream.Close();
            response.Close();

            JObject start = JObject.Parse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result = "false";
        }
        return result;
    }
}
