using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

		enum UnicodeCharacterClass : byte
		{
			Cn,
			Cc,
			Cf,
			Co,
			Cs,
			Ll,
			Lm,
			Lo,
			Lt,
			Lu,
			Mc,
			Me,
			Mn,
			Nd,
			Nl,
			No,
			Pc,
			Pd,
			Pe,
			Pf,
			Pi,
			Po,
			Ps,
			Sc,
			Sk,
			Sm,
			So,
			Zl,
			Zp,
			Zs
		}

		enum UnicodeCharacterClassName : byte
		{
			OtherNotAssigned,
			Control,
			Format,
			PrivateUse,
			Surrogate,
			LowercaseLetter,
			ModifierLetter,
			OtherLetter,
			TitlecaseLetter,
			UppercaseLetter,
			SpacingCombiningMark,
			EnclosingMark,
			NonSpacingMark,
			DecimalDigitNumber,
			LetterNumber,
			OtherNumber,
			ConnectorPunctuation,
			DashPunctuation,
			ClosePunctuation,
			FinalQuotePunctuation,
			InitialQuotePunctuation,
			OtherPunctuation,
			OpenPunctuation,
			CurrencySymbol,
			ModifierSymbol,
			MathSymbol,
			OtherSymbol,
			LineSeparator,
			ParagraphSeparator,
			SpaceSeparator,
		}

		struct Range {
			public int First;
			public int Last;
			public int Size => Last - First + 1;
			
			public override String ToString() => First == Last ? First.ToString() : $"{First}-{Last}: {Size}";
		}

		enum State
		{
			StartMaybeRange,
			ExplicitRange
		}

		// static List<List<UInt32>> GetFactors(UInt32 x) {
		// 	var factors = new List<List<UInt32>>();
		// 	var hold = new List<UInt32>();
		// 	for (var i = 2; i < x; i++) {
		// 		if (hold.Aggregate((y, z) => y *= z) == x) {

		// 		}
		// 	}
		// }

        static void Main(String[] args)
        {
	        //var lines = File.ReadAllLines(@"C:\Users\nicks\Downloads\UnicodeData.txt");
	        var lines = File.ReadAllLines(@"/Users/Nick/Downloads/UCD/UnicodeData.txt");
			var classRanges = new Dictionary<UnicodeCharacterClass, List<Range>>();
			var unicodeCharacterClassNames = Enum.GetNames(typeof(UnicodeCharacterClass));
			foreach (UnicodeCharacterClass x in Enum.GetValues(typeof(UnicodeCharacterClass)))
				classRanges[x] = new List<Range>();
			
			var range = default(Range);
			var hold = default(UnicodeCharacterClass);
			var previousCodePoint = -1;
			var state = 0;
			foreach (var line in lines)
			{
				var parts = line.Split(';');
				var codePoint = Int32.Parse(parts[0], NumberStyles.HexNumber);
				var ucc = (UnicodeCharacterClass) Enum.Parse(typeof(UnicodeCharacterClass), parts[2]);
				var isFirst = parts[1].EndsWith("First>");
				var isLast = parts[1].EndsWith("Last>");
				switch (state)
				{
					case 0:
						range.First = codePoint;
						hold = ucc;
						state = isFirst ? 1 : 2;
						break;
					case 1:
						if (hold != ucc)
							throw new Exception("First-Last pair with mismatched character class");
						range.Last = codePoint;
						classRanges[ucc].Add(range);
						range = default;
						state = 0;
						break;
					case 2:
						if (hold == ucc)
							break;
						range.Last = previousCodePoint;
						classRanges[hold].Add(range);
						range = default;
						hold = ucc;
						state = 0;
						goto case 0;
				}
				previousCodePoint = codePoint;
			}
			var orderByDescMax = classRanges.OrderByDescending(x => x.Value.Any() ? x.Value.Max(y => y.Size) : 0).ToList();
			var orderByDescSum = classRanges.OrderByDescending(x => x.Value.Any() ? x.Value.Sum(y => y.Size) : 0).ToList();
			var max = classRanges.Max(x => x.Value.Any() ? x.Value.Max(y => y.Last) : 0);
			var bits = Math.Ceiling(Math.Log(max, 2));
			var tableSize = (Int32)Math.Pow(2, bits);
			var table = new Byte[tableSize];
			foreach (var classRange in classRanges)
				foreach (var x in classRange.Value)
					for (var i = x.First; i <= x.Last; i++)
						table[i] = (Byte) classRange.Key;
			//File.WriteAllBytes("UnicodeCharacterClassesTable.data", table);
			
			// calculate list of non-zero numbers which sum to 21
			var levelSizes = (Int32) bits / 3;
			var levelMask = (Int32) Math.Pow(2, levelSizes) - 1;
			// var level1 = new List<Int32>();
			// var level2 = new List<Int32>();
			// var level3 = new List<Int32>();
			// for (var i = 0; i != table) {
			// 	var a = x >> (levelSizes * 2);
			// 	var b = (x >> levelSizes) & levelMask;
			// 	var c = x  & levelMask;
			// }
			var level1 = table.Select((x,i) => (i >> (levelSizes * 2), x)).Distinct().GroupBy(x => x.Item1, x => x.).ToList();
			var level2 = table.Select((x,i) => ((i >> levelSizes) & levelMask, x)).Distinct().ToList();
			var level3 = table.Select((x,i) => (i & levelMask, x)).Distinct().ToList();
			unsafe {
				void * p = Marshal.AllocHGlobal(32).ToPointer();
				Marshal.FreeHGlobal(new IntPtr(p));
			}
			var final = (UnicodeCharacterClass[]) Enum.GetValues(typeof(UnicodeCharacterClass));
			return;

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
