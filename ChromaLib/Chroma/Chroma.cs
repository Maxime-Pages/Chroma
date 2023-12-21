namespace Chroma;

using Npgsql;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Oracle.ManagedDataAccess.Client;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.Dynamic;
using System.Reflection;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;

public class Chroma
{
    public static DbConnection conn;

    public Chroma(DbConnection dbconnection)
    {
        conn = dbconnection;
    }

    public Chroma(string source, string connectionstring)
    {
        switch (source)
        {
            case "mysql":
                conn = new MySqlConnection(connectionstring);
                break;

            case "postgres":
                conn = new NpgsqlConnection(connectionstring);
                break;

            case "oracle":
                conn = new OracleConnection(connectionstring);
                break;

            case "sqlite":
                conn = new SqliteConnection(connectionstring);
                break;

            case "sqlserver":
                conn = new SqlConnection(connectionstring);
                break;

            default:
                throw new ArgumentException("Invalid source");
        }
    }


    public static T PaintHue<T>(DbDataReader data)
    {
        if(data == null) throw new ArgumentNullException();

        T result = Activator.CreateInstance<T>();
        
        for (int i = 0; i < data.FieldCount; i++)
        {
            string name = data.GetName(i).ToLower();
            object value = data.GetValue(i);

            foreach(PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.Name.ToLower() == name && !(value is DBNull))
                {
                    property.SetValue(result, value, null);
                }
            }
        }
        return result;
    }


    public static List<T> Query<T>(string query, List<(string, object)> parameters = null)
    {
        if (Chroma.conn.State != System.Data.ConnectionState.Open) Chroma.conn.Open();

        List<T> results = new List<T>();
        using (DbCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = query;
            if (parameters != null)
            {
                foreach ((string, object) param in parameters)
                {
                    DbParameter dbparam = cmd.CreateParameter();
                    dbparam.ParameterName = param.Item1;
                    dbparam.Value = param.Item2;
                    cmd.Parameters.Add(dbparam);
                }
            }
            Console.WriteLine(cmd.CommandText);
            using (DbDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {

                    results.Add(PaintHue<T>(reader));
                }

            }
            conn.Close();
            return results;
        }
    }

    public static void NonQuery(string query, List<(string,object)> parameters = null)
    {
        if (Chroma.conn.State != System.Data.ConnectionState.Open) Chroma.conn.Open();

        using (DbCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = query;
            if (parameters != null)
            {
                foreach ((string,object) param in parameters)
                {
                    DbParameter dbparam = cmd.CreateParameter();
                    dbparam.ParameterName = param.Item1;
                    dbparam.Value = param.Item2;
                    cmd.Parameters.Add(dbparam);
                }
            }
            cmd.ExecuteNonQuery();
        }
    }
    public void Insert<T>(T hue)
    {
        List<string> columns = new List<string>();
        List<string> values = new List<string>();
        List<(string, object)> parameters = new List<(string, object)>();
        string name;
        string palette = typeof(T).Name;
        foreach(System.Attribute attribute in hue.GetType().GetCustomAttributes(true))
        {
            if (attribute is NameAttribute attr)
            {
                palette = attr.name;
            }
        }
        foreach (System.Reflection.PropertyInfo property in hue.GetType().GetProperties())
        {
            name = property.Name;

            foreach (System.Attribute attribute in property.GetCustomAttributes(true))
            {
                if (attribute is PrimaryAttribute) goto skip;
                if (property.GetValue(hue) == null)
                {
                    if (!(attribute is NullableAttribute)) throw new Exception($"Non nullable field {property.Name} sent with null value.");
                    else goto skip;
                }

                else if (attribute is StringSizeAttribute sattr)
                {
                    if (!(property.GetValue(hue) is String str)) throw new Exception($"Field {property.Name} with StringSize attribute is not a String.");

                    if (sattr.size <= str.Length) throw new Exception($"Field {property.Name} with max size {sattr.size} sent with size {str.Length}.");

                }

                else if (attribute is NameAttribute nattr)
                {
                    name = nattr.name;
                }

            }

            columns.Add(name);
            values.Add($"@{name}");
            parameters.Add(($"@{name}", property.GetValue(hue)));

        skip:
            continue;
        }

        Chroma.NonQuery($"INSERT INTO {palette} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)});", parameters);
    }

    public void MultiInsert<T>(List<T> hueList)
    {
        if (hueList.Count == 0) return;
        List<string> columns = new List<string>();
        List<string> values = new List<string>();
        List<string> items = new List<string>();
        List<(string, object)> parameters = new List<(string, object)>();
        int i = 0;
        string palette = hueList[0].GetType().Name;
        foreach (System.Attribute attribute in hueList[0].GetType().GetCustomAttributes(true))
        {
            if (attribute is NameAttribute attr)
            {
                palette = attr.name;
            }
        }
        foreach (T hue in hueList)
        {
            i++;
            string name;

            foreach (System.Reflection.PropertyInfo property in hue.GetType().GetProperties())
            {
                name = property.Name;

                foreach (System.Attribute attribute in property.GetCustomAttributes(true))
                {
                    if (attribute is PrimaryAttribute) goto skip;

                    if (property.GetValue(hue) == null && !(attribute is NullableAttribute)) throw new Exception($"Non nullable field {property.Name} sent with null value.");

                    else if (attribute is StringSizeAttribute sattr)
                    {
                        if (!(property.GetValue(hue) is String str)) throw new Exception($"Field {property.Name} with StringSize attribute is not a String.");

                        if (sattr.size <= str.Length) throw new Exception($"Field {property.Name} with max size {sattr.size} sent with size {str.Length}.");

                    }

                    else if (attribute is NameAttribute nattr)
                    {
                        name = nattr.name;
                    }

                }

                if (!columns.Contains(name)) columns.Add(name);
                values.Add($"@{name}{i}");
                parameters.Add(($"@{name}{i}", property.GetValue(hue)));

            skip:
                continue;
            }
            items.Add($"({string.Join(", ", values)})");
            values = new List<string>();
        }
        Chroma.NonQuery($"INSERT INTO {palette} ({string.Join(", ", columns)}) VALUES {string.Join(", ", items)};", parameters);
    }

    public List<T> Draw<T>(Brush? brush = null)
    {
        T temp = default(T);
        string palette = temp.GetType().Name;
        foreach (System.Attribute attribute in temp.GetType().GetCustomAttributes())
        {
            if (attribute is NameAttribute attr)
            {
                palette = attr.name;
            }
        }
        return Chroma.Query<T>($"SELECT * FROM {palette} {(brush is null ? "" : brush.condition)};");

    }


    public void Edit<T>(T hue, Brush? brush = null)
    {
        List<string> operations = new List<string>();
        List<(string, object)> parameters = new List<(string, object)>();
        string name;
        string palette = typeof(T).Name;
        object? primary = null;
        foreach (System.Attribute attribute in hue.GetType().GetCustomAttributes(true))
        {
            if (attribute is NameAttribute attr)
            {
                palette = attr.name;
            }
        }
        foreach (System.Reflection.PropertyInfo property in hue.GetType().GetProperties())
        {
            name = property.Name;

            foreach (System.Attribute attribute in property.GetCustomAttributes(true))
            {
                if (attribute is PrimaryAttribute) 
                {
                    primary = property.GetValue(hue);
                    goto skip;
                }
                if (property.GetValue(hue) == null)
                {
                    if (!(attribute is NullableAttribute)) throw new Exception($"Non nullable field {property.Name} sent with null value.");
                    else goto skip;
                }

                else if (attribute is StringSizeAttribute sattr)
                {
                    if (!(property.GetValue(hue) is String str)) throw new Exception($"Field {property.Name} with StringSize attribute is not a String.");

                    if (sattr.size <= str.Length) throw new Exception($"Field {property.Name} with max size {sattr.size} sent with size {str.Length}.");

                }

                else if (attribute is NameAttribute nattr)
                {
                    name = nattr.name;
                }

            }
            operations.Add($"{name} = @{name}");
            parameters.Add(($"@{name}", property.GetValue(hue)));

        skip:
            continue;
        }

        Chroma.NonQuery($"UPDATE {palette} SET {string.Join(", ",operations)} {(brush is null ? new Brush().PrimaryEquals<object?>(primary).condition : brush.condition)};", parameters);
    }

    public void Delete<T>(T hue, Brush? brush = null)
    {
        string name;
        string palette = typeof(T).Name;
        object? primary = null;
        bool flag = false;
        foreach (System.Attribute attribute in hue.GetType().GetCustomAttributes(true))
        {
            if (attribute is NameAttribute attr)
            {
                palette = attr.name;
            }
        }
        foreach (System.Reflection.PropertyInfo property in hue.GetType().GetProperties())
        {
            name = property.Name;

            name = property.Name;

            foreach (System.Attribute attribute in property.GetCustomAttributes(true))
            {
                if (attribute is PrimaryAttribute)
                {
                    flag = true;
                }

                else if (attribute is NameAttribute nattr)
                {
                    name = nattr.name;
                }

            }
            if (flag) break;
        }

        Chroma.NonQuery($"DELETE FROM {palette} {(brush is null ? new Brush().PrimaryEquals<object?>(primary).condition : brush.condition)};");
    }



}

