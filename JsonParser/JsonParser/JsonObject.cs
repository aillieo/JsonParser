using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AillieoUtils
{
    public class JsonObject: JsonValue
    {
        private Dictionary<string, JsonValue> dict = new Dictionary<string, JsonValue>();

        public JsonObject() : base(ValueType.JsonObj)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{').AppendLine();
            foreach (var pair in dict)
            {
                sb.Append(pair.Key);
                sb.Append(':');
                sb.Append(pair.Value.ToString());
                sb.AppendLine();
            }
            sb.Append('}').AppendLine();
            return sb.ToString();
        }

        public void Add(string key, JsonValue value)
        {
            //if(dict.ContainsKey(key))
            //{
            //    ThrowHelper.Throw("key exist in object: " + key);
            //}
            dict.Add(key, value);
        }

    }
}
