namespace ChromaRunner;

using Chroma;

[Name("Users")]
public class User : Hue
{
    [Primary]
    public int id { get; set; }

    [Name("name")]
    [StringSize(32)]
    public string firstname { get; set; }

    [StringSize(1)]
    public string gender { get; set; }

    [Nullable]
    public int? points { get; set; }

    public User(int id, string name, string gender, int? phone = null)
    {
        this.id = id;
        this.name = name;
        this.gender = gender;
        this.phone = phone;
    }
    public User(string name, string gender, int? phone = null)
    {
        this.id = -1;
        this.name = name;
        this.gender = gender;
        this.phone = phone;
    }
    public User() 
    {
        this.id = -1;
        this.name = "";
        this.gender = "";
        this.phone = null;
    }
    public override string ToString()
    {
        return $"ID : {id}; Name : {name}; Gender : {gender}";
    }
}