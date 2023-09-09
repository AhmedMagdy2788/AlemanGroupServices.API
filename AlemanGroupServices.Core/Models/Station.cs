using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemanGroupServices.Core.Models
{

    public class Station
    {

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        [ForeignKey("Subcompany")]
        public Guid Owner_Company_Id { get; set; }
        [ForeignKey("MarketingCompny")]
        public Guid? Partner_Ship_Id { get; set; }

        public virtual Subcompany? Subcompany { get; set; }
        public virtual MarketingCompany? MarketingCompny { get; set; }
    }

    public class StationCreateDto
    {
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Owner_Company_Name { get; set; } = null!;
        public string? Partner_Ship_Name { get; set; }
    }

    public class StationResponseDto
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Owner_Company_Name { get; set; } = null!;
        public string? Partner_Ship_Name { get; set; }
    }


    public class StationMapper : Profile
    {
        public StationMapper()
        {
            // Create a map from Station to StationResponseDto
            CreateMap<Station, StationResponseDto>()
            // Map the Owner_Company_Id property to the Owner_Company_Name property
            // using the Subcompany navigation property
            .ForMember(dest => dest.Owner_Company_Name, opt => opt.MapFrom(src => src.Subcompany.Name))
            // Map the Partner_Ship_Id property to the Partner_Ship_Name property
            // using the MarketingCompny navigation property
            .ForMember(dest => dest.Partner_Ship_Name, opt => opt.MapFrom(src => src.MarketingCompny.Name));
        }
    }

    public class StationIdNamePairs
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
