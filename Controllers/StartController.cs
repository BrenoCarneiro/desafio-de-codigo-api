using LacunaGenetics.Models;
using LacunaGenetics.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using LacunaGenetics.Services;
using System.Text;
using Newtonsoft.Json;
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

    public async Task<IActionResult> Index()
    {
        if (_connectionService == null)
        {

            HttpContext httpContext = HttpContext;
            await _connectionService.RefreshToken(httpContext);
        }
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Job()
    {
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        HttpContext httpContext = HttpContext;
        await _connectionService.Job(httpContext);
        ResponseModel response = _connectionService.ResponseObject;
        if (response.Code != Codes.Success)
        {
           
            await _connectionService.Job(httpContext);
            response = _connectionService.ResponseObject;
            if (response.Code != Codes.Success)
            {
                ViewBag.Message = "Your job has expired, please request another one";
                View("Index");
            }
        }
        
        Job jobSolved = new Job();
        string description = string.Empty;
        List<string> options = new List<string>();
        switch (response.Job.Type)
        {
            case Types.DecodeStrand:
                description = "The job is to take the StrandEncoded parameter, which is a Base64 string of the strand in Binary format, and decode it to the String";
                options.Add("Strand Encoded");
                jobSolved.Strand = DecodeToString(response.Job.StrandEncoded);
                jobSolved.Type = Types.DecodeStrand;
                break;
            case Types.EncodeStrand:
                description = "The job is to take the strand parameter, which is the strand in String format, and encode it to the Binary format";
                options.Add("Strand");
                jobSolved.StrandEncoded = EncodeToBinary(response.Job.Strand);
                jobSolved.Type = Types.EncodeStrand;
                break;
            case Types.CheckGene:
                description = "The job is to tell whether or not a particular gene is activated in the retrieved DNA strand. Both gene and DNA strands are retrieved in Binary formats. For this experiment, a gene is considered activated if more than 50 % of its content is present in the DNA template strand.";
                options.Add("Strand Encoded");
                options.Add("Gene Encoded");
                jobSolved.IsActivated = CheckGene(response.Job.GeneEncoded, response.Job.StrandEncoded );
                jobSolved.Type = Types.CheckGene;
                break;
            default:
                break;
        }
        
        jobSolved.Id = response.Job.Id;
        ViewBag.Job = response.Job;
        ViewBag.Description = description;
        ViewBag.Options = options;
        var jobSolvedJson = JsonConvert.SerializeObject(jobSolved);
        HttpContext.Session.SetString("jobSolved", jobSolvedJson);
        

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> JobDone()
    {
        ViewBag.Username = HttpContext.Session.GetString("_Username");
        var jobSolved = HttpContext.Session.GetString("jobSolved");
        Job job = JsonConvert.DeserializeObject<Job>(jobSolved);

        HttpContext httpContext = HttpContext;
        
        await _connectionService.SendJob(job, httpContext);
        if(_connectionService.ResponseObject.Code == Codes.Success)
        {
            ViewBag.Job = job;
            return View();

        }
        else
        {
            ViewBag.Message = ("Your job has expired, please request another one!");
            return View("Index");
        }
       
    }


    private string DecodeToString(string strandEncoded) // funcionando bem até hexa
    {
        byte[] bytes = Convert.FromBase64String(strandEncoded);
        string hex = BitConverter.ToString(bytes).Replace("-", "");
        StringBuilder binaryStrand = new StringBuilder();
        
        foreach(char c in hex.ToCharArray())
        {
            string tempData = string.Empty;
            tempData = Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2);
            while(tempData.Length < 4)
            {
                tempData = "0" + tempData;
            }
            if (tempData.StartsWith("00"))
                binaryStrand.Append("A");
            if (tempData.StartsWith("01"))
                binaryStrand.Append("C");
            if (tempData.StartsWith("11"))
                binaryStrand.Append("T");
            if (tempData.StartsWith("10"))
                binaryStrand.Append("G");

            if (tempData.EndsWith("00"))
                binaryStrand.Append("A");
            if (tempData.EndsWith("01"))
                binaryStrand.Append("C");
            if (tempData.EndsWith("11"))
                binaryStrand.Append("T");
            if (tempData.EndsWith("10"))
                binaryStrand.Append("G");
        }

        return binaryStrand.ToString();
    }

    private string EncodeToBinary(string encoded)
    {
        string result = "0b";
        result += encoded.Replace("A", "00").Replace("C", "01").Replace("T", "11").Replace("G", "10");
        return result;
    }

    private bool CheckGene(string geneEncodede, string strandEncoded)
    {
        string strand = DecodeToString(strandEncoded);
        StringBuilder templateStrand = new StringBuilder();
        if (!strand.StartsWith("CAT"))
        {
            foreach(char c in strand.ToCharArray())
            {
                if (c == 'G')
                    templateStrand.Append("C");
                if (c == 'T')
                    templateStrand.Append("A");
                if (c == 'A')
                    templateStrand.Append("T");
                if (c == 'C')
                    templateStrand.Append("G");
            }
            strand = templateStrand.ToString();
        }
        string geneDecoded = DecodeToString(geneEncodede);
        string substring;
        int length = geneDecoded.Length;
        bool isActivated = false;
        for (int i = 0; i < geneDecoded.Length/2; i++)
        {
            substring = geneDecoded.Substring(i, (length/2)+i+1);
            if (strand.Contains(substring))
            {
                isActivated = true;
                break;
            }
        }

        return isActivated;
    }

}

