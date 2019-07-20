using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{
    internal class JsonToken
    {
        [System.Flags]
        internal enum TokenType
        {
            BeginObject         = 1 << 0,
            EndObject           = 1 << 1,
            BeginArray          = 1 << 2,
            EndArray            = 1 << 3,

            String              = 1 << 4,
            Null                = 1 << 5,
            Number              = 1 << 6,
            Boolean             = 1 << 7,

            SepComma            = 1 << 8, // ,
            SepColon            = 1 << 9, // :
        }

        public TokenType tokenType { get; private set; }

        public JsonValue Value { get; private set; }

        public JsonToken(TokenType tokenType, char c)
        {
            this.tokenType = tokenType;
            switch(tokenType)
            {
            case TokenType.Boolean:
                    JsonValue.SharedValue value;
                    if (c == 't')
                    {
                       value = JsonValue.SharedValue.Boolean(true);
                    }
                    else
                    {
                        value = JsonValue.SharedValue.Boolean(false);
                    }
                    this.Value = new JsonValue(JsonValue.ValueType.Boolean, value);
                break;
            case TokenType.Null:
                this.Value = new JsonValue(JsonValue.ValueType.Null, null);
                break;
            case TokenType.SepColon:
            case TokenType.SepComma:
            case TokenType.BeginArray:
            case TokenType.EndArray:
            case TokenType.BeginObject:
            case TokenType.EndObject:
                // this.Value = null;
                break;
            default:
                ThrowHelper.Throw("unhandled: " + tokenType.ToString());
                break;
            }
        }

        public JsonToken(TokenType tokenType, string str)
        {
            this.tokenType = tokenType;
            switch (tokenType)
            {
            case TokenType.String:
                this.Value = new JsonValue(JsonValue.ValueType.String, str);
                break;
            case TokenType.Number:
                this.Value = JsonValue.CreateNumberValue(str);
                break;
            default:
                ThrowHelper.Throw("unhandled: " + tokenType.ToString());
                break;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]",this.tokenType,this.Value);
        }
    }
}
