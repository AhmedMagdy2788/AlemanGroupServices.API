using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class MarketingCompany
    {
        public MarketingCompany()
        {
            Tblstations = new HashSet<Station>();
            Tblwarehouses = new HashSet<Tblwarehouse>();
        }
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Station> Tblstations { get; set; }
        public virtual ICollection<Tblwarehouse> Tblwarehouses { get; set; }
    }

    public class MarketingCompanyCreateDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class MarketingCompanyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class MarketingCompanyCreateResponseDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Success { get; set; } // indicates if the entity was added successfully
        public string? ErrorMessage { get; set; } // contains the error message if any
    }

    public class MarketingCompanyMapper : Profile
    {
        public MarketingCompanyMapper()
        {
            CreateMap<MarketingCompanyCreateDto, MarketingCompany>()
                .ForMember(
                dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<MarketingCompany, MarketingCompanyResponseDto>();

            // Map from MarketingCompanyCreateDto to MarketingCompanyCreateResponseDto
            CreateMap<MarketingCompanyCreateDto, MarketingCompanyCreateResponseDto>()
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

            CreateMap<MarketingCompany, MarketingCompanyCreateResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore());
        }
    }
}
