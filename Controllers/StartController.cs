using LacunaGenetics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using LacunaGenetics.Services;
using System.Text;
using Newtonsoft.Json;

namespace LacunaGenetics.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class StartController : Controller
{
    private readonly ConnectionService _connectionService;
    JobService jobService = new JobService();

    public StartController(ConnectionService connectionService)
    {
        _connectionService = connectionService;
        
    }

    public async Task<IActionResult> Index()
    {
        if (_connectionService == null)
        {

            HttpContext httpContext = HttpContext;
            await _connectionService.RefreshToken(httpContext);
        }
        ViewBag.Username = HttpContext.Session.GetString("_Username").ToUpper();
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Job()
    {
        Job jobSolved = new Job();
        string description = string.Empty;
        List<string> options = new List<string>();
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        HttpContext httpContext = HttpContext;
        await _connectionService.Job(httpContext);
        ResponseModel response = _connectionService.ResponseObject;
        try
        {
            
            if (response.Code != Codes.Success)
            {
                await _connectionService.Job(httpContext);
                response = _connectionService.ResponseObject;
                if (response.Code != Codes.Success)
                {
                    return View("Index");
                }
            }
        }
        catch(Exception ex) 
        {
            if (ex != null)
            {
                return View("Index");
            }
        }
        try
        {
            if (response.Job.Id == jobSolved.Id || jobSolved.Id == null || response.Job.Id != null)
            {
                jobSolved.Id = response.Job.Id;
                ViewBag.Job = response.Job;
                switch (response.Job.Type)
                {
                    case Types.DecodeStrand:
                        description = "The job is to take the StrandEncoded parameter, which is a Base64 string of the strand in Binary format, and decode it to the String";
                        options.Add("Strand Encoded");
                        jobSolved.Strand = jobService.DecodeToString(response.Job.StrandEncoded);
                        jobSolved.Type = Types.DecodeStrand;
                        break;
                    case Types.EncodeStrand:
                        description = "The job is to take the strand parameter, which is the strand in String format, and encode it to the Binary format";
                        options.Add("Strand");
                        jobSolved.StrandEncoded = jobService.EncodeToBinary(response.Job.Strand);
                        jobSolved.Type = Types.EncodeStrand;
                        break;
                    case Types.CheckGene:
                        description = "The job is to tell whether or not a particular gene is activated in the retrieved DNA strand. Both gene and DNA strands are retrieved in Binary formats. For this experiment, a gene is considered activated if more than 50 % of its content is present in the DNA template strand.";
                        options.Add("Strand Encoded");
                        options.Add("Gene Encoded");
                        jobSolved.IsActivated = jobService.CheckGene(response.Job.GeneEncoded, response.Job.StrandEncoded);
                        jobSolved.Type = Types.CheckGene;
                        break;
                    default:
                        break;

                }
                ViewBag.Description = description;
                ViewBag.Options = options;
                var jobSolvedJson = JsonConvert.SerializeObject(jobSolved);
                HttpContext.Session.SetString("jobSolved", jobSolvedJson);
            }
        }

        catch (Exception ex)
        {
            if (ex != null)
            {
                ViewBag.Message = "Your job has expired, please request another one";
                return View("Index");
            }
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> JobDone()
    {
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        var jobSolved = HttpContext.Session.GetString("jobSolved");
        Job job = JsonConvert.DeserializeObject<Job>(jobSolved);
        HttpContext httpContext = HttpContext;
        try
        {
            await _connectionService.SendJob(job, httpContext);   
        }
        catch(Exception ex)
        {
            if (httpContext.Session.GetString("_Message") != "Success" || ex != null)
            {
                ViewBag.Message = ("Your job has expired, please request another one!");
                return View("Index");
                
            }
        }
        ViewBag.Job = job;
        return View();

    }


    

}

