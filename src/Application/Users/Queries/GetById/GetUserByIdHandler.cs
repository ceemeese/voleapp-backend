using Application.Abstractions.DTO;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserById, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(request.UserId));
        }

        var userResponse = _mapper.Map<UserResponse>(user);
        return Result.Success(userResponse);
    }
}