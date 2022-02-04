using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using MongoDBTest.Model;


namespace MongoDBTest.DAL
{
    class ScrapDAL
    {
        private string CnnStr;
        public string CnnStrMongo;


        public ScrapDAL(IConfiguration iconfiguration)
        {
            CnnStr = iconfiguration.GetConnectionString("Default");
            CnnStrMongo = iconfiguration.GetConnectionString("Mongo");

        }



        SqlConnection cnn = new SqlConnection();
        SqlTransaction tran;

        // CONVERT  UNIX TO LOCAL  DATETIME

        public DateTime ConvertUnixToLocal(string Country, string UnixTime)
        {
            DateTimeOffset offset1 = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(UnixTime));
            if (Country == "MY" || Country == "PH" || Country == "SG")
            {
                return offset1.DateTime.ToLocalTime().AddHours(1);

            }

            else if (Country == "TH" || Country == "ID" || Country == "VN")
            {
                return offset1.DateTime.ToLocalTime();

            }
            return DateTime.Now;
        }


        public DateTime getCurrentTime(string Country)

        {
            if (Country == "MY" || Country == "PH" || Country == "SG")
            {
                return DateTime.Now.AddHours(1);
            }
            else
            {
                return DateTime.Now;
            }

        }


        /// SAVE PROMOTOIN DATA INTO DATA BASE  //////////////////////////

