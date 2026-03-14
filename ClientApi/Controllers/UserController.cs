using System.Net.Mime;
using ClientApi.Models;
using ClientApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    /// <summary>Gets all users</summary>
    /// <returns>A list of all clinents</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/user
    /// 
    /// </remarks>
    /// <response code="200">Returns the list of all users</response>
    /// <response code="401">If unauthorized</response>
    [HttpGet]
    [ProducesResponseType<IEnumerable<UserResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>Gets a user by id</summary>
    /// <param name="id"></param>
    /// <returns>A user with that id</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/user/1
    /// 
    /// </remarks>
    /// <response code="200">User is found</response>
    /// <response code="400">If id is not a number</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If user is not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    /// <summary>Gets a user by login</summary>
    /// <param name="login"></param>
    /// <returns>A user with this login</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/user/1
    /// 
    /// </remarks>
    /// <response code="200">User is found</response>
    /// <response code="400">If login is null or empty</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If user is not found</response>

    [HttpGet("by-login")]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByLogin([FromQuery] string login)
    {
        var user = await _userService.GetUserByLoginAsync(login);
        return Ok(user);
    }

    /// <summary>Creates a new user</summary>
    /// <param name="userDto"></param>
    /// <returns>A newly created user</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/user
    ///     {
    ///         "login": "login",
    ///         "password": "password"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If body is not valid</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="422">If the user with this login already exists</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<UserResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Add(UserRequestDto userDto)
    {
        var user = await _userService.AddUserAsync(userDto);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Updates a user by id</summary>
    /// <param name="id"></param>
    /// <param name="userDto"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/user/1
    ///     {
    ///         "login": "new_login",
    ///         "password": "new_password"
    ///     }
    /// 
    /// </remarks>
    /// <returns>NoContent</returns>
    /// <response code="204">The user has been successfully updated</response>
    /// <response code="400">If id is not a number or the body is not valid</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If user with this id is not found</response>
    /// <response code="422">If the user with this new login already exists</response>
    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, UserRequestDto userDto)
    {
        await _userService.UpdateUserAsync(id, userDto);
        return NoContent();
    }

    /// <summary>Partially updates a user by id</summary>
    /// <param name="id"></param>
    /// <param name="userDto"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PATCH /api/user/1
    ///     {
    ///         "login": "new_login"
    ///     }
    /// 
    /// </remarks>
    /// <returns>NoContent</returns>
    /// <response code="204">The user has been successfully updated</response>
    /// <response code="400">If id is not a number, the body is not valid or nothing to update</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If user with this id is not found</response>
    /// <response code="422">If the user with this new login already exists</response>
    [HttpPatch("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PartialUpdate(int id, UserPartialUpdateRequestDto userDto)
    {
        await _userService.PartialUpdateUserAsync(id, userDto);
        return NoContent();
    }

    /// <summary>Deletes a user by id</summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/client/1
    /// 
    /// </remarks>
    /// <returns>NoContent</returns>
    /// <response code="204">Client has been successfully deleted</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If client with this id is not found</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
