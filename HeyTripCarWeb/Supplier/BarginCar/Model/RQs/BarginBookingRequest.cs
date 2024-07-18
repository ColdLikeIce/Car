namespace HeyTripCarWeb.Supplier.BarginCar.Model.RQs
{
    public class BarginBookingRequest

    {
        /// <summary>
        ///
        /// </summary>
        public string method { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategorytypeid { get; set; }

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
        public int ageid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string bookingtype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string insuranceid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string extrakmsid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string transmission { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string numbertravelling { get; set; }

        /// <summary>
        ///
        /// </summary>
        public BarginCustomer customer { get; set; }

        public List<optionalfees> optionalfees { get; set; }
    }

    public class optionalfees
    {
        public int id { get; set; }
        public int qty { get; set; }
    }

    public class BarginCustomer
    {
        /// <summary>
        ///
        /// </summary>
        public string firstname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string lastname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string dateofbirth { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string licenseno { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string email { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string state { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string city { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string postcode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string address { get; set; }
    }
}