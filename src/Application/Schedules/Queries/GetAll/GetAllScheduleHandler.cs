using Application.Abstractions.DTO.Schedule;
using AutoMapper;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Schedules.Queries.GetAll;

internal sealed class GetAllScheduleHandler : IRequestHandler<GetAllSchedule, Result<List<ScheduleResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;

    public GetAllScheduleHandler(IMapper mapper, IClubRepository clubRepository)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
    }

    public async Task<Result<List<ScheduleResponse>>> Handle(GetAllSchedule request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubByIdAsync(request.ClubId, cancellationToken);
        if (club is null)
        {
            return Result.Failure<List<ScheduleResponse>>(ClubErrors.NotFound(request.ClubId));
        }

        var schedulesMapped = _mapper.Map<List<ScheduleResponse>>(club.Schedules);
        return Result.Success<List<ScheduleResponse>>(schedulesMapped);
    }
}