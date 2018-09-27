namespace PlaProGameService.Logic.Steam
{
    using System.Collections.Generic;

    public class SteamProfilesResult
    {
        public Response response { get; set; }

        public class Response
        {
            public List<Profile> players { get; set; }
        }

        public class Profile
        {
            public string steamid { get; set; }
            public string communityvisibilitystate { get; set; }
            public string profilestate { get; set; }
            public string personaname { get; set; }
            public string lastlogoff { get; set; }
            public string commentpermission { get; set; }
            public string profileurl { get; set; }
            public string avatar { get; set; }
            public string avatarmedium { get; set; }
            public string avatarfull { get; set; }
            public string personastate { get; set; }
            public string primaryclanid { get; set; }
            public string timecreated { get; set; }
            public string personastateflags { get; set; }
            public string gameextrainfo { get; set; }
            public string gameid { get; set; }
            public string lobbysteamid { get; set; }
        }
    }
}
