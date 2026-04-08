using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Club.Entities;
using Domain.Club.Enum;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Commands.Register;

internal sealed class RegisterClubMemberHandler : IRequestHandler<RegisterClubMember, Result<ClubMemberCompleteResponse>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IMapper _mapper;

    public RegisterClubMemberHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork,  IUserContext userContext,  IClubMemberQueries clubMemberQueries, IIdentityService identityService, IMapper mapper)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _mapper = mapper;
    }

    public async Task<Result<ClubMemberCompleteResponse>> Handle(RegisterClubMember request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<ClubMemberCompleteResponse>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<ClubMemberCompleteResponse>(ClubErrors.Forbidden);
            }
        }
        
        var club = await _clubRepository.GetClubWithMembers(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubMemberErrors.NotFound(request.ClubId));
        }
        
        if (!Enum.TryParse<MemberRole>(request.Role, ignoreCase: true, out var memberRole))
        {
            return Result.Failure<ClubMemberCompleteResponse>(ClubMemberErrors.InvalidType);
        }

        var clubMemberResult = club.AddMember(request.UserId, memberRole);
        if (clubMemberResult.IsFailure)
        {
            return Result.Failure<ClubMemberCompleteResponse>(clubMemberResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var clubMemberMapped = _mapper.Map<ClubMemberCompleteResponse>(clubMemberResult.Value);
        return Result.Success(clubMemberMapped);
    }
}