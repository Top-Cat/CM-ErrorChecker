using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Jint;
using Jint.Native;
using SimpleJSON;

class JSONWraper
{
    private readonly Engine engine;
    internal readonly JSONNode wrapped;
    private readonly Func<bool> deleteObj;

    private readonly Dictionary<string, JsValue> observe = new Dictionary<string, JsValue>();
    private readonly Dictionary<string, JSONWraper> children = new Dictionary<string, JSONWraper>();
    private Action checkObserved;
    private bool cleanObserved = true;

    public JSONWraper(Engine engine, ref Action parent, JSONNode wrapped, Func<bool> deleteObj)
    {
        this.engine = engine;
        this.wrapped = wrapped;
        this.deleteObj = deleteObj;
        parent += CheckObserved;
    }

    private void CheckObserved()
    {
        cleanObserved = false;
        checkObserved?.Invoke();
        foreach (var keyValuePair in observe)
        {
            this[keyValuePair.Key] = keyValuePair.Value;
            deleteObj();
        }

        cleanObserved = true;
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
                if (wrapped[aIndex] == null)
                    return null;

                return ToObserve(aKey, wrapped[aIndex]);
            }
            return wrapped.HasKey(aKey) ? (wrapped[aKey].IsObject ? (object) GetChild(aKey) : ToObserve(aKey, wrapped[aKey])) : null;
        }
        set
        {
            deleteObj();
            if (cleanObserved) observe.Remove(aKey);
            if (wrapped.IsArray && int.TryParse(aKey, out var aIndex))
            {
                wrapped[aIndex] = castObjToJSON(value); 
                return;
            }
            children.Remove(aKey);
            wrapped[aKey] = castObjToJSON(value);
        }
    }

    private JSONWraper GetChild(string key)
    {
        if (!children.ContainsKey(key))
            children.Add(key, new JSONWraper(engine, ref checkObserved, wrapped[key], deleteObj));

        return children[key];
    }

    private JsValue ToObserve(string key, JSONNode original)
    {
        if (!observe.ContainsKey(key))
            observe.Add(key, JSONToJS(original));

        return observe[key];
    }

    private JsValue JSONToJS(JSONNode node)
    {
        if (node.IsString)
        {
            return node.Value;
        }
        
        if (node.IsArray)
        {
            var asArr = node.AsArray;
            var nativeArr = engine.Realm.Intrinsics.Array.Construct(0);

            foreach (var kv in asArr.Values)
            {
                engine.Realm.Intrinsics.Array.PrototypeObject.Push(nativeArr, new[] { JSONToJS(kv) });
            }

            return nativeArr;
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
            var obj = engine.Realm.Intrinsics.Object.Construct(new JsValue[0]);
            foreach (var kv in node)
            {
                obj.Set(kv.Key, JSONToJS(kv.Value));
            }
            return obj;
        }

        return null;
    }

    public override string ToString()
    {
        return wrapped.ToString();
    }

    public JsValue ToJSON(JsValue receiver)
    {
        return JSONToJS(wrapped);
    }
}
