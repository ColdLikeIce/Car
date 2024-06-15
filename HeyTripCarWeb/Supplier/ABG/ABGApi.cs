using CommonCore.Mapper;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ABG.Config;
using HeyTripCarWeb.Supplier.ABG.Models.RQs;
using HeyTripCarWeb.Supplier.ABG.Models.RSs;
using HeyTripCarWeb.Supplier.ABG.Util;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;

namespace HeyTripCarWeb.Supplier.ABG
{
    public class ABGApi : ICarSupplierApi
    {
        private readonly ABGAppSetting _setting;
        private readonly IMapper _mapper;

        public ABGApi(IOptions<ABGAppSetting> options, IMapper mapper)
        {
            _setting = options.Value;
            _mapper = mapper;
        }

        #region 原始接口

        /// <summary>
        /// 查询车辆集合。
        /// 支持：IATA/AirportID/CityID/Geo
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> VehAvailRate(OTA_VehAvailRateRQ availRateRQ, int timeout = 45000)
        {
            // 创建 SOAP 请求实例
            var envelope = new CommonEnvelope
            {
                Header = new SOAP_ENV_Header
                {
                    Credentials = new Credentials
                    {
                        UserID = new EncodedString { Value = _setting.UserID, EncodingType = "xsd:string" },
                        Password = new EncodedString { Value = _setting.Password, EncodingType = "xsd:string" }
                    },
                    WSBangRoadmap = new WSBang_Roadmap()
                },
                Body = new SOAP_ENV_Body
                {
                    Request = new ns_Request
                    {
                        OTA_VehAvailRateRQ = availRateRQ
                    }
                }
            };

            // Serialize the object to XML
            // 序列化请求
            XmlSerializer serializer = new XmlSerializer(typeof(CommonEnvelope));
            string soapRequest;
            using (StringWriter writer = new Utf8StringWriter())
            {
                // 使用 XmlWriterSettings 控制命名空间的定义
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true; // 忽略 XML 声明
                settings.Indent = true;
                settings.NewLineOnAttributes = false;

                using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                {
                    // 添加命名空间声明
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
                    namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");
                    namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
                    namespaces.Add("ns", "http://wsg.avis.com/wsbang");

                    // 序列化对象
                    serializer.Serialize(xmlWriter, envelope, namespaces);
                }

                soapRequest = writer.ToString();
            }
            var res = ABGXmlHelper.PostRequest(_setting.Url, soapRequest);
            var model = ABGXmlHelper.GetResponse<OTA_VehAvailRateRS>(res);
            /* if (!string.IsNullOrWhiteSpace(model.Errors?.Error?.ShortText))
             {
                 throw new Exception(model.Errors.Error.ShortText);
             }*/
            if (model == null)
            {
            }
            var rentCore = model.VehAvailRSCore.VehRentalCore;
            var avails = model.VehAvailRSCore.VehVendorAvails.VehVendorAvail;
            foreach (var avail in avails)
            {
                var veh = avail.VehAvails.VehAvail;
                foreach (var ve in veh)
                {
                    var veCore = ve.VehAvailCore;
                    if (veCore.Status == "Available")
                    {
                        StdVehicle std = new StdVehicle();
                        std.Supplier = EnumCarSupplier.ABG;

                        std.VehicleCode = veCore.Vehicle?.VehMakeModel?.Code;
                        std.ProductCode = $"{EnumCarSupplier.ABG}_usertodo_{std.VehicleCode}";
                        std.VehicleName = veCore.Vehicle?.VehMakeModel?.Name;
                        std.DoorCount = (EnumCarDoorCount)veCore.Vehicle.VehType.DoorCount;
                        //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、
                        //8（旅行车）或 9（皮卡）
                        std.VehicleCategory = BuildCarType(veCore.Vehicle.VehType.VehicleCategory);
                        // 尺寸必须等于以下产品之一：1（小型）、2（小型型）、3（经济型）、4（紧凑型）、
                        //  5（中型）、6（中级）、7（标准）、8（全尺寸）、9（豪华）、10（高级）或11（小型货车）。
                        std.VehicleClass = (EnumCarVehicleClass)veCore.Vehicle.VehClass.Size;
                        std.AirConditioning = veCore.Vehicle.AirConditionInd;
                        std.DriveType = EnumCarDriveType.None; //驱动类型是啥  usertodo
                        //std.FuelType = //usertodo
                        std.TransmissionType = veCore.Vehicle.TransmissionType == "Manual" ? EnumCarTransmissionType.Manual : EnumCarTransmissionType.Automatic;
                        //std.FuelPolicy //usertodo
                        var rentalRate = veCore.RentalRate;
                        var rateDistance = rentalRate.RateDistance;
                        //里程限制
                        StdRateDistance rateinfo = new StdRateDistance
                        {
                            Unlimited = rateDistance.Unlimited,
                            DistUnitName = rateDistance.DistUnitName == "Km" ? EnumCarDistUnitName.Km : EnumCarDistUnitName.Mile,
                            Quantity = rateDistance.Quantity,
                            //VehiclePeriodUnitName 只有这三种“Rental Period”, “Day” or “Hour”
                            VehiclePeriodUnitName = rateDistance.VehiclePeriodUnitName == "Day" ? EnumCarPeriodUnitName.Day : rateDistance.VehiclePeriodUnitName == "Hour" ? EnumCarPeriodUnitName.Hour : EnumCarPeriodUnitName.RentalPeriod,
                        };
                        //Purpose枚举值 目的设置为：1租车2单程费用5升级5机场/城市/其他附加费5机场特许费5年龄附加费
                        //6额外距离收费6车辆登记费6以色列印花税7地方税7奥地利合同税7货物和服务税
                        //8额外里程/额外距离收费9额外10额外一周10天11小时13年龄附加费22预支付费用
                        //28可选费用
                        var charges = rentalRate.VehicleCharges.VehicleCharge.Where(n => n.Purpose == "6").ToList();
                        if (charges.Count > 0)
                        {
                            Log.Information($"charges数量为{charges}");
                            rateinfo.Currency = charges.FirstOrDefault().CurrencyCode;
                            rateinfo.Amount = charges.FirstOrDefault().Amount;
                            rateinfo.Description = charges.FirstOrDefault().Description;
                        }
                        std.RateDistance = rateinfo;
                    }
                }
            }
            return null;
        }

