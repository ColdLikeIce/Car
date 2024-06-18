using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehRateRuleRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehRateRuleRS
    {
        [XmlElement(ElementName = "Success")]
        public string Success { get; set; }

        [XmlElement(ElementName = "VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement(ElementName = "Vehicle")]
        public Vehicle Vehicle { get; set; }

        [XmlElement(ElementName = "RentalRate")]
        public RentalRate RentalRate { get; set; }

        [XmlElement(ElementName = "TotalCharge")]
        public TotalCharge TotalCharge { get; set; }

        [XmlElement(ElementName = "RateRules")]
        public RateRules RateRules { get; set; }

        [XmlElement(ElementName = "LocationDetails")]
        public List<LocationDetails> LocationDetails { get; set; }

        [XmlArray(ElementName = "VendorMessages")]
        [XmlArrayItem(ElementName = "VendorMessage")]
        public List<VendorMessage> VendorMessages { get; set; }
    }

    public class RateRules
    {
        [XmlAttribute(AttributeName = "MinimumKeep")]
        public string MinimumKeep { get; set; }

        [XmlAttribute(AttributeName = "MaximumKeep")]
        public string MaximumKeep { get; set; }

        [XmlAttribute(AttributeName = "MaximumRental")]
        public string MaximumRental { get; set; }

        [XmlElement(ElementName = "AdvanceBooking")]
        public AdvanceBooking AdvanceBooking { get; set; }

        [XmlElement(ElementName = "PickupReturnRules")]
        public PickupReturnRules PickupReturnRules { get; set; }

        [XmlElement(ElementName = "RateGuarantee")]
        public RateGuarantee RateGuarantee { get; set; }

        [XmlElement(ElementName = "PaymentRules")]
        public PaymentRules PaymentRules { get; set; }
    }

    public class AdvanceBooking
    {
        [XmlAttribute(AttributeName = "OffsetTimeUnit")]
        public string OffsetTimeUnit { get; set; }

        [XmlAttribute(AttributeName = "OffsetUnitMultiplier")]
        public int OffsetUnitMultiplier { get; set; }

        [XmlAttribute(AttributeName = "RequiredInd")]
        public bool RequiredInd { get; set; }
    }

    public class PickupReturnRules
    {
        [XmlElement(ElementName = "EarliestPickup")]
        public EarliestPickup EarliestPickup { get; set; }

        [XmlElement(ElementName = "LatestPickup")]
        public LatestPickup LatestPickup { get; set; }

        [XmlElement(ElementName = "LatestReturn")]
        public LatestReturn LatestReturn { get; set; }
    }

    public class EarliestPickup
    {
        [XmlAttribute(AttributeName = "Time")]
        public DateTime Time { get; set; }
    }

    public class LatestPickup
    {
        [XmlAttribute(AttributeName = "Time")]
        public DateTime Time { get; set; }
    }

    public class LatestReturn
    {
        [XmlAttribute(AttributeName = "Time")]
        public DateTime Time { get; set; }
    }

    public class RateGuarantee
    {
        [XmlAttribute(AttributeName = "OffsetTimeUnit")]
        public string OffsetTimeUnit { get; set; }

        [XmlAttribute(AttributeName = "OffsetUnitMultiplier")]
        public int OffsetUnitMultiplier { get; set; }
    }

    public class PaymentRules
    {
        [XmlArray(ElementName = "AcceptablePayments")]
        [XmlArrayItem(ElementName = "AcceptablePayment")]
        public List<AcceptablePayment> AcceptablePayments { get; set; }
    }

    public class AcceptablePayment
    {
        [XmlAttribute(AttributeName = "CreditCardCode")]
        public string CreditCardCode { get; set; }
    }

    public class LocationDetails
    {
        [XmlAttribute(AttributeName = "AtAirport")]
        public bool AtAirport { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "CodeContext")]
        public string CodeContext { get; set; }

        [XmlAttribute(AttributeName = "AssocAirportLocList")]
        public string AssocAirportLocList { get; set; }

        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; }

        [XmlElement(ElementName = "Telephone")]
        public List<Telephone> Telephones { get; set; }

        [XmlElement(ElementName = "AdditionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }
    }

    public class Address
    {
        [XmlElement(ElementName = "StreetNmbr")]
        public string StreetNmbr { get; set; }

        [XmlElement(ElementName = "AddressLine")]
        public string AddressLine { get; set; }

        [XmlElement(ElementName = "CityName")]
        public string CityName { get; set; }

        [XmlElement(ElementName = "PostalCode")]
        public string PostalCode { get; set; }

        [XmlElement(ElementName = "CountryName")]
        public CountryName CountryName { get; set; }
    }

    public class CountryName
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }

        [XmlText]
        public string Name { get; set; }
    }

    public class Telephone
    {
        [XmlAttribute(AttributeName = "PhoneTechType")]
        public int PhoneTechType { get; set; }

        [XmlAttribute(AttributeName = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [XmlAttribute(AttributeName = "DefaultInd")]
        public bool DefaultInd { get; set; }
    }

    public class AdditionalInfo
    {
        [XmlElement(ElementName = "ParkLocation")]
        public ParkLocation ParkLocation { get; set; }

        [XmlElement(ElementName = "CounterLocation")]
        public CounterLocation CounterLocation { get; set; }

        [XmlElement(ElementName = "OperationSchedules")]
        public OperationSchedules OperationSchedules { get; set; }

        [XmlElement(ElementName = "Shuttle")]
        public Shuttle Shuttle { get; set; }
    }

    public class ParkLocation
    {
        [XmlAttribute(AttributeName = "Location")]
        public int Location { get; set; }
    }

    public class CounterLocation
    {
        [XmlAttribute(AttributeName = "Location")]
        public int Location { get; set; }
    }

    public class OperationSchedules
    {
        [XmlElement(ElementName = "OperationSchedule")]
        public OperationSchedule OperationSchedule { get; set; }
    }

    public class OperationSchedule
    {
        [XmlElement(ElementName = "OperationTimes")]
        public List<OperationTime> OperationTimes { get; set; }
    }

    public class OperationTime
    {
        [XmlAttribute(AttributeName = "Mon")]
        public bool Mon { get; set; }

        [XmlAttribute(AttributeName = "Tue")]
        public bool Tue { get; set; }

        [XmlAttribute(AttributeName = "Weds")]
        public bool Weds { get; set; }

        [XmlAttribute(AttributeName = "Thur")]
        public bool Thur { get; set; }

        [XmlAttribute(AttributeName = "Fri")]
        public bool Fri { get; set; }

        [XmlAttribute(AttributeName = "Sat")]
        public bool Sat { get; set; }

        [XmlAttribute(AttributeName = "Sun")]
        public bool Sun { get; set; }

        [XmlAttribute(AttributeName = "Start")]
        public string Start { get; set; }

        [XmlAttribute(AttributeName = "End")]
        public string End { get; set; }
    }

    public class Shuttle
    {
        [XmlElement(ElementName = "ShuttleInfos")]
        public List<ShuttleInfo> ShuttleInfos { get; set; }
    }

    public class ShuttleInfo
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlElement(ElementName = "SubSection")]
        public SubSection SubSection { get; set; }
    }

    public class SubSection
    {
        [XmlElement(ElementName = "Paragraph")]
        public Paragraph Paragraph { get; set; }
    }

    public class Paragraph
    {
        [XmlElement(ElementName = "Text")]
        public string Text { get; set; }
    }

    public class VendorMessage
    {
        [XmlAttribute(AttributeName = "InfoType")]
        public int InfoType { get; set; }

        [XmlElement(ElementName = "SubSection")]
        public SubSection SubSection { get; set; }
    }
}