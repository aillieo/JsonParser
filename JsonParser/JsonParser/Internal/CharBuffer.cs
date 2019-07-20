using System;

namespace AillieoUtils
{
    internal class CharBuffer
    {
        private readonly int bufferSize = 1024;
        private string json;
        private int stringOffset = 0;


        char[] arr = Array.Empty<char>();
        private int cursor = 0;

        public CharBuffer(string str)
        {
            json = str;
            stringOffset = 0;
            cursor = 0;
            arr = str.ToCharArray(0, Math.Min(bufferSize, str.Length));
        }

        public char Next()
        {
            char c = Head();
            cursor++;
            return c;
        }

        public char Head()
        {
            EnsureLength(1);
            return arr[cursor];
        }

        public bool EnsureLength(int length)
        {
            if (cursor + length > bufferSize)
            {
                stringOffset += cursor;
                cursor = 0;
                arr = json.ToCharArray(stringOffset, Math.Min(bufferSize, json.Length - stringOffset));
            }
            return true;
        }

        public bool HasMore()
        {
            return stringOffset + cursor <= json.Length - 1;
        }

        public string Get(int len)
        {
            EnsureLength(len);
            string ret = new string(arr, cursor, len);
            cursor += len;
            return ret;
        }

    }
}
