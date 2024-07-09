namespace HeyTripCarWeb.Supplier.Sixt.Models.RSs
{
    public class LocationRs
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Coordinates Coordinates { get; set; }
        public List<string> Subtypes { get; set; }
        public Address Address { get; set; }
        public Address ReturnAddress { get; set; }
        public StationInformation StationInformation { get; set; }
        public List<string> VehicleTypes { get; set; }
        public string Distance { get; set; }
        public string DistanceMiles { get; set; }
    }

    public class Country
    {
        public string Name { get; set; }
        public string Iso2Code { get; set; }
    }

    public class Coordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string House { get; set; }
        public string Postcode { get; set; }
        public Country Country { get; set; }
    }

    public class Contact
    {
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Hint { get; set; }
    }

    public class OpeningHour
    {
        public string Open { get; set; }
        public string Close { get; set; }
    }

    public class Days
    {
        public List<OpeningHour> Openings { get; set; }
    }

    public class OpeningHours
    {
        public string Open247 { get; set; }
        public Dictionary<string, Days> days { get; set; }
    }

    public class Documents
    {
        public string TermsAndConditions { get; set; }
        public string DataPrivacy { get; set; }
    }

    public class StationInformation
    {
        public Contact Contact { get; set; }
        public OpeningHours OpeningHours { get; set; }
        public List<object> Directions { get; set; }
        public Documents Documents { get; set; }
        public string RequiresResidenceCountry { get; set; }
        public string IataCode { get; set; }
        public string IsMeetAndGreet { get; set; }
        public string IsMobileCheckIn { get; set; }
        public string IsOffAirport { get; set; }
    }
}