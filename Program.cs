using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;

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
	        var main = File.ReadAllText("Main.cova");
			var lexemes = new List<Object>();
			for (int i = 0; i < main.Length; i++)
			{
				var ros = main.AsSpan(0);
				if (ros.Contains("namespace".AsSpan()) == 0)
					lexemes.Add("Namespace");
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