        public bool IsRecordExistProInfo(string promid, string Cntry)
        {
            try
            {
                string Qry;
                DataTable dt = new DataTable();
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                Qry = "Select * from fs_promotion where  promotionid = '" + promid + "' and country_code= '" + Cntry + "'";
                SqlDataAdapter Sqldbda = new SqlDataAdapter(Qry, cnn);
                Sqldbda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cnn.Close();
                    return true;
                }
                else
                {
                    cnn.Close();
                    return false;
                }
            }
            catch
            {
                cnn.Close();
                return false;
            }
        }

        public void SavePromotionInfo(string PromotionID, string PromotionName, string Start_Time, string End_Time, string Country, DateTime FSStartTime, DateTime FSEndTime, DateTime ModifiedDate, int ItemCount, string JsonData, DateTime Pro_FSStartTime, DateTime Pro_FSEndTime)
        {

            cnn.ConnectionString = CnnStr;
            try
            {
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                tran = cnn.BeginTransaction();

                string Qry = "Insert into fs_promotion(country_code,promotionid,start_time, end_time, name, description, unix_start_time, unix_end_time, item_count, scrap_start_time, scrap_end_time, modified_date, json_data ) values (@country_code, @promotionid, @start_time, @end_time, @name, @description, @unix_start_time, @unix_end_time, @item_count, @scrap_start_time, @scrap_end_time, @modified_date, @json_data)";

                SqlCommand Comm2 = new SqlCommand(Qry, cnn);

                Comm2.Parameters.Add("@country_code", SqlDbType.NVarChar, 3).Value = Country;
                Comm2.Parameters.Add("@promotionid", SqlDbType.BigInt).Value = Convert.ToInt64(PromotionID);
                Comm2.Parameters.Add("@start_time", SqlDbType.DateTime).Value = FSStartTime;
                Comm2.Parameters.Add("@end_time", SqlDbType.DateTime).Value = FSEndTime;
                Comm2.Parameters.Add("@name", SqlDbType.NVarChar, 255).Value = PromotionName;
                Comm2.Parameters.Add("@description", SqlDbType.NVarChar).Value = PromotionName;
                Comm2.Parameters.Add("@unix_start_time", SqlDbType.BigInt).Value = Convert.ToInt64(Start_Time);
                Comm2.Parameters.Add("@unix_end_time", SqlDbType.BigInt).Value = Convert.ToInt64(End_Time);
                Comm2.Parameters.Add("@item_count", SqlDbType.BigInt).Value = ItemCount;
                Comm2.Parameters.Add("@scrap_start_time", SqlDbType.DateTime).Value = Pro_FSStartTime;  // REVIEW 
                Comm2.Parameters.Add("@scrap_end_time", SqlDbType.DateTime).Value = Pro_FSEndTime;    // REVIEW
                Comm2.Parameters.Add("@modified_date", SqlDbType.DateTime).Value = ModifiedDate;
                Comm2.Parameters.Add("@json_data", SqlDbType.NVarChar).Value = JsonData;

                Comm2.Transaction = tran;
                Comm2.CommandTimeout = 0;
                Comm2.ExecuteNonQuery();
                Comm2.Dispose();
                tran.Commit();
                cnn.Close();
                Comm2.Parameters.Clear();
            }
            catch (SqlException ex)
            {
                tran.Rollback();
                cnn.Close();

            }
            catch (Exception ex)
            {
                tran.Rollback();
                cnn.Close();
            }



        }

        /// SAVE  SKU  and SKU MODEL DATA INTO DATA BASE  //////////////////////////

        public bool IsRecordExistPreScrap(string PromotionID, string Cntry, string Shopid, string prodid)
        {
            try
            {
                string Qry;
                DataTable dt = new DataTable();
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                Qry = "Select * from sku where country_code = '" + Cntry + "' and ShopId ='" + Shopid + "' and itemId ='" + prodid + "' and promotionid = '" + PromotionID + "' ";
                SqlDataAdapter Sqldbda = new SqlDataAdapter(Qry, cnn);
                Sqldbda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cnn.Close();
                    return true;
                }
                else
                {
                    cnn.Close();
                    return false;
                }
            }
            catch
            {
                cnn.Close();
                return false;
            }
        }

        public bool IsRecordExist_Sku_model(string PromotionID, string Cntry, string Modelid, string prodid)
        {
            try
            {
                string Qry;
                DataTable dt = new DataTable();
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                Qry = "Select * from sku_model where country_code = '" + Cntry + "' and modelid ='" + Modelid + "' and itemId ='" + prodid + "' and promotionid = '" + PromotionID + "' ";
                SqlDataAdapter Sqldbda = new SqlDataAdapter(Qry, cnn);
                Sqldbda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cnn.Close();
                    return true;
                }
                else
                {
                    cnn.Close();
                    return false;
                }
            }
            catch
            {
                cnn.Close();
                return false;
            }
        }

        public bool IsRecordExist_ShopDATA(string Cntry, string SHID)
        {
            try
            {
                string Qry;
                DataTable dt = new DataTable();
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                Qry = "Select * from shop where shopid = '" + SHID + "' and country_code = '" + Cntry + "'";
                SqlDataAdapter Sqldbda = new SqlDataAdapter(Qry, cnn);
                Sqldbda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    cnn.Close();
                    return true;
                }

                else
                {
                    cnn.Close();
                    return false;
                }

            }
            catch
            {
                cnn.Close();
                return false;
            }
        }


        public void SavePreScrap(string Country, string PromoID, string ShopID, string ProdID, string STRTTIME, int Userid, int ScrapType, int CbOption, string CTIME, string PRODUCT_NAME, Decimal STAR, int RATING, int TOTAL_SOLD, int MONTHLY_SOLD, string CatID, Decimal PRICE_SLASH_MIN, Decimal PRICE_SLASH_MAX, Decimal M_PRICE, Decimal MX_PRICE, Decimal AvgPrice, int Stock, string CAT_NAME_1, string CAT_NAME_2, string CAT_NAME_3, string ImgUrl, Decimal MonthlyRevenue, string LINK_SKU, int IsPreOrder, int EstimatedDays, string TierVarJson, int IsFsInfo)
        {
            try
            {
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                tran = cnn.BeginTransaction();
                string Qry = "Insert into sku (country_code,	promotionid,	shopid,	itemid,	modified_date,	scrap_type,	cb_option,	ctime,	name,	rating_star,	rating_count,	historical_sold,	monthly_sold,	catid,	price_slash_min,	price_slash_max,	price_min,	price_max,	avg_price,	stock,	catname1,	catname2,	catname3,	image,	monthly_revenue,	product_link,	is_pre_order,	estimated_days,tier_variations , is_fs_info)" +
                "values (@country_code,	@promotionid,	@shopid,	@itemid,	@modified_date,		@scrap_type,	@cb_option,	@ctime,	@name,	@rating_star,	@rating_count,	@historical_sold,	@monthly_sold,	@catid,	@price_slash_min,	@price_slash_max,	@price_min,	@price_max,	@avg_price,	@stock,	@catname1,	@catname2,	@catname3,	@image,	@monthly_revenue,	@product_link,	@is_pre_order,	@estimated_days,@tier_variations,@is_fs_info)";

                Decimal PriceDiv = Convert.ToDecimal(100000.0);

                SqlCommand Comm2 = new SqlCommand(Qry, cnn);
                Comm2.Parameters.Add("@country_code", SqlDbType.VarChar, 3).Value = Country;
                Comm2.Parameters.Add("@promotionid", SqlDbType.BigInt).Value = Convert.ToInt64(PromoID);  // MODIFY
                Comm2.Parameters.Add("@shopid", SqlDbType.BigInt).Value = Convert.ToInt64(ShopID);
                Comm2.Parameters.Add("@itemid", SqlDbType.BigInt).Value = Convert.ToInt64(ProdID);
                Comm2.Parameters.Add("@modified_date", SqlDbType.DateTime).Value = Convert.ToDateTime(STRTTIME);
                //Comm2.Parameters.Add("@userid", SqlDbType.Int).Value = Userid;
                Comm2.Parameters.Add("@scrap_type", SqlDbType.Int).Value = ScrapType;
                Comm2.Parameters.Add("@cb_option", SqlDbType.Int).Value = CbOption;   // MODIFY
                Comm2.Parameters.Add("@ctime", SqlDbType.BigInt).Value = Convert.ToInt64(CTIME);
                Comm2.Parameters.Add("@name", SqlDbType.NVarChar).Value = PRODUCT_NAME;
                Comm2.Parameters.Add("@rating_star", SqlDbType.Decimal).Value = STAR;
                Comm2.Parameters.Add("@rating_count", SqlDbType.Int).Value = RATING;
                Comm2.Parameters.Add("@historical_sold", SqlDbType.Int).Value = TOTAL_SOLD;
                Comm2.Parameters.Add("@monthly_sold", SqlDbType.Int).Value = MONTHLY_SOLD;
                Comm2.Parameters.Add("@catid", SqlDbType.BigInt).Value = Convert.ToInt64(CatID);
                Comm2.Parameters.Add("@price_slash_min", SqlDbType.Decimal).Value = PRICE_SLASH_MIN / PriceDiv;  // MODIFY
                Comm2.Parameters.Add("@price_slash_max", SqlDbType.Decimal).Value = PRICE_SLASH_MAX / PriceDiv;   // MODIFY
                Comm2.Parameters.Add("@price_min", SqlDbType.Decimal).Value = M_PRICE / PriceDiv;
                Comm2.Parameters.Add("@price_max", SqlDbType.Decimal).Value = MX_PRICE / PriceDiv;
                Comm2.Parameters.Add("@avg_price", SqlDbType.Decimal).Value = AvgPrice; // MODIFY
                Comm2.Parameters.Add("@stock", SqlDbType.Int).Value = Stock;  // MODIFY
                Comm2.Parameters.Add("@catname1", SqlDbType.NVarChar, 255).Value = CAT_NAME_1;
                Comm2.Parameters.Add("@catname2", SqlDbType.NVarChar, 255).Value = CAT_NAME_2;
                Comm2.Parameters.Add("@catname3", SqlDbType.NVarChar, 255).Value = CAT_NAME_3;
                Comm2.Parameters.Add("@image", SqlDbType.NVarChar, 255).Value = ImgUrl;
                Comm2.Parameters.Add("@monthly_revenue", SqlDbType.Decimal).Value = MonthlyRevenue;
                Comm2.Parameters.Add("@product_link", SqlDbType.NVarChar).Value = LINK_SKU;
                Comm2.Parameters.Add("@is_pre_order", SqlDbType.Bit).Value = IsPreOrder;  // MODIFY
                Comm2.Parameters.Add("@estimated_days", SqlDbType.Int).Value = EstimatedDays; // MODIFY
                Comm2.Parameters.Add("@tier_variations", SqlDbType.NVarChar).Value = TierVarJson;
                Comm2.Parameters.Add("@is_fs_info", SqlDbType.Bit).Value = IsFsInfo;  // MODIFY
                Comm2.Transaction = tran;
                Comm2.CommandTimeout = 0;
                Comm2.ExecuteNonQuery();
                Comm2.Dispose();
                tran.Commit();
                cnn.Close();

                Comm2.Parameters.Clear();


            }
            catch (SqlException ex)
            {

                tran.Rollback();
                cnn.Close();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message.ToString());
                tran.Rollback();
                cnn.Close();
            }

        }

        public void SaveSku_model(string Country, string PromoID, string ModelID, string ProdID, string VarName, Decimal VarPriceSlash, Decimal VarPrice, int VarStock, string VarImage, Decimal Revenue, Decimal AvgPrice, string TierIndexJson)
        {
            try
            {
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                tran = cnn.BeginTransaction();
                string Qry = "Insert into sku_model (country_code,promotionid,itemid,modelid,name,price_slash,price,stock,image,revenue,avg_price,tier_index)" +
                "values (@country_code,@promotionid,@itemid,@modelid,@name,@price_slash,@price,@stock,@image,@revenue,@avg_price,@tier_index)";

                Decimal PriceDiv = Convert.ToDecimal(100000.0);

                SqlCommand Comm2 = new SqlCommand(Qry, cnn);
                Comm2.Parameters.Add("@country_code", SqlDbType.VarChar, 3).Value = Country;
                Comm2.Parameters.Add("@promotionid", SqlDbType.BigInt).Value = Convert.ToInt64(PromoID);
                Comm2.Parameters.Add("@itemid", SqlDbType.BigInt).Value = Convert.ToInt64(ProdID);
                Comm2.Parameters.Add("@modelid", SqlDbType.BigInt).Value = Convert.ToInt64(ModelID);
                Comm2.Parameters.Add("@name", SqlDbType.NVarChar).Value = VarName;
                Comm2.Parameters.Add("@price_slash", SqlDbType.Decimal).Value = VarPriceSlash / PriceDiv;
                Comm2.Parameters.Add("@price", SqlDbType.Decimal).Value = VarPrice / PriceDiv;
                Comm2.Parameters.Add("@stock", SqlDbType.Int).Value = VarStock;
                Comm2.Parameters.Add("@image", SqlDbType.NVarChar, 255).Value = VarImage;
                Comm2.Parameters.Add("@revenue", SqlDbType.Decimal).Value = Revenue;
                Comm2.Parameters.Add("@avg_price", SqlDbType.Decimal).Value = AvgPrice;
                Comm2.Parameters.Add("@tier_index", SqlDbType.NVarChar).Value = TierIndexJson;

                Comm2.Transaction = tran;
                Comm2.CommandTimeout = 0;
                Comm2.ExecuteNonQuery();
                Comm2.Dispose();
                tran.Commit();
                cnn.Close();

                Comm2.Parameters.Clear();


            }
            catch (SqlException ex)
            {

                tran.Rollback();
                cnn.Close();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message.ToString());
                tran.Rollback();
                cnn.Close();
            }

        }


        public void UpdatePreScrap(string Country, string PromoID, string ShopID, string ProdID, string STRTTIME, int Userid, int ScrapType, int CbOption, string CTIME, string PRODUCT_NAME, Decimal STAR, int RATING, int TOTAL_SOLD, int MONTHLY_SOLD, string CatID, Decimal PRICE_SLASH_MIN, Decimal PRICE_SLASH_MAX, Decimal M_PRICE, Decimal MX_PRICE, Decimal AvgPrice, int Stock, string CAT_NAME_1, string CAT_NAME_2, string CAT_NAME_3, string ImgUrl, Decimal MonthlyRevenue, string LINK_SKU, int IsPreOrder, int EstimatedDays, string TierVarJson, int IsFsInfo)
        {
            try
            {
                cnn.ConnectionString = CnnStr;
                if (cnn.State == ConnectionState.Closed) cnn.Open();
                tran = cnn.BeginTransaction();
               
                tran = cnn.BeginTransaction();
                //string Qry = "Update  sku  set  modified_date = @modified_date,	scrap_type = @scrap_type,	cb_option=@cb_option,	ctime = @ctime,	name= @name,	rating_star=@rating_star,	rating_count=@rating_count,	historical_sold= @historical_sold,	monthly_sold = @monthly_sold,	catid = @catid,	price_slash_min = @price_slash_min,	price_slash_max = @price_slash_max,	price_min = @price_min,	price_max = @price_max,	avg_price = @avg_price,	stock= @stock,	catname1 = @catname1,	catname2 = @catname2,	catname3 = @catname3,	image= @image,	monthly_revenue = @monthly_revenue,	product_link = @product_link,	is_pre_order = @is_pre_order ,	estimated_days = @estimated_days, tier_variations = @tier_variations, is_fs_info = @is_fs_info where country_code = @country_code,	promotionid = @promotionid,	shopid = @shopid,	itemid = @itemid ";
                string Qry = "Update  sku  set  modified_date = @modified_date, is_fs_info = @is_fs_info where country_code = @country_code,	promotionid = @promotionid,	shopid = @shopid,	itemid = @itemid ";


                Decimal PriceDiv = Convert.ToDecimal(100000.0);

                SqlCommand Comm2 = new SqlCommand(Qry, cnn);
                Comm2.Parameters.Add("@country_code", SqlDbType.VarChar, 3).Value = Country;
                Comm2.Parameters.Add("@promotionid", SqlDbType.BigInt).Value = Convert.ToInt64(PromoID);  // MODIFY
                Comm2.Parameters.Add("@shopid", SqlDbType.BigInt).Value = Convert.ToInt64(ShopID);
                Comm2.Parameters.Add("@itemid", SqlDbType.BigInt).Value = Convert.ToInt64(ProdID);
                Comm2.Parameters.Add("@modified_date", SqlDbType.DateTime).Value = Convert.ToDateTime(STRTTIME);
                //Comm2.Parameters.Add("@userid", SqlDbType.Int).Value = Userid;
                //Comm2.Parameters.Add("@scrap_type", SqlDbType.Int).Value = ScrapType;
                //Comm2.Parameters.Add("@cb_option", SqlDbType.Int).Value = CbOption;   // MODIFY
                //Comm2.Parameters.Add("@ctime", SqlDbType.BigInt).Value = Convert.ToInt64(CTIME);
                //Comm2.Parameters.Add("@name", SqlDbType.NVarChar).Value = PRODUCT_NAME;
                //Comm2.Parameters.Add("@rating_star", SqlDbType.Decimal).Value = STAR;
                //Comm2.Parameters.Add("@rating_count", SqlDbType.Int).Value = RATING;
                //Comm2.Parameters.Add("@historical_sold", SqlDbType.Int).Value = TOTAL_SOLD;
                //Comm2.Parameters.Add("@monthly_sold", SqlDbType.Int).Value = MONTHLY_SOLD;
                //Comm2.Parameters.Add("@catid", SqlDbType.BigInt).Value = Convert.ToInt64(CatID);
                //Comm2.Parameters.Add("@price_slash_min", SqlDbType.Decimal).Value = PRICE_SLASH_MIN / PriceDiv;  // MODIFY
                //Comm2.Parameters.Add("@price_slash_max", SqlDbType.Decimal).Value = PRICE_SLASH_MAX / PriceDiv;   // MODIFY
                //Comm2.Parameters.Add("@price_min", SqlDbType.Decimal).Value = M_PRICE / PriceDiv;
                //Comm2.Parameters.Add("@price_max", SqlDbType.Decimal).Value = MX_PRICE / PriceDiv;
                //Comm2.Parameters.Add("@avg_price", SqlDbType.Decimal).Value = AvgPrice; // MODIFY
                //Comm2.Parameters.Add("@stock", SqlDbType.Int).Value = Stock;  // MODIFY
                //Comm2.Parameters.Add("@catname1", SqlDbType.NVarChar, 255).Value = CAT_NAME_1;
                //Comm2.Parameters.Add("@catname2", SqlDbType.NVarChar, 255).Value = CAT_NAME_2;
                //Comm2.Parameters.Add("@catname3", SqlDbType.NVarChar, 255).Value = CAT_NAME_3;
                //Comm2.Parameters.Add("@image", SqlDbType.NVarChar, 255).Value = ImgUrl;
                //Comm2.Parameters.Add("@monthly_revenue", SqlDbType.Decimal).Value = MonthlyRevenue;
                //Comm2.Parameters.Add("@product_link", SqlDbType.NVarChar).Value = LINK_SKU;
                //Comm2.Parameters.Add("@is_pre_order", SqlDbType.Bit).Value = IsPreOrder;  // MODIFY
                //Comm2.Parameters.Add("@estimated_days", SqlDbType.Int).Value = EstimatedDays; // MODIFY
                //Comm2.Parameters.Add("@tier_variations", SqlDbType.NVarChar).Value = TierVarJson;
                Comm2.Parameters.Add("@is_fs_info", SqlDbType.Bit).Value = IsFsInfo;  // MODIFY
                Comm2.Transaction = tran;
                Comm2.CommandTimeout = 0;
                Comm2.ExecuteNonQuery();
                Comm2.Dispose();
                tran.Commit();
                cnn.Close();

                Comm2.Parameters.Clear();


            }
            catch (SqlException ex)
            {

                tran.Rollback();
                cnn.Close();

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message.ToString());
                tran.Rollback();
                cnn.Close();
            }

        }



        public void SaveShopData(string cntry, string SHID, string shpName, DateTime ModefiedDate)
        {

            try
            {

                cnn.ConnectionString = CnnStr;

                try
                {
                    if (cnn.State == ConnectionState.Closed) cnn.Open();
                    tran = cnn.BeginTransaction();

                    string Qry = "Insert into shop(country_code,shopid,name,modified_date,creation_date) values (@country_code,@shopid,@name,@modified_date,@creation_date)";

                    SqlCommand Comm2 = new SqlCommand(Qry, cnn);

                    Comm2.Parameters.Add("@country_code", SqlDbType.NVarChar, 3).Value = cntry;
                    Comm2.Parameters.Add("@shopid", SqlDbType.BigInt).Value = Convert.ToInt64(SHID);
                    Comm2.Parameters.Add("@name", SqlDbType.NVarChar, 255).Value = shpName;
                    Comm2.Parameters.Add("@modified_date", SqlDbType.DateTime).Value = ModefiedDate;
                    Comm2.Parameters.Add("@creation_date", SqlDbType.DateTime).Value = ModefiedDate;
                  
                    Comm2.Transaction = tran;
                    Comm2.CommandTimeout = 0;
                    Comm2.ExecuteNonQuery();
                    Comm2.Dispose();
                    tran.Commit();
                    cnn.Close();
                    Comm2.Parameters.Clear();
                }
                catch (SqlException ex)
                {

                    tran.Rollback();
                    cnn.Close();

                }
                catch (Exception ex)
                {

                    tran.Rollback();
                    cnn.Close();
                }



            }
            catch
            {


            }

        }


    }
}
