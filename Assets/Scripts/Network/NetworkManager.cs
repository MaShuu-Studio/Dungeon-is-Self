using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using ServerCore;

public class NetworkManager : MonoBehaviour
{
    const int PORT_NUMBER = 7777;
    ServerSession session = new ServerSession();
    // Start is called before the first frame update
    void Start()
    {
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, PORT_NUMBER);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return session; }, 1);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
