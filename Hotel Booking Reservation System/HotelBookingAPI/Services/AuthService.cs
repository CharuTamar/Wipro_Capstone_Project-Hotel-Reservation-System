using HotelBookingAPI.Models;
using HotelBookingAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelBookingAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    //  Register User
    public async Task<string> RegisterUserAsync(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            //  Ensure "User" role exists before assigning
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            //  Remove any existing "Guest" role (if added by default)
            if (await _userManager.IsInRoleAsync(user, "Guest"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Guest");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                return $"User registered, but role assignment failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}";
            }

            return "User registered and assigned to 'User' role successfully!";
        }

        return string.Join(", ", result.Errors.Select(e => e.Description));
    }


    //  Login User
    public async Task<string> LoginUserAsync(LoginModel model)
{
    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

    if (!result.Succeeded) return null; //  Ensure it returns null for login failure

    var user = await _userManager.FindByEmailAsync(model.Email);
    var token = await GenerateJwtToken(user);
    return token;
}


    //  Assign Role
    public async Task<string> AssignRoleAsync(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return "User not found!";

        //  Remove existing roles before assigning a new one
        var existingRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, existingRoles);

        //  Create the role if it doesn't exist
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded) return $"Role '{roleName}' assigned to user '{email}'.";

        return string.Join(", ", result.Errors.Select(e => e.Description));
    }


    //  Generate JWT Token
    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        //  Remove "Guest" role for authenticated users
        userRoles = userRoles.Where(r => r != "Guest").ToList();

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        if (userRoles.Any())
        {
            claims.Add(new Claim(ClaimTypes.Role, userRoles.First())); //  Only one role (User/Admin)
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(2),
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        Console.WriteLine($"Generated JWT Token: {token}");
        Console.WriteLine($"Claims in Token: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

        return token;
    }


    public async Task<bool> CheckUserExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    //  Get Current User
    public async Task<object> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new
        {
            user.Id,
            user.Email,
            user.UserName,
            Roles = roles
        };
    }


}
