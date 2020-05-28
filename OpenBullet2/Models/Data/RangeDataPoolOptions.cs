﻿namespace OpenBullet2.Models.Data
{
    public class RangeDataPoolOptions : DataPoolOptions
    {
        public int Start { get; set; } = 0;
        public int Amount { get; set; } = 100;
        public int Step { get; set; } = 1;
        public bool Pad { get; set; } = false;
    }
}
