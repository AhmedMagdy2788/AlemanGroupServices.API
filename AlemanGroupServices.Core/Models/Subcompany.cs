using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AlemanGroupServices.Core.Models
{
    public partial class Subcompany
    {
        public Subcompany()
        {
            Tblstations = new HashSet<Station>();
        }

        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Tax_Card { get; set; } = null!;
        public string Commercial_Registration { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<Station> Tblstations { get; set; }
    }

    public class SubcompanyCreateDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Tax_Card { get; set; } = null!;
        public string Commercial_Registration { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class SubcompanyResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Tax_Card { get; set; } = null!;
        public string Commercial_Registration { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class SubcompanyCreateResponseDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Tax_Card { get; set; } = null!;
        public string Commercial_Registration { get; set; } = null!;
        public string Fax { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool Success { get; set; } // indicates if the entity was added successfully
        public string? ErrorMessage { get; set; } // contains the error message if any
    }

    public class SubcompanyMapper : Profile
    {
        public SubcompanyMapper()
        {
            CreateMap<SubcompanyCreateDto, Subcompany>()
                .ForMember(
                dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<Subcompany, SubcompanyResponseDto>();

            // when creation success
            CreateMap<Subcompany, SubcompanyCreateResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.ErrorMessage, opt => opt.Ignore());

            // Map from SubcompanyCreateDto to SubcompanyCreateResponseDto when there is an error
            CreateMap<SubcompanyCreateDto, SubcompanyCreateResponseDto>()
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
}
