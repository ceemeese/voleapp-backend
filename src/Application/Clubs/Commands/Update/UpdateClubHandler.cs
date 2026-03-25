using Application.Abstractions.Interfaces;
using Domain.Club;
using Domain.Common.ValueObjects;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Update;

internal sealed class UpdateClubHandler : IRequestHandler<UpdateClub, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;

    public UpdateClubHandler(IUnitOfWork unitOfWork, IClubRepository clubRepository, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _clubRepository = clubRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateClub request, CancellationToken cancellationToken)
    {
        //TODO:Solo puede modificar si es Admin del club
        if (!_userContext.IsAnyAdmin)
        {
            return Result.Failure(ClubErrors.Forbidden);
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