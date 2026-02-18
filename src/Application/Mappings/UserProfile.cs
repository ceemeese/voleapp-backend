using Application.Abstractions.DTO;
using Application.Users.Commands.Register;
using Application.Users.Commands.Update;
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