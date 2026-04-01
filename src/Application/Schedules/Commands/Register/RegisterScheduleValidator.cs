using FluentValidation;
using DayOfWeek = Domain.Club.Enum.DayOfWeek;

namespace Application.Schedules.Commands.Register;

internal sealed class RegisterScheduleValidator : AbstractValidator<RegisterSchedule>
{
    public RegisterScheduleValidator()
    {
        RuleFor(s => s.ClubId)
            .NotEmpty().WithErrorCode("Schedule.ClubRequired").WithMessage("El club es obligatorio");
        
        RuleFor(s => s.DayOfWeek)
            .NotEmpty().WithErrorCode("Schedule.DayOfWeekRequired").WithMessage("El día de la semana es obligatorio")
            .Must(BeAValidDay).WithErrorCode("Schedule.ValidDayOfWeek").WithMessage("El día de la semana no es válido");
        
        RuleFor(s => s.OpeningTime)
            .NotEmpty().WithErrorCode("Schedule.OpeningTimeRequired").WithMessage("La hora de apertura es obligatoria");
        
        RuleFor(s => s.ClosingTime)
            .NotEmpty().WithErrorCode("Schedule.ClosingTimeRequired").WithMessage("La hora de cierre es obligatoria");
    }


    private bool BeAValidDay(string dayOfWeek)
    {
        return Enum.TryParse<DayOfWeek>(dayOfWeek, ignoreCase:true, out _);
    }
}