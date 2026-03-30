using Domain.Club.Enum;
using FluentValidation;

namespace Application.ClubMember.Commands.Update;

internal sealed class UpdateClubMemberValidator : AbstractValidator<UpdateClubMember>
{
    public  UpdateClubMemberValidator()
    {
        RuleFor(c => c.ClubId)
            .NotEmpty().WithErrorCode("ClubMember.ClubRequired").WithMessage("El ID de club es obligatorio");
        
        RuleFor(c => c.UserId)
            .NotEmpty().WithErrorCode("ClubMember.UserRequired").WithMessage("El ID de usuario es obligatorio");
        
        RuleFor(c => c.Role)
            .NotEmpty().WithErrorCode("ClubMember.RoleRequired").WithMessage("El rol es obligatorio");
        
        RuleFor(c => c.Role)
            .NotEmpty().WithErrorCode("ClubMember.RoleRequired").WithMessage("El rol es obligatorio")
            .Must(BeAValidRole).WithErrorCode("ClubMember.ValidRole").WithMessage("El rol no es válido");
        
    }

    private static bool BeAValidRole(string role)
    {
        return Enum.TryParse<MemberRole>(role, ignoreCase: true, out _);
    }
}