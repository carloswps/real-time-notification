using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using real_time_notification.Application.DTO;
using real_time_notification.Application.Interfaces;

namespace real_time_notification.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/login-users")]
public class LoginUsersController(ILoginUserService loginUserService, ILogger<LoginUsersController> logger)
    : ControllerBase
{
    private readonly ILoginUserService _loginUserService = loginUserService;
    private readonly ILogger<LoginUsersController> _logger = logger;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterDTO registerDTO)
    {
        try
        {
            var result = await _loginUserService.RegisterAsync(registerDTO);
            if (!result) return BadRequest("Não foi possivel realizar o cadastro");

            return Ok(new { result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro durante o registro do usuário.");
            throw;
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginUserAsync([FromBody] LoginUserDTO loginUserDTO)
    {
        try
        {
            var token = await _loginUserService.LoginAsync(loginUserDTO);
            if (token == null) return Unauthorized("Credenciais invalidas");

            return Ok(new { token = token, message = "Login realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro durante o login do usuário.");
            throw;
        }
    }
}