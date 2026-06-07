using Application.Abstractions.DTO;
using Application.Abstractions.Interfaces;
using AutoMapper;
using Domain.User;
using MediatR;
using SharedKernel;

namespace Application.Users.Commands.Register;

internal sealed class RegisterUserHandler : IRequestHandler<RegisterUser, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService; 
    private readonly IMapper _mapper;
    
    public RegisterUserHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IIdentityService identityService, IMapper mapper)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<Result<UserResponse>> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistByDniAsync(request.Dni, cancellationToken))
        {
            return Result.Failure<UserResponse>(UserErrors.DniDuplicated);
        }
        
        var identityResult = await _identityService.CreateUserAsync(request.Username, request.Email, request.Password);

        if (identityResult.IsFailure)
        {
            return Result.Failure<UserResponse>(identityResult.Error);
        }

        var user = User.Create(
            identityResult.Value,
            request.Dni,
            request.Name,
            request.LastName,
            request.Username,
            request.Email,
            request.PhoneNumber
        );

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var userMapped = _mapper.Map<UserResponse>(user);
        return Result.Success(userMapped);
    }
}