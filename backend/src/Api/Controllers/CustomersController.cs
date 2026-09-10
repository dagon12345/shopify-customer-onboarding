using Api.Contracts;
using Application.Common.Messaging;
using Application.Customers.Commands.CreateCustomer;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Creates a customer locally and syncs it to Shopify.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCustomerCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.AcceptsMarketing,
            request.Note);

        var result = await _sender.Send(command, cancellationToken);

        var response = new CreateCustomerResponse(
            result.CustomerId,
            result.ShopifySyncSucceeded,
            result.ShopifyCustomerId,
            result.ShopifySyncError);

        return Created($"api/customers/{result.CustomerId}", response);
    }
}
