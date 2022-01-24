using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoDBTest.Model
{
    class BsonModel
    {
        public BsonInt32 promotionid { get; set; }
        public BsonInt32 shopid { get; set; }
        public BsonDouble itemid { get; set; }
        public BsonInt32 Sql_status { get; set; }
    }
}
