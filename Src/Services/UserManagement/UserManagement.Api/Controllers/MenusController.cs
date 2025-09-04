using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserManagement.Application.Menus;

namespace UserManagement.Api.Controllers;

[ApiController]
[Route("v1/menus")]
public sealed class MenusController : ControllerBase
{
    private readonly IMediator _mediator;
    public MenusController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(MenuDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MenuDto>> GetById(long id, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetMenuById(id), ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MenuDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<MenuDto>>> List(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken ct = default)
    {
        if (take <= 0 || take > 100) take = 20;
        var list = await _mediator.Send(new ListMenus(skip, take), ct);
        return Ok(list);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMenu cmd, CancellationToken ct)
    {
        try
        {
            var id = await _mediator.Send(cmd, ct);
            return Created($"/v1/menus/{id}", new { id });
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(title: "Invalid menu data", detail: ex.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMenu cmd, CancellationToken ct)
    {
        if (id != cmd.Id) return BadRequest();
        try
        {
            await _mediator.Send(cmd, ct);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        try
        {
            await _mediator.Send(new DeleteMenu(id), ct);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
