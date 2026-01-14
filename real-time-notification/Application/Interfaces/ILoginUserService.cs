using System;
using real_time_notification.Application.DTO;

namespace real_time_notification.Application.Interfaces;

public interface ILoginUserService
{
    Task<string?> LoginAsync(LoginUserDto loginUserDTO);
    Task<bool> RegisterAsync(RegisterDto registerDTO);
}