        public EnumCarVehicleCategory BuildCarType(string category)
        {
            switch (category)
            {
                case "1":
                    return EnumCarVehicleCategory.Car;

                case "2":
                    return EnumCarVehicleCategory.Van;

                case "3":
                    return EnumCarVehicleCategory.SUV;

                case "4":
                    return EnumCarVehicleCategory.Convertible;

                case "8":
                    return EnumCarVehicleCategory.StationWagon;

                case "9":
                    return EnumCarVehicleCategory.Pickup;
            }
            Log.Error($"遇到其他车型{category}");
            return EnumCarVehicleCategory.Car;
        }

        #endregion 原始接口

        #region 业务接口

        /// <summary>
        /// 查询车辆集合。该供应商支持：目前只发现 IATA
        /// </summary>
        /// <param name="vehicleRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            //构建请求body 其他都一样
            var availRateRQ = new OTA_VehAvailRateRQ
            {
                //返回请求数
                MaxResponses = 10000,
                ReqRespVersion = "medium",
                Version = "1.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID { ID = _setting.UserID, Type = 1 }
                    }
                },
                VehAvailRQCore = new VehAvailRQCore
                {
                    Status = "Available",
                    VehRentalCore = new VehRentalCore
                    {
                        //取车日期 还车日期
                        PickUpDateTime = Convert.ToDateTime(vehicleRQ.PickUpDateTime),
                        ReturnDateTime = Convert.ToDateTime(vehicleRQ.ReturnDateTime),
                        //出发地 目的地方
                        PickUpLocation = new Location { LocationCode = vehicleRQ.PickUpLocationCode },
                        ReturnLocation = new Location { LocationCode = vehicleRQ.ReturnLocationCode }
                    },
                    VendorPrefs = new VendorPrefs
                    {
                        VendorPref = new VendorPref { CompanyShortName = "Avis" }
                    },
                    //这个信息 必填 首选 Preferred
                    VehPrefs = new VehPrefs
                    {
                        VehPref = new VehPref
                        {
                            AirConditionPref = "Preferred", //指空调偏好
                            ClassPref = "Preferred",  //指车辆等级偏好
                            TransmissionPref = "Preferred", //指变速箱偏好。
                            TransmissionType = "Automatic", //指变速箱类型。
                            TypePref = "Preferred", //指车辆类型偏好
                            VehType = new VehType { VehicleCategory = "1" },  //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、8（旅行车）或 9（皮卡）。
                            VehClass = new VehClass { Size = 4 } //当 VehicleCategory 为 “Car(1)” 时，DoorCount 必须是 “2” 或 “4”。 其他值可忽略
                        }
                    },
                    //年龄
                    DriverType = new DriverType { Age = vehicleRQ.DriverAge },
                    //<!-- 包括全部价格：商务和休闲 -->
                    RateQualifier = new RateQualifier { RateCategory = "6" }
                },
                VehAvailRQInfo = new VehAvailRQInfo
                {
                    Customer = new Customer
                    {
                        //出生国家
                        Primary = new Primary
                        {
                            CitizenCountryName = new CitizenCountryName { Code = vehicleRQ.CitizenCountryCode }
                        }
                    }
                }
            };
            return await VehAvailRate(availRateRQ);
        }

        public Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            throw new NotImplementedException();
        }

        public Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        public Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            throw new NotImplementedException();
        }

        #endregion 业务接口
    }
}

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}