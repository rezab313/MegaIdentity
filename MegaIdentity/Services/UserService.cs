using MegaIdentity.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Security.Claims;
using MegaIdentity.Models;
using MegaIdentity.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MegaIdentity.Services;

public interface IUserService
{
    Task<SignInResult> Login(LoginViewModel model);
    bool IsSignedIn(ClaimsPrincipal user);
    Task<IdentityResult> Register(RegisterViewModel model);
    Task<IdentityResult> ChangePassword(ClaimsPrincipal user, ChangePasswordViewModel model);
    Task<AddressDto> Address(ClaimsPrincipal user);
    Task<bool> UpdateAddress(ClaimsPrincipal user, AddressViewModel model);
}

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly AppDbContext _appDbContext;
    public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext appDbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _appDbContext = appDbContext;
    }
    /// <summary>
    /// Attempts to sign in the specified <paramref name="model.Email"/> and <paramref name="model.password"/> combination
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<SignInResult> Login(LoginViewModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);

        return result;
    }

    public bool IsSignedIn(ClaimsPrincipal user)
    {
        var result = _signInManager.IsSignedIn(user);

        return result;
    }

    public async Task<IdentityResult> Register(RegisterViewModel model)
    {
        var user = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email,
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        return result;
    }

    public async Task<IdentityResult> ChangePassword(ClaimsPrincipal user, ChangePasswordViewModel model)
    {
        var user_ = await _userManager.GetUserAsync(user);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        var result = await _userManager.ChangePasswordAsync(user_, model.CurrentPassword, model.NewPassword);
        return result;
    }

    public async Task<AddressDto> Address(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);
        if (userId == null) return null;
        var address = await _appDbContext
            .Addresses
            .Where(m => m.User.Id == userId)
            .Select(m => new AddressDto { City = m.City, State = m.State, Street = m.Street, ZipCode = m.ZipCode })
            .FirstOrDefaultAsync();
        return address;
    }

    public async Task<bool> UpdateAddress(ClaimsPrincipal user, AddressViewModel model)
    {
        var userId = _userManager.GetUserId(user);
        if (userId == null)
            return false;

        var address = await _appDbContext.Addresses.FirstOrDefaultAsync(m => m.User.Id == userId);
        if (address == null)
        {
            address = new Address() { UserId = userId };
            _appDbContext.Addresses.Add(address);
        }

        Map(model, address);//can use autoMapper: _mapper.Map(model, address);

        await _appDbContext.SaveChangesAsync();
        return true;
    }

    private void Map(AddressViewModel model, Address address)
    {
        address.State = model.State;
        address.City = model.City;
        address.Street = model.Street;
        address.ZipCode = model.ZipCode;
    }
}
