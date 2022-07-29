using System;
using System.Collections.Generic;

#nullable disable

namespace Shop_DT.Models
{
    public partial class Slider
    {
        public int SliderId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
    }
}
