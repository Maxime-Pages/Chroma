namespace ChromaRunner;

using Chroma;

[Name("Users")]
public class User
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

    public User(int id, string firstname, string gender, int? points = null)
    {
        this.id = id;
        this.firstname = firstname;
        this.gender = gender;
        this.points = points;
    }
    public User(string firstname, string gender, int? points = null)
    {
        this.id = -1;
        this.firstname = firstname;
        this.gender = gender;
        this.points = points;
    }
    public User() 
    {
        this.id = -1;
        this.firstname = "";
        this.gender = "";
        this.points = null;
    }
    public override string ToString()
    {
        return $"ID : {id}; Name : {firstname}; Gender : {gender}; Points : {(points is null ? "Null" : points.ToString())}";
    }
}
