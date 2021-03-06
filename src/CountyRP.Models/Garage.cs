﻿namespace CountyRP.Models
{
    public class Garage
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public float[] EntrancePosition { get; set; }
        public uint EntranceDimension { get; set; }
        public float EntranceRotation { get; set; }
        public uint ExitDimension { get; set; }
        public bool Lock { get; set; }
    }
}
