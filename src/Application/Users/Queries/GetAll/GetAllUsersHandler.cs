using Application.Abstractions.DTO;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetAll;

internal sealed class GetAllUsersHandler : IRequestHandler<GetAllUsers, Result<List<UserResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public async Task<Result<List<UserResponse>>> Handle(GetAllUsers request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAll(cancellationToken);
        var userResponse = _mapper.Map<List<UserResponse>>(users);
        return Result.Success(userResponse);
    }
}