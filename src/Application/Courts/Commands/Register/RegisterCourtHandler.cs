using Application.Abstractions.DTO.Court;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club.Entities;
using Domain.Court;
using Domain.Court.Enum;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Courts.Commands.Register;

internal sealed class RegisterCourtHandler : IRequestHandler<RegisterCourt, Result<CourtResponse>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IClubMemberQueries _clubMemberQueries;
    private readonly IMapper _mapper;

    public RegisterCourtHandler(ICourtRepository courtRepository, IUnitOfWork unitOfWork,  IUserContext userContext, IClubMemberQueries clubMemberQueries, IMapper mapper)
    {
        _courtRepository = courtRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _clubMemberQueries = clubMemberQueries;
        _mapper = mapper;
    }

    public async Task<Result<CourtResponse>> Handle(RegisterCourt request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsAnyAdmin())
        {
            return Result.Failure<CourtResponse>(UserErrors.Forbidden);
        }
        
        if (!_userContext.IsOnlySuperadmin())
        {
            var hasPermission = await _clubMemberQueries.IsAdminInClub(request.ClubId, _userContext.UserId, cancellationToken);

            if (!hasPermission)
            {
                return Result.Failure<CourtResponse>(ClubMemberErrors.Forbidden);
            }
        }
        
        var isDuplicate = await _courtRepository.ExistsByNameInClubAsync(request.ClubId, request.Name, cancellationToken);
        if (isDuplicate)
        {
            return Result.Failure<CourtResponse>(CourtErrors.DuplicateName(request.Name));
        }
        
        if (!Enum.TryParse<CourtType>(request.Type, ignoreCase: true, out var type))
        {
            return Result.Failure<CourtResponse>(CourtErrors.InvalidType);
        }
        
        var courtResult = Court.Create(request.ClubId, request.Name, type, request.BasePrice, request.IsActive);

        if (courtResult.IsFailure)
        {
            return Result.Failure<CourtResponse>(courtResult.Error);
        }
     
        _courtRepository.Add(courtResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var courtMapped = _mapper.Map<CourtResponse>(courtResult.Value);
        return Result.Success(courtMapped);
    }
}