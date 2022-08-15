using Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Sockets;

namespace TrackService
{
    public abstract class TrackService:ITrackService
    {
        public IPAddress Addr { set; protected get; } = IPAddress.Any;
        public int Port { set; protected get; } = 0;
        public abstract void SaveData(byte[] buffer, int received);

        protected readonly IServiceScopeFactory _serviceScopeFactory;

        protected TrackService(IServiceScopeFactory factory)
        {
            _serviceScopeFactory = factory;
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