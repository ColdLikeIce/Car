using CommonCore.Mapper;
using HeyTripCarWeb.Db;
using HeyTripCarWeb.Share;
using HeyTripCarWeb.Supplier.ACE.Config;
using HeyTripCarWeb.Supplier.ACE.Models.Dbs;
using HeyTripCarWeb.Supplier.ACE.Models.RQs;
using HeyTripCarWeb.Supplier.ACE.Models.RSs;
using HeyTripCarWeb.Supplier.ACE.Util;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Drawing;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Twilio.TwiML.Voice;
using XiWan.Car.Business.Pay.PingPong.Models.RQs;
using XiWan.Car.BusinessShared.Enums;
using XiWan.Car.BusinessShared.Stds;
using static Dapper.SqlMapper;

namespace HeyTripCarWeb.Supplier.ACE
{
    public class ACEApi : IACEApi
    {
        private readonly IRepository<AceLocation> _locRepository;
        private readonly IRepository<AceReservation> _aceOrderRepository;
        private readonly IRepository<ACERateCache> _aceCacheRepository;
        private readonly AceAppSetting _setting;
        private readonly IMapper _mapper;

        public ACEApi(IOptions<AceAppSetting> options, IMapper mapper, IRepository<AceLocation> locRepository,
             IRepository<AceReservation> aceOrderRepository, IRepository<ACERateCache> aceCacheRepository)
        {
            _locRepository = locRepository;
            _setting = options.Value;
            _mapper = mapper;
            _aceOrderRepository = aceOrderRepository;
            _aceCacheRepository = aceCacheRepository;
        }

        #region 原始接口

        /// <summary>
        /// 构建门店营业时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Dictionary<string, string> BuildOperationTimeJson(OperationTime time)
        {
            var type = "Mon";
            if (time.Tue)
            {
                type = "Tus";
            }
            else if (time.Weds)
            {
                type = "Weds";
            }
            else if (time.Thur)
            {
                type = "Thur";
            }
            else if (time.Fri)
            {
                type = "Fri";
            }
            else if (time.Sat)
            {
                type = "Sat";
            }
            else if (time.Sun)
            {
                type = "Sun";
            }
            return new Dictionary<string, string> { { $"@{type.ToString()}", "true" }, { "@Start", time.Start }, { "@End", time.End } };
        }

