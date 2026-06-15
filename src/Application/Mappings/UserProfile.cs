using Application.Abstractions.DTO.User;
using AutoMapper;
using Domain.User;

namespace Application.Mappings;

internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
    }
}