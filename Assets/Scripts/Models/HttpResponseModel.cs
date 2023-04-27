using Newtonsoft.Json;
using System;

[Serializable]
public class HttpResponseModel
{
    [JsonProperty("error", Required = Required.Always)]
    public bool Error { get; set; }
    [JsonProperty("msg", Required = Required.Default)]
    public string Msg { get; set; }
    [JsonProperty("data", Required = Required.Default)]
    public string Data { get; set; }
}