using MegaIdentity.Services;
using MegaIdentity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MegaIdentity.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [Authorize]
        public async Task<IActionResult> Detail()
        {
            return View(await _addressService.Address(User) ?? new Dtos.AddressDto());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Detail(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _addressService.UpdateAddress(User, model);

                if (result)
                    return RedirectToAction("Index", "Home");

                ModelState.AddModelError("", result.ToString());
            }
            return View();
        }


    }
}
