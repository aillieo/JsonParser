using System.Runtime.InteropServices;

namespace AillieoUtils
{
    public class JsonValue
    {
        public enum ValueType
        {
            JsonObj = 0,
            Array = 1,
            Long = 2,
            Integer = 3,
            Double = 4,
            Boolean = 5,
            String = 6,
            Null = 7,
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        public struct SharedValue
        {
            [FieldOffset(0)]
            public char c;

            [FieldOffset(0)]
            public long l;

            [FieldOffset(0)]
            public int i;

            [FieldOffset(0)]
            public double d;

            [FieldOffset(0)]
            public bool b;


            static public SharedValue Char(char c)
            {
                SharedValue value = new SharedValue();
                value.c = c;
                return value;
            }

            static public SharedValue Long(long l)
            {
                SharedValue value = new SharedValue();
                value.l = l;
                return value;
            }

            static public SharedValue Integer(int i)
            {
                SharedValue value = new SharedValue();
                value.i = i;
                return value;
            }

            static public SharedValue Boolean(bool b)
            {
                SharedValue value = new SharedValue();
                value.b = b;
                return value;
            }

            static public SharedValue Double(double d)
            {
                SharedValue value = new SharedValue();
                value.d = d;
                return value;
            }
        }

        public readonly ValueType valueType;

        private readonly SharedValue sharedValue;
        private readonly string stringValue;


        protected JsonValue(ValueType valueType)
        {
            this.valueType = valueType;

        }

        public JsonValue(ValueType valueType, SharedValue value) : this(valueType)
        {
            sharedValue = value;
        }

        public JsonValue(ValueType valueType, string value) : this(valueType)
        {
            stringValue = value;
        }

        public static JsonValue CreateNumberValue(string value)
        {
            if (value.Contains(".") || value.Contains("e") || value.Contains("E"))
            {
                JsonValue jsonValue = new JsonValue(ValueType.Double, SharedValue.Double(double.Parse(value)));
                return jsonValue;
            }
            else
            {
                long number = long.Parse(value);
                if (number > int.MaxValue || number < int.MinValue)
                {
                    JsonValue jsonValue = new JsonValue(ValueType.Long, SharedValue.Long(number));
                    return jsonValue;
                }
                else
                {
                    JsonValue jsonValue = new JsonValue(ValueType.Integer, SharedValue.Integer((int)number));
                    return jsonValue;
                }
            }
        }

        public JsonObject GetJsonObjectValue()
        {
            if (valueType != ValueType.JsonObj)
            {
                return default(JsonObject);
            }
            return this as JsonObject;
        }

        public JsonArray GetJsonArrayValue()
        {
            if (valueType != ValueType.Array)
            {
                return default(JsonArray);
            }
            return this as JsonArray;
        }

        public bool GetBooleanValue()
        {
            if (valueType != ValueType.Boolean)
            {
                return default(bool);
            }
            return sharedValue.b;
        }

        public string GetStringValue()
        {
            if (valueType != ValueType.String)
            {
                return default(string);
            }
            return stringValue;
        }

        public double GetNumberValue()
        {
            if (valueType != ValueType.Double)
            {
                return default(double);
            }
            return sharedValue.d;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return this.valueType == ValueType.Null;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            switch (this.valueType)
            {
                case ValueType.Boolean:
                    return this.sharedValue.b.ToString();
                case ValueType.String:
                    return this.stringValue;
                case ValueType.Double:
                    return this.sharedValue.d.ToString();
                case ValueType.Long:
                    return this.sharedValue.l.ToString();
                case ValueType.Integer:
                    return this.sharedValue.i.ToString();
                case ValueType.Null:
                    return "null";
                default:
                    break;
            }
            return string.Format("{0}[{1}]", this.valueType, this.stringValue);
        }
    }
}
