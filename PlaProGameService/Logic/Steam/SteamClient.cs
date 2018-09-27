namespace PlaProGameService.Logic.Steam
{
    using Enum;
    using Logging;
    using Queries;
    using SteamKit2;
    using SteamKit2.GC;
    using SteamKit2.GC.Dota.Internal;
    using SteamKit2.Internal;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SteamClient: ISteamClient
    {
        private const int Dota2AppId = 570;

        private readonly SteamKit2.SteamClient steamClient;
        private readonly SteamUser steamUser;
        private readonly SteamGameCoordinator gameCoordinator;
        private readonly CallbackManager manager;

        private readonly ILogger logger;
        private BotManager botManager;

        private TaskCompletionSource<bool> gameConnectedTask;
        private Queue<Task> botActionsTaskQueue;
        private CancellationTokenSource botActionsCancellationSource;
        private CancellationTokenSource waitingCallbacksCancellationSource;

        public SteamClient(ILogger logger)
        {
            steamClient = new SteamKit2.SteamClient();
            manager = new CallbackManager(steamClient);
            steamUser = steamClient.GetHandler<SteamUser>();
            gameCoordinator = steamClient.GetHandler<SteamGameCoordinator>();

            this.logger = logger;

            SubscribeOnCallbacks();
        }

        #region Private Methods

        private void SubscribeOnCallbacks()
        {
            manager.Subscribe<SteamKit2.SteamClient.ConnectedCallback>(OnConnected);
            manager.Subscribe<SteamKit2.SteamClient.DisconnectedCallback>(OnDisconnected);

            manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
            manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

            manager.Subscribe<SteamGameCoordinator.MessageCallback>(OnGCMessage);
        }

        private void WaitCallbacks()
        {
            while (!waitingCallbacksCancellationSource.IsCancellationRequested)
            {
                manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
            }
        }

        private void WaitLeavePartyBot(int botLogOffTime)
        {
            Task.Run(() =>
            {
                Thread.Sleep(botLogOffTime);
                LeaveParty();
            });
        }

        public bool WaitBotActions()
        {
            try
            {
                botActionsTaskQueue.Last().Wait();
            }
            catch (Exception e)
            {
                logger.Error("Something go wrong while bot's actions, all tasks is canceled... ", e);
            }

            return !botActionsTaskQueue.Last().IsCanceled;
        }

        private void ResetParameters()
        {
            botActionsCancellationSource.Cancel();
            waitingCallbacksCancellationSource.Cancel();

            botManager = null;

            logger.Info("All tasks are canceled and all parameters are cleared.");
        }

        #endregion

        #region Properties

        private string BotName => botManager?.GetBotLogin;
        private SteamUser.LogOnDetails LogOnParameters => botManager?.GetLogOnParameters;

        private ulong ProPlayerId { get; set; }

        #endregion

        #region Callbacks

        private void OnConnected(SteamKit2.SteamClient.ConnectedCallback obj)
        {
            logger.Info("Connection to SteamClient is successful.");

            logger.Info($"Logging on to Steam with bot '{BotName}'...");
            steamUser.LogOn(LogOnParameters);
        }

        private void OnDisconnected(SteamKit2.SteamClient.DisconnectedCallback obj)
        {
            botManager.UpdateBotState(EBotState.Free);

            logger.Info("Disconnected from SteamClient.");

            ResetParameters();
        }

        private void OnLoggedOn(SteamUser.LoggedOnCallback obj)
        {
            logger.Info($"Log on to Steam with bot {BotName} is successful.");

            var playGame = new ClientMsgProtobuf<CMsgClientGamesPlayed>(EMsg.ClientGamesPlayed);

            playGame.Body.games_played.Add(new CMsgClientGamesPlayed.GamePlayed
            {
                game_id = new GameID(Dota2AppId)
            });

            logger.Info("Hello message is sending...");
            steamClient.Send(playGame);

            logger.Info("Connecting to Game Coordinator...");
            Thread.Sleep(5000);
            logger.Info("Connection to Game Coordinator is successful (just waiting 5 seconds).");

            var clientHello = new ClientGCMsgProtobuf<CMsgClientHello>((uint)EGCBaseClientMsg.k_EMsgGCClientHello);
            clientHello.Body.engine = ESourceEngine.k_ESE_Source2;

            logger.Info("Sending Hello message...");
            gameCoordinator.Send(clientHello, Dota2AppId);
        }

        private void OnLoggedOff(SteamUser.LoggedOffCallback obj)
        {
            logger.Info($"Log off from Steam with bot {BotName} by reason: {obj.Result.ToString()}");
            if (obj.Result == EResult.LoggedInElsewhere)
            {
                logger.Error($"Error occuried with bot {BotName}. Maybe there are some problems with bot... Change bot state to 'Crashed'", new Exception());

                botManager.UpdateBotState(EBotState.Crashed);

                return;
            }

            if (obj.Result == EResult.LogonSessionReplaced)
            {
                return;
            }

            logger.Info("Disconnecting from SteamClient...");
            steamClient.Disconnect();
        }

        private void OnGCMessage(SteamGameCoordinator.MessageCallback obj)
        {
            logger.Info($"Game Coordinator message {obj.EMsg} callback is obtained.");
            var messageMap = new Dictionary<uint, Action<IPacketGCMsg>>
            {
                { ( uint ) EGCBaseClientMsg.k_EMsgGCClientWelcome, OnClientWelcome },
                { ( uint ) ESOMsg.k_ESOMsg_UpdateMultiple, OnUpdateMultiple }
            };

            if (!messageMap.TryGetValue(obj.EMsg, out var func))
                return;

            func(obj.Message);
        }

        private void OnClientWelcome(IPacketGCMsg obj)
        {
            logger.Info("Welcome to game!");
            gameConnectedTask.SetResult(true);
        }
            
        private void OnUpdateMultiple(IPacketGCMsg packetGcMsg)
        {
            logger.Info("Somebody come to/exit from party.");
            SetLeader(ProPlayerId);
        }

        #endregion

        #region Game Methods

        public void InviteToParty(InviteQuery query)
        {
            logger.Info($"Inviting to party with userIds from {string.Join(", ", query.UserIds)}:");

            foreach (var userId in query.UserIds)
            {
                var cmMsg = new ClientGCMsgProtobuf<CMsgInviteToParty>((uint)EGCBaseMsg.k_EMsgGCInviteToParty);
                cmMsg.Body.steam_id = userId;
                gameCoordinator.Send(cmMsg, Dota2AppId);
                logger.Info($"User with id {userId} is sent.");
            }

            ProPlayerId = query.UserIds.FirstOrDefault();

            WaitLeavePartyBot(query.BotLeaveTime);
        }

        public void SetLeader(ulong proUserId)
        {
            var cmMsg = new ClientGCMsgProtobuf<CMsgDOTASetGroupLeader>((uint)EDOTAGCMsg.k_EMsgClientToGCSetPartyLeader);
            cmMsg.Body.new_leader_steamid = proUserId;
            gameCoordinator.Send(cmMsg, Dota2AppId);

            logger.Info($"Trying to make pro player with id '{proUserId}' leader ... (if Pro already in party)");

            SetCoach();
        }

        public void SetCoach()
        {
            var cmMsg = new ClientGCMsgProtobuf<CMsgDOTAPartyMemberSetCoach>((uint)EDOTAGCMsg.k_EMsgGCPartyMemberSetCoach);
            cmMsg.Body.wants_coach = true;
            gameCoordinator.Send(cmMsg, Dota2AppId);

            logger.Info($"Trying to make bot '{BotName}' coach in party...");
        }

        private void LeaveParty()
        {
            var cmMsg = new ClientGCMsgProtobuf<CMsgLeaveParty>((uint)EGCBaseMsg.k_EMsgGCLeaveParty);
            gameCoordinator.Send(cmMsg, Dota2AppId);
            logger.Info($"Bot '{BotName}' is left party.");

            LogOffBot();
        }

        #endregion

        public void ConnectBot(BotManager managerBot)
        {
            botManager = managerBot;
            botManager.UpdateBotState(EBotState.Worked);

            waitingCallbacksCancellationSource = new CancellationTokenSource();
            Task.Run(() => WaitCallbacks(), waitingCallbacksCancellationSource.Token);

            botActionsCancellationSource = new CancellationTokenSource();
            gameConnectedTask = new TaskCompletionSource<bool>();

            botActionsTaskQueue = new Queue<Task> ();
            botActionsTaskQueue.Enqueue(gameConnectedTask.Task);

            logger.Info("Connecting to SteamClient...");
            steamClient.Connect();
        }

        public void LogOffBot()
        {
            logger.Info($"Logging off from Steam with bot '{BotName}'...");
            steamUser.LogOff();
            // DisconnectCallback будет вызван - все верно.
        }

        public void DoBotAction(Action<object> methodAction, object parameter)
        {
            var task = botActionsTaskQueue.Last().ContinueWith(
                    (x, y) => methodAction(y),
                    parameter,
                    botActionsCancellationSource.Token,
                    TaskContinuationOptions.DenyChildAttach,
                    TaskScheduler.Default
                );

            botActionsTaskQueue.Enqueue(task);
        }

        ~SteamClient()
        {
            ResetParameters();
        }
    }
}
