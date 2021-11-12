using System.Diagnostics;

namespace Cova.Model
{
	// Independent, Interdependent

	public interface ISymbol : IHasParent<ISymbol?>, IHasChildren<ISymbol>, IHasDefinitionSource {}

	//public interface INamedType : IType, IHasName {}
}