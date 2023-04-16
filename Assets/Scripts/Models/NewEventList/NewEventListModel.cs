using Newtonsoft.Json;

public class NewEventListModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("published_time")]
    public string PublishedDate { get; set; }
    [JsonProperty("image")]
    public string CoverUrl { get; set; }
}
