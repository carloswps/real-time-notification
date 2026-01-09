using System;
using real_time_notification.Application.DTO;

namespace real_time_notification.Application.Interfaces;

public interface ILoginUserService
{
    Task<String?> LoginAsync(LoginUserDTO loginUserDTO);
    Task<bool> RegisterAsync(RegisterDTO registerDTO);
}
