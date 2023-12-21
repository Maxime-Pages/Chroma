namespace ChromaRunner;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System.Data.Common;
using System.Data.SqlClient;
using Chroma;
using System.Runtime.ConstrainedExecution;

class Program
{
    static void Main(string[] args)
    {
        string mail = "toto@gmail.com";
        string connection = "server=localhost;uid=toto;pwd=bob;database=testdb";
        Chroma DB = new Chroma("mysql", connection);
        Chroma.ShowQueries = true; //Enables Debugging

        #region Wiping DB
        DB.Delete<User>(new Brush().Where("ID").GreaterThanOrEquals(0));
        #endregion Wiping DB


        User John = new User("John","M",100);
        DB.Insert<User>(John);
        //WOW, she really does have a long name
        User ThatGirlhasaverylongnamethatsmorethan32characterslong = new User("Mariehasaverylongnamethatsmorethan32characterslong", "F", 200);
        try
        {
            DB.Insert<User>(ThatGirlhasaverylongnamethatsmorethan32characterslong);
        } catch (Exception ex) //Rejected due to name length
        {
            Console.WriteLine("Name was too long");
        }
        List<User> users = DB.Draw<User>();

        foreach(User user in users) {
            Console.WriteLine(user);//Only John is there
        }
        Console.ReadKey();
        Console.Clear();

        List<User> JohnsFriend = new List<User>() { new User("Bobs","M",10), new User("Jane","F"), new User("Mary","F",70)};

        DB.MultiInsert(JohnsFriend);

        users = DB.Draw<User>(); 
        foreach(User user in users) {
            Console.WriteLine(user); //Now John has some friends
        }
        Console.ReadKey();
        Console.Clear();
        
        Brush Morethan50PointAndNotMale = new Brush().Where("Points").GreaterThan(50).And().Not().Where("Gender").Equals("M");
        
        users = DB.Draw<User>(Morethan50PointAndNotMale);
        foreach(User user in users) {
            Console.WriteLine(user); //Only Mary has more than 50 Points
        }
        Console.ReadKey();
        Console.Clear();

        User Jane = DB.Draw<User>(new Brush().Where("Name").Equals("Jane"))[0]; //Let's get Jane by looking for her name

        Jane.points = 100; //Let's give Jane some points
        DB.Edit<User>(Jane,new Brush().Where("Id").Equals(Jane.id)); // And now we push it to the DB
        
        users = DB.Draw<User>(Morethan50PointAndNotMale);
        foreach(User user in users) { //Janes joins the 50+ Points Gang
            Console.WriteLine(user);
        }
        Console.ReadKey();
        Console.Clear();
    }
}
