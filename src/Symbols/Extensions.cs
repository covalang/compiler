using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cova.Symbols
{
	public static class Extensions
	{
		public static Boolean TryFindAncestor<T>(this ISymbol symbol, [MaybeNullWhen(false)] out T ancestor) where T : ISymbol
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

		public static T? FindAncestorOrNull<T>(this ISymbol symbol) where T : class, ISymbol
		{
			return symbol.TryFindAncestor<T>(out var ancestor) ? ancestor : null;
		}

		public static T FindAncestor<T>(this ISymbol symbol) where T : class, ISymbol
		{
			return symbol.FindAncestorOrNull<T>() ?? throw new AncestorNotFoundException<T>();
		}

		public static IEnumerable<ISymbol> Ancestors(this ISymbol symbol)
		{
			while (symbol.TryFindAncestor<ISymbol>(out var ancestor))
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
