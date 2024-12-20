﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportSpot.V1.Exceptions;
using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.Session.Enums;
using SportSpot.V1.Session.Services;
using SportSpot.V1.User.Entities;
using SportSpot.V1.User.Extensions;
using SportSpot.V1.User.Services;

namespace SportSpot.V1.Session.Controller
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SessionController(ISessionService _sessionService, IUserService _userService, UserManager<AuthUserEntity> _userManager) : ControllerBase
    {
        [Authorize]
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> CreateSession([FromBody] SessionCreateRequestDto request)
        {
            SessionDto session = await _sessionService.CreateSession(request, await User.GetAuthUser(_userManager));
            return new ObjectResult(session)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

        [Authorize]
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SessionDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetSessions([FromQuery] SessionUserSearchQueryDto requestDto)
        {
            (List<SessionDto> sessions, bool hasMoreEntries) = await _sessionService.GetSessionsFromUser(await User.GetAuthUser(_userManager), requestDto);
            Response.Headers.Append("X-Has-More-Entries", hasMoreEntries.ToString());
            return Ok(sessions);
        }

        [Authorize]
        [HttpGet("{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> GetSession(Guid sessionId)
        {
            SessionDto session = await _sessionService.GetDto(sessionId, await User.GetAuthUser(_userManager));
            return Ok(session);
        }

        [Authorize]
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> SearchSessions([FromQuery] SessionSearchQueryDto sessionSearchDto)
        {
            (List<SessionDto> sessions, bool hasMoreEntries) = await _sessionService.GetSessionsInRange(sessionSearchDto, await User.GetAuthUser(_userManager));
            Response.Headers.Append("X-Has-More-Entries", hasMoreEntries.ToString());
            return Ok(sessions);
        }


        [Authorize]
        [HttpPut("{sessionId}/join")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> JoinSession(Guid sessionId)
        {
            await _sessionService.Join(await User.GetAuthUser(_userManager), await _sessionService.Get(sessionId));
            return Ok();
        }

        [Authorize]
        [HttpPut("{sessionId}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> LeaveSession(Guid sessionId)
        {
            await _sessionService.Leave(await User.GetAuthUser(_userManager), await _sessionService.Get(sessionId));
            return Ok();
        }

        [Authorize]
        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SportType>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public IActionResult GetSportTypes()
        {
            return Ok(Enum.GetValues<SportType>());
        }

        [Authorize]
        [HttpDelete("{sessionId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> KickUser(Guid sessionId, Guid userID)
        {
            AuthUserEntity targetUser = await _userService.GetUser(userID);
            AuthUserEntity currentUser = await User.GetAuthUser(_userManager);
            SessionEntity session = await _sessionService.Get(sessionId);
            await _sessionService.KickUser(targetUser, session, currentUser);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{sessionId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SessionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<ErrorResult>))]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            AuthUserEntity currentUser = await User.GetAuthUser(_userManager);
            SessionEntity session = await _sessionService.Get(sessionId);
            await _sessionService.Delete(currentUser, session);
            return Ok();
        }
    }
}
