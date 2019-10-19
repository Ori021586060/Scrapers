using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models
{
    public class ResponseParseUrlOnAirdna
    {
        public ResponseStateModel State { get; set; } = new ResponseStateModel();
        public AirdnaParseModel City { get; set; } = new AirdnaParseModel();
    }
}
