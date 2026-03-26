using Application.Abstractions.DTO.Club;
using AutoMapper;
using Domain.Club;
using MediatR;
using SharedKernel;

namespace Application.Clubs.Queries.GetById;

internal sealed class GetClubByIdHandler : IRequestHandler<GetClubById, Result<ClubResponse>>
{
    private readonly IMapper _mapper;
    private readonly IClubRepository _clubRepository;

    public GetClubByIdHandler(IMapper mapper, IClubRepository clubRepository)
    {
        _mapper = mapper;
        _clubRepository = clubRepository;
    }

    public async Task<Result<ClubResponse>> Handle(GetClubById request, CancellationToken cancellationToken)
    {
        var club = await _clubRepository.GetClubById(request.ClubId, cancellationToken);

        if (club is null)
        {
            return Result.Failure<ClubResponse>(ClubErrors.NotFound(request.ClubId));
        }
        var clubMapped = _mapper.Map<ClubResponse>(club);
        return Result.Success(clubMapped);
    }
}
