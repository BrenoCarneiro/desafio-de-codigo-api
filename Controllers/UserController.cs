using Microsoft.AspNetCore.Mvc;
using LacunaGenetics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using LacunaGenetics.Services;

namespace LacunaGenetics.Controllers;

[AllowAnonymous]
public class UserController : Controller
{
    private readonly ConnectionService _connectionService;

    public UserController(ConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult Login()
    {

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/User/Register")]
    public async Task<IActionResult> RegisterUser([FromForm] UserModel user)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Message = "Invalid data";
            return View("Register");
        }
        else
        {
            await _connectionService.Register(user, HttpContext);
            ResponseModel responseObject = _connectionService.ResponseObject;
            if (responseObject?.Code == Codes.Error)
                ViewBag.Message = $"{responseObject?.Message.ToString()}";
            else
                ViewBag.Message = $"{responseObject?.Code.ToString()}";
            return View("Register");
        }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/User/Login")]
    public async Task<IActionResult> LoginUser([FromForm] UserModel user)
    {
        if (!string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
        {
            await _connectionService.Login(user, HttpContext);
            ResponseModel responseObject = _connectionService.ResponseObject;
            if (!string.IsNullOrWhiteSpace(responseObject.AccessToken))
            {
                return RedirectToAction("Index", "Start");
            }
            else
            {
                ViewBag.Message = $"{responseObject?.Message.ToString()}";
            }
        }
        return View("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}



