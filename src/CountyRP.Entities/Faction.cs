﻿using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CountyRP.DAO
{
    public class Faction
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string[] Ranks
        {
            get { return JsonConvert.DeserializeObject<string[]>(_Ranks); }
            set { _Ranks = JsonConvert.SerializeObject(value); }
        }
        public Models.FactionType Type { get; set; }

        [Column("Ranks")]
        public string _Ranks { get; set; }
    }
}
