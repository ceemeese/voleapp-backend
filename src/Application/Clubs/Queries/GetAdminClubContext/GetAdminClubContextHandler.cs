using Application.Abstractions.Interfaces;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAdminClubContext;

internal sealed class GetAdminClubContextHandler : IRequestHandler<GetAdminClubContext, Result<Guid>>
{
    private readonly IClubContext _clubContext;
    
    public GetAdminClubContextHandler(IClubContext clubContext)
    {
        _clubContext = clubContext;
    }

    public async Task<Result<Guid>> Handle(GetAdminClubContext request, CancellationToken cancellationToken)
    {
        var clubId = await _clubContext.GetClubIdAsync();

        if (clubId == Guid.Empty)
        {
            return Result.Failure<Guid>(ClubErrors.AdminContextNotFound);
        }
        
        return Result.Success(clubId);
    }
}