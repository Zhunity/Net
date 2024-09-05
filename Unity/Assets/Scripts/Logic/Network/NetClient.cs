using System;
using System.Net;
using Lockstep.Network;
using Lockstep.Serialization;
using Lockstep.Game;
using NetMsg.Common;
using Lockstep.Logging;

namespace Lockstep.Game{
	/// <summary>
	/// TODO 感觉可以和NetOuterProxy合体？
	/// 也许不能，NetOuterProxy是框架类，这个是业务实现
	/// </summary>
	public class NetClient : IMessageDispatcher {
        public static IPEndPoint serverIpPoint = NetworkUtil.ToIPEndPoint("127.0.0.1", 10083);
        private NetOuterProxy _netProxy = new NetOuterProxy();
        private Session _session;
		public Session Session
        {
            get => _session;
            set
            {
                Debug.Log("set session " +  value.Id + "  " + this.id);
                _session = value;
            }
        }
        public Action<ushort, object> NetMsgHandler;

        public long id = -1;

        public NetClient()
        {
			id = IdGenerater.GenerateId();
		}

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
            Debug.Log("SendMessage " + type + " " + Session?.Id + "  " + this.id);
            Session?.Send(0x00, (ushort) type, bytes);
        }
        
    }
}