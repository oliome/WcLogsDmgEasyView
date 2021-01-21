using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WcLogsDmgEasyView.Models
{
    public class Player
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public long Guid { get; set; }
        public string Type { get; set; }
        public string Server { get; set; }
        public string Icon { get; set; }
        public byte[] Image { get; set; }
    }
    public class Boss
    {
        public string Name { get; set; }
        public long Id { get; set; }
    }

    public class ResponseObject
    {
        public List<Player> Players { get; set; }
        public List<Boss> Bosses { get; set; }
    }
}
