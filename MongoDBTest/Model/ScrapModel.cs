using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MongoDBTest.Model
{
    class ScrapModel
    {

        public string HostName { get; set; }
        public string HostImg { get; set; }
        public string ItemID { get; set; }
        public string Session { get; set; }
        public string CountryCode { get; set; }
        public string Country { get; set; }
        public string PromotionId { get; set; }
        public string PromotionId_LS { get; set; }
        public string PromotionName { get; set; }
        public string ProStartTime { get; set; }
        public string ProEndTime { get; set; }
        public string Pro_FS_StartTime { get; set; }
        public string Pro_FS_EndTime { get; set; }
        public string CatId { get; set; }
        public string CatName { get; set; }
        public string CatImage { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ShopID { get; set; }
        public string ShopName { get; set; }
        public string ScrapName { get; set; }
        public string ProductURL { get; set; }
        public string ImageURL { get; set; }
        public string SellerType { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string FSCategory { get; set; }
        public string FSCategoryURL { get; set; }
        public string VariationName { get; set; }
        public string VariationImageURL { get; set; }
        public string ModelID { get; set; }
        public string Model_Name { get; set; }
        public string PriceRange { get; set; }
        public string Feedback_Date { get; set; }
        public string Debugging { get; set; }
        public string TierVariations { get; set; }
        public string TierIndex { get; set; }




        public int SrNO { get; set; }
        public int Slot { get; set; }
        public int UnknownID { get; set; }
        public int ItemCount { get; set; }
        public int UNIT_SOLD { get; set; }
        public int AllocatedStock { get; set; }
        public int AvailableStock { get; set; }
        public int Var_UNIT_SOLD { get; set; }
        public int FSBalanceStock { get; set; }
        public int FSLatestSold { get; set; }
        public int FSUnitSold_1 { get; set; }
        public int FSUnitSold_2 { get; set; }
        public int FSUnitSold_3 { get; set; }
        public int FSUnitSold_4 { get; set; }
        public int FSUnitSold_5 { get; set; }
        public int FSUnitSold_6 { get; set; }
        public int FSUnitSold_7 { get; set; }
        public int FSUnitSold_8 { get; set; }
        public int FSUnitSold_9 { get; set; }
        public int FSUnitSold_10 { get; set; }
        public int FSUnitSold_11 { get; set; }
        public int FSUnitSold_12 { get; set; }
        public int FSUnitSold_13 { get; set; }
        public int VariationStock { get; set; }
        public int Variation_Allocated_Stock { get; set; }
        public int VariationUnitSold { get; set; }
        public int VariationBal { get; set; }
        public int MonthlySold { get; set; }
        public int TotalSold { get; set; }
        public int Rating { get; set; }
        public int SlotInfo { get; set; }
        public int Stock { get; set; }
        public int Estimated_Days { get; set; }
        public int IsPreOrder { get; set; }
        public int SoldOutTime { get; set; }
        public int FSTotalSold { get; set; }
        public int IsFsInfo { get; set; }
        public int IsFsEligible { get; set; }
        public int RescrapPending { get; set; }





        public decimal PriceMin { get; set; }
        public decimal PriceMax { get; set; }
        public decimal PriceSlashMin { get; set; }
        public decimal PriceSlashMax { get; set; }
        public decimal PriceSlash { get; set; }
        public decimal PriceFS { get; set; }
        public decimal FSRevenue { get; set; }
        public decimal SKUAvgPrice { get; set; }
        public decimal SkuModelAvgPrice { get; set; }
        public decimal Star { get; set; }
        public decimal UnixCTime { get; set; }
        public decimal Feedback_Ratio { get; set; }
        public decimal VariationPrice { get; set; }
        public decimal VariationPriceSlash { get; set; }
        public decimal VariationRevenue { get; set; }
        public decimal VariationAvgPrice { get; set; }
        public decimal Model_Price { get; set; }
        public decimal MonthlyRevenue { get; set; }


        public DateTime ProductCreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime FSSlotEndTime { get; set; }
        public DateTime FSSlotStartTime { get; set; }

        public DateTime FS_StartTime { get; set; }
        public DateTime FS_EndTime { get; set; }
       
        public DateTime FSSlotTM_1 { get; set; }
        public DateTime FSSlotTM_2 { get; set; }
        public DateTime FSSlotTM_3 { get; set; }
        public DateTime FSSlotTM_4 { get; set; }
        public DateTime FSSlotTM_5 { get; set; }
        public DateTime FSSlotTM_6 { get; set; }
        public DateTime FSSlotTM_7 { get; set; }
        public DateTime FSSlotTM_8 { get; set; }
        public DateTime FSSlotTM_9 { get; set; }
        public DateTime FSSlotTM_10 { get; set; }
        public DateTime FSSlotTM_11 { get; set; }
        public DateTime FSSlotTM_12 { get; set; }
        public DateTime FSSlotTM_13 { get; set; }
        public TimeSpan FSSlotTimeDiff { get; set; }

        public bool FSSoldOutStatus { get; set; }
    }

}

