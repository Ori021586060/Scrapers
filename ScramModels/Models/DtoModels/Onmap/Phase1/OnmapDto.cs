using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class OnmapDto
    {
        public MetaModel meta { get; set; }
        public List<DataRow> data { get; set; }
    }
}
