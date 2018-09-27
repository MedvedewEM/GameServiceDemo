namespace PlaProGameService.Queries
{
    public class InviteQuery
    {
        public ulong[] UserIds { get; set; }
        public uint AppId { get; set; } = 570;
        public int BotLeaveTime { get; set; } = 3 * 60 * 1000;
    }
}
