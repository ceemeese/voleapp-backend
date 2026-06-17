using Application.Abstractions.DTO.ClubMember;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.ClubMember.Queries.GetClubsByUser;

internal sealed class GetClubsByUserHandler : IRequestHandler<GetClubsByUser, Result<List<UserClubResponse>>>
{
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public GetClubsByUserHandler(IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<List<UserClubResponse>>> Handle(GetClubsByUser request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOwnerOrSuperadmin(request.UserId))
        {
            return Result.Failure<List<UserClubResponse>>(UserErrors.Forbidden);
        }

        var userClubs = await _clubMemberQueries.GetClubsByUserId(request.UserId, cancellationToken);
        return Result.Success(userClubs);
    }
}