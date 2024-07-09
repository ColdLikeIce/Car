namespace HeyTripCarWeb.Supplier.Sixt.Models.RSs
{
    public class QueryRs
    {
        public string errorcode { get; set; }
        public string message { get; set; }
        public string customerType { get; set; }
        public Trip trip { get; set; }
        public Documents documents { get; set; }
        public List<Offers> offers { get; set; }
        public string branchCurrency { get; set; }
    }

    public class Trip
    {
        public string pickupStationId { get; set; }
        public bool pickupStationIsMobileCheckIn { get; set; }
        public string returnStationId { get; set; }
        public DateTime pickupDate { get; set; }
        public DateTime returnDate { get; set; }
        public string vehicleType { get; set; }
    }

    public class Baggage
    {
        public int bags { get; set; }
        public int suitcases { get; set; }
    }

    public class Offers
    {
        public string Id { get; set; }
        public string AcrissCode { get; set; }
        public int RentalDays { get; set; }
        public int RentalHours { get; set; }
        public string Title { get; set; }
        public VehicleGroupInfo VehicleGroupInfo { get; set; }
        public Prices Prices { get; set; }
        public List<IncludedItem> Included { get; set; }
        public PaymentInfo Payment { get; set; }
        public bool IsOnRequest { get; set; }
        public decimal? DepositAmount { get; set; }
        public string DepositCurrency { get; set; }
        public bool YoungDriverFeeApplied { get; set; }
        public List<ChargeItem> Charges { get; set; }
        public List<MileagePlan> MileagePlans { get; set; }
        public Packages Packages { get; set; }
    }

    public class VehicleGroupInfo
    {
        public string Type { get; set; }
        public GroupInfo GroupInfo { get; set; }
        public string ImageUrl { get; set; }
        public bool IsElectric { get; set; }
        public string MaxRangeWltp { get; set; }
        public string ChargingCable { get; set; }
    }

    public class GroupInfo
    {
        public string VehicleGroup { get; set; }
        public string BodyStyle { get; set; }
        public string BodyStyleTitle { get; set; }
        public bool IsLuxury { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool IsAutomatic { get; set; }
        public bool HasNavigationSystem { get; set; }
        public bool IsModelGuaranteed { get; set; }
        public int MaxPassengers { get; set; }
        public int Doors { get; set; }
        public bool IsElectric { get; set; }
        public string ImageUrl { get; set; }
        public DriverRequirements DriverRequirements { get; set; }
        public List<string> Examples { get; set; }
        public BaggageInfo Baggage { get; set; }
    }

    public class DriverRequirements
    {
        public int MinAge { get; set; }
        public string LicenseCategory { get; set; }
        public int LicenseMinYears { get; set; }
        public int YoungDriverAge { get; set; }
    }

    public class BaggageInfo
    {
        public int Bags { get; set; }
        public int Suitcases { get; set; }
    }

    public class Prices
    {
        public PriceInfo BasePrice { get; set; }
        public PriceInfo TotalPrice { get; set; }
        public PriceInfo DayPrice { get; set; }
        public PriceInfo BaseDayPrice { get; set; }
        public PriceInfo YoungDriverChargeDayPrice { get; set; }
        public bool YoungDriverFeeApplied { get; set; }
        public bool IsHourlyBooking { get; set; }
    }

    public class PriceInfo
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public decimal Net { get; set; }
        public decimal Gross { get; set; }
    }

    public class IncludedItem
    {
        public string Title { get; set; }
    }

    public class PaymentInfo
    {
        public string SelectedPaymentOption { get; set; }
        public List<PaymentOption> PaymentOptions { get; set; }
    }

    public class PaymentOption
    {
        public string Id { get; set; }
        public PriceInfo Price { get; set; }
        public List<PriceBreakdownItem> PriceBreakdown { get; set; }
    }

    public class PriceBreakdownItem
    {
        public string Reference { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public PriceInfo Price { get; set; }
    }

    public class ChargeItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public List<string> OneOf { get; set; }
        public string Status { get; set; }
        public PriceInfo Price { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool Enabled { get; set; }
        public ExcessAmount ExcessAmount { get; set; }
    }

    public class ExcessAmount
    {
        public decimal? Value { get; set; }
        public string Currency { get; set; }
    }

    public class MileagePlan
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
        public bool Unlimited { get; set; }
        public PriceInfo Price { get; set; }
        public IncludedMileage IncludedMileage { get; set; }
        public AdditionalMileage AdditionalMileage { get; set; }
    }

    public class IncludedMileage
    {
        public int Value { get; set; }
        public string Unit { get; set; }
    }

    public class AdditionalMileage
    {
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }

    public class Packages
    {
        public List<object> Protection { get; set; }
    }
}