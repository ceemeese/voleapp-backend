using MediatR;
using SharedKernel;

namespace Application.Supports.Commands;

public sealed record SendContact(string Name, string Email, string Message) : IRequest<Result>
{
    
}