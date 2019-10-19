﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.HomeLess
{
    public class CoordinateDtoModel
    {
        public string place_id { get; set; }
        public string licence { get; set; }
        public string osm_type { get; set; }
        public string osm_id { get; set; }
        public List<string> boundingbox { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string display_name { get; set; }
        public string @class { get; set; }
        public string type { get; set; }
		public string importance { get; set; }
}
}
