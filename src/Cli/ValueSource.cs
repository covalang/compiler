using System.CommandLine.Binding;
using Microsoft.Extensions.DependencyInjection;

namespace Cova.Cli;

sealed class ValueSource<T> : BinderBase<T> where T : notnull
{
	protected override T GetBoundValue(BindingContext bindingContext) => bindingContext.GetRequiredService<T>();
}