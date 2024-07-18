namespace HeyTripCarWeb.Supplier.BarginCar.Model.RSs
{
    public enum Method
    {
        step1,
        step2,
        step3,
        Booking
    }

    public class BaseResult
    {
        /// <summary>
        ///
        /// </summary>
        public string status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string error { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<AgentinfoItem> agentinfo { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int ms { get; set; }

        /// <summary>
        ///
        /// </summary>
        //public List<string> issues { get; set; }
    }

    public class AgentinfoItem
    {
        /// <summary>
        ///
        /// </summary>
        public string agentid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string agentcode { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string agentbranchid { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string agency { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string agentbranch { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string agentcollected { get; set; }
    }
}