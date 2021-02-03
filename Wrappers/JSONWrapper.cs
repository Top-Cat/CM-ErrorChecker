using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using SimpleJSON;
using UnityEngine;
using Types = Jint.Runtime.Types;

class JSONWraper : ObjectInstance
{
    private readonly Engine engine;
    private readonly JSONNode wrapped;
    private readonly Action deleteObj;

    public JSONWraper(Engine engine, JSONNode wrapped, Action deleteObj) : base(engine)
    {
        this.engine = engine;
        this.wrapped = wrapped;
        this.deleteObj = deleteObj;
    }

    private static JSONNode arrToJSON(IEnumerable arr)
    {
        var node = new JSONArray();
        foreach (var o in arr)
        {
            node.Add(castObjToJSON(o));
        }

        return node;
    }

    private static JSONNode castJSToJSON(JsValue o)
    {
        if (o.IsString())
        {
            return o.ToString();
        }
        
        if (o.IsArray())
        {
            var vals = o.AsArray();
            var result = new JSONArray();
            foreach (var v in vals)
            {
                result.Add(castJSToJSON(v));
            }

            return result;
        }

        if (o.IsNumber())
        {
            return o.AsNumber();
        }

        if (o.IsBoolean())
        {
            return o.AsBoolean();
        }

        if (o.IsObject())
        {
            var result = new JSONObject();
            var obj = o.AsObject();
            var k = obj.GetOwnProperties()
                .Where(x => x.Value.Enumerable)
                .Select(x => x.Key)
                .ToList();

            foreach (var p in k)
            {
                result[p.AsString()] = castJSToJSON(obj.Get(p, obj));
            }
            return result;
        }

        return null;
    }
    
    public static JSONNode castObjToJSON(object o)
    {
        switch (o)
        {
            case JSONWraper w:
                return w.wrapped;
            case JsValue v:
                return castJSToJSON(v);
            case Array a:
                return arrToJSON(a);
            case ExpandoObject obj:
                return dictToJSON(obj);
            case float f:
                return f;
            case double d:
                return d;
            case int i:
                return i;
            case bool b:
                return b;
            case string s:
                return s;
            default:
                return null;
        }
    }
    
    public static JSONNode dictToJSON(ExpandoObject o)
    {
        var dict = (IDictionary<string, object>) o;
        var node = new JSONObject();

        foreach (var x in dict)
        {
            node[x.Key] = castObjToJSON(x.Value);
        }
        
        return node;
    }

    public object this[string aKey]
    {
        get
        {
            if (wrapped.IsArray && int.TryParse(aKey, out var aIndex))
            {
                return new JSONWraper(engine, wrapped[aIndex], deleteObj);
            }
            return wrapped.HasKey(aKey) ? new JSONWraper(engine, wrapped[aKey], deleteObj) : null;
        }
        set
        {
            Debug.Log("Set");
            deleteObj();
            if (wrapped.IsArray && int.TryParse(aKey, out var aIndex))
            {
                wrapped[aIndex] = castObjToJSON(value); 
                return;
            }
            Debug.Log(castObjToJSON(value));
            wrapped[aKey] = castObjToJSON(value);
        }
    }

    public override IEnumerable<KeyValuePair<JsValue, PropertyDescriptor>> GetOwnProperties()
    {
        var result = new List<KeyValuePair<JsValue, PropertyDescriptor>>();
        foreach (var k in wrapped.Keys)
        {
            result.Add(new KeyValuePair<JsValue, PropertyDescriptor>(k, new PropertyDescriptor(k, true, true, true)));
        }
        return result;
    }

    private JsValue JSONToJS(JSONNode node)
    {
        if (node.IsString)
        {
            return node.Value;
        }
        
        if (node.IsArray)
        {
            var vals = new List<JsValue>();
            foreach (var kv in node.AsArray.Values)
            {
                vals.Add(JSONToJS(kv));
            }

            return engine.Array.Construct(vals.ToArray());
        }

        if (node.IsNumber)
        {
            return node.AsDouble;
        }

        if (node.IsBoolean)
        {
            return node.AsBool;
        }

        if (node.IsObject)
        {
            return new JSONWraper(engine, node, deleteObj);
        }

        return null;
    }
    
    public override JsValue Get(JsValue property, JsValue receiver)
    {
        var obj = wrapped[property.ToString()];
        return obj != null ? JSONToJS(obj) : property;
    }

    public override bool Set(JsValue property, JsValue value, JsValue receiver)
    {
        Debug.Log("SetA");
        Debug.Log(property.ToString());
        this[property.ToString()] = value;
        return true;
    }

    public override List<JsValue> GetOwnPropertyKeys(Types types = Types.None | Types.String | Types.Symbol)
    {
        var result = new List<JsValue>();
        foreach (var k in wrapped.Keys)
        {
            result.Add(k);
        }
        return result;
    }

    public override string ToString()
    {
        return wrapped.ToString();
    }

    public JsValue ToJSON(JsValue receiver)
    {
        return this;
    }
}
