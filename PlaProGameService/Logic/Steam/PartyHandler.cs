namespace PlaProGameService.Logic.Steam
{
    using SteamKit2;

    public class PartyHandler: ClientMsgHandler
    {
        public class MyCallback : CallbackMsg
        {
            public EResult Result { get; private set; }
            
            internal MyCallback(EResult res)
            {
                Result = res;
            }
        }

        public override void HandleMsg(IPacketMsg packetMsg)
        {
            switch (packetMsg.MsgType)
            {

                // we want to custom handle this message, for the sake of an example
                case EMsg.ClientLogOnResponse:
                    HandleLogonResponse(packetMsg);
                    break;

            }
        }

        void HandleLogonResponse(IPacketMsg packetMsg)
        {
            //var logonResponse = new ClientMsgProtobuf<>(packetMsg);
            
            //EResult result = (EResult)logonResponse.Body.eresult;
            
            //Console.WriteLine("HandleLogonResponse: {0}", result);
            
            //Client.PostCallback(new MyCallback(result));
        }
    }
}
