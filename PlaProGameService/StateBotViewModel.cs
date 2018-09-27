using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaProGameService.Enum;

namespace PlaProGameService
{
    public class StateBotViewModel
    {
        public string Login { get; set; }
        public EBotState State { get; set; }
        public DateTime LastUsedDate { get; set; }
    }
}
