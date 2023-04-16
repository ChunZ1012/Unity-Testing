using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class NewEventDetailModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("published_time")]
    public DateTime PublishTime { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("images")]
    public List<NewEventImageModel> Images { get; set; }
}
