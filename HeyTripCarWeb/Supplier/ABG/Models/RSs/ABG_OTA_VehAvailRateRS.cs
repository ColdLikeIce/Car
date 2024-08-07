﻿using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using System.Xml.Serialization;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    [XmlRoot(ElementName = "OTA_VehAvailRateRS", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ABG_OTA_VehAvailRateRS
    {
        [XmlElement(ElementName = "Errors")]
        public Errors Errors { get; set; }

        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }

        [XmlAttribute(AttributeName = "Target")]
        public string Target { get; set; }

        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        [XmlAttribute(AttributeName = "SequenceNmbr")]
        public string SequenceNumber { get; set; }

        [XmlElement(ElementName = "Success")]
        public Success Success { get; set; }

        [XmlElement(ElementName = "VehAvailRSCore")]
        public VehAvailRSCore VehAvailRSCore { get; set; }
    }

    public class VehAvailRSCore
    {
        [XmlElement(ElementName = "VehRentalCore")]
        public VehRentalCore VehRentalCore { get; set; }

        [XmlElement(ElementName = "VehVendorAvails")]
        public VehVendorAvails VehVendorAvails { get; set; }
    }

    public class VehVendorAvails
    {
        [XmlElement(ElementName = "VehVendorAvail")]
        public List<VehVendorAvail> VehVendorAvail { get; set; }
    }

    [XmlRoot("Info")]
    public class Info
    {
        [XmlElement("LocationDetails")]
        public List<LocationDetails> LocationDetails { get; set; }
    }

    public class VehVendorAvail
    {
        [XmlElement(ElementName = "Vendor")]
        public string Vendor { get; set; }

        [XmlElement(ElementName = "VehAvails")]
        public VehAvails VehAvails { get; set; }

        [XmlElement(ElementName = "Info")]
        public Info Info { get; set; }
    }

    public class VehAvails
    {
        [XmlElement(ElementName = "VehAvail")]
        public List<VehAvail> VehAvail { get; set; }
    }

    public class VehAvail
    {
        [XmlElement(ElementName = "VehAvailCore")]
        public VehAvailCore VehAvailCore { get; set; }

        [XmlElement(ElementName = "VehAvailInfo")]
        public VehAvailInfo VehAvailInfo { get; set; }
    }

    public class VehAvailInfo
    {
        [XmlElement("PricedCoverages")]
        public PricedCoverages PricedCoverages { get; set; }
    }

    public class PricedCoverages
    {
        [XmlElement("PricedCoverage")]
        public List<PricedCoverage> PricedCoverageList { get; set; }
    }

    public class VehAvailCore
    {
        [XmlAttribute(AttributeName = "Status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "Vehicle")]
        public Vehicle Vehicle { get; set; }

        [XmlElement(ElementName = "RentalRate")]
        public RentalRate RentalRate { get; set; }

        [XmlElement(ElementName = "TotalCharge")]
        public TotalCharge TotalCharge { get; set; }

        [XmlArrayItem("Fee")]
        public List<Fee> Fees { get; set; }
    }
}