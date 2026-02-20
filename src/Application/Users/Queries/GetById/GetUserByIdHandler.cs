using Application.Abstractions.DTO;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Queries.GetById;

internal sealed class GetUserByIdHandler : IRequestHandler<GetUserById, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetUserByIdHandler(IUserRepository userRepository, IMapper mapper, IUserContext userContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<UserResponse>> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        if (!_userContext.IsOwnerOrSuperadmin(request.UserId))
        {
            return Result.Failure<UserResponse>(UserErrors.NotAuthorized);
        }
        
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(request.UserId));
        }

        var userResponse = _mapper.Map<UserResponse>(user);
        return Result.Success(userResponse);
    }
}