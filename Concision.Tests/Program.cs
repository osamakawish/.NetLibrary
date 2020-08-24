using Concision.Thorns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Concision.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            TreeTest();
        }

        
        private static void TreeTest()
        {
            Tree<int> tree = new Tree<int>(4);

            List<Tree<int>> children = tree.AddChildrenYielded(-2, -9642).ToList();

            foreach (var child in children)
            {
                int i = child.Value;
                child.AddChildren(i, i + 1, i + 2, i + 3);
            }

            IEnumerable<int> iter = TreeIter.BreadthFirstByValue(tree);

            Console.WriteLine(String.Join(' ',iter.ToArray()));
        }
    }
}
