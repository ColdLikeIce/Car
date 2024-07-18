namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class BarginCancelBooking : BaseResult
    {
        public CancelBookingResults results { get; set; }
    }
    public class CancelBookingResults
    {
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string successmsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string reservationref { get; set; }
    }
}
