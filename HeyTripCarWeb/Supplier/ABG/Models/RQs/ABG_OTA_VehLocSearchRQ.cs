using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    [XmlRoot("OTA_VehLocSearchRQ", Namespace = "http://www.w3.org/2008/XMLSchema-instance")]
    public class ABG_OTA_VehLocSearchRQ
    {
        [XmlAttribute("MaxResponses")]
        public int MaxResponses { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        public POS POS { get; set; }

        public VehLocSearchCriterion VehLocSearchCriterion { get; set; }

        public LocVendor Vendor { get; set; }

        public TPA_Extensions TPA_Extensions { get; set; }
    }

    public class LocVendor
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }
    }

    public class VehLocSearchCriterion
    {
        public Address Address { get; set; }
        public Radius Radius { get; set; }
    }

    public class Address
    {
        public string AddressLine { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string County { get; set; }
        public StateProv StateProv { get; set; }
        public CountryName CountryName { get; set; }
    }

    public class StateProv
    {
        [XmlAttribute("StateCode")]
        public string StateCode { get; set; }
    }

    public class CountryName
    {
        [XmlAttribute("Code")]
        public string Code { get; set; }
    }

    public class Radius
    {
        [XmlAttribute("DistanceMax")]
        public int DistanceMax { get; set; }

        [XmlAttribute("DistanceMeasure")]
        public string DistanceMeasure { get; set; }
    }

    public class TPA_Extensions
    {
        public string SortOrderType { get; set; }
        public string TestLocationType { get; set; }
        public string LocationStatusType { get; set; }
        public string LocationType { get; set; }
    }
}