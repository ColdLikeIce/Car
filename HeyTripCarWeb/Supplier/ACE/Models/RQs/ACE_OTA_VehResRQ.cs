using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ACE.Models.RQs
{
    /// <summary>
    /// 创建预订单请求实体
    /// </summary>
    // 命名空间定义
    [XmlRoot(ElementName = "OTA_VehResRQ", Namespace = "http://www.opentravel.org/OTA/2003/05")]
    public class ACE_OTA_VehResRQ
    {
        public POS POS { get; set; }
        public CreateOrder_VehResRQCore VehResRQCore { get; set; }
        public VehResRQInfo VehResRQInfo { get; set; }

        [XmlAttribute("TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [XmlAttribute("Target")]
        public string Target { get; set; }

        [XmlAttribute("Version")]
        public string Version { get; set; }
    }

    public class CreateOrder_VehResRQCore
    {
        public VehRentalCore VehRentalCore { get; set; }
        public Customer Customer { get; set; }
    }

    public class Customer
    {
        public Primary Primary { get; set; }
    }

    public class Primary
    {
        [XmlAttribute]
        public DateTime BirthDate { get; set; }

        public PersonName PersonName { get; set; }
        public Telephone Telephone { get; set; }
        public string Email { get; set; }
        public Address Address { get; set; }
        public Document Document { get; set; }
    }

    public class PersonName
    {
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string Surname { get; set; }
    }

    public class Telephone
    {
        [XmlAttribute]
        public string CountryAccessCode { get; set; }

        [XmlAttribute]
        public string AreaCityCode { get; set; }

        [XmlAttribute]
        public string PhoneNumber { get; set; }

        [XmlAttribute]
        public string Extension { get; set; }
    }

    public class Address
    {
        public string StreetNmbr { get; set; }
        public string AddressLine { get; set; }
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string StateProv { get; set; }
        public string CountryName { get; set; }
    }

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

    public class VehResRQInfo
    {
        public string SpecialReqPref { get; set; }
        public ArrivalDetails ArrivalDetails { get; set; }
        public Reference Reference { get; set; }
    }

    public class ArrivalDetails
    {
        [XmlAttribute]
        public string TransportationCode { get; set; }

        [XmlAttribute]
        public string Number { get; set; }

        public OperatingCompany OperatingCompany { get; set; }
    }

    public class OperatingCompany
    {
        [XmlAttribute]
        public string Code { get; set; }
    }
}