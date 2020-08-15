using System;
using System.Collections.Generic;
using System.Text;

namespace RegExt
{
    internal class PatternFuncRange
    {
        public PatternFunc Prev { get; internal set; }
        public PatternFunc Func { get; internal set; }
        public PatternFunc Next { get; internal set; }
        public Range Range { get; internal set; }

        public PatternFuncRange(PatternFunc func, Range range, PatternFunc prev = null, PatternFunc next = null)
        {
            Func = func; Range = range; Prev = prev; Next = next;
        }

        public void ApplyTo(in string s, ref int i)
        {
            int j = 0;
            while (j < Range.Start.Value)
            {
                Func(s, ref i);
                j++;
            }
        }
    }
}
