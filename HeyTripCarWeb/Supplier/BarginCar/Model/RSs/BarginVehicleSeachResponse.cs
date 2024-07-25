namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class BarginVehicleSeachResponse : BaseResult
    {
        /// <summary>
        ///
        /// </summary>
        public BarginResults results { get; set; }
    }

    public class BarginBathVehicleSeachResponse : BaseResult
    {
        /// <summary>
        ///
        /// </summary>
        public List<BarginResults> results { get; set; }
    }

    public class BarginResults
    {
        /// <summary>
        ///
        /// </summary>
        public List<CountriesItem> countries { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> rentalsource { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<LocationsItem> locations { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<OfficetimesItem> officetimes { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<CategorytypesItem> categorytypes { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<DriveragesItem> driverages { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<object> holidays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<LocationfeesItem> locationfees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<SeasonalratesItem> seasonalrates { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<AvailablecarsItem> availablecars { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<MandatoryfeesItem> mandatoryfees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<OptionalfeesItem> optionalfees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<InsuranceoptionsItem> insuranceoptions { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<KmchargesItem> kmcharges { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> availablecars_p { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> mandatoryfees_p { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> insuranceoptions_p { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<string> kmcharges_p { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool taxinclusive { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double taxrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal statetax { get; set; }
    }

    public class CountriesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string country { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string code { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isdefault { get; set; }
    }

    public class LocationsItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string location { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isdefault { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ispickupavailable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isdropoffavailable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isflightinrequired { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int minimumbookingday { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int noticerequired_numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int quoteisvalid_numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string officeopeningtime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string officeclosingtime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string afterhourbookingaccepted { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int afterhourfeeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string unattendeddropoffaccepted { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int unattendeddropofffeeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int minimumage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string email { get; set; }
    }

    public class OfficetimesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int locationid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int dayofweek { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string openingtime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string closingtime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string startpickup { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string endpickup { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string startdropoff { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string enddropoff { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string startdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string enddate { get; set; }
    }

    public class CategorytypesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategorytype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string displayorder { get; set; }
    }

    public class DriveragesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int driverage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isdefault { get; set; }
    }

    public class LocationfeesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int loctypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string loctype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int locationid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string currencyname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string currencysymbol { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string locdate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string loctime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string dayofweekname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string location { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string locationdatetimenow { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int errorcode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string availablemessage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string isavailable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string flightnumberrequired { get; set; }
    }

    public class SeasonalratesItem
    {
        /// <summary>
        ///
        /// </summary>
        public string vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string season { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string rateperiod { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int rateperiod_number { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double rateperiod_rateafterdiscount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int rateperiod_remainingnumberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? rateperiod_dailyrateafterdiscountforremainingdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? numberofhours { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? dailyratebeforediscount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? dailyrateafterdiscount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? ratesubtotal { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string discounttype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double? discountrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string discountname { get; set; }
    }

    public class AvailablecarsItem
    {
        /// <summary>
        ///
        /// </summary>
        public int vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int available { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string availablemessage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int errorcode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int minimumbookingday { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int minimumage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int maximumage { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategory { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string categoryfriendlydescription { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int categorybrandid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string categorybrandname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string numberofhours { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double hourlyrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double avgrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal totalratebeforediscount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double discounteddailyrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal totalrateafterdiscount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double totaldiscountamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal discountrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string discounttype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string discountname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int freedays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal freedaysamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string imageurl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofadults { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofchildren { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberoflargecases { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofsmallcases { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string sippcode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehicledescription1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehicledescription2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehicledescription3 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehicledescriptionurl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal agentcommissionrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int agencypaymenttype { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numbervehiclesavailable { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclesbookedpercent { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string offerdescription { get; set; }
    }

    public class InsuranceoptionsItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal fees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal maximumprice { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal totalinsuranceamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string payagency { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string stampduty { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool gst { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ispercentageontotalcost { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal excessamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool isdefault { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription3 { get; set; }
    }

    public class MandatoryfeesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int qty { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal? fees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal? totalfeeamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string payagency { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal? maximumprice { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string stampduty { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool gst { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ispercentageontotalcost { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string merchantfee { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription3 { get; set; }
    }

    public class OptionalfeesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int vehiclecategoryid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string locationid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int displayorder { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int feegroupid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feegroupname { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string payagency { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double fees { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal totalfeeamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal maximumprice { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string stampduty { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string gst { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ispercentageontotalcost { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string merchantfee { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string qtyapply { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription1 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription2 { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string feedescription3 { get; set; }
    }

    public class KmchargesItem
    {
        /// <summary>
        ///
        /// </summary>
        public int id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string vehiclecategorytypeid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofdays { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string mileagedesc { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string description { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double totalamount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int numberofkmsfree { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal feeforeachadditionalkm { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool isdefault { get; set; }

        /// <summary>
        ///
        /// </summary>
        public decimal maximumprice { get; set; }

        /// <summary>
        ///
        /// </summary>
        public double dailyrate { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string notes { get; set; }
    }
}