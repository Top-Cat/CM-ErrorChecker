using System;
using System.Collections.Generic;

public abstract class IParam
{
    public readonly string name;

    protected IParam(string name)
    {
        this.name = name;
    }

    public abstract string getDefString();

    public abstract IParamValue Parse(string inputFieldText);
}

public abstract class Param<T> : IParam
{
    public T def;

    public Param(string name, T def) : base(name)
    {
        this.def = def;
    }

    public override string getDefString() => def.ToString();
}

public class FloatParam : Param<float>
{
    public FloatParam(string name, float def) : base(name, def) { }

    public override IParamValue Parse(string inputFieldText)
    {
        float.TryParse(inputFieldText, out float val);
        return new ParamValue<float>(val);
    }
}

public class StringParam : Param<string>
{
    public StringParam(string name, string def) : base(name, def) { }

    public override IParamValue Parse(string inputFieldText)
    {
        return new ParamValue<string>(inputFieldText);
    }
}

public class BoolParam : Param<bool>
{
    public BoolParam(string name, bool def) : base(name, def) { }

    public override IParamValue Parse(string inputFieldText)
    {
        bool.TryParse(inputFieldText, out bool val);
        return new ParamValue<bool>(val);
    }
}

public class ListParam : Param<List<string>>
{
    public ListParam(string name, List<string> def) : base(name, def) { }

    public override IParamValue Parse(string inputFieldText)
    {
        return new ParamValue<string>(def[int.Parse(inputFieldText)]);
    }
}

/// Param values
public interface IParamValue {}

public class ParamValue<T> : IParamValue
{
    public readonly T value;

    public ParamValue(T value)
    {
        this.value = value;
    }
}
