using Newtonsoft.Json;
//class model for staff detail
public class StaffListModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("contact")]
    public string Contact { get; set; }
    [JsonProperty("email")]
    public string Email { get; set; }
    [JsonProperty("position")]
    public string Position { get; set; }
    [JsonProperty("location")]
    public string Location { get; set; }

}
