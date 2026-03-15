using System.Net.Mime;
using ClientApi.Models;
using ClientApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers;

[Route("api/client")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class ClientController(IClientService clientService) : ControllerBase
{
    private readonly IClientService _clientService = clientService;

    /// <summary>Gets all clients</summary>
    /// <returns>A list of all clinents</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/client
    ///     Authorization: Bearer my-access-token
    /// 
    /// </remarks>
    /// <response code="200">Returns the list of all clients</response>
    /// <response code="401">If unauthorized</response>
    [HttpGet]
    [ProducesResponseType<IEnumerable<ClientResponseDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllClientsAsync();
        return Ok(clients);
    }

    /// <summary>Gets a client by id</summary>
    /// <param name="id"></param>
    /// <returns>A client with this id</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/client/1
    ///     Authorization: Bearer my-access-token
    /// 
    /// </remarks>
    /// <response code="200">Client is found</response>
    /// <response code="400">If id is not a number</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If client is not found</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ClientResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _clientService.GetClientByIdAsync(id);
        return Ok(client);
    }

    /// <summary>Gets a client by email</summary>
    /// <param name="email"></param>
    /// <returns>A client with this email</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/client/by-email?email=example@example.com
    ///     Authorization: Bearer my-access-token
    /// 
    /// </remarks>
    /// <response code="200">Client is found</response>
    /// <response code="400">If email is null or empty</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If client is not found</response>
    [HttpGet("by-email")]
    [ProducesResponseType<ClientResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail([FromQuery] string email)
    {
        var client = await _clientService.GetClientByEmailAsync(email);
        return Ok(client);
    }

    /// <summary>Creates a new client</summary>
    /// <param name="clientDto"></param>
    /// <returns>A newly created client</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/client
    ///     Authorization: Bearer my-access-token
    ///     {
    ///         "full_name": "Client's FullName",
    ///         "email": "example@example.com"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Returns the newly created client</response>
    /// <response code="400">If body is not valid</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="422">If the client with this email already exists</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<ClientResponseDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Add(ClientRequestDto clientDto)
    {
        var client = await _clientService.AddClientAsync(clientDto);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    /// <summary>Updates a client by id</summary>
    /// <param name="id"></param>
    /// <param name="clientDto"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/client/1
    ///     Authorization: Bearer my-access-token
    ///     {
    ///         "full_name": "Client's FullName",
    ///         "email": "example@example.com"
    ///     }
    /// 
    /// </remarks>
    /// <returns>NoContent</returns>
    /// <response code="204">The client has been successfully updated</response>
    /// <response code="400">If id is not a number or the body is not valid</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If client with this id is not found</response>
    /// <response code="422">If the client with this new email already exists</response>
    [HttpPut("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, ClientRequestDto clientDto)
    {
        await _clientService.UpdateClientAsync(id, clientDto);
        return NoContent();
    }

    /// <summary>Partially updates a client by id</summary>
    /// <param name="id"></param>
    /// <param name="clientDto"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PATCH /api/client/1
    ///     Authorization: Bearer my-access-token
    ///     {
    ///         "full_name": "New client's FullName",
    ///         "email": "new_example@example.com"
    ///     }
    /// 
    /// </remarks>
    /// <returns>NoContent</returns>
    /// <response code="204">The client has been successfully updated</response>
    /// <response code="400">If id is not a number, the body is not valid or nothing to update</response>
    /// <response code="401">If unauthorized</response>
    /// <response code="404">If client with this id is not found</response>
    /// <response code="422">If the client with this new email already exists</response>
    [HttpPatch("{id:int}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PartialUpdate(int id, ClientPartialUpdateRequestDto clientDto)
    {
        await _clientService.PartialUpdateClientAsync(id, clientDto);
        return NoContent();
    }

    /// <summary>Deletes a client by id</summary>
    /// <param name="id"></param>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/client/1
    ///     Authorization: Bearer my-access-token
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
        await _clientService.DeleteClientAsync(id);
        return NoContent();
    }
}
