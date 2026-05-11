using Application.PricingConfig.Commands;
using Application.PricingConfig.Commands.Update;
using Application.PricingConfig.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Infrastructure;

namespace Web.Api.Controllers.PricingConfig;

[ApiController]
[Route("api/clubs")]
public sealed class PricingConfigController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public PricingConfigController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AuthorizeAdmins]
    [HttpPut("{clubId:guid}/pricing-config")]
    public async Task<IActionResult> UpdatePricingConfig([FromRoute] Guid clubId, [FromBody] UpdatePricingConfigRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePricingConfig(
            clubId,
            request.RainDiscountPercent,
            request.WindThreshold,
            request.WindDiscountPercent,
            request.HeatThreshold,
            request.HeatDiscountPercent,
            request.ColdThreshold,
            request.ColdDiscountPercent
        );
        
        var pricingConfigResult = await _mediator.Send(command, cancellationToken);

        return pricingConfigResult.IsSuccess
            ? Ok(pricingConfigResult.Value)
            : CustomResults.Problem(pricingConfigResult);
    }
    
    [AuthorizeAdmins]
    [HttpGet("{clubId:guid}/pricing-config")]
    public async Task<IActionResult> Get([FromRoute] Guid clubId, CancellationToken cancellationToken)
    {
        var query = new GetPricingConfig(clubId);
        
        var pricingConfigResult = await _mediator.Send(query, cancellationToken);

        return pricingConfigResult.IsSuccess
            ? Ok(pricingConfigResult.Value)
            : CustomResults.Problem(pricingConfigResult);
    }
    
}