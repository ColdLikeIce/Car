using CarrentalWeb.Config;
using CarrentalWeb.Db;
using CarrentalWeb.Entity;
using CarrentalWeb.Impl;
using CarrentalWeb.Util;
using CommonCore.Mapper;
using Microsoft.Extensions.Options;
using System.Xml.Linq;

namespace CarrentalWeb.Service
{
    public class CarDomain : ICarSupplierApi
    {
        private readonly IRepository<Products> _productRepository;
        private readonly IConfiguration _configuration;
        private readonly AppSetting _setting;
        private readonly IMapper _mapper;

        public CarDomain(IRepository<Products> productRepository, IConfiguration configuration,
            IOptions<AppSetting> options, IMapper mapper)
        {
            _productRepository = productRepository;
            _setting = options.Value;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task GetProduct()
        {
            Products model = new Products()
            {
                Name = "产品1"
            };
            await _productRepository.InsertAsync(model);
            var ss = await _productRepository.GetAllAsync();
        }

        public async Task GetPrice()
        {
            XElement body = new XElement("OTA_VehAvailRateRQ",
               new XAttribute("MaxResponses", "1"),
               new XAttribute("ReqRespVersion", "small"),
               new XAttribute("Version", "1.0"),
               new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2008/XMLSchema-instance"),
               new XElement("POS",
                   new XElement("Source",
                       new XElement("RequestorID",
                           new XAttribute("ID", _setting.UserID),
                           new XAttribute("Type", "1")
                       )
                   )
               ),
               new XElement("VehAvailRQCore",
                   new XAttribute("Status", "Available"),
                   new XElement("VehRentalCore",
                       new XAttribute("PickUpDateTime", "2024-06-15T09:00:00"),
                       new XAttribute("ReturnDateTime", "2024-06-16T09:00:00"),
                       new XElement("PickUpLocation", new XAttribute("LocationCode", "JFK")),
                       new XElement("ReturnLocation", new XAttribute("LocationCode", "JFK"))
                   ),
                   new XElement("VendorPrefs",
                       new XElement("VendorPref", new XAttribute("CompanyShortName", "Avis"))
                   ),
                   new XElement("VehPrefs",
                       new XElement("VehPref",
                           new XAttribute("AirConditionPref", "Preferred"),
                           new XAttribute("ClassPref", "Preferred"),
                           new XAttribute("TransmissionPref", "Preferred"),
                           new XAttribute("TransmissionType", "Automatic"),
                           new XAttribute("TypePref", "Preferred"),
                           new XElement("VehType", new XAttribute("VehicleCategory", "1")),
                           new XElement("VehClass", new XAttribute("Size", "4"))
                       )
                   ),
                   new XElement("RateQualifier", new XAttribute("RateCategory", "6"))
               ),
               new XElement("VehAvailRQInfo",
                   new XElement("Customer",
                       new XElement("Primary",
                           new XElement("CitizenCountryName", new XAttribute("Code", "US"))
                       )
                   )
               )
           );
            var res = CarrentalXmlHelper.BuildXmlAndPostData(_setting, body);
        }
    }
}