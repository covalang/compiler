using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Cova.Model
{
    public static class ModelExtensions
    {
        public static void AddChild<TParent, TChild>(this TParent parent, TChild child)
            where TParent : Symbol
            where TChild : Symbol
        {
            parent.Children.Add(child);
            if (parent is IHasChildren<TChild> hasChildren)
                hasChildren.Children.Add(child);
            // TypedChildrenGetterStaticCache<TParent, TChild>.GetChildrenOrDefault?.Invoke(parent)?.Add(child);
        }

        // private static class TypedChildrenGetterStaticCache<TParent, TChild>
        //     where TParent : Symbol
        //     where TChild : Symbol
        // {
        //     public static readonly Func<TParent, ICollection<TChild>?>? GetChildrenOrDefault;
        //
        //     static TypedChildrenGetterStaticCache()
        //     {
        //         var propertyInfo =
        //             typeof(TParent)
        //                 .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        //                 .SingleOrDefault(x => x.PropertyType == typeof(ICollection<TChild>));
        //         GetChildrenOrDefault = propertyInfo?.GetMethod?.CreateDelegate<Func<TParent, ICollection<TChild>?>>();
        //     }
        // }
        
        public static Boolean TryFindAncestor<T>(this Symbol symbol, [MaybeNullWhen(false)] out T ancestor) where T : Symbol
        {
            while (symbol.Parent != null)
            {
                if (symbol.Parent is T x)
                {
                    ancestor = x;
                    return true;
                }
                symbol = symbol.Parent;
            }
            ancestor = default;
            return false;
        }

        public static T? FindAncestorOrNull<T>(this Symbol symbol) where T : Symbol
        {
            return symbol.TryFindAncestor<T>(out var ancestor) ? ancestor : null;
        }

        public static T FindAncestor<T>(this Symbol symbol) where T : Symbol
        {
            return symbol.FindAncestorOrNull<T>() ?? throw new AncestorNotFoundException<T>();
        }

        public static IEnumerable<Symbol> Ancestors(this Symbol symbol)
        {
            while (symbol.TryFindAncestor<Symbol>(out var ancestor))
            {
                symbol = ancestor;
                yield return ancestor;
            }
        }

        sealed class AncestorNotFoundException<T> : ArgumentException
        {
            public AncestorNotFoundException() : base("Expected to find ancestor of type: " + typeof(T).FullName) { }
        }
    }
}