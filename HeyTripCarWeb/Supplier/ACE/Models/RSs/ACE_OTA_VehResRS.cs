using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using System.Xml.Serialization;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    // Root element OTA_VehResRS
    [XmlRoot(ElementName = "OTA_VehResRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehResRS
    {
        [XmlElement("Errors")]
        public Errors Errors { get; set; }

        [XmlAttribute]
        public string TimeStamp { get; set; }

        [XmlAttribute]
        public string Target { get; set; }

        [XmlAttribute]
        public string Version { get; set; }

        public Success Success { get; set; }
        public VehResRSCore VehResRSCore { get; set; }
    }

    // VehResRSCore element
    public class VehResRSCore
    {
        public VehReservation VehReservation { get; set; }
    }

    // VehReservation element
    public class VehReservation
    {
        public Customer Customer { get; set; }
        public VehSegmentCore VehSegmentCore { get; set; }
        public VehSegmentInfo VehSegmentInfo { get; set; }
    }

    public class Errors
    {
        [XmlElement(ElementName = "Error")]
        public List<Error> ErrorList { get; set; }
    }

    public class Error
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "ShortText")]
        public string ShortText { get; set; }

        [XmlText]
        public string Message { get; set; }
    }

    // Document element
    public class Document
    {
        [XmlAttribute]
        public string DocIssueLocation { get; set; }

        [XmlAttribute]
        public string DocID { get; set; }

        [XmlAttribute]
        public string DocType { get; set; }

        [XmlAttribute]
        public DateTime ExpireDate { get; set; }
    }

    // VehSegmentCore element
    public class VehSegmentCore
    {
        public ConfID ConfID { get; set; }
        public Vendor Vendor { get; set; }
        public VehRentalCore VehRentalCore { get; set; }
        public Vehicle Vehicle { get; set; }
        public RentalRate RentalRate { get; set; }
        public Fees Fees { get; set; }
        public TotalCharge TotalCharge { get; set; }
    }

    // ConfID element
    public class ConfID
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string ID { get; set; }
    }

    // Fees element
    public class Fees
    {
        public Fee[] Fee { get; set; }
    }

    // VehSegmentInfo element
    public class VehSegmentInfo
    {
        public LocationDetails LocationDetails { get; set; }
    }
}