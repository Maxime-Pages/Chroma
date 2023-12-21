namespace Chroma;

[System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class NameAttribute : System.Attribute
{
    public string name;
    public NameAttribute(string name) {  this.name = name; }
}

[System.AttributeUsage(AttributeTargets.Property)]
public class NullableAttribute : System.Attribute
{
    public NullableAttribute() {}
}

[System.AttributeUsage(AttributeTargets.Property)]
public class StringSizeAttribute : System.Attribute
{
    public int size;
    public StringSizeAttribute(int size) {  this.size = size; }
}

[System.AttributeUsage(AttributeTargets.Property)]
public class PrimaryAttribute : System.Attribute
{
    public PrimaryAttribute(){}
}
