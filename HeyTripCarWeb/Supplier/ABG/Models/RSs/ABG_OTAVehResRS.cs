using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot("OTA_VehResRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTAVehResRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlAttribute("SequenceNmbr")]
        public int SequenceNumber { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("Success")]
        public Success Success { get; set; }

        [XmlElement("VehResRSCore")]
        public VehResRSCore VehResRSCore { get; set; }
    }

    public class VehResRSCore
    {
        [XmlElement("VehReservation")]
        public VehReservation VehReservation { get; set; }
    }

    public class VehReservation
    {
        [XmlElement("Customer")]
        public Customer Customer { get; set; }

        [XmlElement("VehSegmentCore")]
        public VehSegmentCore VehSegmentCore { get; set; }

        [XmlElement("VehSegmentInfo")]
        public VehSegmentInfo VehSegmentInfo { get; set; }
    }

    public class VehSegmentCore
    {
        [XmlElement("ConfID")]
        public ConfID ConfID { get; set; }

        [XmlElement("Vendor")]
        public string Vendor { get; set; }

        [XmlElement("VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement("Vehicle")]
        public Vehicle Vehicle { get; set; }

        [XmlElement("RentalRate")]
        public RentalRate RentalRate { get; set; }

        [XmlElement("TotalCharge")]
        public TotalCharge TotalCharge { get; set; }
    }

    public class ConfID
    {
        [XmlAttribute("ID")]
        public string ID { get; set; }

        [XmlAttribute("Type")]
        public int Type { get; set; }
    }

    public class TaxAmounts
    {
        [XmlElement("TaxAmount")]
        public TaxAmount TaxAmount { get; set; }
    }

    public class VehSegmentInfo
    {
        [XmlElement("LocationDetails")]
        public List<OrderLocationDetails> LocationDetails { get; set; }
    }

    public class OrderLocationDetails
    {
        [XmlAttribute("AtAirport")]
        public bool AtAirport { get; set; }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("CodeContext")]
        public string CodeContext { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlElement("Address")]
        public OrderAddress Address { get; set; }

        [XmlElement("Telephone")]
        public Telephone Telephone { get; set; }
    }

    public class OrderAddress
    {
        [XmlElement("StreetNmbr")]
        public string StreetNumber { get; set; }

        [XmlElement("AddressLine")]
        public string AddressLine { get; set; }

        [XmlElement("CityName")]
        public string CityName { get; set; }

        [XmlElement("PostalCode")]
        public string PostalCode { get; set; }

        [XmlElement("StateProv")]
        public StateProv StateProv { get; set; }

        [XmlElement("CountryName")]
        public CountryName CountryName { get; set; }
    }
}