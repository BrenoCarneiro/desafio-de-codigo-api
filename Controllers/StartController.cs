using LacunaGenetics.Models;
using LacunaGenetics.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using LacunaGenetics.Services;
using Microsoft.AspNetCore.Http;

namespace LacunaGenetics.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class StartController : Controller
{
    private readonly ConnectionService _connectionService;

    public StartController(ConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public IActionResult Index()
    {
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Job()
    {
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        HttpContext httpContext = HttpContext;
        try
        {
            await _connectionService.Job(httpContext);
        }
        finally
        {
            ResponseModel response = _connectionService.ResponseObject;
            if (response.Code == Codes.Success)
            {
                GetTypeDetails(response.Job);

            }
            else
            {
                await _connectionService.RefreshToken(httpContext);
                await _connectionService.Job(httpContext);
                response = _connectionService.ResponseObject;
                GetTypeDetails(response.Job);
            }
        }
        return View();
    }

    private void GetTypeDetails(Job job)
    {
        string description = string.Empty;
        List<string> options = new List<string>();
            switch (job.Type)
            {
                case Types.DecodeStrand:
                    description = "The job is to take the StrandEncoded parameter, which is a Base64 string of the strand in Binary format, and decode it to the String";
                    options.Add("Strand Encoded");
                    break;
                case Types.EncodeStrand:
                    description = "The job is to take the strand parameter, which is the strand in String format, and encode it to the Binary format";
                    options.Add("Strand");
                    break;
                case Types.CheckGene:
                    description = "The job is to tell whether or not a particular gene is activated in the retrieved DNA strand. Both gene and DNA strands are retrieved in Binary formats. For this experiment, a gene is considered activated if more than 50 % of its content is present in the DNA template strand.";
                    options.Add("Strand Encoded");
                    options.Add("Gene Encoded");
                    break;
                default:
                    description = string.Empty;
                    break;
            }
        ViewBag.Job = job;
        ViewBag.Description = description;
        ViewBag.Options = options;

    }
}

