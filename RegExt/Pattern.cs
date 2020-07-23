using System;
using System.Collections.Generic;
using System.Threading;

namespace RegExt
{
    public delegate bool PatternFunc(in string s, ref int i, PatternFunc prev=null, PatternFunc next=null);

    public class Pattern
    {
        internal readonly string pattern;
        private int i = 0;
        private List<PatternFunc> funcs;
        private Thread computeThread;

        public Pattern(string s)
        {
            pattern = s;
            computeThread = new Thread(new ThreadStart(SetupRegexAtoms));
            computeThread.Start();
        }

        private void SetupRegexAtoms()
        {
            throw new NotImplementedException();
        }

        public PatternMatch Match(string s)
        {
            TryMatch tryMatch = new TryMatch();

            computeThread.Join();
            foreach (PatternFunc func in funcs)
            {
                func(s, ref i); i++;
            }

            return new PatternMatch(tryMatch);
        }

        // Deal with matches later.
    }
}
