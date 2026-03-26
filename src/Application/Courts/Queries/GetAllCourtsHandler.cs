using Application.Abstractions.DTO;
using AutoMapper;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries;

internal sealed class GetAllCourtsHandler : IRequestHandler<GetAllCourts, Result<List<CourtResponse>>>
{
    private readonly IMapper _mapper;
    private readonly ICourtRepository _courtRepository;

    public GetAllCourtsHandler(IMapper mapper, ICourtRepository courtRepository)
    {
        _mapper = mapper;
        _courtRepository = courtRepository;
    }

    public async Task<Result<List<CourtResponse>>> Handle(GetAllCourts request, CancellationToken cancellationToken)
    {
        var courts = await _courtRepository.GetAll(cancellationToken);

        var courtsMapped = _mapper.Map<List<CourtResponse>>(courts);
        return Result.Success(courtsMapped);
    }
}