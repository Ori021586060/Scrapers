using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Scale
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public Scale Half (Scale orig)
        {
            return new Scale()
            {
                Height = orig.Height / 2,
                Width = orig.Width / 2
            };
        }
    }
}
