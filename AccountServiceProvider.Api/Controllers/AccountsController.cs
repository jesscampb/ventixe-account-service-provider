using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountServiceProvider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.CreateAsync(dto);
        if (!result.Succeeded)
            return Conflict(result.Message);

        return Ok(result.Result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _accountService.GetByIdAsync(id);
        if (!result.Succeeded)
            return NotFound(result.Message);

        return Ok(result.Result);
    }
}
