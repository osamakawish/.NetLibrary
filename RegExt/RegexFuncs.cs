using System;
using System.Collections.Generic;
using System.Text;

namespace RegExt
{
    public static class RegexFuncs
    {
        public static PatternFunc AnyChar => 
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return true; };

        /// <summary>
        /// Tests whether the next condition applies. (ie. Place this func BEFORE the func being applied to in the list)
        /// </summary>
        /// <returns></returns>
        public static PatternFunc Optional =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { if (next(s, ref i)) { i++; } return true; };

        public static PatternFunc StartOfString =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == 0; };

        public static PatternFunc EndOfString =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == s.Length; };

        public static PatternFunc LineStart =>
            delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { return i == 0 || "\r\n".Contains(s[i - 1].ToString()); };

        public static PatternFunc OptionalCountable 
            => delegate (in string s, ref int i, PatternFunc prev, PatternFunc next) { while (prev(s, ref i) && !next(s, ref i)) i++; i--; return true; };
    }
}
