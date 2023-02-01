using LacunaGenetics.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace LacunaGenetics.Services;

public class ConnectionService
{
    public ConnectionService() { ResponseObject = new ResponseModel(); }

    public readonly IHttpClientFactory _httpClientFactory;
    public ConnectionService(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

    private ResponseModel _responseObject;
    public ResponseModel ResponseObject
    {
        get { return _responseObject; }
        set { _responseObject = value; }
    }

    public const string SessionKeyUsername = "_Username";
    public const string SessionKeyMessage = "_Message";
    private const string SessionKeyPassword = "_Password";
    private const string SessionKeyToken = "_Token";

    public async Task Login(UserModel user, HttpContext httpContext)
    {
        var httpClient = _httpClientFactory.CreateClient("gene");
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/users/login", user);
        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            ResponseObject = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            if (!string.IsNullOrWhiteSpace(ResponseObject.AccessToken))
            {
                httpContext.Session.SetString(SessionKeyUsername, user.Username);
                httpContext.Session.SetString(SessionKeyPassword, user.Password);
                httpContext.Session.SetString(SessionKeyToken, ResponseObject.AccessToken);
                var userClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.AuthenticationMethod, ResponseObject.AccessToken),
                    };
                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                {
                    IsPersistent = user.IsPersistent,
                    AllowRefresh = true,

                });
            }
        }
    }

    public async Task Job(HttpContext httpContext)
    {
        var httpClient = _httpClientFactory.CreateClient("gene");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContext.Session.GetString("_Token"));

        
        HttpResponseMessage response = await httpClient.GetAsync("/api/dna/jobs");
        if (!response.IsSuccessStatusCode)
        {
            ResponseObject.Job = null;
            await RefreshToken(httpContext);
        }
        string jsonString = await response.Content.ReadAsStringAsync();
        ResponseObject = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
    }

    public async Task SendJob(Job job, HttpContext httpContext)
    {
        var httpClient = _httpClientFactory.CreateClient("gene");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContext.Session.GetString("_Token"));
        string id = job.Id;
        string type = string.Empty;
        switch (job.Type)
        {
            case Types.DecodeStrand:
                type = "decode";
                break;
            case Types.EncodeStrand:
                type = "encode";
                break;
            case Types.CheckGene:
                type = "gene";
                break;
            default:
                break;
        }

        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"/api/dna/jobs/{id}/{type}", job);
        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            ResponseObject = JsonConvert.DeserializeObject<ResponseModel>(jsonString);
            if (ResponseObject.Code == Codes.Success)
            {
                httpContext.Session.SetString(SessionKeyMessage, ResponseObject.Message);
                ResponseObject.Job = null;
            }
        }
    }
    public async Task RefreshToken(HttpContext httpContext)
    {
        UserModel user = new UserModel();
        user.Username = httpContext.Session.GetString("_Username");
        user.Password = httpContext.Session.GetString("_Password");

        await Login(user, httpContext);
    }


}
