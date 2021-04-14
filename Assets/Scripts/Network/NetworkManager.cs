using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using ServerCore;
using System;

public class NetworkManager : MonoBehaviour
{
    const int PORT_NUMBER = 7777;
    static ServerSession session = new ServerSession();

    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            var obj = FindObjectOfType<NetworkManager>();
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
        // 게임 쓰레드에서 Pop하여 작동하는 부분.
        List<IPacket> packets = PacketQueue.Instance.PopAll();
        foreach (IPacket packet in packets)
        {
            PacketManager.Instance.HandlePacket(session, packet);
        }
        packets.Clear();
    }

    public void Send(ArraySegment<byte> segment)
    {
        session.Send(segment);
    }
}
