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

    [HttpGet]
    public IActionResult Login()
    {

        return View();
    }

    //[HttpPost]
    //public async Task<IActionResult> Register(User user)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        ViewBag.Message = "Invalid data";
    //        return View();
    //    }
    //    else
    //    {
    //        try
    //        {
    //            var httpClient = _httpClientFactory.CreateClient("gene");
    //            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/users/create", user);
    //            if (response.IsSuccessStatusCode)
    //            {
    //                string jsonString = await response.Content.ReadAsStringAsync();
    //                ResponseObject = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
    //            }

    //        }
    //        catch (Exception e)
    //        {
    //            ViewBag.Message = e.Message;
    //        }
    //        finally
    //        {
    //            if (ResponseObject?.Code == Codes.Error)
    //                ViewBag.Message = $"{ResponseObject?.Message.ToString()}";
    //            else
    //                ViewBag.Message = $"{ResponseObject?.Code.ToString()}";
    //        }
    //        return View();
    //    }
    //}


  
    [HttpPost]
    [ValidateAntiForgeryToken]
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
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}



