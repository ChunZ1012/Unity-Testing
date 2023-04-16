using Newtonsoft.Json;

public class NewEventImageModel
{
    [JsonProperty("Id")]
    public int Id { get; set; }
    [JsonProperty("path")]
    public string ImageUrl { get; set; }
    [JsonProperty("description")]
    public string ImageAltText { get; set; }
    [JsonProperty("content")]
    public string ImageContent { get; set; }
}
