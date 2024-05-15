using AutoMapper;
using Kiosk.Abstractions.Enums;
using Kiosk.Abstractions.Models.Staff;

namespace KioskAPI.Mappers;

public class AcademicProfile : Profile
{
    public AcademicProfile()
    {
        CreateMap<Academic, AcademicResponse>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom((src, _, _, context) => src[(Language)context.Items["language"]]))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((_, _, _, context) => (Language)context.Items["language"]));
    }
}

public class AcademicToListProfile : Profile
{
    public AcademicToListProfile() {
        CreateMap<Academic, AcademicToListResponse>()
            .ForMember(dest => dest.Units, opt => opt.MapFrom((src, _, _, context) => 
                src[(Language)context.Items["language"]].Posts.SelectMany(p => p.Faculty).Distinct().ToList()
            ))
            .ForMember(dest => dest.Language, opt => opt.MapFrom((_, _, _, context) => (Language)context.Items["language"]));
    }
}

public class AcademicRequestProfile : Profile
{
    public AcademicRequestProfile()
    {
        CreateMap<AcademicRequest, SimplifiedAcademic>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => new AcademicSimplifiedContent
            {
                Posts = src.Content.Posts.Select(p => new AcademicSimplifiedPost
                {
                    Position = p.Position,
                    Faculty = string.Join("; ", p.Faculty)
                }).ToList(),
                Tutorial = src.Content.Tutorial
            }));
    }
}

public class AcademicSimplifiedContentProfile : Profile
{
    private static readonly string[] Separator = ["; "];
    public AcademicSimplifiedContentProfile()
    {
        CreateMap<AcademicSimplifiedContent, AcademicContent>()
            .ForPath(dest => dest.Posts, opt => opt.MapFrom(src =>
                src.Posts.Select(p => new AcademicPost
                {
                    Position = p.Position,
                    Faculty = p.Faculty.Split(Separator, StringSplitOptions.None).ToList(),
                }).ToList()))
            .ForPath(dest => dest.Tutorial, opt => opt.MapFrom(src => src.Tutorial));
    }
}