﻿using Domain;
using System.Net;
using System.Net.Sockets;

namespace TrackService
{
    public abstract class TrackService:ITrackService
    {
        public IPAddress Addr { set; protected get; }
        public int Port { set; protected get; }
        public abstract void SaveData(byte[] buffer, int received);

        protected AppDataContext _context;

        protected TrackService(AppDataContext context)
        {
            _context = context;
        }

        public void Start()
        {
            TcpListener listener = new TcpListener(Addr, Port);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptCallback, listener);
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            TcpListener? listener = ar.AsyncState as TcpListener;
            TcpClient? client = listener?.EndAcceptTcpClient(ar);
            listener?.BeginAcceptTcpClient(AcceptCallback, listener);
            StateObject stateObject = new StateObject(client?.Client);
            stateObject?.WorkSocket?.BeginReceive(stateObject.Buffer, 0, stateObject.Buffer.Length, SocketFlags.None, SocketReceiveCallback, stateObject);
        }

        private void SocketReceiveCallback(IAsyncResult ar)
        {
            StateObject? stateObject = ar.AsyncState as StateObject;
            int readed = stateObject.WorkSocket.EndReceive(ar);
            SaveData(stateObject.Buffer, readed);
            stateObject.WorkSocket.BeginReceive(stateObject.Buffer, 0, stateObject.Buffer.Length, SocketFlags.None, SocketReceiveCallback, stateObject);
        }
    }
}