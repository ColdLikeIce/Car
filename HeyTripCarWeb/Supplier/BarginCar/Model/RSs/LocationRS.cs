namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class LocationRS
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public Step1Results Results { get; set; }
    }

    public class LocationInfo
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPickupAvailable { get; set; }
        public bool IsDropoffAvailable { get; set; }
        public bool IsFlightInRequired { get; set; }
        public int MinimumBookingDay { get; set; }
        public int NoticeRequiredNumberOfDays { get; set; }
        public int QuoteIsValidNumberOfDays { get; set; }
        public string OfficeOpeningTime { get; set; }
        public string OfficeClosingTime { get; set; }
        public bool AfterHourBookingAccepted { get; set; }
        public int AfterHourFeeId { get; set; }
        public bool UnattendedDropoffAccepted { get; set; }
        public int UnattendedDropoffFeeId { get; set; }
        public int MinimumAge { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class OfficeTime
    {
        public int LocationId { get; set; }
        public int DayOfWeek { get; set; }
        public string OpeningTime { get; set; }
        public string ClosingTime { get; set; }
        public string StartPickup { get; set; }
        public string EndPickup { get; set; }
        public string StartDropoff { get; set; }
        public string EndDropoff { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CategoryType
    {
        public int Id { get; set; }
        public string VehicleCategoryType { get; set; }
        public string DisplayOrder { get; set; }
    }

    public class DriverAge
    {
        public int Id { get; set; }
        public int driverage { get; set; }
        public bool isdefault { get; set; }
    }

    public class Holiday
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Type { get; set; }
        public int Weekdays { get; set; }
        public string HolidayName { get; set; }
        public string ClosingTime { get; set; }
    }

    public class Step1Results
    {
        public List<LocationInfo> Locations { get; set; }
        public List<OfficeTime> OfficeTimes { get; set; }
        public List<CategoryType> CategoryTypes { get; set; }
        public List<DriverAge> DriverAges { get; set; }
        public List<Holiday> Holidays { get; set; }
    }
}