using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using Compiler;

namespace Cova
{
    class Program
    {
		class CompliationUnit
		{
			public List<UseDirective> UseDirectives { get; } = new List<UseDirective>();
			public List<Type>         Types         { get; } = new List<Type>();
		}

		class UseDirective
		{
			public String Namespace { get; set; }
		}

		class Type
		{
			public Visibility Visibility               { get; set; }
			public Storage Storage                     { get; set; }
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name                         { get; set; }
			public Kind Kind                           { get; set; }

			public List<Field> Fields                  { get; } = new List<Field>();
			public List<Property> Properties           { get; } = new List<Property>();
			public List<Method> Methods                { get; } = new List<Method>();
		}

		enum Kind : byte
		{
			Class,
			Primitive,
			Trait,
			Interface
		}

		enum Visibility : byte
		{
			Public,
			Private,
			Internal,
			Protected
		}

		enum Storage : byte
		{
			Static,
			Instance
		}

		enum ReferralSemantics : byte
		{
			Value,
			Reference,
			Tag,

		}

		class Field
		{
			public Visibility Visibility               { get; set; }
			public Storage Storage                     { get; set; }
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name                         { get; set; }
			public Type Type                           { get; set; }
		}

		class Property
		{
			public Visibility GetterVisibility         { get; set; }
			public Visibility SetterVisibility         { get; set; }
			public Storage Storage                     { get; set; }
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name                         { get; set; }
			public Type Type                           { get; set; }
		}

		class Method
		{
			public Visibility Visibility               { get; set; }
			public Storage Storage                     { get; set; }
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name                         { get; set; }
			public Type ReturnType                     { get; set; }

			public List<Parameter>Parameters           { get; } = new List<Parameter>();
			public List<Local> Locals                  { get; } = new List<Local>();
			public List<Statement>Statements           { get; } = new List<Statement>();
		}

		class Parameter
		{
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name                         { get; set; }
			public Type Type                           { get; set; }
		}

		class Local
		{
			public Storage Storage { get; set; }
			public ReferralSemantics ReferralSemantics { get; set; }
			public String Name { get; set; }
			public Type Type { get; set; }
		}

		class Statement
		{
			public List<Expression> Expressions { get; } = new List<Expression>();
		}

		class Expression
		{
		}

        static void Main(String[] args)
        {
	        var lines = File.ReadAllLines(@"C:\Users\nicks\Downloads\UnicodeData.txt");
	        var asdf = new (UInt32, UnicodeCategory)[0x10ffff];
	        for (UInt32 i = 0; i < lines.Length; i++)
	        {
		        var parts = lines[i].Split(';');
		        var code = UInt32.Parse(parts[0], NumberStyles.HexNumber);
		        if (parts[1].EndsWith("First>"))
		        {
			        i++;
			        var parts2 = lines[i].Split(';');
			        if (!parts2[1].EndsWith("Last>"))
				        throw new Exception();
			        asdf[i] = (code, Unicode.GetUniCat(parts[2]));
			        var code2 = UInt32.Parse(parts2[0], NumberStyles.HexNumber);
			        for (UInt32 j = code; j <= code2; j++)
				        asdf[i] = (code, Unicode.GetUniCat(parts2[2]));
			        continue;
		        }
		        var uc1 = Unicode.GetUniCat(parts[2]);
		        var uc2 = CharUnicodeInfo.GetUnicodeCategory((Int32)code);
		        if (uc1 != uc2)
			        ;
		        asdf[i] = (code, uc1);
	        }

	        for (var i = 0; i < asdf.Length; i++)
	        {
		        if (asdf[i].Item1 != i)
			        throw new Exception();
	        }


			//      var methods = typeof(CharUnicodeInfo).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)/*.Where(x => x.Name == "InternalGetUnicodeCategory").ToArray()*/;
			//      var method = typeof(CharUnicodeInfo).GetMethod("InternalGetCategoryValue", BindingFlags.Static | BindingFlags.NonPublic);

			//var what = (Func<int, int, UnicodeCategory>)method
			//	.CreateDelegate(typeof(Func<int, int, UnicodeCategory>));
			//Console.WriteLine(what.Invoke(1, 0));
			for (var i = 0; i != Int32.MaxValue - 1; i++)
			{
				var uc1 = CharUnicodeInfo.GetUnicodeCategory(i);
				var uc2 = Unicode.GetUnicodeCategory((UInt32) i);
				if (uc1 != uc2)
				{
					var c = (char)i;
					throw new Exception(c.ToString());
				}
			}

	        return;
	        var main = File.ReadAllText("Main.cova");
			var lexemes = new List<Object>();
			for (int i = 0; i < main.Length; i++)
			{
				var ros = main.AsSpan(0);
				//if (ros.Contains("namespace".AsSpan()) == 0)
				//	lexemes.Add("Namespace");
			}

	        var si = new StringInfo(main);
	        var tee = StringInfo.GetTextElementEnumerator(main);
			var indices = StringInfo.ParseCombiningCharacters(main);
			Int32 index = 0;
			Int32 length = 0;
			var graphemes = new List<(String @string, Int32 index, Int32 length)>();
			while (tee.MoveNext())
			{
				if (tee.ElementIndex != 0)
				index = tee.ElementIndex;
			}
			graphemes.Add((main, index, main.Length - length));
			using (var mmf = MemoryMappedFile.CreateFromFile("Main.cova"))
			using (var va = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
			{
				new ReadOnlyMemory<byte>();
				new ReadOnlySpan<byte>();
			}
        }
	}

	class Grapheme {
		public readonly String String;
		public readonly Int32 Index;
		public readonly Int32 Length;

        public Grapheme(String @string, Int32 index, Int32 length)
        {
        }
    }

	static class StringExtensions
	{
		public static IEnumerable<String> AsTextElementEnumerable(this String s)
		{
			var enumerator = StringInfo.GetTextElementEnumerator(s);
			while (enumerator.MoveNext())
				yield return enumerator.GetTextElement();
		}
	}
}
