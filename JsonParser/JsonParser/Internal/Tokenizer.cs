using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AillieoUtils
{
    internal static class Tokenizer
    {
        public static Queue<JsonToken> GetTokens(CharBuffer charBuffer)
        {
            Queue<JsonToken> tokens = new Queue<JsonToken>();
            while (charBuffer.HasMore())
            {
                JsonToken jsonToken = Read(charBuffer);
                if (jsonToken != null)
                {
                    tokens.Enqueue(jsonToken);
                }
            }
            return tokens;
        }

        private static JsonToken Read(CharBuffer charBuffer)
        {
            char c = charBuffer.Head();
            while (IsWhiteSpace(c) && charBuffer.HasMore())
            {
                charBuffer.Next();
                if (charBuffer.HasMore())
                {
                    c = charBuffer.Head();
                }
                else
                {
                    return null;
                }
            }

            switch (c)
            {
                case '{':
                    return new JsonToken(JsonToken.TokenType.BeginObject, charBuffer.Next());
                case '}':
                    return new JsonToken(JsonToken.TokenType.EndObject, charBuffer.Next());
                case '[':
                    return new JsonToken(JsonToken.TokenType.BeginArray, charBuffer.Next());
                case ']':
                    return new JsonToken(JsonToken.TokenType.EndArray, charBuffer.Next());
                case ',':
                    return new JsonToken(JsonToken.TokenType.SepComma, charBuffer.Next());
                case ':':
                    return new JsonToken(JsonToken.TokenType.SepColon, charBuffer.Next());
                case 'n':
                    return new JsonToken(JsonToken.TokenType.Null, ReadNull(charBuffer));
                case 't':
                case 'f':
                    return new JsonToken(JsonToken.TokenType.Boolean, ReadBoolean(charBuffer));
                case '"':
                    return new JsonToken(JsonToken.TokenType.String, ReadString(charBuffer));
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '.':
                    return new JsonToken(JsonToken.TokenType.Number, ReadNumber(charBuffer));
                default:
                    ThrowHelper.Throw("unknown char :" + c);
                    break;
            }
            return null;

        }


        private static char ReadBoolean(CharBuffer charBuffer)
        {
            char c = charBuffer.Head();
            switch (c)
            {
                case 'f':
                    charBuffer.Get(5);
                    break;
                case 't':
                    charBuffer.Get(4);
                    break;
                default:
                    ThrowHelper.Throw("invalid boolean start: " + c);
                    break;
            }
            return c;
        }

        private static char ReadNull(CharBuffer charBuffer)
        {
            char c = charBuffer.Head();
            charBuffer.Get(4);
            return c;
        }

        private static string ReadString(CharBuffer charBuffer)
        {
            charBuffer.Next();
            StringBuilder sb = new StringBuilder();
            while (charBuffer.HasMore())
            {
                char c = charBuffer.Next();
                if (c == '\\')
                {
                    sb.Append(c);
                    c = charBuffer.Head();
                    if (!IsEscape(c))
                    {
                        ThrowHelper.Throw("invalid escape char \\" + c);
                    }
                    sb.Append(c);
                }
                else if (c == '"')
                {
                    return sb.ToString();
                }
                else if (c == '\r' || c == '\n')
                {
                    ThrowHelper.Throw("Invalid character");
                }
                else
                {
                    sb.Append(c);
                }
            }

            ThrowHelper.Throw("unfinished string");
            return null;
        }

        private static string ReadNumber(CharBuffer charBuffer)
        {
            char c = charBuffer.Head();
            StringBuilder sb = new StringBuilder();
            if (c == '-')
            {
                sb.Append(c);
                charBuffer.Next();
                c = charBuffer.Head();
            }

            if(!IsDigit(c))
            {
                ThrowHelper.Throw("invalid char for number: " + c);
            }

            while(IsDigit(c) || IsDecimal(c) || IsExp(c))
            {
                sb.Append(c);
                charBuffer.Next();
                c = charBuffer.Head();
            }
            return sb.ToString();
        }


        private static bool IsWhiteSpace(char c)
        {
            return (c == ' ' || c == '\t' || c == '\r' || c == '\n');
        }

        private static bool IsEscape(char c)
        {
            return (c == '"' || c == '\\' || c == 'u' || c == 'r'
                    || c == 'n' || c == 'b' || c == 't' || c == 'f');
        }

        private static bool IsHex(char c)
        {
            return (IsDigit(c)|| ('a' <= c && c <= 'f')
                    || ('A' <= c && c <= 'F'));
        }


        private static bool IsExp(char c)
        {
            return c == 'e' || c == 'E';
        }

        private static bool IsDecimal(char c)
        {
            return c == '.';
        }

        private static bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

    }
}
