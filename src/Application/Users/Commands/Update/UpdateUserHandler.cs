using Application.Abstractions.DTO;
using Application.Abstractions.Errors;
using Application.Abstractions.Extensions;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Update;

internal sealed class UpdateUserHandler : IRequestHandler<UpdateUser, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UpdateUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityService identityService, IUserContext userContext, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(UpdateUser request, CancellationToken cancellationToken)
    {
        
        if (!_userContext.IsOwnerOrSuperadmin(request.Id))
        {
            return Result.Failure<UserResponse>(UserErrors.Forbidden);
        }
        
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserResponse>(UserErrors.NotFound(request.Id));
        }
        
        if (user.Username == "superadmin")
        {
            return Result.Failure<UserResponse>(IdentityErrors.CannotUpdateSuperAdmin);
        }
        
        var oldUserName = user.Username;
        
        var identityResult = await _identityService.UpdateUserProfileAsync(request.Id, oldUserName, request.Username, request.Email);

        if (identityResult.IsFailure)
        {
            return Result.Failure<UserResponse>(identityResult.Error);
        }
        
        user.UpdateProfile(request.Username, request.PhoneNumber, request.Email);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var userMapped = _mapper.Map<UserResponse>(user);
        return Result.Success(userMapped);
    }
}