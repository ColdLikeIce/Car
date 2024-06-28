using System.ComponentModel.DataAnnotations.Schema;

namespace HeyTripCarWeb.Supplier.ABG.Models.Dbs
{
    [Table("Abg_YoungDriver")]
    public class AbgYoungDriver
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Code { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Station { get; set; }
        public string LocationName { get; set; }
        public string CarGroup { get; set; }
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public int? YoungAge { get; set; }
        public string? EnglandYoungDriverSurcharge { get; set; }
        public string? SpainYoungDriverSurcharge { get; set; }
        public string? CzechYoungDriverSurcharge { get; set; }
        public string? DenmarkYoungDriverSurcharge { get; set; }
        public string? GermanyYoungDriverSurcharge { get; set; }
        public string? FranceYoungDriverSurcharge { get; set; }
        public string? ItalyYoungDriverSurcharge { get; set; }
        public string? NorwayYoungDriverSurcharge { get; set; }
        public string? NethlandsYoungDriverSurcharge { get; set; }
        public string? PortugalYoungDriverSurcharge { get; set; }
        public string? SwedenYoungDriverSurcharge { get; set; }
        public string? SlovakYoungDriverSurcharge { get; set; }
        public string? EnglandMaximumAgeSurcharge { get; set; }
        public string? SpainMaximumAgeSurcharge { get; set; }
        public string? CzechMaximumAgeSurcharge { get; set; }
        public string? DenmarkMaximumAgeSurcharge { get; set; }
        public string? GermanyMaximumAgeSurcharge { get; set; }
        public string? FranceMaximumAgeSurcharge { get; set; }
        public string? ItalyMaximumAgeSurcharge { get; set; }
        public string? NorwayMaximumAgeSurcharge { get; set; }
        public string? NetherlandsMaximumAgeSurcharge { get; set; }
        public string? PortugalMaximumAgeSurcharge { get; set; }
        public string? SwedenMaximumAgeSurcharge { get; set; }
        public string? SlovakMaximumAgeSurcharge { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}