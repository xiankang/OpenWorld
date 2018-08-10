using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core.Utils.Lib {
    public class Lib
    {
        public static unsafe string FirstCharToUpper(string input)
        {
            if (input == null) return null;
            string ret = string.Copy(input);
            fixed (char* ptr = ret)
                *ptr = char.ToUpper(*ptr);
            return ret;
        }

        public static unsafe string FirstCharToLower(string input)
        {
            if (input == null) return null;
            string ret = string.Copy(input);
            fixed (char* ptr = ret)
                *ptr = char.ToLower(*ptr);
            return ret;
        }
    }
}


