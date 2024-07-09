namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public class BaseResponse<T>
    {
        public string status { get; set; }
        public T result { get; set; }
    }
}