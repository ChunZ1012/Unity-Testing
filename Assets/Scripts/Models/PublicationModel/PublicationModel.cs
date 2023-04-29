using Newtonsoft.Json;

public class PublicationModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("category")]
    public string Category { get; set; }
    [JsonProperty("cover")]
    public string CoverUrl { get; set; }
    [JsonProperty("pdf")]
    public string PDFUrl { get; set; }
}
