using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace LacunaGenetics.Models;

public class ResponseModel
{
    [JsonProperty("code")]
    public Codes Code { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("accessToken")]
    public string? AccessToken { get; set; }

    [JsonProperty("job")]
    public Job? Job { get; set; }

}

[JsonConverter(typeof(StringEnumConverter))]
public enum Codes
{
    [EnumMember(Value = "success")]
    Success,

    [EnumMember(Value = "error")]
    Error,

    [EnumMember(Value = "unauthorized")]
    Unauthorized
}
public class Job
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public Types Type { get; set; }

    [JsonProperty("strand")]
    public string? Strand { get; set; }

    [JsonProperty("strandEncoded")]
    public string? StrandEncoded { get; set; }

    [JsonProperty("geneEncoded")]
    public string? GeneEncoded { get; set; }

}

[JsonConverter(typeof(StringEnumConverter))]
public enum Types
{
    [EnumMember(Value = "decodeStrand")]
    DecodeStrand,

    [EnumMember(Value = "encodeStrand")] 
    EncodeStrand, 
    
    [EnumMember(Value = "checkGene")] 
    CheckGene
}
