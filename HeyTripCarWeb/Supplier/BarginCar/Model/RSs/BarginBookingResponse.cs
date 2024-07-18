namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class BarginBookingResponse : BaseResult
    {
        /// <summary>
        ///
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ResultsItem results { get; set; }
    }

    public class ResultsItem
    {
        /// <summary>
        ///
        /// </summary>
        public string reservationref { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string reservationno { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string customerid { get; set; }
    }
}