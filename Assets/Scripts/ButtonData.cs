public class ButtonData
{
    private int _id;
    private string _name;
    private string _url;

    public ButtonData(int id, string name, string url)
    {
        Id = id;
        Name = name;
        Url = url;
    }

    public int Id { get => _id; set => _id = value; }
    public string Name { get => _name; set => _name = value; }
    public string Url { get => _url; set => _url = value; }
}
