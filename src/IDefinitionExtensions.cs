using System;

static class IDefinitionExtensions
{
	public static T? FindAncestorOrNull<T>(this IDefinition? definition)
	{
		while (definition?.Parent != null)
		{
			if (definition.Parent is T x)
				return x;
			definition = definition.Parent;
		}

		return default;
	}

	public static T FindAncestor<T>(this IDefinition? definition) =>
		definition.FindAncestorOrNull<T>() ?? throw new AncestorNotFoundException<T>();

	sealed class AncestorNotFoundException<T> : ArgumentException
	{
		public AncestorNotFoundException() : base("Expected to find ancestor of type: " + typeof(T).FullName) { }
	}
}