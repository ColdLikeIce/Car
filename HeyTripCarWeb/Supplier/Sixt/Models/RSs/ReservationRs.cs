using HeyTripCarWeb.Supplier.Sixt.Models.RQs;

namespace HeyTripCarWeb.Supplier.Sixt.Models.RSs
{
    public class ReservationRs
    {
        public string errorCode { get; set; }
        public string message { get; set; }
        public string Id { get; set; }
        public string DisplayId { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string PointOfSale { get; set; }
        public DateTime CreateDate { get; set; }
        public Vehicle Vehicle { get; set; }
        public List<Driver> Drivers { get; set; }
        public DateTime PickupDate { get; set; }
        public Station PickupStation { get; set; }
        public DateTime ReturnDate { get; set; }
        public Station ReturnStation { get; set; }
        public DeliveryCollection Delivery { get; set; }
        public DeliveryCollection Collection { get; set; }
        public OfferConfiguration OfferConfiguration { get; set; }
        public SelfService SelfService { get; set; }
        public ReferenceFields ReferenceFields { get; set; }
        public BonusProgram BonusProgram { get; set; }
        public string CustomerType { get; set; }
        public string CorporateCustomerNumber { get; set; }
    }

    public class Vehicle
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public string Transmission { get; set; }
        public FuelTank PrimaryFuelTank { get; set; }
        public FuelTank SecondaryFuelTank { get; set; }
        public bool HasGPS { get; set; }
        public bool HasAC { get; set; }
        public bool IsKeyless { get; set; }
        public bool HasBluetooth { get; set; }
        public bool HasWinterTires { get; set; }
        public string AcrissCode { get; set; }
        public string BodyStyle { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string InteriorColor { get; set; }
        public int Seats { get; set; }
        public int Doors { get; set; }
        public int Odometer { get; set; }
        public int Horsepower { get; set; }
        public int Kilowatts { get; set; }
        public string FuelConsumption { get; set; }
    }

    public class FuelTank
    {
        public string Type { get; set; }
        public string FilterKey { get; set; }
    }

    public class IdCard
    {
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuingAuthority { get; set; }
        public string CountryCode { get; set; }
    }

    public class DriversLicense
    {
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IssuingAuthority { get; set; }
        public string CountryCode { get; set; }
    }

    public class Station
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<ExternalId> ExternalIds { get; set; }
    }

    public class ExternalId
    {
        public string Type { get; set; }
        public string Id { get; set; }
    }

    public class DeliveryCollection
    {
        public int StationId { get; set; }
        public Address Address { get; set; }
    }

    public class OfferConfiguration
    {
        public string VehicleGroup { get; set; }
        public int RentalDays { get; set; }
        public string VehicleGroupImage { get; set; }
        public bool IsPrepaid { get; set; }
        public List<Purchase> Purchases { get; set; }
        public MileagePlan MileagePlan { get; set; }
        public Prices Prices { get; set; }
        public List<IncludedCoverage> IncludedCoverages { get; set; }
        public List<Charge> IncludedCharges { get; set; }
    }

    // 主类：表示每个包含的保险项
    public class IncludedCoverage
    {
        // 保险项的代码
        public string Code { get; set; }

        // 保险项的名称
        public string Name { get; set; }

        // 可选的多余金额信息
        public ExcessAmount ExcessAmount { get; set; }
    }

    public class Purchase
    {
        public Payment Payment { get; set; }
        public Voucher AppliedVoucher { get; set; }
        public List<Charge> Charges { get; set; }
    }

    public class Payment
    {
        public string GrossAmount { get; set; }
        public string NetAmount { get; set; }
        public string Currency { get; set; }
        public string PaymentType { get; set; }
        public Invoice Invoice { get; set; }
    }

    public class Invoice
    {
        public List<InvoiceAddress> Addresses { get; set; }
    }

    public class InvoiceAddress
    {
        public string Addressee { get; set; }
        public string AddressLine { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }

    public class Voucher
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Currency { get; set; }
        public Price Price { get; set; }
    }

    public class Charge
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string NetAmount { get; set; }
        public string GrossAmount { get; set; }
    }

    public class BasePrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class TotalPrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class DayPrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class BaseDayPrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class YoungDriverChargeDayPrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class SecondaryInvoiceTotalPrice
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string UnitKey { get; set; }
        public string Net { get; set; }
        public string Gross { get; set; }
    }

    public class Price
    {
        public string GrossAmount { get; set; }
        public string NetAmount { get; set; }
        public string Currency { get; set; }
    }

    public class SelfService
    {
        public bool IsCancelable { get; set; }
        public string CancellationFees { get; set; }
    }

    public class ReferenceFields
    {
        public string ReferenceField1 { get; set; }
        public string ReferenceField2 { get; set; }
        public string ReferenceField3 { get; set; }
    }

    public class BonusProgram
    {
        public string Id { get; set; }
        public string Number { get; set; }
    }
}