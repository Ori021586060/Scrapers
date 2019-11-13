using Npgsql;
using NpgsqlTypes;
using NetTopologySuite.Geometries;

namespace ScraperModels.Models.Db
{
    public class BaseDbModel
    {
        public NetTopologySuite.Geometries.Geometry geom { get; set; }
    }
}