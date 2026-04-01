using FluentValidation;

namespace Application.Schedules.Commands.Update;

internal sealed class UpdateScheduleValidator : AbstractValidator<UpdateSchedule>
{
    public UpdateScheduleValidator()
    {
        RuleFor(s => s.ClubId)
            .NotEmpty().WithErrorCode("Schedule.ClubRequired").WithMessage("El club es obligatorio");
        
        RuleFor(s => s.ScheduleId)
            .NotEmpty().WithErrorCode("Schedule.ScheduleRequired").WithMessage("El calendario es obligatorio");
        
        RuleFor(s => s.OpeningTime)
            .NotEmpty().WithErrorCode("Schedule.OpeningTimeRequired").WithMessage("La hora de apertura es obligatoria");

        RuleFor(s => s.ClosingTime)
            .NotEmpty().WithErrorCode("Schedule.ClosingTimeRequired").WithMessage("La hora de cierre es obligatoria");
    }
}