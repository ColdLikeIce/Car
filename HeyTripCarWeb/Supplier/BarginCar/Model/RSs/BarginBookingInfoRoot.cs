namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class BarginBookingInfoRoot : BaseResult
    {
        /// <summary>
        ///
        /// </summary>
        public BookingInfoResults results { get; set; }
    }

    public class BookingInfoResults
    {
        /// <summary>
        ///
        /// </summary>
        public List<BarginBookinginfoItem> bookinginfo { get; set; }

        /* /// <summary>
         ///
         /// </summary>
         public List<CustomerinfoItem> customerinfo { get; set; }
         /// <summary>
         ///
         /// </summary>
         public List<CompanyinfoItem> companyinfo { get; set; }
         /// <summary>
         ///
         /// </summary>
         public List<RateinfoItem> rateinfo { get; set; }
         /// <summary>
         ///
         /// </summary>
         public List<ExtrafeesItem> extrafees { get; set; }
         /// <summary>
         ///
         /// </summary>
         public List<PaymentinfoItem> paymentinfo { get; set; }
         /// <summary>
         ///
         /// </summary>
         public List<ExtradriversItem> extradrivers { get; set; }*/
    }
}