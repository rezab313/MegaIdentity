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

public interface IAddressService
{
    Task<AddressDto> Address(ClaimsPrincipal user);

    Task<bool> UpdateAddress(ClaimsPrincipal user, AddressViewModel model);
}

public class AddressService : IAddressService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly AppDbContext _appDbContext;

    public AddressService(UserManager<IdentityUser> userManager, AppDbContext appDbContext)
    {
        _userManager = userManager;
        _appDbContext = appDbContext;
    }
    
    public async Task<AddressDto> Address(ClaimsPrincipal user)
    {
        var userId = _userManager.GetUserId(user);

        if (userId == null) return null;

        var address = await _appDbContext
            .Addresses
            .Where(m => m.User.Id == userId)
            .Select(m => new AddressDto
            {
                City = m.City,
                State = m.State,
                Street = m.Street,
                ZipCode = m.ZipCode
            })
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
