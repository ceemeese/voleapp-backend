using Application.Abstractions.DTO;
using AutoMapper;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAllSearch;

internal sealed class GetAllClubsSearchHandler : IRequestHandler<GetAllClubsSearch, Result<List<ClubSummaryResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;

    public GetAllClubsSearchHandler(IMapper mapper, IClubRepository clubRepository)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
    }

    public async Task<Result<List<ClubSummaryResponse>>> Handle(GetAllClubsSearch request, CancellationToken cancellationToken)
    {
        var clubs = await _clubRepository.GetAllSearch(request.Name, cancellationToken);

        var clubsMapped = _mapper.Map<List<ClubSummaryResponse>>(clubs);
        return Result.Success(clubsMapped);
    }
}