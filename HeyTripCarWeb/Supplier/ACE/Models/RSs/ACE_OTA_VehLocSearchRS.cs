using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RSs
{
    [XmlRoot("OTA_VehLocSearchRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehLocSearchRS
    {
        [XmlAttribute("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }

        [XmlElement("Errors")]
        public Errors Errors { get; set; }

        [XmlElement("Success")]
        public Success Success { get; set; }

        [XmlArray("VehMatchedLocs")]
        [XmlArrayItem("VehMatchedLoc")]
        public List<VehMatchedLoc> VehMatchedLocs { get; set; }

        [XmlElement("Vendor")]
        public Vendor Vendor { get; set; }
    }

    public class VehMatchedLoc
    {
        [XmlElement("LocationDetail")]
        public LocationDetail LocationDetail { get; set; }
    }

    public class LocationDetail
    {
        [XmlAttribute("AtAirport")]
        public bool AtAirport { get; set; }

        [XmlAttribute("Code")]
        public string Code { get; set; }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("CodeContext")]
        public string CodeContext { get; set; }

        [XmlAttribute("AssocAirportLocList")]
        public string AssocAirportLocList { get; set; }

        [XmlElement("Address")]
        public Address Address { get; set; }

        [XmlElement("Telephone")]
        public List<LocTelephone> Telephones { get; set; }

        [XmlElement("AdditionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }
    }

    public class OperationTimes
    {
        [XmlElement("OperationTime")]
        public List<OperationTime> OperationTime { get; set; }
    }

    public class ShuttleInfos
    {
        [XmlElement("ShuttleInfo")]
        public List<ShuttleInfo> ShuttleInfo { get; set; }
    }
}