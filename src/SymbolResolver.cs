using Cova.Symbols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Cova
{
	public class SymbolResolver
	{
		private readonly ConcurrentDictionary<(IScope scope, String name), ISymbol> symbols = new();

		public IEnumerable<ISymbol> Resolve(IScope local, String name)
		{
			static IEnumerable<IScope> selector(IScope x) => new[] { x }.Concat(x.Imported);
			IEnumerable<IScope> scopes = selector(local).Concat(local.Ancestors().OfType<IScope>().SelectMany(selector));
			foreach (IScope scope in scopes)
			{
				if (symbols.TryGetValue((scope, name), out var found))
					yield return found;
			}
		}

		public Boolean TryRegister<TSymbol>(TSymbol symbol, [MaybeNullWhen(true)] out ISymbol existing) where TSymbol : class, ISymbol, IHasName
		{
			var scope = symbol.FindAncestor<IScope>();
			var result = symbols.GetOrAdd((scope, symbol.Name), symbol);
			if (result == symbol)
			{
				existing = null;
				return true;
			}
			existing = result;
			return false;
		}
	}
}