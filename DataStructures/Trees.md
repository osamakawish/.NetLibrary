# Trees

This assembly contains two tree classes: `Tree` and `OrderedTree`. Their methods and implementations are more or less the seem, with the only difference being the `Tree` uses a `HashSet` to store its children, whereas an `OrderedTree` uses a `List`. In short, the elements of the `Tree` are not ordered, whereas the elements of `OrderedTree` are.

### Examples

#### Create and Modify

Here's a general example of how to create and modify trees.

```c#
Tree<int> t = new Tree<int>(4);				// 1
Tree<int> ch = t.AddChild(3);				// 2
t.AddChildren(4,7,9);						// 3
t.AddChildren(new List<int> {-34,0,16});	// 4
ch.AddChild(12);
t.RemoveChild(7);							// 5
t.RemoveChild(63);							// Does nothing
```

This image shows how the tree node `t` is modified by the code above.

![](..\TreeExampleModify.png)

The lines of code:

```c#
Tree<int> ch = t.AddChild(3);				// 2
ch.AddChild(12);
```

show how to insert grandchild nodes into the tree. 

To ensure safe and secure implementations of the tree, we don't have a method which adds a tree node as a child, as the tree instance may already have a parent.

#### Iterations and Containment

The two typical iterations over trees are provide in the `TreeIter` static class.

##### `HasChild` and `Has`

The `HasChild` method simply checks if the tree instance has the given child.

The `Has` method iterates over all descendants, breadth-first, by default. If you want, you can define and pass a custom iteration for it to implement. Use this method with default arguments to see if the tree has a given descendant:

```c#
Tree<string> tree = new Tree("Abc");

Tree<string> ch1 = tree.AddChild("Cha");
Tree<string> ch2 = ch1.AddChild("Rac");

bool hasChild = tree.HasChild(ch1); // true
bool hasChild2 = tree.HasChild(new Tree<string>("Cha")); // false
bool hasDescendant = tree.Has(ch2); // true
```

Note that creating a new tree is not

## Properties and Methods

All of the methods below also apply to `OrderedTree`.

| Return Type | Method | Description |
| ------ | ----------- | ------ |
| **Constructor** | Tree(T val) | Creates a root tree node. |
| HashSet<Tree<T>> | Children() | The set of all children of the tree. |
| Tree<T> | AddChild(T val) | Adds a child to the tree, then returns the instance of the tree node create. |
| IEnumerable<Tree<T>> | AddChildren(params T[] values) | Adds children with the provided values to the tree, then returns an iterator to the instances created. |
| IEnumerable<Tree<T>> | AddChildren(IEnumerable<T> values) | Adds children with the provided values to the tree, then returns an iterator to the instances created. |
| bool | RemoveChild(Tree<T> child) | Removes the child from this instance, if it exists. If it does, return true. If not, return false. |
| bool | HasChild(Tree<T> child) | True if this instance has the child parameter as its child, false if not. |
| bool | Has(Tree<T> tree, Func<Tree<T>, IEnumerable<Tree<T>> iter) | Iterates over the tree by `iter` checks if the tree contains it. |
| bool | HasValue(T value) | Checks if the tree contains the given value |
| IEnumerable<Tree<T>> | WhereByValue(Func<T, bool> cond, Func<Tree<T>, IEnumerable<Tree<T>> iter) | Returns all elements in the iteration that satisfy the given condition by value. |
| IEnumerable<Tree<T>> | WhereByValue(T value, Func<Tree<T>, IEnumerable<Tree<T>> iter) | Returns all elements in the iteration by value. |
| Tree<T> | FirstByValue(T value, Func<Tree<T>, IEnumerable<Tree<T>> iter) | Returns the first tree instance with the given value. |
| Tree<T> | Root | The root of the tree. |
| HashSet<Tree<T>> | Siblings | The siblings of this tree instance. |

