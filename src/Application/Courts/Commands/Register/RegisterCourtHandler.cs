using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club.Entities;
using Domain.Court;
using Domain.Court.Enum;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Register;

internal sealed class RegisterCourtHandler : IRequestHandler<RegisterCourt, Result<Guid>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public RegisterCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork,  IUserContext userContext, IClubMemberQueries clubMemberQueries)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result<Guid>> Handle(RegisterCourt request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<Guid>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<Guid>(ClubMemberErrors.Forbidden);
            }
        }
        
        var isDuplicate = await _courtRepository.ExistsByNameInClub(request.ClubId, request.Name, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<Guid>(CourtErrors.DuplicateName(request.Name));
        }
        
        if (!Enum.TryParse<CourtType>(request.Type, ignoreCase: true, out var courtType))
        {
            return Result.Failure<Guid>(CourtErrors.InvalidType);
        }
        
        var courtResult = Court.Create(request.ClubId, request.Name, courtType, request.BasePrice, request.IsActive);

        if (courtResult.IsFailure)
        {
            return Result.Failure<Guid>(courtResult.Error);
        }
     
        _courtRepository.Add(courtResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success<Guid>(courtResult.Value.Id);
    }
}