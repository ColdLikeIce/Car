using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehRateRuleRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTA_VehRateRuleRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        public Success Success { get; set; }
        public VehRentalCore VehRentalCore { get; set; }
        public Vehicle Vehicle { get; set; }
        public RentalRate RentalRate { get; set; }
        public TotalCharge TotalCharge { get; set; }
        public List<PricedEquip> PricedEquips { get; set; }
        public List<PricedCoverage> PricedCoverages { get; set; }
        public LocationDetails LocationDetails { get; set; }
        public List<VendorMessage> VendorMessages { get; set; }
    }

    public class TaxAmount
    {
        [XmlAttribute]
        public decimal Total { get; set; }

        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public string Description { get; set; }
    }

    public class Calculation
    {
        [XmlAttribute]
        public decimal UnitCharge { get; set; }

        [XmlAttribute]
        public string UnitName { get; set; }

        [XmlAttribute]
        public int Quantity { get; set; }
    }

    public class PricedEquip
    {
        public Equipment Equipment { get; set; }
        public Charge Charge { get; set; }
    }

    public class Equipment
    {
        [XmlAttribute]
        public string EquipType { get; set; }
    }

    public class Charge
    {
        [XmlAttribute]
        public bool TaxInclusive { get; set; }

        [XmlAttribute]
        public bool IncludedInRate { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }

        [XmlAttribute]
        public string CurrencyCode { get; set; }

        public Calculation Calculation { get; set; }
        public MinMax MinMax { get; set; }
    }

    public class MinMax
    {
        [XmlAttribute]
        public decimal MaxCharge { get; set; }

        [XmlAttribute]
        public decimal MinCharge { get; set; }
    }

    public class PricedCoverage
    {
        public Coverage Coverage { get; set; }
        public Charge Charge { get; set; }
    }

    public class Coverage
    {
        [XmlAttribute]
        public string CoverageType { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        public Details CoverageDetails { get; set; }
    }

    public class Details
    {
        [XmlAttribute]
        public string CoverageTextType { get; set; }

        [XmlText]
        public string Description { get; set; }
    }

    public class LocationDetails
    {
        [XmlAttribute]
        public bool AtAirport { get; set; }

        [XmlAttribute]
        public string Code { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string CodeContext { get; set; }

        [XmlAttribute]
        public string ExtendedLocationCode { get; set; }

        public Address Address { get; set; }
        public Telephone Telephone { get; set; }
    }

    public class Address
    {
        public string StreetNmbr { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public StateProv StateProv { get; set; }
        public CountryName CountryName { get; set; }
    }

    public class StateProv
    {
        [XmlAttribute]
        public string StateCode { get; set; }

        [XmlText]
        public string Name { get; set; }
    }

    public class CountryName
    {
        [XmlAttribute]
        public string Code { get; set; }

        [XmlText]
        public string Name { get; set; }
    }

    public class Telephone
    {
        [XmlAttribute]
        public string PhoneNumber { get; set; }
    }

    public class VendorMessage
    {
        [XmlAttribute]
        public string Title { get; set; }

        public List<SubSection> SubSection { get; set; }
    }

    public class SubSection
    {
        [XmlAttribute]
        public string SubTitle { get; set; }

        public Paragraph Paragraph { get; set; }
    }

    public class Paragraph
    {
        public List<ListItem> ListItem { get; set; }
    }

    public class ListItem
    {
        [XmlAttribute]
        public bool Formatted { get; set; }

        [XmlAttribute]
        public string TextFormat { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}