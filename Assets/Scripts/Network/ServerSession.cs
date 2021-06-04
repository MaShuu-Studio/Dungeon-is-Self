using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;
using UnityEngine;

namespace Network
{
    class ServerSession : PacketSession
    {
        private bool isConnected = false;
        public bool Connected { get { return isConnected; } }
        public override void OnConnected(EndPoint endPoint)
        {
            Debug.Log($"[System] OnConnected : {endPoint}");
            isConnected = true;
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Debug.Log($"[System] OnDisconnected : {endPoint}");
            isConnected = false;
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer, (s, p) => PacketQueue.Instance.Push(p));
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"[System] Transferred bytes: {numOfBytes}");
        }
    }
}
