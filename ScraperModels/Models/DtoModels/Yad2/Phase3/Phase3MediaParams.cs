using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3MediaParams
    {
        public string CityID { get; set; }
        public string yad1City { get; set; }
        public string HomeTypeID { get; set; }
        public string AssetType { get; set; }
        public string fromRooms { get; set; }
        public string untilRooms { get; set; }
        public string fromPrice { get; set; }
        public string untilPrice { get; set; }
        public string FromFloor { get; set; }
        public string ToFloor { get; set; }
        public string FromSquareMeter { get; set; }
        public string ToSquareMeter { get; set; }
        public string SecondaryAreaID { get; set; }
        public string PrimaryAreaID { get; set; }
        public string SecondaryArea { get; set; }
        public string PrimaryArea { get; set; }
        public string searchB144FromYad2 { get; set; }
        public string tivuch { get; set; }
        public string artiMedia_iscroll_group { get; set; }
        public List<string> eratePopUnderPerIds { get; set; }
        public string abovePrice { get; set; }
        public string AppType { get; set; }
        public string mainpage { get; set; }
        public string measurements { get; set; }
        public string merchant { get; set; }
        public string catID { get; set; }
        public string subCatID { get; set; }
        public string current_zone { get; set; }
        public string location { get; set; }
        public string pageType { get; set; }
        public Phase3MediaParamsMainCategoryZones mainCategoryZones { get; set; }
        public string zonePrefix { get; set; }
        public string version { get; set; }
        public string B144_CatID { get; set; }
        public string Cohort { get; set; }
        public string userid { get; set; }
        public string isQA { get; set; }
    }
}
