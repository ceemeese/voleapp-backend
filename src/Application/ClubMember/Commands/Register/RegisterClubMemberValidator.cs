using Domain.Club.Enum;
using FluentValidation;

namespace Application.ClubMember.Commands.Register;

internal sealed class RegisterClubMemberValidator : AbstractValidator<RegisterClubMember>
{
    public RegisterClubMemberValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithErrorCode("ClubMember.UserRequired").WithMessage("El usuario es obligatorio");
        
        RuleFor(c => c.Role)
            .NotEmpty().WithErrorCode("ClubMember.RoleRequired").WithMessage("El rol es obligatorio")
            .Must(BeAValidRole).WithErrorCode("ClubMember.ValidRole").WithMessage("El rol no es válido");
    }
    
    private bool BeAValidRole(string type)
    {
        return Enum.TryParse<MemberRole>(type, ignoreCase:true, out _);
    }
}