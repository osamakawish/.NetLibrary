using System;
using System.Collections.Generic;

namespace Strings
{
    public class Pattern
    {
        private List<AtomGroup> atomGroups;

        public string String { get; private set; }

        public Pattern(string s="")
        {
            String = s;
            GenerateAtomGroups(s);
        }

        /// <summary>
        /// Generates the atom groups based on the string provided.
        /// </summary>
        /// <param name="s"></param>
        private void GenerateAtomGroups(string s)
        {
            throw new NotImplementedException();
        }

        public bool IsMatch(in string s, int start=0)
        {
            // Counts the number of times the current match group has been implemented.
            int count = 0;

            for (int i = 0; i < atomGroups.Count; i++)
            {
                // If the current match group has reached its minimum, try to go to next matchgroup

                // If the current match group has reached its maximum, and next matchgroup doesn't match either, return false

                // Note, pattern may start in the middle of the current test. Always make sure to test starting from the middle

            }

            return true;
        }
    }
}