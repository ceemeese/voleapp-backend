using Application.Abstractions.DTO.Club;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAllSearch;

internal sealed class GetAllClubsSearchHandler : IRequestHandler<GetAllClubsSearch, Result<List<ClubSummaryResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;
    private readonly IUserContext _userContext;

    public GetAllClubsSearchHandler(IMapper mapper, IClubRepository clubRepository, IUserContext userContext)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
        _userContext = userContext;
    }

    public async Task<Result<List<ClubSummaryResponse>>> Handle(GetAllClubsSearch request, CancellationToken cancellationToken)
    {
        var clubs = _userContext.IsOnlySuperadmin()
            ? await _clubRepository.GetAllSearchAsync(request.Name, cancellationToken)
            : await _clubRepository.GetAllSearchActiveAsync(request.Name, cancellationToken);

        var clubsMapped = _mapper.Map<List<ClubSummaryResponse>>(clubs);
        return Result.Success(clubsMapped);
    }
}