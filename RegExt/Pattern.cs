using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace RegExt
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="i"></param>
    /// <param name="prev"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    internal delegate bool PatternFunc(in string s, ref int i, PatternFunc prev=null, PatternFunc next=null);

    public class Pattern
    {
        internal readonly string pattern;
        private int i = 0;
        private readonly List<PatternFunc> funcs;
        private readonly Thread computeThread;
        // The indices in the range refer to indices in funcs where the captures are to be applied.
        private readonly List<Range> captureRanges;

        public Pattern(string s)
        {
            pattern = s;
            captureRanges = new List<Range>();
            computeThread = new Thread(new ThreadStart(SetupRegexAtoms));
            computeThread.Start();
        }

        private void SetupRegexAtoms()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the first match for the pattern.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public PatternMatch Match(string s)
        {
            // Note: need to implement captures here.
            PatternMatch match = new PatternMatch();

            computeThread.Join();

            try
            {
                int j = i;
                for (int f = 0; f < funcs.Count; )
                {
                    PatternFunc func = funcs[f];
                    bool applies = func(s, ref i, funcs.ElementAtOrDefault(f - 1), funcs.ElementAtOrDefault(f + 1));

                    if (applies) { f++; } else { f = 0; j++; i = j; }
                }
            }
            catch (IndexOutOfRangeException) { return null; }

            return match;
        }
    }
}
