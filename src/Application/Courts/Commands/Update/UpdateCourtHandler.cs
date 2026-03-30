using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club.Entities;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Update;

internal sealed class UpdateCourtHandler : IRequestHandler<UpdateCourt, Result>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    
    public UpdateCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork, IUserContext userContext, IClubMemberQueries clubMemberQueries)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result> Handle(UpdateCourt request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtById(request.Id, cancellationToken);
        if (court is null)
        {
            return Result.Failure<Unit>(CourtErrors.NotFound(request.Id));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<Unit>(ClubMemberErrors.Forbidden);
            }
        }
        
        var isDuplicate = await _courtRepository.ExistsByNameInClubExcludeId(court.ClubId, request.Name, request.Id, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<Unit>(CourtErrors.DuplicateName(request.Name));
        }

        var courtResult = court.UpdateProfile(request.Name, request.BasePrice);
        if (courtResult.IsFailure)
        {
            return Result.Failure<Unit>(courtResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}