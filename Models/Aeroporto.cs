using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace NearAirports.Models
{
    public class Aeroporto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> loc { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string code { get; set; }

    }
}