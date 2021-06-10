using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace ServerCore
{
    public class Connector
    {
        public bool connectionFailed { get; private set; } = false;
        Func<Session> _sessionFactory;
        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _sessionFactory = sessionFactory;

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;
                args.UserToken = socket;

                RegisterConnect(args);
            }            
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            connectionFailed = false;
            Socket socket = args.UserToken as Socket;

            if (socket == null) return;

            bool pending = socket.ConnectAsync(args);
            if (pending == false) OnConnectCompleted(null, args);
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Init(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
                connectionFailed = false;
            }
            else
            {
                Debug.Log($"OnConnectCompleted Fail: {args.SocketError}");
                connectionFailed = true;
            }
        }
    }
}
