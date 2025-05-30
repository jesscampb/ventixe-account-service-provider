using AccountServiceProvider.Api.Data.Entities;
using AccountServiceProvider.Api.Dtos;
using AccountServiceProvider.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountServiceProvider.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService = accountService;


    /// <summary>
    /// Creates a new account profile.
    /// </summary>
    /// <param name="dto">The request DTO containing IdentityId and profile details.</param>
    /// <returns>The created account profile.</returns>
    /// <response code="200">The profile was created successfully.</response>
    /// <response code="400">The request was invalid (validation failed).</response>
    /// <response code="409">Conflict: a profile with the given IdentityId already exists.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AccountProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.CreateAsync(dto);
        if (!result.Succeeded)
            return Conflict(result.Message);

        return Ok(result.Result);
    }


    /// <summary>
    /// Retrieves an existing account profile by its IdentityId.
    /// </summary>
    /// <param name="id">The IdentityId of the profile to retrieve.</param>
    /// <returns>The requested account profile along with its address.</returns>
    /// <response code="200">The profile was found and is returned.</response>
    /// <response code="404">No profile with the specified IdentityId was found.</response>
    [HttpGet("{id}", Name = "GetAccountById")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AccountProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _accountService.GetByIdAsync(id);
        if (!result.Succeeded)
            return NotFound(result.Message);

        return Ok(result.Result);
    }


    /// <summary>
    /// Updates an existing account profile.
    /// </summary>
    /// <param name="id">The IdentityId of the profile to update.</param>
    /// <param name="dto">The request DTO containing updated profile details.</param>
    /// <returns>The updated account profile.</returns>
    /// <response code="200">The profile was updated successfully.</response>
    /// <response code="400">The request was invalid (validation failed).</response>
    /// <response code="404">No profile with the specified IdentityId was found.</response>
    [HttpPut("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AccountProfile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProfileRequest dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.UpdateAsync(id, dto);
        if (!result.Succeeded)
            return NotFound(result.Message);

        return Ok(result.Result);
    }

    /// <summary>
    /// Deletes an existing account profile.
    /// </summary>
    /// <param name="id">The IdentityId of the profile to delete.</param>
    /// <response code="204">The profile was deleted successfully.</response>
    /// <response code="404">No profile with the specified IdentityId was found.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _accountService.DeleteAsync(id);
        if (!result.Succeeded)
            return NotFound(result.Message);

        return NoContent();
    }
}
