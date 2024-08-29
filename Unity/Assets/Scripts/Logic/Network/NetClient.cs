using System;
using System.Net;
using Lockstep.Network;
using Lockstep.Serialization;
using Lockstep.Game;
using NetMsg.Common;

namespace Lockstep.Game{
    public class NetClient : IMessageDispatcher {
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);
        private NetOuterProxy _netProxy = new NetOuterProxy();
        public Session Session;
        public Action<ushort, object> NetMsgHandler;

        private int count = 0;
        public int id;

        public void DoStart(){
            _netProxy.StartAsClient(NetworkProtocol.TCP);
            _netProxy.MessageDispatcher = this;
            _netProxy.MessagePacker = MessagePacker.Instance;
            Session = _netProxy.Create(serverIpPoint);
            Session.Start();
        }

        public void DoDestroy(){
            if (Session != null) {
                _netProxy.Dispose();
                Session = null;
            }
        }

        public void Dispatch(Session session, ushort opcode, BaseMsg msg)
		{
            NetMsgHandler?.Invoke(opcode, msg);
        }


        public void Send(IMessage msg){
            Session.Send(msg);
        }
        public void SendMessage(EMsgSC type, byte[] bytes){
            Session?.Send(0x00, (ushort) type, bytes);
        }
        
    }
}