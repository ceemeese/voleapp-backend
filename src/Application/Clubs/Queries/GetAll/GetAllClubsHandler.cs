using Application.Abstractions.DTO.Club;
using AutoMapper;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetAll;

internal sealed class GetAllClubsHandler : IRequestHandler<GetAllClubs, Result<List<ClubResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;
    
    public GetAllClubsHandler(IMapper mapper, IClubRepository clubRepository)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
    }

    public async Task<Result<List<ClubResponse>>> Handle(GetAllClubs request, CancellationToken cancellationToken)
    {
        var clubs = await _clubRepository.GetAll(cancellationToken);

        var clubsMapped = _mapper.Map<List<ClubResponse>>(clubs);
        return Result.Success(clubsMapped);
    }
}