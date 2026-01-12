using System;
using Microsoft.EntityFrameworkCore;
using real_time_notification.Application.DTO;
using real_time_notification.Application.JwtGenerate;
using real_time_notification.Domain.Entities;
using real_time_notification.Infra;
using real_time_notification.Application.Interfaces;

namespace real_time_notification.Application.Services;

public class LoginUserService(AppDbContext context, ILogger<LoginUserService> logger, TokenService tokeService)
    : ILoginUserService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<LoginUserService> _logger = logger;
    private readonly TokenService _tokeService = tokeService;

    public async Task<bool> RegisterAsync(RegisterDTO registerDTO)
    {
        try
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDTO.Email)) return false;

            var password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            var user = new User
            {
                Email = registerDTO.Email,
                Password = password
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during user registration.");
            throw;
        }
    }


    public async Task<string?> LoginAsync(LoginUserDTO loginUserDTO)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginUserDTO.Email);

            if (user == null) return null;

            var senhaOk = BCrypt.Net.BCrypt.Verify(loginUserDTO.Password, user.Password);

            if (!senhaOk) return null;

            return _tokeService.GenerateToke(user);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred during user login.");
            throw;
        }
        
    }
}