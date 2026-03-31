using Application.Abstractions.DTO.Court;
using AutoMapper;
using Domain.Court;
using MediatR;
using SharedKernel;

namespace Application.Courts.Queries.GetById;

internal sealed class GetCourtByIdHandler : IRequestHandler<GetCourtById, Result<CourtSummaryResponse>>
{
    private readonly ICourtRepository _courtRepository;
    private readonly IMapper _mapper;

    public GetCourtByIdHandler(ICourtRepository courtRepository, IMapper mapper)
    {
        _courtRepository = courtRepository;
        _mapper = mapper;
    }

    public async Task<Result<CourtSummaryResponse>> Handle(GetCourtById request, CancellationToken cancellationToken)
    {
        var court = await _courtRepository.GetCourtById(request.CourtId, cancellationToken);

        if (court is null)
        {
            return Result.Failure<CourtSummaryResponse>(CourtErrors.NotFound(request.CourtId));
        }
        var courtMapped = _mapper.Map<CourtSummaryResponse>(court);
        return Result.Success(courtMapped);
    }
}