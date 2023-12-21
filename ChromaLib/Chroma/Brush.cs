using Google.Protobuf.WellKnownTypes;
using System.Xml.Linq;

namespace Chroma;

public class Brush
{
    public string condition { get => _condition;}

    public List<(string, object)> parameters { get => _parameters; }
    private List<(string, object)> _parameters;
    private string _condition;

    public Brush()
    {
        _condition = "WHERE";
        _parameters = new List<(string,object)>();
    }

    public Brush Where(string name)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count()+1}", name));
        this._condition += $" {name}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }

    new public Brush Equals(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" = {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush NotEquals(object compared) 
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" <> {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush GreaterThan(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" > {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush GreatherThanOrEquals(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" >= {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush LessThan(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" < {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush LessThanOrEqual(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" <= {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush Between(object small, object big)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", small));
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", big));
        this._condition += $" {(small is string ? "'" + small + "'" : small)} AND {(big is string ? "'" + big + "'" : big)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }

    public Brush Like(object compared)
    {
       // _parameters.Add(($"@ChromaBrush{_parameters.Count() + 1}", compared));
        this._condition += $" LIKE {(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";
        return this;
    }
    public Brush In(List<object> compared)
    {
        this._condition += $" IN (";
        foreach (object comparedItem in compared)
        {
           // _parameters.Add(($", @ChromaBrush{_parameters.Count() + 1}", compared));
            this._condition += $"{(compared is string ? "'"+ compared+"'" : compared)}" ; //@ChromaBrush{_parameters.Count()}";

        }
        this._condition += $")";
        return this;
    }

    public Brush OrderBy(object compared)
    {
        this._condition += $" ORDER BY {compared}"; //@ChromaBrush{_parameters.Count()}";
        return this;
    }

    public Brush Or()
    {
        this._condition += $" OR";
        return this;
    }
    public Brush Not()
    {
        this._condition += $" NOT";
        return this;
    }
    public Brush And()
    {
        this._condition += $" AND";
        return this;
    }

    public Brush PrimaryEquals<T>(object compared)
    {
        T temp = default(T);
        bool flag = false;
        string name = "";
        foreach (System.Reflection.PropertyInfo property in temp.GetType().GetProperties())
        {
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
        this._condition += $" WHERE {name} = {(compared is string ? "'" + compared + "'" : compared)}";
        return this;
    }
}




