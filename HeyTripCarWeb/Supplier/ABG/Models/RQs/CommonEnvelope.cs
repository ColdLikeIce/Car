﻿using HeyTripCarWeb.Share.Dtos;
using System.Xml.Serialization;

namespace HeyTripCarWeb.Supplier.ABG.Models.RQs
{
    /// <summary>
    /// 统一外层包装xml节点 包括验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [XmlRoot("Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces { get; set; } = new XmlSerializerNamespaces();

        public Envelope()
        {
            Namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            Namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");
            Namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
            Namespaces.Add("ns", "http://wsg.avis.com/wsbang");
        }

        [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAP_ENV_Header Header { get; set; }

        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public SOAP_ENV_Body Body { get; set; }
    }

    public class SOAP_ENV_Header
    {
        [XmlElement(ElementName = "credentials", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public Credentials Credentials { get; set; }

        [XmlElement(ElementName = "WSBang-Roadmap", Namespace = "http://wsg.avis.com/wsbang")]
        public WSBang_Roadmap WSBangRoadmap { get; set; }
    }

    public class Credentials
    {
        [XmlElement(ElementName = "userID", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public EncodedString UserID { get; set; }

        [XmlElement(ElementName = "password", Namespace = "http://wsg.avis.com/wsbang/authInAny")]
        public EncodedString Password { get; set; }
    }

    public class EncodedString
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute(AttributeName = "encodingType")]
        public string EncodingType { get; set; }
    }

    public class WSBang_Roadmap
    {
    }

    public class SOAP_ENV_Body
    {
        [XmlElement(ElementName = "Request", Namespace = "http://wsg.avis.com/wsbang")]
        public ns_Request Request { get; set; }
    }

    public class ns_Request
    {
        [XmlElement(ElementName = "OTA_VehAvailRateRQ", Namespace = "")]
        public ABG_OTA_VehAvailRateRQ OTA_VehAvailRateRQ { get; set; }

        [XmlElement(ElementName = "OTA_VehRateRuleRQ", Namespace = "")]
        public ABG_OTA_VehRateRuleRQ OTA_VehRateRuleRQ { get; set; }

        [XmlElement(ElementName = "OTA_VehResRQ", Namespace = "")]
        public ABG_OTA_VehResRQ ABG_OTA_VehResRQ { get; set; }

        [XmlElement(ElementName = "OTA_VehCancelRQ", Namespace = "")]
        public ABG_OTAVehCancelRQ ABG_OTAVehCancelRQ { get; set; }

        [XmlElement(ElementName = "OTA_VehRetResRQ", Namespace = "")]
        public ABG_OTA_VehRetResRQ ABG_OTA_VehRetResRQ { get; set; }

        [XmlElement(ElementName = "OTA_VehLocSearchRQ", Namespace = "")]
        public ABG_OTA_VehLocSearchRQ ABG_OTA_VehLocSearchRQ { get; set; }
    }

    #region 公用的节点

    public class VehRentalCore
    {
        [XmlAttribute(AttributeName = "PickUpDateTime")]
        public DateTime PickUpDateTime { get; set; }

        [XmlAttribute(AttributeName = "ReturnDateTime")]
        public DateTime ReturnDateTime { get; set; }

        [XmlElement(ElementName = "PickUpLocation")]
        public Location PickUpLocation { get; set; }

        [XmlElement(ElementName = "ReturnLocation")]
        public Location ReturnLocation { get; set; }
    }

    #endregion 公用的节点

    #region 公用请求

    [XmlRoot(ElementName = "VehGroup")]
    public class VehGroup
    {
        [XmlAttribute(AttributeName = "GroupType")]
        public string GroupType { get; set; }

        [XmlAttribute(AttributeName = "GroupValue")]
        public string GroupValue { get; set; }
    }

    public class CommonRequest
    {
        /// <summary>
        ///
        /// </summary>
        public ApiEnum Type { get; set; }

        /// <summary>
        /// 搜价接口
        /// </summary>

        public ABG_OTA_VehAvailRateRQ OTA_VehAvailRateRQ { get; set; }

        /// <summary>
        /// 速率规则
        /// </summary>
        public ABG_OTA_VehRateRuleRQ OTA_VehRateRuleRQ { get; set; }

        public ABG_OTA_VehResRQ ABG_OTA_VehResRQ { get; set; }

        /// <summary>
        /// 订单取消
        /// </summary>
        public ABG_OTAVehCancelRQ ABG_OTAVehCancelRQ { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public ABG_OTA_VehRetResRQ ABG_OTA_VehRetResRQ { get; set; }

        /// <summary>
        /// 地址详情
        /// </summary>
        public ABG_OTA_VehLocSearchRQ ABG_OTA_VehLocSearchRQ { get; set; }
    }

    public class VehClass
    {
        [XmlAttribute]
        public int Size { get; set; }
    }

    #endregion 公用请求
}