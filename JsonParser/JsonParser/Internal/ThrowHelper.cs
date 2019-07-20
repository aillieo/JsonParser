using System.Collections;
using System.Collections.Generic;
using System;

namespace AillieoUtils
{
    internal static class ThrowHelper
    {
        public static void Throw(string exception)
        {
            throw new Exception(exception);
        }
    }
}
