using AutoMapper;
using WebGame.Models;

namespace WebGame.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Define mappings for Game entities
            CreateMap<Game, Game>()
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.Screenshots, opt => opt.Ignore())
                .ForMember(dest => dest.GamePlatforms, opt => opt.Ignore());

            // Define mappings for NewsPost entities
            CreateMap<NewsPost, NewsPost>()
                .ForMember(dest => dest.GameCategory, opt => opt.Ignore());
                
            // Define mappings for Product entities
            
        }
    }
} 