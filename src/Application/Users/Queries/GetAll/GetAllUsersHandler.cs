using Application.Abstractions.DTO.User;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetAll;

internal sealed class GetAllUsersHandler : IRequestHandler<GetAllUsers, Result<List<UserResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAllUsersHandler(IUserRepository userRepository, IMapper mapper, IUserContext userContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userContext = userContext;
    }
    
    public async Task<Result<List<UserResponse>>> Handle(GetAllUsers request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOnlySuperadmin())
        {
            return Result.Failure<List<UserResponse>>(UserErrors.Forbidden);
        }
        var users = await _userRepository.GetAll(cancellationToken);
        var usersMapped = _mapper.Map<List<UserResponse>>(users);
        return Result.Success(usersMapped);
    }
}