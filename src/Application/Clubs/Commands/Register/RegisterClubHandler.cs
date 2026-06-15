using Application.Abstractions.DTO.Club;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using Domain.Common.ValueObjects;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Commands.Register;

internal sealed class RegisterClubHandler : IRequestHandler<RegisterClub, Result<ClubResponse>>
{
    private readonly IClubRepository _clubRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public RegisterClubHandler(IClubRepository clubRepository, IUnitOfWork unitOfWork,  IUserContext userContext, IMapper mapper)
    {
        _clubRepository = clubRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        
    }

    public async Task<Result<ClubResponse>> Handle(RegisterClub request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<ClubResponse>(ClubErrors.Forbidden);     
        }
        
        var addressResult = Address.Create(request.Street, request.City, request.ZipCode, request.Country);
        if (addressResult.IsFailure)
        {
            return Result.Failure<ClubResponse>(addressResult.Error);
        }

        var club = Club.Create(
            request.Name,
            addressResult.Value,
            request.PhoneNumber,
            request.Email
        );
        
        _clubRepository.Add(club);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var clubMapped = _mapper.Map<ClubResponse>(club);
        return Result.Success(clubMapped);
    }
}
