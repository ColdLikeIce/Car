using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using System.Xml.Serialization;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    // Define namespaces for XML serialization
    [XmlRoot(ElementName = "OTA_VehResRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehResRS
    {
        [XmlElement(ElementName = "Errors")]
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

    public class Errors
    {
        [XmlElement(ElementName = "Error")]
        public List<Error> ErrorList { get; set; }
    }

    public class Error
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }

        [XmlText]
        public string Message { get; set; }
    }

    public class VehResRSCore
    {
        public VehReservation VehReservation { get; set; }
    }

    public class VehReservation
    {
        public Customer Customer { get; set; }

        public VehSegmentCore VehSegmentCore { get; set; }

        public VehSegmentInfo VehSegmentInfo { get; set; }
    }

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

    public class ConfID
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string ID { get; set; }
    }

    public class ReturnLocation
    {
        [XmlAttribute]
        public string LocationCode { get; set; }

        [XmlAttribute]
        public string CodeContext { get; set; }
    }

    public class VehicleCharges
    {
        public List<VehicleCharge> VehicleCharge { get; set; }
    }

    public class Fees
    {
        public List<Fee> Fee { get; set; }
    }

    public class VehSegmentInfo
    {
        public List<LocationDetails> LocationDetails { get; set; }
    }
}