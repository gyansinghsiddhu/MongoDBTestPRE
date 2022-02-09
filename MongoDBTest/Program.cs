using MongoDBTest.Model;
using MongoDBTest.DAL;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;



namespace MongoDBTest
{
    class Program
    {

        private static IConfiguration _iconfiguration;

        static void Main(string[] args)
        {


            if (args.Length == 0)
            {
                args = new string[1];
                args[0] = "th";
                Scrapping("TH");
            }

            switch (args[0].ToString())
            {
                case "th":
                    Scrapping("TH");
                    break;
                case "id":
                    Scrapping("ID");
                    break;
                case "vi":
                    Scrapping("VN");
                    break;
                case "my":
                    Scrapping("MY");
                    break;
                case "ph":
                    Scrapping("PH");
                    break;
                case "sg":
                    Scrapping("SG");
                    break;


            }


        }



        static void Scrapping(string Country )
        {


            GetAppSettingsFile();
            var sDAL = new ScrapDAL(_iconfiguration); 
            ScrapModel sModel = new ScrapModel();
            //BsonModel bModel = new BsonModel();

            if (Country == "TH") { sModel.Country = "TH"; sModel.HostName = "https://shopee.co.th/"; sModel.ImageURL = "https://cf.shopee.co.th/file/"; }
            else if (Country == "ID") { sModel.Country = "ID"; sModel.HostName = "https://shopee.co.id/"; sModel.ImageURL = "https://cf.shopee.co.id/file/"; }
            else if (Country == "MY") { sModel.Country = "MY"; sModel.HostName = "https://shopee.com.my/"; sModel.ImageURL = "https://cf.shopee.com.my/file/"; }
            else if (Country == "VN") { sModel.Country = "VN"; sModel.HostName = "https://shopee.vn/"; sModel.ImageURL = "https://cf.shopee.vn/file/"; }
            else if (Country == "PH") { sModel.Country = "PH"; sModel.HostName = "https://shopee.ph/"; sModel.ImageURL = "https://cf.shopee.ph/file/"; }
            else if (Country == "SG") { sModel.Country = "SG"; sModel.HostName = "https://shopee.sg/"; sModel.ImageURL = "https://cf.shopee.sg/file/"; }


           // var dbClient = new MongoClient("mongodb://myUserAdmin:1qazXSW%40@168.138.189.225:27727/?authSource=admin&readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false");
            var dbClient = new MongoClient(sDAL.CnnStrMongo);
           

            IMongoDatabase db = dbClient.GetDatabase("owl_"+ sModel.Country);
            var Coll_SessionInfo = db.GetCollection<BsonDocument>("promotion");
            var Coll_SkuPre = db.GetCollection<BsonDocument>("sku_pre");
            var Coll_SkuPost = db.GetCollection<BsonDocument>("sku_post");

            for (; ; )
            {
                try
                {
                    //string search = "220119";
                    var builder = Builders<BsonDocument>.Filter;
                    // var Sessionfilter = builder.Regex("scrap_name", "^" + search + ".*");
                    //var Sessionfilter = builder.Eq("sql_status", 0) & builder.Regex("scrap_name", "^" + search + ".*"); 
                    var Sessionfilter = builder.Eq("sql_status", 0); // & builder.Gt("scrap_name", "220127_2359"); 
                    var Session_docs = Coll_SessionInfo.Find(Sessionfilter).ToList();
                    foreach (BsonDocument Session_doc in Session_docs)
                    {

                        sModel.Country = Session_doc["country_code"].ToString();
                        sModel.PromotionId = Session_doc["promotionid"].ToString();
                        sModel.Pro_FS_StartTime = Session_doc["pre_scrap"]["unix_start_time"].ToString();
                        sModel.Pro_FS_EndTime = Session_doc["pre_scrap"]["unix_end_time"].ToString();
                        sModel.FS_StartTime = Convert.ToDateTime(Session_doc["start_time"]);
                        sModel.FS_EndTime = Convert.ToDateTime(Session_doc["end_time"]);
                        sModel.PromotionName = Session_doc["name"].ToString();
                        sModel.ProStartTime = Session_doc["unix_start_time"].ToString();
                        sModel.ProEndTime = Session_doc["unix_end_time"].ToString();
                        sModel.ScrapName = Session_doc["scrap_name"].ToString();
                        sModel.ItemCount = Convert.ToInt32(Session_doc["fs_total_item_scrap"]);
                        if (Session_doc["post_scrap_count"].IsBsonNull == true) sModel.Slot = 0; else sModel.Slot = Convert.ToInt32(Session_doc["post_scrap_count"]);

                        if (IsRecordExistProInfo(sModel.PromotionId, sModel.Country) == false)
                            SavePromotionInfo(sModel.PromotionId, sModel.PromotionName, sModel.ProStartTime, sModel.ProEndTime, sModel.Country, ConvertUnixToLocal(sModel.Country,sModel.ProStartTime), ConvertUnixToLocal(sModel.Country,sModel.ProEndTime) , getCurrentTime(sModel.Country), sModel.ItemCount, " " , ConvertUnixToLocal(sModel.Country, sModel.Pro_FS_StartTime) , ConvertUnixToLocal(sModel.Country, sModel.Pro_FS_EndTime));



                        //   SET ALL VALUE zero or Blank for SKU TABLE 
                        sModel.ShopID = ""; sModel.ItemID = "";
                        sModel.SellerType = ""; sModel.UnixCTime = 0; sModel.ProductName = "";
                        sModel.Star = 0; sModel.Rating = 0; sModel.TotalSold = 0;
                        sModel.MonthlySold = 0; sModel.CatId = ""; sModel.CatName = "";
                        sModel.PriceSlashMin = 0; sModel.PriceSlashMax = 0;
                        sModel.PriceMin = 0; sModel.PriceMax = 0; sModel.PriceRange = "0";
                        sModel.Stock = 0; sModel.Category1 = ""; sModel.Category2 = ""; sModel.Category3 = "";
                        sModel.ImageURL = ""; sModel.MonthlyRevenue = 0;
                        sModel.ProductURL = ""; sModel.IsPreOrder = 0; sModel.Estimated_Days = 0;
                        sModel.TierVariations = ""; sModel.PriceFS = 0; sModel.IsFsInfo = 0;
                        sModel.FSLatestSold = 0; sModel.VariationBal = 0; sModel.IsFsEligible = 1; sModel.RescrapPending = 0;

                        var PreScrapfilter = builder.Eq("promotionid", Convert.ToDouble(sModel.PromotionId)) & builder.Eq("sql_status", 0) & builder.Ne("catid", 100642) & builder.Ne("catid", 102053);
                        var Pre_docs = Coll_SkuPre.Find(PreScrapfilter).ToList();

                        foreach (BsonDocument Pre_doc in Pre_docs)
                        {
                            try
                            {
                                sModel.PromotionId = Pre_doc["promotionid"].ToString();
                                sModel.ShopID = Pre_doc["shopid"].ToString();
                                sModel.ItemID = Pre_doc["itemid"].ToString();

                                try { sModel.SellerType = Pre_doc["cb_option"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 11
                                try { sModel.UnixCTime = Convert.ToDecimal(Pre_doc["ctime"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }    // 12
                                try { sModel.ProductName = Pre_doc["item_name"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 13
                                try { sModel.Star = Math.Round(Convert.ToDecimal(Pre_doc["item_rating"]["rating_star"]), 2); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } //14
                                try { sModel.Rating = Convert.ToInt32(Pre_doc["item_rating"]["rating_count"][0]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  // 15
                                try { sModel.TotalSold = Convert.ToInt32(Pre_doc["historical_sold"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  //16
                                try { sModel.MonthlySold = Convert.ToInt32(Pre_doc["monthly_sold"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }   // 17
                                try { sModel.CatId = Pre_doc["fs_catid"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 18
                                try { sModel.CatId = Pre_doc["categories"][0]["catid"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 18
                                try { sModel.CatName = Pre_doc["fs_catname"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 18
                                try { sModel.PriceSlashMin = Convert.ToDecimal(Pre_doc["price_min_before_discount"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  // 19
                                try { sModel.PriceSlashMax = Convert.ToDecimal(Pre_doc["price_max_before_discount"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 20
                                try { sModel.PriceMin = Convert.ToDecimal(Pre_doc["price_min"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }// 21
                                try { sModel.PriceMax = Convert.ToDecimal(Pre_doc["price_max"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 22
                                try { sModel.Stock = Convert.ToInt32(Pre_doc["stock"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  // 24



                                var allCatg = Pre_doc["categories"].AsBsonArray;
                                int catcnt = 0;
                                foreach (var cat in allCatg)
                                {
                                    catcnt++;
                                    try { if (catcnt == 1) sModel.Category1 = cat[1].ToString(); }  // 25
                                    catch { sModel.Category1 = "N/A"; }
                                    try { if (catcnt == 2) sModel.Category2 = cat[1].ToString(); }  // 26
                                    catch { sModel.Category2 = "N/A"; }
                                    try { if (catcnt == 3) sModel.Category3 = cat[1].ToString(); }   // 27
                                    catch { sModel.Category3 = "N/A"; }
                                }

                                try { sModel.ImageURL = sModel.HostImg + "" + Pre_doc["image"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }// 28
                                try { sModel.MonthlyRevenue = 0; } catch (Exception ex) { Console.WriteLine(ex.ToString()); } //  29 Need To Modify 
                                try { sModel.ProductURL = sModel.HostName + "--i." + sModel.ShopID + "." + sModel.ItemID; } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  // 30
                                try { sModel.IsPreOrder = Convert.ToInt32(Pre_doc["is_pre_order"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  //31
                                try { sModel.Estimated_Days = Convert.ToInt32(Pre_doc["estimated_days"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } //32
                                try { sModel.ShopName = Pre_doc["shop_name"].ToString(); } catch { }
                                
                                try { sModel.IsFsInfo = Convert.ToInt32(Pre_doc["is_fs_eligible"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  //31
                                //if (sModel.IsFsInfo == 0) sModel.IsFsInfo = 1; else sModel.IsFsInfo = 0;

                                // ADD NEW COLUMNS
                                try { sModel.IsFsEligible = Convert.ToInt32(Pre_doc["is_fs_eligible"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  //31
                                try { sModel.RescrapPending = Convert.ToInt32(Pre_doc["rescrap_pending"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  //31



                                if (IsRecordExist_ShopDATA(sModel.Country, sModel.ShopID) == false)
                                    SaveShopData(sModel.Country, sModel.ShopID, sModel.ShopName, getCurrentTime(sModel.Country));



                                if (IsRecordExistPreScrap(sModel.PromotionId, sModel.Country, sModel.ShopID, sModel.ItemID) == false)
                                    SavePreScrap(sModel.Country, sModel.PromotionId, sModel.ShopID, sModel.ItemID, getCurrentTime(sModel.Country).ToString(), -1, 0, Convert.ToInt32(sModel.SellerType), sModel.UnixCTime.ToString(), sModel.ProductName, sModel.Star, sModel.Rating, sModel.TotalSold, sModel.MonthlySold, sModel.CatId, sModel.PriceSlashMin, sModel.PriceSlashMax, sModel.PriceMin, sModel.PriceMax, 0, sModel.Stock, sModel.Category1, sModel.Category2, sModel.Category3, sModel.ImageURL, sModel.MonthlyRevenue, sModel.ProductURL, sModel.IsPreOrder, sModel.Estimated_Days, sModel.TierVariations, sModel.IsFsInfo, sModel.RescrapPending, sModel.IsFsEligible);
                                else
                                    UpdatePreScrap(sModel.Country, sModel.PromotionId, sModel.ShopID, sModel.ItemID, getCurrentTime(sModel.Country).ToString(), -1, 0, Convert.ToInt32(sModel.SellerType), sModel.UnixCTime.ToString(), sModel.ProductName, sModel.Star, sModel.Rating, sModel.TotalSold, sModel.MonthlySold, sModel.CatId, sModel.PriceSlashMin, sModel.PriceSlashMax, sModel.PriceMin, sModel.PriceMax, 0, sModel.Stock, sModel.Category1, sModel.Category2, sModel.Category3, sModel.ImageURL, sModel.MonthlyRevenue, sModel.ProductURL, sModel.IsPreOrder, sModel.Estimated_Days, sModel.TierVariations, sModel.IsFsInfo, sModel.RescrapPending, sModel.IsFsEligible);


                                sModel.ModelID = "";
                                sModel.VariationName = "";
                                sModel.VariationPriceSlash = 0;
                                sModel.VariationPrice = 0;
                                sModel.VariationStock = 0;
                                sModel.VariationUnitSold = 0;
                                sModel.TierIndex = "";
                                sModel.VariationImageURL = "";


                                // VARIATION DETAILS ...........................

                                var allModels = Pre_doc["models"].AsBsonArray;
                                foreach (var model in allModels)
                                {
                                    try
                                    {

                                        try { sModel.ModelID = model["modelid"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }  // 38
                                        try { sModel.VariationName = model["name"].ToString(); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }    // 39
                                        try { sModel.VariationPriceSlash = Convert.ToDecimal(model["price_before_discount"]); ; } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 40
                                        try { sModel.VariationPrice = Convert.ToDecimal(model["price"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 41
                                        try { sModel.VariationStock = Convert.ToInt32(model["stock"]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); } // 42
                                        try { sModel.TierIndex = Newtonsoft.Json.JsonConvert.SerializeObject(model["extinfo"]["tier_index"][0]); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                                        try
                                        {
                                            int nso = Convert.ToInt32(model["extinfo"]["tier_index"][0]);
                                            sModel.VariationImageURL = sModel.HostImg + Pre_doc["tier_variations"][0]["images"][nso];  // 43

                                        }
                                        catch
                                        {
                                            sModel.VariationImageURL = sModel.HostImg + "" + Pre_doc["image"].ToString();    // 43

                                        }


                                        if (IsRecordExist_Sku_model(sModel.PromotionId, sModel.Country, sModel.ModelID, sModel.ItemID) == false)
                                            SaveSku_model(sModel.Country, sModel.PromotionId, sModel.ModelID, sModel.ItemID, sModel.VariationName, sModel.VariationPriceSlash, sModel.VariationPrice, sModel.VariationStock, sModel.VariationImageURL, 0, 0, sModel.TierIndex);
                                        else
                                            UpdateSku_model(sModel.Country, sModel.PromotionId, sModel.ModelID, sModel.ItemID, sModel.VariationName, sModel.VariationPriceSlash, sModel.VariationPrice, sModel.VariationStock, sModel.VariationImageURL, 0, 0, sModel.TierIndex);

                                    }
                                    catch { continue; }

                                    sModel.ModelID = "";
                                    sModel.VariationName = "";
                                    sModel.VariationPriceSlash = 0;
                                    sModel.VariationPrice = 0;
                                    sModel.VariationStock = 0;
                                    sModel.VariationUnitSold = 0;
                                    sModel.TierIndex = "";
                                    sModel.VariationImageURL = "";

                                }



                                // UPDATE SQL STATUS
                                var PreScrapItemfilter = builder.Eq("promotionid", Convert.ToInt64(sModel.PromotionId)) & builder.Eq("shopid", Convert.ToInt32(sModel.ShopID)) & builder.Eq("itemid", Convert.ToInt64(sModel.ItemID));
                                var updateItemSqlStatus = Builders<BsonDocument>.Update.Set("sql_status", 1);
                                var result = Coll_SkuPre.UpdateOne(PreScrapItemfilter, updateItemSqlStatus);

                                if (sModel.RescrapPending == 1)
                                {
                                    //  OPEN PROMOTION FOR RESCRAPPING (Post SLOT)
                                    var SessionProIdfilter = builder.Eq("promotionid", Convert.ToInt64(sModel.PromotionId));
                                    var updateSesionSqlStatus = Builders<BsonDocument>.Update.Set("sql_status", 0);
                                    var resultSession = Coll_SessionInfo.UpdateOne(SessionProIdfilter, updateSesionSqlStatus);
                                    //  OPEN POST SKU & SLOT FOR RESCRAPPING (Post SLOT)
                                    var PostScrapItemfilter = builder.Eq("promotionid", Convert.ToInt64(sModel.PromotionId)) & builder.Eq("shopid", Convert.ToInt32(sModel.ShopID)) & builder.Eq("itemid", Convert.ToInt64(sModel.ItemID)) & builder.Eq("slot", Convert.ToInt32(sModel.Slot)) & builder.Eq("is_fs_eligible", true);
                                    var updatePostItemSqlStatus = Builders<BsonDocument>.Update.Set("sql_status", 0); 
                                    var resultPost = Coll_SkuPost.UpdateOne(PostScrapItemfilter, updatePostItemSqlStatus);

                                    //  UPDATE RESCRAPPING STATUS  in PRE COLLECTION 
                                    var updateItemRescap = Builders<BsonDocument>.Update.Set("rescrap_pending", false);
                                    var resultPre = Coll_SkuPre.UpdateOne(PreScrapItemfilter, updateItemRescap);
                                }


                            }
                            catch { continue; }

                        }

                    }

                    System.Threading.Thread.Sleep(40000);

                }
                 
                catch
                {
                continue;
                }

        }
           

        }




       


        static void GetAppSettingsFile()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _iconfiguration = builder.Build();


        }

        static DateTime ConvertUnixToLocal(string Country, string UnixTime)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.ConvertUnixToLocal(Country, UnixTime);
        }

        static DateTime getCurrentTime(string Country)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.getCurrentTime(Country);
        }

        static bool IsRecordExistProInfo(string promid, string Cntry)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.IsRecordExistProInfo(promid, Cntry);
        }


        static bool IsRecordExist_ShopDATA(string Cntry, string SHID)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.IsRecordExist_ShopDATA(Cntry, SHID);
        }


        static void SaveShopData(string cntry, string SHID, string shpName, DateTime ModefiedDate)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.SaveShopData(cntry, SHID, shpName, ModefiedDate);
        }

        static void SavePromotionInfo(string PromotionID, string PromotionName, string Start_Time, string End_Time, string Country, DateTime FSStartTime, DateTime FSEndTime, DateTime ModifiedDate, int ItemCount, string JsonData, DateTime Pro_FSStartTime, DateTime Pro_FSEndTime)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.SavePromotionInfo(PromotionID, PromotionName, Start_Time, End_Time, Country, FSStartTime, FSEndTime, ModifiedDate, ItemCount, JsonData, Pro_FSStartTime,  Pro_FSEndTime);
        }

        static bool IsRecordExistPreScrap(string PromotionID, string Cntry, string Shopid, string prodid)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.IsRecordExistPreScrap(PromotionID, Cntry, Shopid, prodid);
        }
        
        static void SavePreScrap(string Country, string PromoID, string ShopID, string ProdID, string STRTTIME, int Userid, int ScrapType, int CbOption, string CTIME, string PRODUCT_NAME, Decimal STAR, int RATING, int TOTAL_SOLD, int MONTHLY_SOLD, string CatID, Decimal PRICE_SLASH_MIN, Decimal PRICE_SLASH_MAX, Decimal M_PRICE, Decimal MX_PRICE, Decimal AvgPrice, int Stock, string CAT_NAME_1, string CAT_NAME_2, string CAT_NAME_3, string ImgUrl, Decimal MonthlyRevenue, string LINK_SKU, int IsPreOrder, int EstimatedDays, string TierVarJson , int IsFSInfo , int RescrapPending, int IsFsEligible)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.SavePreScrap(Country, PromoID, ShopID, ProdID, STRTTIME, Userid, ScrapType, CbOption, CTIME, PRODUCT_NAME, STAR, RATING, TOTAL_SOLD, MONTHLY_SOLD, CatID,  PRICE_SLASH_MIN,  PRICE_SLASH_MAX,  M_PRICE,  MX_PRICE, AvgPrice, Stock, CAT_NAME_1, CAT_NAME_2, CAT_NAME_3, ImgUrl, MonthlyRevenue, LINK_SKU, IsPreOrder, EstimatedDays, "", IsFSInfo , RescrapPending, IsFsEligible);
        }

        static void UpdatePreScrap(string Country, string PromoID, string ShopID, string ProdID, string STRTTIME, int Userid, int ScrapType, int CbOption, string CTIME, string PRODUCT_NAME, Decimal STAR, int RATING, int TOTAL_SOLD, int MONTHLY_SOLD, string CatID, Decimal PRICE_SLASH_MIN, Decimal PRICE_SLASH_MAX, Decimal M_PRICE, Decimal MX_PRICE, Decimal AvgPrice, int Stock, string CAT_NAME_1, string CAT_NAME_2, string CAT_NAME_3, string ImgUrl, Decimal MonthlyRevenue, string LINK_SKU, int IsPreOrder, int EstimatedDays, string TierVarJson, int IsFSInfo , int RescrapPending, int IsFsEligible)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.UpdatePreScrap(Country, PromoID, ShopID, ProdID, STRTTIME, Userid, ScrapType, CbOption, CTIME, PRODUCT_NAME, STAR, RATING, TOTAL_SOLD, MONTHLY_SOLD, CatID, PRICE_SLASH_MIN, PRICE_SLASH_MAX, M_PRICE, MX_PRICE, AvgPrice, Stock, CAT_NAME_1, CAT_NAME_2, CAT_NAME_3, ImgUrl, MonthlyRevenue, LINK_SKU, IsPreOrder, EstimatedDays, "", IsFSInfo, RescrapPending, IsFsEligible);
        }



        static bool IsRecordExist_Sku_model(string PromotionID, string Cntry, string Modelid, string prodid)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            return ScrapDAL.IsRecordExist_Sku_model(PromotionID, Cntry, Modelid, prodid);
        }

        static void SaveSku_model(string Country, string PromoID, string ModelID, string ProdID, string VarName, Decimal VarPriceSlash, Decimal VarPrice, int VarStock, string VarImage, Decimal Revenue, Decimal AvgPrice, string TierIndexJson)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.SaveSku_model(Country,PromoID, ModelID, ProdID, VarName, VarPriceSlash,  VarPrice, VarStock,  VarImage, Revenue, AvgPrice, TierIndexJson);

        }

        static void UpdateSku_model(string Country, string PromoID, string ModelID, string ProdID, string VarName, Decimal VarPriceSlash, Decimal VarPrice, int VarStock, string VarImage, Decimal Revenue, Decimal AvgPrice, string TierIndexJson)
        {
            var ScrapDAL = new ScrapDAL(_iconfiguration);
            ScrapDAL.UpdateSku_modelSku_model(Country, PromoID, ModelID, ProdID, VarName, VarPriceSlash, VarPrice, VarStock, VarImage, Revenue, AvgPrice, TierIndexJson);

        }



    }

}
