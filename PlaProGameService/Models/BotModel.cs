namespace PlaProGameService.Models
{
    using System;
    using MongoDB.Bson;
    using Enum;

    public class BotModel
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ulong SteamId { get; set; }
        public byte[] Hash { get; set; }
        public EBotState State { get; set; }
        public DateTime LastUsedDate { get; set; }
    }
}
