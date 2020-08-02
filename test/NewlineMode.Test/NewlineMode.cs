using Antlr4.Runtime;
using System;
using System.Linq;
using Xunit;

namespace NewlineMode.Test
{
	public class NewlineMode
	{
		[Theory]
		[InlineData("A\n\tB\n\t\tC\n\t\t\tD", "A\n\t<Indent>B\n\t\t<Indent>C\n\t\t\t<Indent>D<Dedent><Dedent><Dedent>")]
		public void Test(String source, String expected)
		{
			var inputStream = new AntlrInputStream(source);
			var lexer = new Lexer(inputStream);
			string joinedTokens = String.Join(String.Empty, lexer.GetAllTokens().Select(x => x.Text));
			Assert.Equal(expected, joinedTokens);
		}
	}
}
