using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using Microsoft.VisualBasic.FileIO;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RSs
{
    public class Success
    {
        // This class represents an empty <Success/> tag
    }

    public class Vehicle
    {
        [XmlAttribute(AttributeName = "AirConditionInd")]
        public bool AirConditionInd { get; set; }

        [XmlAttribute(AttributeName = "TransmissionType")]
        public string TransmissionType { get; set; }

        [XmlElement(ElementName = "VehType")]
        public VehType VehType { get; set; }

        [XmlElement(ElementName = "VehClass")]
        public VehClass VehClass { get; set; }

        [XmlElement(ElementName = "VehMakeModel")]
        public VehMakeModel VehMakeModel { get; set; }

        [XmlElement(ElementName = "PictureURL")]
        public string PictureURL { get; set; }

        [XmlElement(ElementName = "FuelType")]
        public string FuelType { get; set; }

        [XmlElement(ElementName = "DriveType")]
        public string DriveType { get; set; }

        [XmlElement(ElementName = "PassengerQuantity")]
        public string PassengerQuantity { get; set; }

        [XmlElement(ElementName = "BaggageQuantity")]
        public int BaggageQuantity { get; set; }
    }

    public class VehMakeModel
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    public class RentalRate
    {
        [XmlElement(ElementName = "RateDistance")]
        public RateDistance RateDistance { get; set; }

        [XmlElement(ElementName = "VehicleCharges")]
        public VehicleCharges VehicleCharges { get; set; }

        [XmlElement(ElementName = "RateQualifier")]
        public RateQualifier RateQualifier { get; set; }
    }

    public class RateDistance
    {
        [XmlAttribute(AttributeName = "Unlimited")]
        public bool Unlimited { get; set; }

        [XmlAttribute(AttributeName = "DistUnitName")]
        public string DistUnitName { get; set; }

        [XmlAttribute(AttributeName = "VehiclePeriodUnitName")]
        public string VehiclePeriodUnitName { get; set; }

        [XmlAttribute(AttributeName = "Quantity")]
        public int Quantity { get; set; }
    }

    public class VehicleCharges
    {
        [XmlElement(ElementName = "VehicleCharge")]
        public List<VehicleCharge> VehicleCharge { get; set; }
    }

    public class VehicleCharge
    {
        [XmlAttribute]
        public bool TaxInclusive { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public bool GuaranteedInd { get; set; }

        [XmlAttribute]
        public bool IncludedInRate { get; set; }

        [XmlAttribute]
        public decimal Amount { get; set; }

        [XmlAttribute]
        public string CurrencyCode { get; set; }

        [XmlAttribute]
        public string Purpose { get; set; }

        public List<TaxAmount> TaxAmounts { get; set; }
        public Calculation Calculation { get; set; }
    }

    public class TotalCharge
    {
        [XmlAttribute(AttributeName = "RateTotalAmount")]
        public decimal RateTotalAmount { get; set; }

        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }

        [XmlAttribute(AttributeName = "EstimatedTotalAmount")]
        public decimal EstimatedTotalAmount { get; set; }
    }
}