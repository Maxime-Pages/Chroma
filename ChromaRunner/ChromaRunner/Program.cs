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

        User John = new User("John","M",100);
        DB.Insert<User>(John);

        User Mariehasaverylongnamethatsmorethan32characterslong = new User("Mariehasaverylongnamethatsmorethan32characterslong", "F", 200);
        try
        {
            DB.Insert<User>(Mariehasaverylongnamethatsmorethan32characterslong);
        } catch (Exception ex) //Rejected due to name lenght
        {
            Console.WriteLine(ex.ToString());
        }

        Console.WriteLine(DB.Draw<User>()); //Only John is there

        List<User> JohnsFriend = new List<User>() { new User("Bob","M",10), new User("Jane","F"), new User("Mary","F",70)};

        DB.MultiInsert(JohnsFriend);

        Console.WriteLine(DB.Draw<User>()); //Now John has some friends


    }
}
