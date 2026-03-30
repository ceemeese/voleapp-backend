using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Common.ValueObjects;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Update;

internal sealed class UpdateClubHandler : IRequestHandler<UpdateClub, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;

    public UpdateClubHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext,  IClubMemberQueries clubMemberQueries)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
    }

    public async Task<Result> Handle(UpdateClub request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<int>(UserErrors.Forbidden);
        }

        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.Id, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<int>(ClubErrors.Forbidden);
            }
        }

        var club = await _clubRepository.GetClubById(request.Id, cancellationToken);

        if (club is null)
        {
            return Result.Failure<Unit>(ClubErrors.NotFound(request.Id));
        }
        
        var addressResult = Address.Create(request.Street, request.City, request.ZipCode, request.Country);
        if (addressResult.IsFailure)
        {
            return Result.Failure<Guid>(addressResult.Error);
        }
        
        club.UpdateProfile(
            request.Name, 
            request.Cif, 
            addressResult.Value,
            request.PhoneNumber,
            request.Email
            );
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(Unit.Value);
    }
}