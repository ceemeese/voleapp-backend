
using Application.Abstractions.DTO.ClubMember;
using AutoMapper;
using Domain.Club.Enum;

namespace Application.Mappings;

internal sealed class ClubMemberProfile : Profile
{
    public ClubMemberProfile()
    {
        CreateMap<MemberRole, RoleResponse>().ConvertUsing(src => new RoleResponse((int)src, src.ToString()));
        CreateMap<Domain.Club.Entities.ClubMember, ClubMemberResponse>();
    }
}