        /// <summary>
        /// 构建门店信息
        /// </summary>
        /// <param name="rq"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<bool> BuildLocation(ACE_OTA_VehLocSearchRQ rq, int timeout = 4500)
        {
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""e1733a17-51d6-4f2a-9684-4ed0551db7ad""
            xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehLocSearchRS
                xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2016-01-20T15:00:04.2414232-05:00"" Target=""Test""
Version=""5.0""
                xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success   />
                <VehMatchedLocs>
                    <VehMatchedLoc>
                        <LocationDetail AtAirport=""true"" Code=""TEST01"" Name=""RezPower
Int’l Airport"" CodeContext=""RP"" AssocAirportLocList=""REZ"">
                            <Address>
                                <StreetNmbr>1600 Pennsylvania Ave NW</StreetNmbr>
                                <CityName>Washington</CityName>
                                <PostalCode>20500</PostalCode>
                                <StateProv  StateCode=""DC"">District of Columbia</StateProv>
                                <CountryName  Code=""US"">United States</CountryName>
                            </Address>
                            <Telephone PhoneTechType=""1"" PhoneNumber=""202-456-1111""
DefaultInd=""true"" />
                            <Telephone  PhoneTechType=""3"" PhoneNumber=""202-456-1111"" />
                            <AdditionalInfo>
                                <ParkLocation  Location=""3"" />
                                <CounterLocation  Location=""3"" />
                                <OperationSchedules>
                                    <OperationSchedule>
                                        <OperationTimes>
                                            <OperationTime  Mon=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Tue=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Weds=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Thur=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Fri=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Sat=""true"" Start=""07:00"" End=""23:00"" />
                                            <OperationTime  Sun=""true"" Start=""08:00"" End=""18:00"" />
                                        </OperationTimes>
                                    </OperationSchedule>
                                </OperationSchedules>
                                <Shuttle>
                                    <ShuttleInfos>
                                        <ShuttleInfo  Type=""Transportation"">
                                            <SubSection>
                                                <Paragraph>
                                                    <Text>RezPower Rent A Car</Text>
                                                </Paragraph>
                                            </SubSection>
                                        </ShuttleInfo>
                                        <ShuttleInfo  Type=""PickupInfo"">
                                            <SubSection>
                                                <Paragraph>
                                                    <Text>CALL 202-456-1111 FOR SHUTTLE</Text>
                                                </Paragraph>
                                            </SubSection>
                                        </ShuttleInfo>
                                        <ShuttleInfo  Type=""Miscellaneous"">
                                            <SubSection>
                                                <Paragraph>
                                                    <Text>RezPower Rent a Car is a short shuttle ride
from the terminal. On arrival from baggage claim using courtesy phone
call 202-456-1111 for shuttle. Proceed from baggage claim area thru
Red Doors 1 or 2 or Blue Doors 1 or 2 to ground transportation on
circular drive.</Text>
                                                </Paragraph>
                                            </SubSection>
                                        </ShuttleInfo>
                                    </ShuttleInfos>
                                </Shuttle>
                            </AdditionalInfo>
                        </LocationDetail>
                    </VehMatchedLoc>
                </VehMatchedLocs>
                <Vendor  CompanyShortName=""REZ"" TravelSector=""2"" Code=""RP"">RezPower
Rent A Car</Vendor>
            </OTA_VehLocSearchRS>
        </ns:Response>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            //var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehLocSearchRQ = rq, Type = 5 });
            var model = ACEXmlHelper.GetResponse<ACE_OTA_VehLocSearchRS>(res);
            List<AceLocation> locList = new List<AceLocation>();
            foreach (var matchedLoc in model.VehMatchedLocs)
            {
                List<Dictionary<string, string>> operationTimes = new List<Dictionary<string, string>>();
                foreach (var date in matchedLoc.LocationDetail.AdditionalInfo.OperationSchedules.OperationSchedule.OperationTimes.OperationTime)
                {
                    operationTimes.Add(BuildOperationTimeJson(date));
                }
                AceLocation location = new AceLocation
                {
                    LocationCode = matchedLoc.LocationDetail.Code,
                    LocationName = matchedLoc.LocationDetail.Name,
                    StreetNmbr = matchedLoc.LocationDetail.Address.StreetNmbr,
                    CityName = matchedLoc.LocationDetail.Address.CityName,
                    PostalCode = matchedLoc.LocationDetail.Address.PostalCode,
                    //StateProv = matchedLoc.LocationDetail.Address.StateProv.Name,
                    CountryCode = matchedLoc.LocationDetail.Address.CountryName.Code,
                    CountryName = matchedLoc.LocationDetail.Address.CountryName.Name,
                    PhoneNumber = matchedLoc.LocationDetail.Telephones.Find(t => t.DefaultInd)?.PhoneNumber,
                    AtAirport = matchedLoc.LocationDetail.AtAirport,
                    AssocAirportLocList = matchedLoc.LocationDetail.AssocAirportLocList,
                    AirportCode = matchedLoc.LocationDetail.AssocAirportLocList,
                    OperationTime = JsonConvert.SerializeObject(operationTimes),
                    ParkLocation = matchedLoc.LocationDetail.AdditionalInfo.ParkLocation?.Location.ToString(),
                    CounterLocation = matchedLoc.LocationDetail.AdditionalInfo.CounterLocation?.Location,
                    VendorName = model.Vendor.Name,
                    VendorCode = model.Vendor.Code,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                await _locRepository.InsertAsync(location);
                locList.Add(location);
            }
            return true;
        }

        /// <summary>
        /// 查询车辆集合。
        /// 支持：IATA/AirportID/CityID/Geo
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> VehAvailRate(ACE_OTA_VehAvailRateRQ availRateRQ, int timeout = 45000)
        {
            List<StdVehicle> result = new List<StdVehicle>();
            //var res = await BuildEnvelope(new CommonRequest { OTA_VehAvailRateRQ = availRateRQ, Type = 1 });
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""d0e417a0-86e3-46b9-907f-4ed0551d1ea9""
            xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehAvailRateRS
                xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                xmlns:xsi=""http:/2312/www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2015-12-02T11:54:11.8261044-05:00"" Target=""Test""
Version=""5.0""
                xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success  />《》

                <VehAvailRSCore>
                    <VehRentalCore PickUpDateTime=""2015-12-10T12:00:00""
ReturnDateTime=""2015-12-18T15:00:00"">
                        <PickUpLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                        <ReturnLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                    </VehRentalCore>
                    <VehVendorAvails>
                        <VehVendorAvail>
                            <Vendor CompanyShortName=""REZ"" TravelSector=""2""
Code=""RP"">RezPower Rent A Car</Vendor>
                            <VehAvails>
                                <VehAvail>
                                    <VehAvailCore  Status=""Available"">
                                        <Vehicle AirConditionInd=""true""
TransmissionType=""Automatic"" FuelType=""Unspecified""
DriveType=""Unspecified"" PassengerQuantity=""4"" BaggageQuantity=""3""
Code=""CCAR"" CodeContext=""SIPP"">
                                            <VehType  VehicleCategory=""1"" DoorCount=""2-4"" />
                                            <VehClass  Size=""4"" />
                                            <VehMakeModel  Name=""Hyundai Accent"" />
                                            <PictureURL>hyundai-accent-2013.png</PictureURL>
                                        </Vehicle>
                                        <RentalRate>
                                            <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                            <VehicleCharges>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""183.40""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                                    <Calculation UnitCharge=""183.40"" UnitName=""Week""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""22.93""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                                    <Calculation UnitCharge=""22.93"" UnitName=""Day""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""13.77""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                                    <Calculation UnitCharge=""4.59"" UnitName=""Hour""
Quantity=""3"" />
                                                </VehicleCharge>
                                            </VehicleCharges>
                                            <RateQualifier  RateCategory=""16"" />
                                            <RateRestrictions AdvancedBookingInd=""true""
GuaranteeReqInd=""true"" />
                                        </RentalRate>
                                        <TotalCharge RateTotalAmount=""209.09""
EstimatedTotalAmount=""337.67"" CurrencyCode=""USD"" />
                                        <Fees>
                                            <Fee CurrencyCode=""USD"" Amount=""11.01""
Description=""Percentage Discount"" IncludedInEstTotalInd=""true""
Purpose=""3"">
                                                <Calculation  UnitCharge=""220.10"" Percentage=""5.0"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                                <Calculation UnitCharge=""5.95"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""15.75""
Description=""License Recovery Fee"" IncludedInEstTotalInd=""true""
Purpose=""7"">
                                                <Calculation UnitCharge=""1.75"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""18.45""
Description=""State Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation UnitCharge=""2.05"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""22.24""
Description=""Airport Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""224.84"" Percentage=""9.89"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""18.59""
Description=""Sales Tax"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""265.53"" Percentage=""7.0"" />
                                            </Fee>
                                        </Fees>
                                        <Reference  Type=""16"" ID=""402398868"" />
                                    </VehAvailCore>
                                    <VehAvailInfo>
                                        <PaymentRules>
                                            <PaymentRule CurrencyCode=""USD"" Amount=""337.67""
RuleType=""2"" />
                                        </PaymentRules>
                                    </VehAvailInfo>
                                </VehAvail>
                                <VehAvail>
                                    <VehAvailCore  Status=""Available"">
                                        <Vehicle AirConditionInd=""true""
TransmissionType=""Automatic"" FuelType=""Unspecified""
DriveType=""Unspecified"" PassengerQuantity=""4"" BaggageQuantity=""3""
Code=""CCAR"" CodeContext=""SIPP"">
                                            <VehType  VehicleCategory=""1"" DoorCount=""2-4"" />
                                            <VehClass  Size=""4"" />
                                            <VehMakeModel  Name=""Hyundai Accent"" />
                                            <PictureURL>hyundai-accent-2013.png</PictureURL>
                                        </Vehicle>
                                        <RentalRate>
                                            <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                            <VehicleCharges>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""183.40""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                                    <Calculation UnitCharge=""183.40"" UnitName=""Week""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""22.93""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                                    <Calculation UnitCharge=""22.93"" UnitName=""Day""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""13.77""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                                    <Calculation UnitCharge=""4.59"" UnitName=""Hour""
Quantity=""3"" />
                                                </VehicleCharge>
                                            </VehicleCharges>
                                            <RateQualifier  RateCategory=""16"" />
                                            <RateRestrictions AdvancedBookingInd=""true""
GuaranteeReqInd=""true"" />
                                        </RentalRate>
                                        <TotalCharge RateTotalAmount=""220.10""
EstimatedTotalAmount=""350.61"" CurrencyCode=""USD"" />
                                        <Fees>
                                            <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                                <Calculation UnitCharge=""5.95"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""15.75""
Description=""License Recovery Fee"" IncludedInEstTotalInd=""true""
Purpose=""7"">
                                                <Calculation UnitCharge=""1.75"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""18.45""
Description=""State Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation UnitCharge=""2.05"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""23.33""
Description=""Airport Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""235.85"" Percentage=""9.89"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""19.43""
Description=""Sales Tax"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""277.63"" Percentage=""7.0"" />
                                            </Fee>
                                        </Fees>
                                        <Reference  Type=""16"" ID=""402398869"" />
                                    </VehAvailCore>
                                    <VehAvailInfo>
                                        <PaymentRules>
                                            <PaymentRule CurrencyCode=""USD"" Amount=""25.00""
RuleType=""3"" />
                                        </PaymentRules>
                                    </VehAvailInfo>
                                </VehAvail>
                                <VehAvail>
                                    <VehAvailCore  Status=""Available"">
                                        <Vehicle AirConditionInd=""true""
TransmissionType=""Automatic"" FuelType=""Unspecified""
DriveType=""Unspecified"" PassengerQuantity=""5"" BaggageQuantity=""5""
Code=""CFAR"" CodeContext=""SIPP"">
                                            <VehType  VehicleCategory=""3"" DoorCount=""2"" />
                                            <VehClass  Size=""4"" />
                                            <VehMakeModel  Name=""Mazda CX-5"" />
                                            <PictureURL>mazda-cx5-2013.png</PictureURL>
                                        </Vehicle>
                                        <RentalRate>
                                            <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                            <VehicleCharges>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""249.55""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                                    <Calculation UnitCharge=""249.55"" UnitName=""Week""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""31.19""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                                    <Calculation UnitCharge=""31.19"" UnitName=""Day""
Quantity=""1"" />
                                                </VehicleCharge>
                                                <VehicleCharge CurrencyCode=""USD"" Amount=""18.72""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                                    <Calculation UnitCharge=""6.24"" UnitName=""Hour""
Quantity=""3"" />
                                                </VehicleCharge>
                                            </VehicleCharges>
                                            <RateQualifier  RateCategory=""16"" />
                                            <RateRestrictions AdvancedBookingInd=""true""
GuaranteeReqInd=""true"" />
                                        </RentalRate>
                                        <TotalCharge RateTotalAmount=""299.46""
EstimatedTotalAmount=""443.92"" CurrencyCode=""USD"" />
                                        <Fees>
                                            <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                                <Calculation UnitCharge=""5.95"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""15.75""
Description=""License Recovery Fee"" IncludedInEstTotalInd=""true""
Purpose=""7"">
                                                <Calculation UnitCharge=""1.75"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""18.45""
Description=""State Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation UnitCharge=""2.05"" UnitName=""Day""
Quantity=""9"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""31.17""
Description=""Airport Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""315.21"" Percentage=""9.89"" />
                                            </Fee>
                                            <Fee CurrencyCode=""USD"" Amount=""25.54""
Description=""Sales Tax"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                <Calculation  UnitCharge=""364.83"" Percentage=""7.0"" />
                                            </Fee>
                                        </Fees>
                                        <Reference  Type=""16"" ID=""402398870"" />
                                    </VehAvailCore>
                                    <VehAvailInfo>
                                        <PaymentRules>
                                            <PaymentRule CurrencyCode=""USD"" Amount=""33.00""
RuleType=""3"" />
                                        </PaymentRules>
                                    </VehAvailInfo>
                                </VehAvail>
                            </VehAvails>
                        </VehVendorAvail>
                    </VehVendorAvails>
                </VehAvailRSCore>
            </OTA_VehAvailRateRS>
        </ns:Response>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            var model = ACEXmlHelper.GetResponse<ACE_OTA_VehAvailRateRS>(res);
            if (model != null)
            {
                var rentCore = model.VehAvailRSCore.VehRentalCore;
                var avails = model.VehAvailRSCore.VehVendorAvails;
                foreach (var avail in avails)
                {
                    var veh = avail.VehAvails;
                    foreach (var ve in veh)
                    {
                        var veCore = ve.VehAvailCore;
                        if (veCore.Status == "Available")
                        {
                            StdVehicle std = new StdVehicle();
                            std.Supplier = EnumCarSupplier.None;
                            std.Vendor = EnumCarVendor.ACE_Rent_A_Car;
                            std.VehicleCode = veCore.Vehicle?.Code;
                            std.ProductCode = $"ACE_usertodo_{std.VehicleCode}";
                            std.VehicleName = veCore.Vehicle?.VehMakeModel?.Name;
                            std.DoorCount = BuildDoorCount(veCore.Vehicle.VehType.DoorCount);
                            //VehicleCategory 必须等于以下之一：1（轿车）、2（面包车）、3（SUV）、4（敞篷车）、
                            //8（旅行车）或 9（皮卡）
                            std.VehicleCategory = (EnumCarVehicleCategory)veCore.Vehicle.VehType.VehicleCategory;// BuildCarType(veCore.Vehicle.VehType.VehicleCategory);
                            // 尺寸必须等于以下产品之一：1（小型）、2（小型型）、3（经济型）、4（紧凑型）、
                            //  5（中型）、6（中级）、7（标准）、8（全尺寸）、9（豪华）、10（高级）或11（小型货车）。
                            std.VehicleClass = (EnumCarVehicleClass)veCore.Vehicle.VehClass.Size;
                            std.AirConditioning = veCore.Vehicle.AirConditionInd;
                            if (!string.IsNullOrWhiteSpace(veCore.Vehicle.DriveType))
                            {
                                std.DriveType = EnumHelper.GetEnumTypeByStr<EnumCarDriveType>(veCore.Vehicle.DriveType);// BuildDriveType(veCore.Vehicle.DriveType); //驱动类型是啥
                            }
                            std.FuelType = EnumHelper.GetEnumTypeByStr<EnumCarFuelType>(veCore.Vehicle.FuelType);
                            std.PassengerQuantity = veCore.Vehicle.PassengerQuantity;
                            std.PassengerQuantity = veCore.Vehicle.BaggageQuantity;
                            std.PictureURL = veCore.Vehicle.PictureURL;
                            std.TransmissionType = veCore.Vehicle.TransmissionType == "Manual" ? EnumCarTransmissionType.Manual : EnumCarTransmissionType.Automatic;

                            /*   std.MinDriverAge = 0; //userloss
                               std.MaxDriverAge = 99; //userloss*/
                            var rentalRate = veCore.RentalRate;
                            var rateDistance = rentalRate.RateDistance;
                            //里程限制
                            StdRateDistance rateinfo = new StdRateDistance
                            {
                                Unlimited = rateDistance.Unlimited,
                                DistUnitName = rateDistance.DistUnitName == "Km" ? EnumCarDistUnitName.Km : EnumCarDistUnitName.Mile,
                                Quantity = rateDistance.Quantity,
                                VehiclePeriodUnitName = (EnumCarPeriodUnitName)Enum.Parse(typeof(EnumCarPeriodUnitName), rateDistance.VehiclePeriodUnitName)
                            };
                            //Purpose枚举值 目的设置为：1租车2单程费用5升级5机场/城市/其他附加费5机场特许费5年龄附加费
                            //6额外距离收费6车辆登记费6以色列印花税7地方税7奥地利合同税7货物和服务税
                            //8额外里程/额外距离收费9额外10额外一周10天11小时13年龄附加费22预支付费用
                            //28可选费用
                            if (rentalRate.RateDistance.Unlimited == false)
                            {
                                var charges = rentalRate.VehicleCharges.Where(n => n.Purpose == "6").ToList();
                                if (charges.Count > 0)
                                {
                                    Log.Information($"charges数量为{charges}");
                                    rateinfo.Currency = charges.FirstOrDefault().CurrencyCode;
                                    rateinfo.Amount = charges.FirstOrDefault().Amount;
                                    rateinfo.Description = charges.FirstOrDefault().Description;
                                }
                            }
                            std.RateDistance = rateinfo;
                            //价格
                            var totalEl = veCore.TotalCharge;
                            var totalCharge = new StdTotalCharge
                            {
                                PayType = EnumCarPayType.Prepaid, //到付 usertodo RuleType
                                PriceType = EnumCarPriceType.NetRate, //底价模式
                                Currency = totalEl.CurrencyCode,
                                TotalAmount = totalEl.EstimatedTotalAmount,
                                RentalAmount = totalEl.RateTotalAmount
                            };
                            //其他费用
                            List<StdCurrencyAmount> stdAmount = new List<StdCurrencyAmount>();
                            var FeesList = veCore.Fees;

                            foreach (var item in FeesList)
                            {
                                StdCurrencyAmount stdCurrencyAmount = new StdCurrencyAmount
                                {
                                    Type = EnumCarCoverageType.MEDIUM, //usertodo
                                    //PayWhen = //usertodo
                                    Desc = item.Description,
                                    Currency = item.CurrencyCode,
                                    Amount = item.Amount
                                };
                                stdAmount.Add(stdCurrencyAmount);
                            }
                            //押金添加进来
                            var payRule = ve.VehAvailInfo.PaymentRules;
                            if (payRule.Count > 0)
                            {
                                StdCurrencyAmount red = new StdCurrencyAmount
                                {
                                    Type = EnumCarCoverageType.Deposit,
                                    Desc = "",
                                    Currency = payRule.FirstOrDefault().CurrencyCode,
                                    Amount = payRule.FirstOrDefault().Amount,
                                    PayWhen = EnumCarPayWhen.DropOff,
                                };
                                stdAmount.Add(red);
                            }
                            //设备信息
                            var equips = veCore.PricedEquips;

                            List<StdPricedEquip> stdeq = new List<StdPricedEquip>();
                            foreach (var equip in equips)
                            {
                                var charge = equip.Charge;
                                StdPricedEquip stdPricedEquip = new StdPricedEquip
                                {
                                    IncludedInEstTotalInd = charge.IncludedInEstTotalInd,
                                    Currency = charge.CurrencyCode,
                                    UnitPrice = charge.Amount,
                                    MaxPrice = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    EquipDescription = charge.Description,
                                    EquipType = BuildEquipType(equip.Equipment.EquipType, charge.Description)
                                };
                                stdeq.Add(stdPricedEquip);
                            }
                            std.PricedEquips = stdeq;
                            //ratecode
                            std.RateCode = $"{EnumCarVendor.ACE_Rent_A_Car}_{veCore.Reference.Type}_{veCore.Reference.ID}";
                            //保险
                            List<StdPricedCoverage> stdPricedCoverages = new List<StdPricedCoverage>();
                            foreach (var pricedCoverage in veCore.PricedCoverages)
                            {
                                var charge = pricedCoverage.Charge;
                                var code = pricedCoverage.Coverage.Code;
                                StdPricedCoverage stdPriced = new StdPricedCoverage()
                                {
                                    CoverageType = BuildEnumCarCoverageType(pricedCoverage.Coverage.Code),
                                    CoverageDescription = charge.Description,
                                    Description = charge.Description,
                                    CurrencyCode = charge.CurrencyCode,
                                    Amount = charge.Amount,
                                    TaxInclusive = charge.TaxInclusive,
                                    IncludedInRate = charge.IncludedInRate,
                                    IncludedInEstTotalInd = charge.IncludedInEstTotalInd,
                                    Calculation = new StdCalculation
                                    {
                                        Quantity = charge.Calculation.Quantity,
                                        UnitName = charge.Calculation.UnitName,
                                        UnitCharge = charge.Calculation.UnitCharge
                                    },
                                    // MaxCharge = Charge.MinMax.MaxCharge, //userloss
                                    MinCharge = pricedCoverage.Deductible.Amount,  //起赔额
                                };
                                stdPricedCoverages.Add(stdPriced);
                                if (stdPriced.CoverageType == EnumCarCoverageType.CollisionDamageWaiver)
                                {
                                    StdCurrencyAmount otherAmount = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.CollisionDamageWaiver,
                                        Desc = stdPriced.Description,
                                        Currency = stdPriced.CurrencyCode,
                                        Amount = stdPriced.Amount,
                                        PayWhen = EnumCarPayWhen.PickUp, //usertodo
                                    };
                                    stdAmount.Add(otherAmount);
                                }
                                if (stdPriced.CoverageType == EnumCarCoverageType.ThirdPartyLiability)
                                {
                                    StdCurrencyAmount otherAmount = new StdCurrencyAmount
                                    {
                                        Type = EnumCarCoverageType.ThirdPartyLiability,
                                        Desc = stdPriced.Description,
                                        Currency = stdPriced.CurrencyCode,
                                        Amount = stdPriced.Amount,
                                        PayWhen = EnumCarPayWhen.PickUp, //usertodo
                                    };
                                    stdAmount.Add(otherAmount);
                                }
                            }
                            std.PricedCoverages = stdPricedCoverages;
                            //其他金额 摘要
                            totalCharge.OtherAmounts = stdAmount;
                            std.TotalCharge = totalCharge;

                            var fuelPolicy = rentalRate.VehicleCharges.Where(n => n.Description.Contains("FUEL POLICY")).ToList();
                            if (fuelPolicy.Count > 0)
                            {
                                //燃油政策
                                if (fuelPolicy.Exists(n => n.Description.Contains("Full To Full")))
                                {
                                    std.FuelPolicy = EnumCarFuelPolicy.FullToFull;
                                }
                                else
                                {
                                    Log.Information($"燃油政策:{JsonConvert.SerializeObject(fuelPolicy)}");
                                }
                            }
                            //
                            var rateRule = await GetRateRule(veCore);
                            if (rateRule != null)
                            {
                                List<StdVehicleCharge> vehCharges = new List<StdVehicleCharge>();
                                var vehChargesList = rentalRate.VehicleCharges;
                                //费率

                                List<StdFee> stdfee = new List<StdFee>();
                                foreach (var fe in FeesList)
                                {
                                    var stdFee = _mapper.Map<Fee, StdFee>(fe);
                                    stdfee.Add(stdFee);
                                }
                                std.Fees = stdfee;
                                //车辆收费
                                var vehicleChargesList = vehChargesList.ToList();
                                foreach (var item in vehicleChargesList)
                                {
                                    Log.Information($"车辆收费信息{JsonConvert.SerializeObject(item)}");

                                    StdVehicleCharge newItem = new StdVehicleCharge
                                    {
                                        Purpose = EnumCarCoverageType.OtherTaxesAndServiceCharges,
                                        PurposeDescription = item.Description,
                                        Description = item.Description,
                                        CurrencyCode = item.CurrencyCode,
                                        Amount = item.Amount,
                                        IncludedInEstTotalInd = item.IncludedInEstTotalInd,
                                        TaxInclusive = item.TaxInclusive,
                                    };
                                    if (item.Calculation != null)
                                    {
                                        var calculation = _mapper.Map<Calculation, StdCalculation>(item.Calculation);
                                        newItem.Calculation = calculation;
                                    }
                                    vehCharges.Add(newItem);
                                }
                                std.VehicleCharges = vehCharges;
                                //取车地点 还车地点
                                var loc = rateRule.LocationDetails.FirstOrDefault();
                                var startLoc = BuildLocationDetail(loc);
                                var stdLoc = new StdLocation { PickUp = startLoc };
                                if (rateRule.LocationDetails.Count > 1)
                                {
                                    var endLoc = BuildLocationDetail(rateRule.LocationDetails.LastOrDefault());
                                    stdLoc.DropOff = endLoc;
                                }
                                std.Location = stdLoc;
                                //政策条款
                                var vendorMessages = rateRule.VendorMessages;
                                List<StdTermAndCondition> termsAndConditions = new List<StdTermAndCondition>();
                                foreach (var msg in vendorMessages)
                                {
                                    StdTermAndCondition sut = new StdTermAndCondition
                                    {
                                        Code = msg.InfoType.ToString(),
                                        Sections = new List<StdSection>
                                        {
                                            new StdSection{Text = msg.SubSection.Paragraph.Text}
                                        }
                                    };
                                    termsAndConditions.Add(sut);
                                }
                                std.TermsAndConditions = termsAndConditions;
                            }
                            result.Add(std);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 构建取车还车地址
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        private StdLocationInfo BuildLocationDetail(LocationDetails loc)
        {
            StdLocationInfo startLoc = new StdLocationInfo
            {
                //CounterLocation = BuildLocationType(), //usertodo
                LocationName = loc.Name,
                //LocationId=loc.Code, //locationid没有？
                CountryCode = loc.Address.CountryName.Code,
                CountryName = loc.Address.CountryName.Name,
                StateProv = loc.Address.StateProv?.StateName,
                CityName = loc.Address.CityName,
                //Address = loc.Address.AddressLine,
                PostalCode = loc.Address.PostalCode,
                Telephone = loc.Telephone.FirstOrDefault()?.PhoneNumber,
                VendorLocId = loc.AdditionalInfo.ParkLocation?.Location.ToString(), //停车位置 usertodo
                SuppLocId = loc.AdditionalInfo.CounterLocation?.Location.ToString(),//柜台位置 usertodo
                ParkLocation = $"{loc.AdditionalInfo.ParkLocation?.Location.ToString()}"
            };
            List<StdOperationTime> operationTimes = new List<StdOperationTime>();
            foreach (var date in loc.AdditionalInfo.OperationSchedules.OperationSchedule.OperationTimes.OperationTime)
            {
                operationTimes.Add(BuildOperationTime(date));
            }
            startLoc.OperationTimes = operationTimes;
            return startLoc;
        }

        /// <summary>
        /// 构建门店营业时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public StdOperationTime BuildOperationTime(OperationTime time)
        {
            var type = EnumCarWeek.Monday;
            if (time.Tue)
            {
                type = EnumCarWeek.Tuesday;
            }
            else if (time.Weds)
            {
                type = EnumCarWeek.Wednesday;
            }
            else if (time.Thur)
            {
                type = EnumCarWeek.Thursday;
            }
            else if (time.Fri)
            {
                type = EnumCarWeek.Friday;
            }
            else if (time.Sat)
            {
                type = EnumCarWeek.Saturday;
            }
            else if (time.Sun)
            {
                type = EnumCarWeek.Sunday;
            }
            return new StdOperationTime
            {
                Week = type,
                Times = new List<StdTime> { new StdTime { Start = time.Start, End = time.End } }
            };
        }

        /// <summary>
        /// 构建设备类型(不全)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        private EnumCarEquipType BuildEquipType(string type, string desc)
        {
            switch (type)
            {
                case "7":
                    return EnumCarEquipType.InfantSeat;

                case "8":
                    return EnumCarEquipType.ToddlerSeats;

                case "9":
                    return EnumCarEquipType.BoosterSeat;

                case "46":
                    return EnumCarEquipType.GPS;
            }
            Log.Information($"出现没有命中的设备类型{type}【{desc}】");
            return EnumCarEquipType.None;
        }

        /// <summary>
        /// 转化保险类型(不全)
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public EnumCarCoverageType BuildEnumCarCoverageType(string code)
        {
            if (code.Contains("CDW"))
            {
                return EnumCarCoverageType.CollisionDamageWaiver;
            }
            else if (code.Contains("TPW"))
            {
                return EnumCarCoverageType.TheftProtection;
            }
            else if (code.Contains("3PI"))
            {
                return EnumCarCoverageType.ThirdPartyLiability;
            }
            else if (code.Contains("CCP"))
            {
                Log.Information($"CCP保险");
                return EnumCarCoverageType.BaseExcess;
            }
            Log.Information($"出现其他保险类型{code}");
            return EnumCarCoverageType.None;
        }

        /// <summary>
        /// 获取费率规则
        /// </summary>
        /// <param name="availRateRQ"></param>
        /// <param name="veCore"></param>
        /// <returns></returns>
        public async Task<ACE_OTA_VehRateRuleRS> GetRateRule(VehAvailCore veCore)
        {
            var ruleRq = new ACE_OTA_VehRateRuleRQ
            {
                //返回请求数
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                Reference = veCore.Reference
            };
            //var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehRateRuleRQ = ruleRq, Type = 2 });
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""d0e417a0-86e3-46b9-907f-4ed0551d1ea9"" xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehRateRuleRS xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2015-11-18T14:10:50.2858122-05:00"" Target=""Test""
Version=""5.0"" xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success  />
                <VehRentalCore PickUpDateTime=""2016-01-05T12:00:00""
ReturnDateTime=""2016-01-08T12:00:00"">
                    <PickUpLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                    <ReturnLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                </VehRentalCore>
                <Vehicle AirConditionInd=""false"" TransmissionType=""Manual""
FuelType=""Unspecified"" DriveType=""Unspecified"" PassengerQuantity=""2""
BaggageQuantity=""2"" Code=""MBMN"" CodeContext=""SIPP"">
                    <VehType  VehicleCategory=""1"" DoorCount=""2"" />
                    <VehClass  Size=""1"" />
                    <VehMakeModel  Name=""Fiat 500"" />
                    <PictureURL>fiat-500-2013.png</PictureURL>
                </Vehicle>
                <RentalRate>
                    <RateDistance Unlimited=""true"" DistUnitName=""Km""
VehiclePeriodUnitName=""RentalPeriod"" />
                        <VehicleCharges>
                            <VehicleCharge CurrencyCode=""GBP"" Amount=""43.50""
Description=""Daily Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                <Calculation  UnitCharge=""14.50"" UnitName=""Day"" Quantity=""3"" />
                            </VehicleCharge>
                        </VehicleCharges>
                        <RateQualifier RateCategory=""16"" RateQualifier=""FLYDRV""
RatePeriod=""Daily"" />
                            <RateRestrictions  AdvancedBookingInd=""true"" />
                        </RentalRate>
                        <TotalCharge RateTotalAmount=""43.50"" EstimatedTotalAmount=""52.20""
CurrencyCode=""GBP"" />
                            <RateRules  MinimumKeep=""P1D"" MaximumKeep=""P6D"" MaximumRental=""P6D"">
                                <AdvanceBooking OffsetTimeUnit=""Hour"" OffsetUnitMultiplier=""3""
RequiredInd=""true"" />
                                    <PickupReturnRules>
                                        <EarliestPickup  Time=""2016-01-05T12:00:00"" />
                                        <LatestPickup  Time=""2016-01-05T14:00:00"" />
                                        <LatestReturn  Time=""2016-01-08T12:00:00"" />
                                    </PickupReturnRules>
                                    <RateGuarantee  OffsetTimeUnit=""Day"" OffsetUnitMultiplier=""180"" />
                                    <PaymentRules>
                                        <AcceptablePayments>
                                            <AcceptablePayment  CreditCardCode=""VI"" />
                                            <AcceptablePayment  CreditCardCode=""MC"" />
                                            <AcceptablePayment  CreditCardCode=""AX"" />
                                            <AcceptablePayment  CreditCardCode=""DS"" />
                                        </AcceptablePayments>
                                    </PaymentRules>
                                </RateRules>
                                <LocationDetails AtAirport=""true"" Code=""Test01"" Name=""Sample Airport""
CodeContext=""RP"" AssocAirportLocList=""Rez"">
                                    <Address>
                                        <StreetNmbr>Stamford Bridge</StreetNmbr>
                                        <AddressLine>Fulham Road</AddressLine>
                                        <CityName>London</CityName>
                                        <PostalCode>SW61HS</PostalCode>
                                        <CountryName  Code=""GB"">United Kingdom</CountryName>
                                    </Address>
                                    <Telephone PhoneTechType=""1"" PhoneNumber=""442079582190""
DefaultInd=""true"" />
                                        <Telephone  PhoneTechType=""3"" PhoneNumber=""317-248-7251"" />
                                        <AdditionalInfo>
                                            <ParkLocation  Location=""3"" />
                                            <CounterLocation  Location=""3"" />
                                            <OperationSchedules>
                                                <OperationSchedule>
                                                    <OperationTimes>
                                                        <OperationTime  Mon=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Tue=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Weds=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Thur=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Fri=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Sat=""true"" Start=""06:00"" End=""21:00"" />
                                                        <OperationTime  Sun=""true"" Start=""06:00"" End=""21:00"" />
                                                    </OperationTimes>
                                                </OperationSchedule>
                                            </OperationSchedules>
                                            <Shuttle>
                                                <ShuttleInfos>
                                                    <ShuttleInfo  Type=""Transportation"">
                                                        <SubSection>
                                                            <Paragraph>
                                                                <Text>RezPower Car Rental</Text>
                                                            </Paragraph>
                                                        </SubSection>
                                                    </ShuttleInfo>
                                                    <ShuttleInfo  Type=""PickupInfo"">
                                                        <SubSection>
                                                            <Paragraph>
                                                                <Text>Call 442079582190</Text>
                                                            </Paragraph>
                                                        </SubSection>
                                                    </ShuttleInfo>
                                                    <ShuttleInfo  Type=""Miscellaneous"">
                                                        <SubSection>
                                                            <Paragraph>
                                                                <Text>On arrival at Sample Airport please notify the
branch that you require picking up, AFTER collecting your luggage call
442079582190. On return of your vehicle, please return to the RezPower
Office. The branch will take you back to the airport. Please allow an
extra 10 minutes in case of busy periods.</Text>
                                                                </Paragraph>
                                                            </SubSection>
                                                        </ShuttleInfo>
                                                    </ShuttleInfos>
                                                </Shuttle>
                                            </AdditionalInfo>
                                        </LocationDetails>
                                        <VendorMessages>
                                            <VendorMessage  InfoType=""2"">
                                                <SubSection>
                                                    <Paragraph>
                                                        <Text>RezPower vehicles can only be used on the UK mainland
and either Ireland or France. The cost will be calculated as the
Standard rental + £125 surcharge + £20 per part calendar day.
Permission is not assured and must be sought from the RezPower supplying
location 7 days prior to any overseas use. If prior notice is not given
then permission at the rental counter may be refused. When a vehicle is
taken overseas a daily mileage limit of 100 miles will be applied to the
ENTIRE rental. Mileage over the agreed amount with be charged at 50
pence per mile. Renters aged 21-24 are only permitted to rent only
Mini, Economy and Compact Sizes with Manual Transmission. Renters aged
25-29 may rent any vehicle except for Vans or SUVs. No restriction for
renters 30yrs+</Text>
                                                        </Paragraph>
                                                    </SubSection>
                                                </VendorMessage>
                                            </VendorMessages>
                                        </OTA_VehRateRuleRS>
                                    </ns:Response>
                                </SOAP-ENV:Body>
                            </SOAP-ENV:Envelope>";
            var model = ACEXmlHelper.GetResponse<ACE_OTA_VehRateRuleRS>(res);
            return model;
        }

        private EnumCarDoorCount BuildDoorCount(string str)
        {
            if (str.Contains("2") && str.Contains("4"))
            {
                return EnumCarDoorCount.Door2_4;
            }
            else if (str.Contains("4") && str.Contains("5"))
            {
                return EnumCarDoorCount.Door4_5;
            }
            else if (str.Contains("2") && str.Contains("3"))
            {
                return EnumCarDoorCount.Door2_3;
            }
            else if (str.Contains("2"))
            {
                return EnumCarDoorCount.Door2;
            }
            else if (str.Contains("3"))
            {
                return EnumCarDoorCount.Door3;
            }
            else if (str.Contains("4"))
            {
                return EnumCarDoorCount.Door4;
            }
            else if (str.Contains("5"))
            {
                return EnumCarDoorCount.Door5;
            }
            return EnumCarDoorCount.None;
        }

        public async Task<string> BuildEnvelope(CommonRequest model)
        {
            var body = new Body();
            switch (model.Type)
            {
                case 1:
                    body.OTA_VehAvailRateRQ = model.OTA_VehAvailRateRQ;
                    break;

                case 2:
                    body.ACE_OTA_VehRateRuleRQ = model.ACE_OTA_VehRateRuleRQ;
                    break;

                case 3:
                    body.ACE_OTA_VehResRQ = model.ACE_OTA_VehResRQ;
                    break;

                case 4:
                    body.ACE_OTA_VehRetResRQ = model.ACE_OTA_VehRetResRQ;
                    break;

                case 5:
                    body.ACE_OTA_VehLocSearchRQ = model.ACE_OTA_VehLocSearchRQ;
                    break;
            }
            var envelope = new ACEEnvelope
            {
                Body = body
            };
            return await ACEXmlHelper.PostRequest(_setting.Url, envelope);
        }

        /// <summary>
        /// 创建预订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ originModel, ACE_OTA_VehResRQ model, int timeout = 45000)
        {
            StdCreateOrderRS result = new StdCreateOrderRS();
            //var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehResRQ = model, Type = 3 });
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope
    xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""d0e417a0-86e3-46b9-907f-4ed0551d1ea9""
            xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehResRS
                xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2015-11-23T10:07:27.777118-05:00"" Target=""Test"" Version=""5.0""
                xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success  />
                <VehResRSCore>
                    <VehReservation>
                        <Customer>
                            <Primary  BirthDate=""1961-06-01"">
                                <PersonName>
                                    <GivenName>First Name</GivenName>
                                    <MiddleName>Middle Name</MiddleName>
                                    <Surname>Last Name</Surname>
                                </PersonName>
                                <Telephone CountryAccessCode=""01"" AreaCityCode=""123""
PhoneNumber=""123-1234"" Extension=""12345"" />
                                <Email>test@test.com</Email>
                                <Address>
                                    <StreetNmbr>Address 1</StreetNmbr>
                                    <AddressLine>Address 2</AddressLine>
                                    <CityName>City Name</CityName>
                                    <PostalCode>Postal Code</PostalCode>
                                    <StateProv>State Name</StateProv>
                                    <CountryName>Country Name</CountryName>
                                </Address>
                                <Document DocIssueLocation=""IN"" DocID=""1234123412341234""
DocType=""4"" ExpireDate=""2010-01-01"" />
                            </Primary>
                        </Customer>
                        <VehSegmentCore>
                            <ConfID  Type=""14"" ID=""RZF1234567"" />
                            <Vendor CompanyShortName=""Rez"" TravelSector=""2""
Code=""RP"">RezPower Rent A Car</Vendor>
                            <VehRentalCore PickUpDateTime=""2015-12-01T12:00:00""
ReturnDateTime=""2015-12-09T15:00:00"">
                                <PickUpLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                                <ReturnLocation  LocationCode=""TEST01"" CodeContext=""RP"" />
                            </VehRentalCore>
                            <Vehicle AirConditionInd=""true"" TransmissionType=""Automatic""
FuelType=""Unspecified"" DriveType=""Unspecified"" PassengerQuantity=""4""
BaggageQuantity=""3"" Code=""CCAR"" CodeContext=""SIPP"">
                                <VehType  VehicleCategory=""1"" DoorCount=""2-4"" />
                                <VehClass  Size=""4"" />
                                <VehMakeModel  Name=""Hyundai Accent"" />
                                <PictureURL>hyundai-accent-2013.png</PictureURL>
                            </Vehicle>
                            <RentalRate>
                                <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                <VehicleCharges>
                                    <VehicleCharge CurrencyCode=""USD"" Amount=""183.40""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                        <Calculation UnitCharge=""183.40"" UnitName=""Week""
Quantity=""1"" />
                                    </VehicleCharge>
                                    <VehicleCharge CurrencyCode=""USD"" Amount=""22.93""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                        <Calculation UnitCharge=""22.93"" UnitName=""Day""
Quantity=""1"" />
                                    </VehicleCharge>
                                    <VehicleCharge CurrencyCode=""USD"" Amount=""13.77""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                        <Calculation UnitCharge=""4.59"" UnitName=""Hour""
Quantity=""3"" />
                                    </VehicleCharge>
                                </VehicleCharges>
                                <RateQualifier RateCategory=""16"" RateQualifier=""FLYDRV""
RatePeriod=""Weekly"" />
                                <RateRestrictions  AdvancedBookingInd=""true"" />
                            </RentalRate>
                            <Fees>
                                <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                    <Calculation UnitCharge=""5.95"" UnitName=""Day"" Quantity=""9""
/>
                                </Fee>
                                <Fee CurrencyCode=""USD"" Amount=""15.75"" Description=""License
Recovery Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                    <Calculation UnitCharge=""1.75"" UnitName=""Day"" Quantity=""9""
/>
                                </Fee>
                                <Fee CurrencyCode=""USD"" Amount=""18.45"" Description=""State
Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                    <Calculation UnitCharge=""2.05"" UnitName=""Day"" Quantity=""9""
/>
                                </Fee>
                                <Fee CurrencyCode=""USD"" Amount=""23.33"" Description=""Airport
Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                    <Calculation  UnitCharge=""235.85"" Percentage=""9.89"" />
                                </Fee>
                                <Fee CurrencyCode=""USD"" Amount=""19.43"" Description=""Sales Tax""
IncludedInEstTotalInd=""true"" Purpose=""7"">
                                    <Calculation  UnitCharge=""277.63"" Percentage=""7.0"" />
                                </Fee>
                            </Fees>
                            <TotalCharge RateTotalAmount=""220.10""
EstimatedTotalAmount=""350.61"" CurrencyCode=""USD"" />
                        </VehSegmentCore>
                        <VehSegmentInfo>
                            <LocationDetails AtAirport=""true"" Code=""TEST01"" Name=""Sample
Int’l Airport"" CodeContext=""AC"" AssocAirportLocList=""SAM"">
                                <Address>
                                    <StreetNmbr>537 Paper Street</StreetNmbr>
                                    <CityName>Bradford</CityName>
                                    <PostalCode>19808</PostalCode>
                                    <StateProv  StateCode=""DE"">Delaware</StateProv>
                                    <CountryName  Code=""US"">United States</CountryName>
                                </Address>
                                <Telephone PhoneTechType=""1"" PhoneNumber=""288-555-0153""
DefaultInd=""true"" />
                                <Telephone  PhoneTechType=""3"" PhoneNumber=""288-555-0153"" />
                            </LocationDetails>
                        </VehSegmentInfo>
                    </VehReservation>
                </VehResRSCore>
            </OTA_VehResRS>
            <!--                                                                Processed by Direct Connect ver. 15.65 build 1 -->
        </ns:Response>
    </SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
            var spModel = ACEXmlHelper.GetResponse<ACE_OTA_VehResRS>(res);
            if (spModel == null)
            {
                throw new Exception($"解析实体错误{res}");
            }
            if (spModel.Errors != null && spModel.Errors.ErrorList.Count > 0)
            {
                result.OrderSuc = false;
                result.Message = String.Join(",", spModel.Errors.ErrorList.Select(n => n.Message));
                return result;
            }
            //自己db也存一份？
            var veCore = spModel.VehResRSCore.VehReservation.VehSegmentCore;
            var loc = spModel.VehResRSCore.VehReservation.VehSegmentInfo.LocationDetails;
            var customer = spModel.VehResRSCore.VehReservation.Customer;
            AceReservation order = new AceReservation
            {
                OrderNo = originModel.OrderNo,
                ReservationId = veCore.ConfID.ID,
                ReservationType = veCore.ConfID.Type,
                PickUpDateTime = veCore.VehRentalCore.PickUpDateTime,
                ReturnDateTime = veCore.VehRentalCore.ReturnDateTime,
                PickUpLocationCode = veCore.VehRentalCore.PickUpLocation.LocationCode,
                ReturnLocationCode = veCore.VehRentalCore.ReturnLocation.LocationCode,
                CodeContext = veCore.VehRentalCore.PickUpLocation.CodeContext,
                SIPP = veCore.Vehicle.Code,
                TransmissionType = veCore.Vehicle.TransmissionType,
                AirConditionInd = veCore.Vehicle.AirConditionInd.ToString(),
                DriveType = veCore.Vehicle.DriveType,
                PassengerQuantity = veCore.Vehicle.PassengerQuantity.ToString(),
                BaggageQuantity = veCore.Vehicle.BaggageQuantity.ToString(),
                FuelType = veCore.Vehicle.FuelType,
                VehicleCategory = veCore.Vehicle.VehType.VehicleCategory.ToString(),
                DoorCount = veCore.Vehicle.VehType.DoorCount.ToString(),
                VehClass = veCore.Vehicle.VehClass.Size.ToString(),
                CarName = veCore.Vehicle.VehMakeModel.Name,
                PictureURL = veCore.Vehicle.PictureURL,
                RateDistanceInfo = JsonConvert.SerializeObject(veCore.RentalRate.RateDistance),
                RateTotalAmount = veCore.TotalCharge.RateTotalAmount,
                EstimatedTotalAmount = veCore.TotalCharge.EstimatedTotalAmount,
                CurrencyCode = veCore.TotalCharge.CurrencyCode,
                Location_Code = loc.Code,
                BirthDate = customer.Primary.BirthDate.ToString(),
                GivenName = customer.Primary.PersonName.GivenName,
                MiddleName = customer.Primary.PersonName.MiddleName,
                Surname = customer.Primary.PersonName.Surname,
                Email = customer.Primary.Email,
                Telephone = customer.Primary.Telephone.PhoneNumber,
                AddressInfo = JsonConvert.SerializeObject(customer.Primary.Address),
                DocInfo = JsonConvert.SerializeObject(customer.Primary.Document),
                CreateTime = DateTime.Now,
                OrderStatus = "Comfirmed", //usertodo
                CancelTime = DateTime.Now,
                ConfirmTime = DateTime.Now
            };
            await _aceOrderRepository.InsertAsync(order);
            /* if (loc.Count > 1)
             {
                 order.ReturnLocation_Code = loc.LastOrDefault().Code;
             }*/
            result.OrderSuc = true;
            result.SuppOrderId = spModel.VehResRSCore.VehReservation.VehSegmentCore.ConfID.ID;
            result.SuppOrderStatus = "";
            result.SuppConfirmNumber =
            result.SuppCurrency = spModel.VehResRSCore.VehReservation.VehSegmentCore.TotalCharge.CurrencyCode;
            result.SuppTotalPrice = spModel.VehResRSCore.VehReservation.VehSegmentCore.TotalCharge.RateTotalAmount;
            return result;
        }

        /// <summary>
        /// 查询详情接口
        /// </summary>
        /// <param name="queryOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<StdQueryOrderRS> QueryOrderAsync(ACE_OTA_VehRetResRQ queryOrderRQ, int timeout)
        {
            StdQueryOrderRS result = new StdQueryOrderRS();

            //var res = await BuildEnvelope(new CommonRequest { ACE_OTA_VehRetResRQ = queryOrderRQ, Type = 4 });
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""d0e417a0-86e3-46b9-907f-4ed0551d1ea9"" xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehRetResRS xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2015-12-22T10:55:42.237671-05:00"" Target=""Test"" Version=""5.0""
xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success  />
                <VehRetResRSCore>
                    <VehReservation>
                        <Customer>
                            <Primary>
                                <PersonName>
                                    <GivenName>First Name</GivenName>
                                    <MiddleName>Middle Name</MiddleName>
                                    <Surname>Last Name</Surname>
                                </PersonName>
                                <Telephone CountryAccessCode=""01"" AreaCityCode=""123""
PhoneNumber=""123-1234"" Extension=""12345"" />
                                    <Email>test@test.com</Email>
                                    <Address>
                                        <StreetNmbr>Address 1</StreetNmbr>
                                        <AddressLine>Address 2</AddressLine>
                                        <CityName>City Name</CityName>
                                        <PostalCode>Postal Code</PostalCode>
                                        <StateProv>State Name</StateProv>
                                        <CountryName>Country Name</CountryName>
                                    </Address>
                                    <Document DocIssueLocation=""IN"" DocID=""1234123412341234""
DocType=""4"" ExpireDate=""2010-01-01"" />
                                    </Primary>
                                </Customer>
                                <VehSegmentCore>
                                    <ConfID  Type=""14"" ID=""REZ1234567"" />
                                    <Vendor CompanyShortName=""REZ"" TravelSector=""2""
Code=""RZ"">RezPower Rent A Car</Vendor>
                                        <VehRentalCore PickUpDateTime=""2015-12-30T12:00:00""
ReturnDateTime=""2016-01-07T15:00:00"">
                                            <PickUpLocation  LocationCode=""TEST01"" CodeContext=""RZ"" />
                                            <ReturnLocation  LocationCode=""TEST01"" CodeContext=""RZ"" />
                                        </VehRentalCore>
                                        <Vehicle AirConditionInd=""true"" TransmissionType=""Automatic""
FuelType=""Unspecified"" DriveType=""Unspecified"" PassengerQuantity=""4""
BaggageQuantity=""3"" Code=""CCAR"" CodeContext=""SIPP"">
                                            <VehType  VehicleCategory=""1"" DoorCount=""2-4"" />
                                            <VehClass  Size=""4"" />
                                            <VehMakeModel  Name=""Hyundai Accent"" />
                                            <PictureURL>hyundai-accent-2013.png</PictureURL>
                                        </Vehicle>
                                        <RentalRate>
                                            <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                                <VehicleCharges>
                                                    <VehicleCharge CurrencyCode=""USD"" Amount=""183.40""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                                        <Calculation UnitCharge=""183.40"" UnitName=""Week""
Quantity=""1"" />
                                                        </VehicleCharge>
                                                        <VehicleCharge CurrencyCode=""USD"" Amount=""22.93""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                                            <Calculation UnitCharge=""22.93"" UnitName=""Day""
Quantity=""1"" />
                                                            </VehicleCharge>
                                                            <VehicleCharge CurrencyCode=""USD"" Amount=""13.77""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                                                <Calculation UnitCharge=""4.59"" UnitName=""Hour""
Quantity=""3"" />
                                                                </VehicleCharge>
                                                            </VehicleCharges>
                                                            <RateQualifier  RateCategory=""16"" />
                                                            <RateRestrictions AdvancedBookingInd=""true""
GuaranteeReqInd=""true"" />
                                                            </RentalRate>
                                                            <Fees>
                                                                <Fee CurrencyCode=""USD"" Amount=""11.01"" Description=""Percentage
Discount"" IncludedInEstTotalInd=""true"" Purpose=""3"">
                                                                    <Calculation  UnitCharge=""220.10"" Percentage=""5.0"" />
                                                                </Fee>
                                                                <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                                                    <Calculation UnitCharge=""5.95"" UnitName=""Day"" Quantity=""9""
/>
                                                                    </Fee>
                                                                    <Fee CurrencyCode=""USD"" Amount=""15.75"" Description=""License
Recovery Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                        <Calculation UnitCharge=""1.75"" UnitName=""Day"" Quantity=""9""
/>
                                                                        </Fee>
                                                                        <Fee CurrencyCode=""USD"" Amount=""18.45"" Description=""State
Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                            <Calculation UnitCharge=""2.05"" UnitName=""Day"" Quantity=""9""
/>
                                                                            </Fee>
                                                                            <Fee CurrencyCode=""USD"" Amount=""22.24"" Description=""Airport
Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                                <Calculation  UnitCharge=""224.84"" Percentage=""9.89"" />
                                                                            </Fee>
                                                                            <Fee CurrencyCode=""USD"" Amount=""18.59"" Description=""Sales Tax""
IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                                <Calculation  UnitCharge=""265.53"" Percentage=""7.0"" />
                                                                            </Fee>
                                                                        </Fees>
                                                                        <TotalCharge RateTotalAmount=""209.09""
EstimatedTotalAmount=""337.67"" CurrencyCode=""USD"" />
                                                                        </VehSegmentCore>
                                                                        <VehSegmentInfo>
                                                                            <PaymentRules>
                                                                                <PaymentRule CurrencyCode=""USD"" Amount=""337.67"" RuleType=""2""
/>
                                                                                </PaymentRules>
                                                                                <VendorMessages>
                                                                                    <VendorMessage>
                                                                                        <SubSection>
                                                                                            <Paragraph>
                                                                                                <Text>Your non-refundable prepayment of 337.67 USD
remains on account with RezPower Rent A Car. The remainder is due when
you pick up the vehicle. No charge for reservation changes.</Text>
                                                                                                </Paragraph>
                                                                                            </SubSection>
                                                                                        </VendorMessage>
                                                                                    </VendorMessages>
                                                                                    <LocationDetails AtAirport=""true"" Code=""TEST01"" Name=""Test Int’l
Airport"" CodeContext=""RZ"" AssocAirportLocList=""TEST"">
                                                                                        <Address>
                                                                                            <StreetNmbr>111 Sample Street</StreetNmbr>
                                                                                            <CityName>Test</CityName>
                                                                                            <PostalCode>90210</PostalCode>
                                                                                            <StateProv  StateCode=""FL"">Florida</StateProv>
                                                                                            <CountryName  Code=""US"">United States</CountryName>
                                                                                        </Address>
                                                                                        <Telephone PhoneTechType=""1"" PhoneNumber=""555-555-5555""
DefaultInd=""true"" />
                                                                                            <Telephone  PhoneTechType=""3"" PhoneNumber=""555-222-9999"" />
                                                                                        </LocationDetails>
                                                                                    </VehSegmentInfo>
                                                                                </VehReservation>
                                                                            </VehRetResRSCore>
                                                                        </OTA_VehRetResRS>
                                                                    </ns:Response>
                                                                </SOAP-ENV:Body>
                                                            </SOAP-ENV:Envelope>";
            var spModel = ACEXmlHelper.GetResponse<ACE_OTA_VehRetResRS>(res);
            if (spModel == null)
            {
                throw new Exception($"解析实体错误{res}");
            }
            //原始报文
            result.OrigData = res;
            if (spModel.Errors != null && spModel.Errors.ErrorList.Count > 0)
            {
                Log.Error($"查询订单出错{string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message))}");
                result.IsSuccess = false;
                result.ErrorMessage = string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message));
                return result;
            }
            //usertodo 得看他实际接口返回字段 文档没有提及
            result.SuppOrderId = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.ConfID.ID;
            result.SuppOrderStatus = "usertodo";
            result.Status = EnumCarReservationStatus.Pending;//usertodo
            result.Currency = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.TotalCharge.CurrencyCode;
            result.Amount = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.TotalCharge.RateTotalAmount;
            result.SIPP = spModel.VehRetResRSCore.VehReservation.VehSegmentCore.Vehicle.Code;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cancelOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<StdCancelOrderRS> CancelOrderAsync(ACE_OTAVehCancelRQ cancelOrderRQ, int timeout = 15000)
        {
            StdCancelOrderRS result = new StdCancelOrderRS();

            //var res = await BuildEnvelope(new CommonRequest { ACE_OTAVehCancelRQ = cancelOrderRQ, Type = 4 });
            var res = @"<?xml version=""1.0""?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
    <SOAP-ENV:Body>
        <ns:Response TID=""d0e417a0-86e3-46b9-907f-4ed0551d1ea9"" xmlns:ns=""http://wsg.avis.com/wsbang"">
            <OTA_VehCancelRS xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
TimeStamp=""2015-12-22T16:02:26.4912432-05:00"" Target=""Test""
Version=""5.0"" xmlns=""http://www.opentravel.org/OTA/2003/05"">
                <Success  />
                <VehCancelRSCore  CancelStatus=""Cancelled"" />
                <VehCancelRSInfo>
                    <VehReservation>
                        <Customer>
                            <Primary  BirthDate=""1961-06-01"">
                                <PersonName>
                                    <GivenName>Name First</GivenName>
                                    <MiddleName>Middle Name</MiddleName>
                                    <Surname>Name Last</Surname>
                                </PersonName>
                                <Telephone CountryAccessCode=""01"" AreaCityCode=""123""
PhoneNumber=""123-1234"" Extension=""12345"" />
                                    <Email>test@test.com</Email>
                                    <Address>
                                        <StreetNmbr>Address 1</StreetNmbr>
                                        <AddressLine>Address 2</AddressLine>
                                        <CityName>City Name</CityName>
                                        <PostalCode>Postal Code</PostalCode>
                                        <StateProv>State Name</StateProv>
                                        <CountryName>Country Name</CountryName>
                                    </Address>
                                    <Document DocIssueLocation=""IN"" DocID=""1234123412341234""
DocType=""4"" ExpireDate=""2010-01-01"" />
                                    </Primary>
                                </Customer>
                                <VehSegmentCore>
                                    <ConfID  Type=""14"" ID=""REZ1234567"" />
                                    <Vendor CompanyShortName=""REZ"" TravelSector=""2""
Code=""RZ"">RezPower Rent A Car</Vendor>
                                        <VehRentalCore PickUpDateTime=""2015-12-30T12:00:00""
ReturnDateTime=""2016-01-07T15:00:00"">
                                            <PickUpLocation  LocationCode=""TEST01"" CodeContext=""RZ"" />
                                            <ReturnLocation  LocationCode=""TEST01"" CodeContext=""RZ"" />
                                        </VehRentalCore>
                                        <Vehicle AirConditionInd=""true"" TransmissionType=""Automatic""
FuelType=""Unspecified"" DriveType=""Unspecified"" PassengerQuantity=""4""
BaggageQuantity=""3"" Code=""CCAR"" CodeContext=""SIPP"">
                                            <VehType  VehicleCategory=""1"" DoorCount=""2-4"" />
                                            <VehClass  Size=""4"" />
                                            <VehMakeModel  Name=""Hyundai Accent"" />
                                            <PictureURL>hyundai-accent-2013.png</PictureURL>
                                        </Vehicle>
                                        <RentalRate>
                                            <RateDistance Unlimited=""true"" DistUnitName=""Mile""
VehiclePeriodUnitName=""RentalPeriod"" />
                                                <VehicleCharges>
                                                    <VehicleCharge CurrencyCode=""USD"" Amount=""183.40""
Description=""Weekly Rate"" IncludedInEstTotalInd=""true"" Purpose=""1"">
                                                        <Calculation UnitCharge=""183.40"" UnitName=""Week""
Quantity=""1"" />
                                                        </VehicleCharge>
                                                        <VehicleCharge CurrencyCode=""USD"" Amount=""22.93""
Description=""Extra Day Rate"" IncludedInEstTotalInd=""true"" Purpose=""10"">
                                                            <Calculation UnitCharge=""22.93"" UnitName=""Day""
Quantity=""1"" />
                                                            </VehicleCharge>
                                                            <VehicleCharge CurrencyCode=""USD"" Amount=""13.77""
Description=""Extra Hour Rate"" IncludedInEstTotalInd=""true"" Purpose=""11"">
                                                                <Calculation UnitCharge=""4.59"" UnitName=""Hour""
Quantity=""3"" />
                                                                </VehicleCharge>
                                                            </VehicleCharges>
                                                            <RateQualifier  RateCategory=""16"" />
                                                            <RateRestrictions AdvancedBookingInd=""true""
GuaranteeReqInd=""true"" />
                                                            </RentalRate>
                                                            <Fees>
                                                                <Fee CurrencyCode=""USD"" Amount=""11.01"" Description=""Percentage
Discount"" IncludedInEstTotalInd=""true"" Purpose=""3"">
                                                                    <Calculation  UnitCharge=""220.10"" Percentage=""5.0"" />
                                                                </Fee>
                                                                <Fee CurrencyCode=""USD"" Amount=""53.55""
Description=""Consolidated Facility Charge"" IncludedInEstTotalInd=""true""
Purpose=""6"">
                                                                    <Calculation UnitCharge=""5.95"" UnitName=""Day"" Quantity=""9""
/>
                                                                    </Fee>
                                                                    <Fee CurrencyCode=""USD"" Amount=""15.75"" Description=""License
Recovery Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                        <Calculation UnitCharge=""1.75"" UnitName=""Day"" Quantity=""9""
/>
                                                                        </Fee>
                                                                        <Fee CurrencyCode=""USD"" Amount=""18.45"" Description=""State
Surcharge"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                            <Calculation UnitCharge=""2.05"" UnitName=""Day"" Quantity=""9""
/>
                                                                            </Fee>
                                                                            <Fee CurrencyCode=""USD"" Amount=""22.24"" Description=""Airport
Fee"" IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                                <Calculation  UnitCharge=""224.84"" Percentage=""9.89"" />
                                                                            </Fee>
                                                                            <Fee CurrencyCode=""USD"" Amount=""18.59"" Description=""Sales Tax""
IncludedInEstTotalInd=""true"" Purpose=""7"">
                                                                                <Calculation  UnitCharge=""265.53"" Percentage=""7.0"" />
                                                                            </Fee>
                                                                        </Fees>
                                                                        <TotalCharge RateTotalAmount=""209.09""
EstimatedTotalAmount=""337.67"" CurrencyCode=""USD"" />
                                                                        </VehSegmentCore>
                                                                        <VehSegmentInfo>
                                                                            <PaymentRules>
                                                                                <PaymentRule CurrencyCode=""USD"" Amount=""337.67"" RuleType=""2""
/>
                                                                                </PaymentRules>
                                                                                <VendorMessages>
                                                                                    <VendorMessage>
                                                                                        <SubSection>
                                                                                            <Paragraph>
                                                                                                <Text>You have no further obligation with this
reservation.</Text>
                                                                                                </Paragraph>
                                                                                            </SubSection>
                                                                                        </VendorMessage>
                                                                                    </VendorMessages>
                                                                                    <LocationDetails AtAirport=""true"" Code=""TEST01"" Name=""Test Int’l
Airport"" CodeContext=""RZ"" AssocAirportLocList=""REZ"">
                                                                                        <Address>
                                                                                            <StreetNmbr>Sample Street</StreetNmbr>
                                                                                            <CityName>Test</CityName>
                                                                                            <PostalCode>90210</PostalCode>
                                                                                            <StateProv  StateCode=""FL"">Florida</StateProv>
                                                                                            <CountryName  Code=""US"">United States</CountryName>
                                                                                        </Address>
                                                                                        <Telephone PhoneTechType=""1"" PhoneNumber=""555-555-5555""
DefaultInd=""true"" />
                                                                                            <Telephone  PhoneTechType=""3"" PhoneNumber=""555-222-9999"" />
                                                                                        </LocationDetails>
                                                                                    </VehSegmentInfo>
                                                                                </VehReservation>
                                                                            </VehCancelRSInfo>
                                                                        </OTA_VehCancelRS>
                                                                    </ns:Response>
                                                                </SOAP-ENV:Body>
                                                            </SOAP-ENV:Envelope>";
            var spModel = ACEXmlHelper.GetResponse<ACE_OTA_VehCancelRS>(res);
            if (spModel == null)
            {
                throw new Exception($"解析实体错误{res}");
            }
            if (spModel.Errors != null && spModel.Errors.ErrorList.Count > 0)
            {
                Log.Error($"查询订单出错{string.Join(",", spModel.Errors.ErrorList.Select(n => n.Message))}");
                result.CancelSuc = false;
                return result;
            }
            if (spModel.CancelRSCore.CancelStatus == "Cancelled")
            {
                result.SuppCancelFee = 0; //usertodo 取消费用怎么计算
                result.Currency = spModel.CancelRSInfo.VehReservation.VehSegmentCore.TotalCharge.CurrencyCode;

                result.CancelSuc = true;
            }
            else
            {
                result.CancelSuc = false;
                return result;
            }
            return result;
        }

        #endregion 原始接口

        #region 业务接口

        public async Task<StdCancelOrderRS> CancelOrderAsync(StdCancelOrderRQ cancelOrderRQ, int timeout = 15000)
        {
            var postData = new ACE_OTAVehCancelRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehCancelRQCore = new VehCancelRQCore
                {
                    UniqueID = new UniqueID
                    {
                        ID = cancelOrderRQ.SuppOrderId,
                        Type = 14 //写死?
                    }
                }
            };
            return await CancelOrderAsync(postData);
        }

        /// <summary>
        /// 创建预定接口
        /// </summary>
        /// <param name="createOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<StdCreateOrderRS> CreateOrderAsync(StdCreateOrderRQ createOrderRQ, int timeout = 15000)
        {
            //构建客户信息
            var customer = new Customer
            {
                Primary = new Primary
                {
                    PersonName = new PersonName
                    {
                        GivenName = createOrderRQ.FirstName,
                        //MiddleName=createOrderRQ.LastName, 中间名没有
                        Surname = createOrderRQ.LastName,
                    },
                    Telephone = new Models.RQs.Telephone
                    {
                        PhoneNumber = createOrderRQ.ContactNumber,
                        /* CountryAccessCode=createOrderRQ.BillingAddress.Country
                         AreaCityCode="",
                         Extension=""*/
                    },
                    Email = createOrderRQ.Email,
                    Address = new Models.RQs.Address
                    {
                        CityName = createOrderRQ.BillingAddress.CityName,
                        CountryName = createOrderRQ.BillingAddress.Country,
                        PostalCode = createOrderRQ.BillingAddress.PostCode,
                        AddressLine = createOrderRQ.BillingAddress.Address,
                        StreetNmbr = createOrderRQ.BillingAddress.Address
                    },
                    Document = null, //没有驾照信息
                    //BirthDate   =  //没有生日
                }
            };
            //解析一下 createOrderRQ.RateCode
            var ratecode = createOrderRQ.RateCode;
            var queryList = ratecode.Split("_");
            var id = queryList.LastOrDefault();
            var type = "16"; //usertodo queryList[queryList.Length - 2];
            var availRateRQ = new ACE_OTA_VehResRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehResRQCore = new CreateOrder_VehResRQCore
                {
                    VehRentalCore = new VehRentalCore
                    {
                        PickUpDateTime = Convert.ToDateTime(createOrderRQ.PickUpDateTime),
                        ReturnDateTime = Convert.ToDateTime(createOrderRQ.ReturnDateTime),
                        //出发地 目的地方
                        /* PickUpLocation = new PickUpLocation { LocationCode = vehicleRQ.PickUpLocationCode },
                         ReturnLocation = new PickUpLocation { LocationCode = vehicleRQ.ReturnLocationCode }*/
                    },
                    Customer = customer
                },
                VehResRQInfo = new VehResRQInfo
                {
                    Reference = new Reference { ID = id, Type = type }
                }
            };
            return await CreateOrderAsync(createOrderRQ, availRateRQ);
        }

        /// <summary>
        /// 搜索接口
        /// </summary>
        /// <param name="vehicleRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<List<StdVehicle>> GetVehiclesAsync(StdGetVehiclesRQ vehicleRQ, int timeout = 45000)
        {
            List<StdVehicle> result = new List<StdVehicle>();
            //缓存
            var cache_key = $"{vehicleRQ.PickUpLocationCode}_{vehicleRQ.PickUpDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}_{vehicleRQ.ReturnLocationCode}_{vehicleRQ.ReturnDateTime.ToString("yyyy-MM-ddTHH:mm:ss")}";//"HK_2024-04-28T10:00:00_BKK_2024-05-01T10:00:00_BKK_30"
            var md5Key = Md5Helper.ComputeMD5Hash(cache_key);
            var dbModel = await _aceCacheRepository.GetByIdAsync("select * from ACE_RateCache where SearchMD5 = @SearchMD5", new { SearchMD5 = md5Key });
            if (dbModel != null && dbModel.ExpireTime > DateTime.Now)
            {
                var res = GZipHelper.DecompressString(dbModel.RateCache);
                //修改缓存
                await _aceCacheRepository.UpdateBySqlAsync("update ACE_RateCache set searchcount=searchcount+1 where SearchMD5 = @SearchMD5", new { SearchMD5 = md5Key });
                return JsonConvert.DeserializeObject<List<StdVehicle>>(res);
            }
            List<string> pickupLocCodes = new List<string>();
            List<string> returnLocCodes = new List<string>();
            var alllocList = await _locRepository.GetAllAsync();
            //usertodo 先根据locationcode找出门店 现在只处理机场
            if (vehicleRQ.PickUpLocationType == EnumCarLocationType.Airport)
            {
                var pickuploc = alllocList.Where(n => n.AirportCode == vehicleRQ.PickUpLocationCode);
                pickupLocCodes.AddRange(pickuploc.Select(n => n.LocationCode));
            }
            if (vehicleRQ.ReturnLocationType == EnumCarLocationType.Airport)
            {
                var pickuploc = alllocList.Where(n => n.AirportCode == vehicleRQ.ReturnLocationCode);
                returnLocCodes.AddRange(pickuploc.Select(n => n.LocationCode));
            }
            if (pickupLocCodes.Count == 0)
            {
                throw new Exception("找不到对应的门店");
            }
            if (returnLocCodes.Count == 0)
            {
                returnLocCodes = pickupLocCodes;
            }
            foreach (var pickCode in pickupLocCodes)
            {
                foreach (var rtCode in returnLocCodes)
                {
                    //构建请求body 其他都一样
                    var availRateRQ = new ACE_OTA_VehAvailRateRQ
                    {
                        TimeStamp = DateTime.Now,
                        Target = _setting.Target,
                        Version = "5.0",
                        POS = new POS
                        {
                            Source = new Source
                            {
                                RequestorID = new RequestorID
                                {
                                    Type = "22",
                                    ID = _setting.UserName
                                }
                            }
                        },
                        VehAvailRQCore = new VehAvailRQCore
                        {
                            VehRentalCore = new Models.RQs.VehRentalCore
                            {
                                PickUpDateTime = Convert.ToDateTime(vehicleRQ.PickUpDateTime),
                                ReturnDateTime = Convert.ToDateTime(vehicleRQ.ReturnDateTime),
                                //出发地 目的地方
                                PickUpLocation = new PickUpLocation { LocationCode = vehicleRQ.PickUpLocationCode },
                                ReturnLocation = new PickUpLocation { LocationCode = vehicleRQ.ReturnLocationCode }
                            }
                        }
                    };
                    var model = await VehAvailRate(availRateRQ);
                    result.AddRange(model);
                }
            }

            var dbCache = GZipHelper.Compress(JsonConvert.SerializeObject(result));
            var rateMD5 = Md5Helper.ComputeMD5Hash(dbCache);
            if (dbModel != null)
            {
                await _aceCacheRepository.UpdateBySqlAsync("update ACE_RateCache set searchcount=searchcount+1,RateMD5=@RateMD5,RateCache=@RateCache,PreUpdateTime=updatetime,updatetime=@updatetime,ExpireTime=@ExpireTime where SearchMD5=@SearchMD5",
                    new { RateMD5 = rateMD5, RateCache = dbCache, updatetime = DateTime.Now, ExpireTime = DateTime.Now.AddMinutes(10), SearchMD5 = md5Key });
            }
            else
            {
                ACERateCache aCERateCache = new ACERateCache
                {
                    SearchMD5 = md5Key,
                    SearchKey = cache_key,
                    SearchCount = 1,
                    RateCache = dbCache,
                    RateMD5 = rateMD5,
                    CanSaleCount = result.Count,
                    UpdateTime = DateTime.Now,
                    PreUpdateTime = DateTime.Now,
                    ExpireTime = DateTime.Now.AddMinutes(10)
                };
                await _aceCacheRepository.InsertAsync(aCERateCache);
            }
            return result;
        }

        /// <summary>
        /// 查询订单详情
        /// </summary>
        /// <param name="queryOrderRQ"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<StdQueryOrderRS> QueryOrderAsync(StdQueryOrderRQ queryOrderRQ, int timeout)
        {
            var queryDto = new ACE_OTA_VehRetResRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehRetResRQCore = new VehRetResRQCore
                {
                    UniqueID = new UniqueID
                    {
                        ID = queryOrderRQ.SuppOrderId,
                        Type = 14 //写死?
                    }
                }
            };

            return await QueryOrderAsync(queryDto, timeout);
        }

        #endregion 业务接口

        #region 地址门店构建

        public async Task<bool> BuildAllLocation()
        {
            var queryDto = new ACE_OTA_VehLocSearchRQ
            {
                TimeStamp = DateTime.Now,
                Target = _setting.Target,
                Version = "5.0",
                POS = new POS
                {
                    Source = new Source
                    {
                        RequestorID = new RequestorID
                        {
                            Type = "22",
                            ID = _setting.UserName
                        }
                    }
                },
                VehLocSearchCriterion = new VehLocSearchCriterion
                {
                },
                Vendor = new Models.RQs.Vendor
                {
                    Code = "AC"
                }
            };

            return await BuildLocation(queryDto);
        }

        #endregion 地址门店构建
    }
}