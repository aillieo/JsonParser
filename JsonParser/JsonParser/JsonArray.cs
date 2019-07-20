using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AillieoUtils
{
    public class JsonArray: JsonValue
    {
        private List<JsonValue> list = new List<JsonValue>();

        public JsonArray() : base(ValueType.JsonObj)
        {

        }


        public void Add(JsonValue jsonValue)
        {
            list.Add(jsonValue);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[').AppendLine();
            bool isFirst = true;
            foreach (var element in list)
            {
                if(isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    sb.Append(',');
                    sb.AppendLine();
                }
                sb.Append(element.ToString());
            }
            sb.Append(']').AppendLine();
            return sb.ToString();
        }

    }
}
