using Application.Abstractions.DTO.Court;
using AutoMapper;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetByClubId;

internal sealed class GetByClubIdHandler : IRequestHandler<GetByClubId, Result<List<CourtSummaryResponse>>>
{
    private  readonly ICourtRepository _courtRepository;
    private readonly IMapper _mapper;

    public GetByClubIdHandler(ICourtRepository courtRepository, IMapper mapper)
    {
        _courtRepository = courtRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<CourtSummaryResponse>>> Handle(GetByClubId request, CancellationToken cancellationToken)
    {
        var courts = await _courtRepository.GetCourtsByClubId(request.ClubId, cancellationToken);
        var courtsMapped = _mapper.Map<List<CourtSummaryResponse>>(courts);
        return Result.Success(courtsMapped);
    }
}