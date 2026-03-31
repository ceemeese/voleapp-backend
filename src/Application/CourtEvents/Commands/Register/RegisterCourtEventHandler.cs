using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Court;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.CourtEvents.Commands.Register;

internal sealed class RegisterCourtEventHandler : IRequestHandler<RegisterCourtEvent, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly ICourtRepository _courtRepository;
    
    public RegisterCourtEventHandler(IUnitOfWork unitOfWork, IUserContext userContext,  IClubMemberQueries clubMemberQueries,  ICourtRepository courtRepository)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _courtRepository = courtRepository;
    }

    public async Task<Result<int>> Handle(RegisterCourtEvent request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<int>(UserErrors.Forbidden);
        }
        
        var court = await _courtRepository.GetCourtWithEventsByDateRangeAsync(request.CourtId,request.StartTime, cancellationToken);
        if (court is null)
        {
            return Result.Failure<int>(CourtErrors.NotFound(request.CourtId));
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(court.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<int>(ClubErrors.Forbidden);
            }
        }
        
        var courtEventResult = court.AddEvent(request.StartTime, request.EndTime, request.EventName, request.Description);
        if (courtEventResult.IsFailure)
        {
            return Result.Failure<int>(courtEventResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<int>(courtEventResult.Value.Id);
    }
}