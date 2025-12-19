using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using real_time_notification.Application.DTO;
using real_time_notification.Services.Interface;

namespace real_time_notification.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/login-users")]
[ApiVersion("1.0")]
public class LoginUsersController(ILoginUserService loginUserService, ILogger<LoginUsersController> logger) : ControllerBase
{
    private readonly ILoginUserService _loginUserService = loginUserService;
    private ILogger<LoginUsersController> _logger = logger;

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterDTO registerDTO)
    {
        try
        {
            var result = await _loginUserService.RegisterAsync(registerDTO);
            if (!result)
            {
                return BadRequest("Não foi possivel realizar o cadastro");
            }

            return Created();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro durante o registro do usuário.");
            throw;
        }
    }
}

