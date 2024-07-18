namespace HeyTripCarWeb.Supplier.BarginCar.Model.RQs
{
    public class VehicleSeachRequest
    {
        /// <summary>
        ///
        /// </summary>
        public string method { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategorytypeid { get; set; }

        public string vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pickuplocationid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pickupdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string pickuptime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string dropofflocationid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string dropoffdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string dropofftime { get; set; }

        /// <summary>
        ///
        /// </summary>
        //public int ageid
        //{
        //    get
        //    {
        //        switch (driverageId)
        //        {
        //            case 21:
        //                return 4;
        //            case 22:
        //                return 20;
        //            case 23:
        //                return 10;
        //            case 24:
        //                return 11;
        //            case 25:
        //                return 8;
        //            case 26:
        //                return 9;
        //        }
        //        return ageid;
        //    }
        //    set { ageid = value; }
        //}
        public int ageid { get; set; }

        //public int driverageId
        //{
        //    get;
        //    set;
        //}
    }
}