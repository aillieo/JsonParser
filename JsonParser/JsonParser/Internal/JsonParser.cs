using System;
using System.Collections;
using System.Collections.Generic;

namespace AillieoUtils
{

    // ref: http://www.json.org/

    internal static class JsonParser
    {
        public static JsonValue Parse(string json)
        {
            CharBuffer buffer = new CharBuffer(json);
            var tokens = Tokenizer.GetTokens(buffer);
            return Parse(tokens);
        }

        internal static JsonValue Parse(Queue<JsonToken> tokens)
        {
            JsonToken jsonToken = tokens.Dequeue();
            if (jsonToken.tokenType == JsonToken.TokenType.BeginObject)
            {
                return ParseAsObject(tokens);
            }
            else if (jsonToken.tokenType == JsonToken.TokenType.BeginArray)
            {
                return ParseAsArray(tokens);
            }
            else
            {
                ThrowHelper.Throw("begin with invalid token ");
            }
            return null;
        }


        private static JsonArray ParseAsArray(Queue<JsonToken> tokens)
        {

            JsonArray jsonArray = new JsonArray();

            JsonToken.TokenType expectedToken = JsonToken.TokenType.BeginArray | JsonToken.TokenType.EndArray | JsonToken.TokenType.BeginObject | JsonToken.TokenType.Null
                | JsonToken.TokenType.Number | JsonToken.TokenType.Boolean | JsonToken.TokenType.String;
            while (tokens.Count > 0)
            {
                JsonToken jsonToken = tokens.Dequeue();
                JsonToken.TokenType tokenType = jsonToken.tokenType;
                switch (tokenType)
                {
                    case JsonToken.TokenType.BeginObject:
                        EnsureTokenType(tokenType, expectedToken);
                        JsonObject embeddedJsonObject = ParseAsObject(tokens);
                        jsonArray.Add(embeddedJsonObject);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.BeginArray:
                        EnsureTokenType(tokenType, expectedToken);
                        JsonArray embeddedJsonArray = ParseAsArray(tokens);
                        jsonArray.Add(embeddedJsonArray);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.EndArray:
                        EnsureTokenType(tokenType, expectedToken);
                        return jsonArray;
                    case JsonToken.TokenType.Null:
                        EnsureTokenType(tokenType, expectedToken);
                        jsonArray.Add(jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.Number:
                        EnsureTokenType(tokenType, expectedToken);
                        jsonArray.Add(jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.Boolean:
                        EnsureTokenType(tokenType, expectedToken);
                        jsonArray.Add(jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.String:
                        EnsureTokenType(tokenType, expectedToken);
                        jsonArray.Add(jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndArray;
                        break;
                    case JsonToken.TokenType.SepComma:
                        EnsureTokenType(tokenType, expectedToken);
                        expectedToken = JsonToken.TokenType.String | JsonToken.TokenType.Null | JsonToken.TokenType.Number | JsonToken.TokenType.Boolean
                                | JsonToken.TokenType.BeginArray | JsonToken.TokenType.BeginObject;
                        break;
                    default:
                        ThrowHelper.Throw("Unexpected Token.");
                        break;
                }
            }

            ThrowHelper.Throw("Parse error, invalid Token.");
            return null;
        }

        private static JsonObject ParseAsObject(Queue<JsonToken> tokens)
        {

            JsonObject jsonObject = new JsonObject();

            JsonToken.TokenType expectedToken = JsonToken.TokenType.String | JsonToken.TokenType.EndObject;
            string key = null;
            while (tokens.Count > 0)
            {
                JsonToken jsonToken = tokens.Dequeue();
                JsonToken.TokenType tokenType = jsonToken.tokenType;
                switch (tokenType)
                {
                    case JsonToken.TokenType.BeginObject:
                        EnsureTokenType(tokenType, expectedToken);
                        JsonObject embeddedJO = ParseAsObject(tokens);
                        SafeAddValue(jsonObject,ref key,embeddedJO);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        break;
                    case JsonToken.TokenType.EndObject:
                        EnsureTokenType(tokenType, expectedToken);
                        return jsonObject;
                    case JsonToken.TokenType.BeginArray:
                        EnsureTokenType(tokenType, expectedToken);
                        JsonArray embeddedArray = ParseAsArray(tokens);
                        jsonObject.Add(key, embeddedArray);
                        key = null;
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        break;
                    case JsonToken.TokenType.Null:
                        EnsureTokenType(tokenType, expectedToken);
                        SafeAddValue(jsonObject, ref key, jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        break;
                    case JsonToken.TokenType.Number:
                        EnsureTokenType(tokenType, expectedToken);
                        SafeAddValue(jsonObject, ref key, jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        break;
                    case JsonToken.TokenType.Boolean:
                        EnsureTokenType(tokenType, expectedToken);
                        SafeAddValue(jsonObject, ref key, jsonToken.Value);
                        expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        break;
                    case JsonToken.TokenType.String:
                        EnsureTokenType(tokenType, expectedToken);
                        if (key != null)
                        {
                            SafeAddValue(jsonObject, ref key, jsonToken.Value);
                            expectedToken = JsonToken.TokenType.SepComma | JsonToken.TokenType.EndObject;
                        }
                        else
                        {
                            key = jsonToken.Value.GetStringValue();
                            expectedToken = JsonToken.TokenType.SepColon;
                        }
                        break;
                    case JsonToken.TokenType.SepColon:
                        EnsureTokenType(tokenType, expectedToken);
                        expectedToken = JsonToken.TokenType.Null | JsonToken.TokenType.Number | JsonToken.TokenType.Boolean | JsonToken.TokenType.String
                                | JsonToken.TokenType.BeginObject | JsonToken.TokenType.BeginArray;
                        break;
                    case JsonToken.TokenType.SepComma:
                        EnsureTokenType(tokenType, expectedToken);
                        expectedToken = JsonToken.TokenType.String;
                        break;
                    default:
                        ThrowHelper.Throw("unhandled token type" + tokenType);

                        break;
                }
            }

            ThrowHelper.Throw("object not closed!");

            return null;
        }

        private static void SafeAddValue(JsonObject jsonObject, ref string key, JsonValue value)
        {
            EnsureKey(key);
            jsonObject.Add(key, value);
            key = null;
        }

        private static void EnsureTokenType(JsonToken.TokenType tokenType, JsonToken.TokenType expectToken)
        {
            if ((tokenType & expectToken) == 0)
            {
                ThrowHelper.Throw(string.Format("Parse error, invalid Token.  expectToken = {0} but get = {1}", expectToken, tokenType));
            }
        }

        private static void EnsureKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                ThrowHelper.Throw("key is null or empty");
            }
        }

    }
}
