namespace HeyTripCarWeb.Supplier.Sixt.Models.RQs
{
    // 顶级类，代表整个租赁预订信息
    public class RentalBookingRQ
    {
        public string ConfigurationId { get; set; }
        public string CommunicationLanguage { get; set; }
        public List<Driver> Drivers { get; set; }
        public string FlightNumber { get; set; }
        public string Channel { get; set; }
        public bool IsFirstTimeRenter { get; set; }
        public string Comment { get; set; }
        public string BrokerEmail { get; set; }
        public ReferenceFields ReferenceFields { get; set; }
        public BonusProgram BonusProgram { get; set; }
        public string ProfileId { get; set; }
        public PaymentDetails Payment { get; set; }
        public AgencyVoucher AgencyVoucher { get; set; }
        public DeliveryDetails Delivery { get; set; }
        public CollectionDetails Collection { get; set; }
        public string AgencyIataNumber { get; set; }
        public string AgencyNumber { get; set; }
    }

    // 代表驾驶员信息的类
    public class Driver
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Birthdate { get; set; }
        public Address Address { get; set; }
        public ContactDetails Contact { get; set; }
        public IdCard IdCard { get; set; }
        public DriversLicense DriversLicense { get; set; }
    }

    // 代表地址信息的类
    public class Address
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string State { get; set; }
    }

    // 代表联系信息的类
    public class ContactDetails
    {
        public Telephone Telephone { get; set; }
        public string Email { get; set; }
    }

    // 代表电话信息的类
    public class Telephone
    {
        public string CountryCode { get; set; }
        public string Number { get; set; }
    }

    // 代表身份证信息的类
    public class IdCard
    {
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuingAuthority { get; set; }
        public string CountryCode { get; set; }
    }

    // 代表驾驶证信息的类
    public class DriversLicense
    {
        public string Number { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IssuingAuthority { get; set; }
        public string CountryCode { get; set; }
    }

    // 代表引用字段的类
    public class ReferenceFields
    {
        public string ReferenceField1 { get; set; }
        public string ReferenceField2 { get; set; }
        public string ReferenceField3 { get; set; }
    }

    // 代表奖励计划的类
    public class BonusProgram
    {
        public string Id { get; set; }
        public string Number { get; set; }
    }

    // 代表付款信息的类
    public class PaymentDetails
    {
        public string CardHolderName { get; set; }
        public string Token { get; set; }
        public string ExpiryDate { get; set; }
        public string TaxNumber { get; set; }
        public string Password { get; set; }
        public ThreeDSecureDetails ThreeDS { get; set; }
    }

    // 代表 3D 安全验证信息的类
    public class ThreeDSecureDetails
    {
        public string Channel { get; set; }
        public MpiData MpiData { get; set; }
    }

    // 代表 MPI 数据的类
    public class MpiData
    {
        public string Cavv { get; set; }
        public string CavvAlgorithm { get; set; }
        public string Eci { get; set; }
        public string Xid { get; set; }
        public string DirectoryResponse { get; set; }
        public string AuthenticationResponse { get; set; }
    }

    // 代表代理凭证信息的类
    public class AgencyVoucher
    {
        public bool IsFullCredit { get; set; }
        public string BillingNumber { get; set; }
        public bool IsEVoucher { get; set; }
    }

    // 代表交货地址的类
    public class DeliveryDetails
    {
        public Address Address { get; set; }
    }

    // 代表取车地址的类
    public class CollectionDetails
    {
        public Address Address { get; set; }
    }
}