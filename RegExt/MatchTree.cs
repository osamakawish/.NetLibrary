using Thorns;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace RegExt.CSharp
{
    public class MatchTree : OrderedTree<string>
    { internal MatchTree(string match) : base(match) { } }
}
