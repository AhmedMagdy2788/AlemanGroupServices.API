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
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Owner_Company_Name { get; set; } = null!;
        public string? Partner_Ship_Name { get; set; }
    }

    public class StationCreateResponseDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string Owner_Company_Name { get; set; } = null!;
        public string? Partner_Ship_Name { get; set; }
        public bool Success { get; set; } // indicates if the entity was added successfully
        public string? ErrorMessage { get; set; } // contains the error message if any
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

            CreateMap<StationCreateDto, StationCreateResponseDto>()
            // Ignore the Id, Success and ErrorMessage properties as they will be set later
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Success, opt => opt.Ignore())
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore())
            // Use the AfterMap method to access the opt parameter and set the Success and ErrorMessage properties
            .AfterMap((src, dest, opt) =>
            {
                //dest.Id = (Guid)opt.Items["Id"];
                dest.Success = (bool)opt.Items["Success"];
                dest.ErrorMessage = (string)opt.Items["ErrorMessage"];
            });
        }
    }

    public class StationIdNamePairs
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
