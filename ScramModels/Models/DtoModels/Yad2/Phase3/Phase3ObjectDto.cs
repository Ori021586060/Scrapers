using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.Yad2Dto
{
    public class Phase3ObjectDto
    {
        public string No { get => "-"; }
        public List<Phase3AdditionalInfoItemsV2> additional_info_items_v2 { get; set; }
        public List<Phase3AdditionalInfo> additional_info { get; set; }
        public string additional_info_title { get; set; }
        public string address_home_number { get; set; }
        public string ad_highlight_type { get; set; }
        public string ad_number { get; set; }
        public Phase3ObjectDtoAgencyInfo agency_info { get; set; }
        public string area_id { get; set; }
        public string balconies { get; set; }
        public Phase3ButtonBar button_bar { get; set; }
        public string can_hide { get; set; }
        public string cat_id { get; set; }
        public string city_text { get; set; }
        public string contact_name { get; set; }
        public object customer_id { get; set; }
        public string date { get; set; }
        public string date_added { get; set; }
        public string date_of_entry { get; set; }
        public string date_raw { get; set; }
        public List<string> educational_info { get; set; }
        public string email { get; set; }
        public string entrance_text { get; set; }
        public string fair_rent { get; set; }
        public string furniture_info { get; set; }
        public string garden_area { get; set; }
        public string highlight_text { get; set; }
        public string HouseCommittee { get; set; }
        public string id { get; set; }
        public List<string> images { get; set; }
        public List<Phase3ImportantInfoItem> important_info_items { get; set; }
        public string important_info_title { get; set; }
        public List<Phase3InfoBarItems> info_bar_items { get; set; }
        public string info_text { get; set; }
        public string info_title { get; set; }
        public string is_liked { get; set; }
        public string item_type { get; set; }
        public string like_count { get; set; }
        public string link_token { get; set; }
        public string main_title { get; set; }
        public List<string> main_title_params { get; set; }
        public string merchant { get; set; }
        public object navigation_data { get; set; } // Phase3NavigationData
        public Phase3NavigationLinks navigation_links { get; set; }
        public string neighborhood { get; set; }
        public string note_text { get; set; }
        public string order_type_id { get; set; }
        public string payments_in_year { get; set; }
        public string price { get; set; }
        public string pricelist_link_title { get; set; }
        public string pricelist_link_url { get; set; }
        public string property_tax { get; set; }
        public string record_id { get; set; }
        public string second_title { get; set; }
        public string square_meters { get; set; }
        public string street { get; set; }
        public string subcat_id { get; set; }
        public string submit_error_link { get; set; }
        public string TotalFloor_text { get; set; }
        public string type { get; set; }
        public string uid { get; set; }
        public string updated_at { get; set; }
        public string video_url { get; set; }
        public Phase3Media media { get; set; }
        public string is_business { get; set; }
        public string favorites_userid { get; set; }
        public string HomeTypeID { get; set; }
        public string ad_number_second { get; set; }
        public string ad_item_from_memcache { get; set; }
        public Phase3MediaCategoryDic categoryDic { get; set; }
        public List<string> yad1Ads { get; set; }
        public List<string> agency_more_items { get; set; }
        public List<string> pricelist_articles { get; set; }
        public List<string> rating_area { get; set; }
        public List<string> education_info_details { get; set; }
        public List<string> education_level { get; set; }
        public List<string> busInfo { get; set; }
        public List<string> demography { get; set; }
        public Phase3ObjectContactsData ContactInfoData {get;set;}
    }
}
