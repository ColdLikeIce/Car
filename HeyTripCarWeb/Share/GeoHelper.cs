namespace HeyTripCarWeb.Share
{
    public class GeoHelper
    {
        private const double EarthRadiusKm = 6371.0; // 地球半径，单位：公里

        public static (double, double, double, double) FindNearbyCoordinates(double lat, double lon, double distanceKm)
        {
            // 将距离从公里转换为经纬度变化
            double latChange = distanceKm / EarthRadiusKm * (180 / Math.PI);
            double lonChange = distanceKm / (EarthRadiusKm * Math.Cos(lat * Math.PI / 180)) * (180 / Math.PI);

            // 计算附近的点
            double newLatNorth = lat + latChange; // 向北
            double newLatSouth = lat - latChange; // 向南
            double newLonEast = lon + lonChange;  // 向东
            double newLonWest = lon - lonChange;  // 向西
            return (newLatNorth, newLatSouth, newLonEast, newLonWest);
        }

        // 计算 Haversine 公式
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = EarthRadiusKm * c;
            return distance;
        }

        // 判断经纬度是否在 20 公里范围内
        public static bool IsWithinRange(double centerLat, double centerLon, double targetLat, double targetLon, double rangeKm)
        {
            double distance = Haversine(centerLat, centerLon, targetLat, targetLon);
            return distance <= rangeKm;
        }
    }
}