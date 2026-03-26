using Application.Abstractions.DTO.Court;
using AutoMapper;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetAll;

internal sealed class GetAllCourtsHandler : IRequestHandler<GetAllCourts, Result<List<CourtSummaryResponse>>>
{
    private readonly IMapper _mapper;
    private readonly ICourtRepository _courtRepository;

    public GetAllCourtsHandler(IMapper mapper, ICourtRepository courtRepository)
    {
        _mapper = mapper;
        _courtRepository = courtRepository;
    }

    public async Task<Result<List<CourtSummaryResponse>>> Handle(GetAllCourts request, CancellationToken cancellationToken)
    {
        var courts = await _courtRepository.GetAll(cancellationToken);

        var courtsMapped = _mapper.Map<List<CourtSummaryResponse>>(courts);
        return Result.Success(courtsMapped);
    }
}