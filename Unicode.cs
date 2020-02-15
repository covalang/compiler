using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Compiler
{
    static unsafe class Unicode
	{
		// The native pointer to the 12:4:4 index table of the Unicode cateogry data.
		static ushort* s_pCategoryLevel1Index;
		static byte* s_pCategoriesValue;

		// The native pointer to the 12:4:4 index table of the Unicode numeric data.
		// The value of this index table is an index into the real value table stored in s_pNumericValues.
		static ushort* s_pNumericLevel1Index;

		// The numeric value table, which is indexed by s_pNumericLevel1Index.
		// Every item contains the value for numeric value.
		// unsafe static double* s_pNumericValues;
		// To get around the IA64 alignment issue.  Our double data is aligned in 8-byte boundary, but loader loads the embeded table starting
		// at 4-byte boundary.  This cause a alignment issue since double is 8-byte.
		static byte* s_pNumericValues;

		// The digit value table, which is indexed by s_pNumericLevel1Index.  It shares the same indice as s_pNumericValues.
		// Every item contains the value for decimal digit/digit value.
		static DigitValues* s_pDigitValues;

		enum UnicodeClass : byte
		{
			UppercaseLetter,
			LowercaseLetter,
			TitlecaseLetter,
			ModifierLetter,
			OtherLetter,
			NonSpacingMark,
			SpacingCombiningMark,
			EnclosingMark,
			DecimalDigitNumber,
			LetterNumber,
			OtherNumber,
			SpaceSeparator,
			LineSeparator,
			ParagraphSeparator,
			Control,
			Format,
			Surrogate,
			PrivateUse,
			ConnectorPunctuation,
			DashPunctuation,
			OpenPunctuation,
			ClosePunctuation,
			InitialQuotePunctuation,
			FinalQuotePunctuation,
			OtherPunctuation,
			MathSymbol,
			CurrencySymbol,
			ModifierSymbol,
			OtherSymbol,
			OtherNotAssigned,
		}

		public static UnicodeCategory GetUniCat(String code)
		{
			switch (code)
			{
				case "Cc": return UnicodeCategory.Control;
				case "Cf": return UnicodeCategory.Format;
				case "Cn": return UnicodeCategory.OtherNotAssigned;
				case "Co": return UnicodeCategory.PrivateUse;
				case "Cs": return UnicodeCategory.Surrogate;
				case "Ll": return UnicodeCategory.LowercaseLetter;
				case "Lm": return UnicodeCategory.ModifierLetter;
				case "Lo": return UnicodeCategory.OtherLetter;
				case "Lt": return UnicodeCategory.TitlecaseLetter;
				case "Lu": return UnicodeCategory.UppercaseLetter;
				case "Mc": return UnicodeCategory.SpacingCombiningMark;
				case "Me": return UnicodeCategory.EnclosingMark;
				case "Mn": return UnicodeCategory.NonSpacingMark;
				case "Nd": return UnicodeCategory.DecimalDigitNumber;
				case "Nl": return UnicodeCategory.LetterNumber;
				case "No": return UnicodeCategory.OtherNumber;
				case "Pc": return UnicodeCategory.ConnectorPunctuation;
				case "Pd": return UnicodeCategory.DashPunctuation;
				case "Pe": return UnicodeCategory.ClosePunctuation;
				case "Pf": return UnicodeCategory.FinalQuotePunctuation;
				case "Pi": return UnicodeCategory.InitialQuotePunctuation;
				case "Po": return UnicodeCategory.OtherPunctuation;
				case "Ps": return UnicodeCategory.OpenPunctuation;
				case "Sc": return UnicodeCategory.CurrencySymbol;
				case "Sk": return UnicodeCategory.ModifierSymbol;
				case "Sm": return UnicodeCategory.MathSymbol;
				case "So": return UnicodeCategory.OtherSymbol;
				case "Zl": return UnicodeCategory.LineSeparator;
				case "Zp": return UnicodeCategory.ParagraphSeparator;
				case "Zs": return UnicodeCategory.SpaceSeparator;
				default: throw new ArgumentOutOfRangeException(nameof(code));
			}
		}

		static Unicode()
		{
			const String path = @"C:\Users\nicks\Downloads\charinfo.nlp";
			var fileInfo = new FileInfo(path);
			var bytes = File.ReadAllBytes(path);
			var aligned = MarshalEx.AllocHGlobalAligned((Int32) fileInfo.Length, 8);

			// Go to native side and get pointer to the native table
			var pDataTable = (Byte*) aligned.ToPointer();
			fixed (byte* pBytes = bytes)
				Buffer.MemoryCopy(pBytes, pDataTable, fileInfo.Length, fileInfo.Length);


			UnicodeDataHeader* mainHeader = (UnicodeDataHeader*) pDataTable;

			// Set up the native pointer to different part of the tables.
			s_pCategoryLevel1Index = (ushort*) (pDataTable + mainHeader->OffsetToCategoriesIndex);
			s_pCategoriesValue = (byte*) (pDataTable + mainHeader->OffsetToCategoriesValue);
			s_pNumericLevel1Index = (ushort*) (pDataTable + mainHeader->OffsetToNumbericIndex);
			s_pNumericValues = (byte*) (pDataTable + mainHeader->OffsetToNumbericValue);
			s_pDigitValues = (DigitValues*) (pDataTable + mainHeader->OffsetToDigitValue);
		}

		public static UnicodeCategory GetUnicodeCategory(uint ch)
	    {
		    if (ch > 0x10ffff)
			    return (UnicodeCategory)(-1);

		    // Get the level 2 item from the highest 12 bit (8 - 19) of ch.
		    ushort index = s_pCategoryLevel1Index[ch >> 8];
		    // Get the level 2 WORD offset from the 4 - 7 bit of ch.  This provides the base offset of the level 3 table.
		    // Note that & has the lower precedence than addition, so don't forget the parathesis.
		    index = s_pCategoryLevel1Index[index + ((ch >> 4) & 0x000f)];
		    byte* pBytePtr = (byte*)&(s_pCategoryLevel1Index[index]);
		    // Get the result from the 0 -3 bit of ch.
		    byte valueIndex = pBytePtr[(ch & 0x000f)];
		    byte uc = s_pCategoriesValue[valueIndex * 2];
		    //
		    // Make sure that OtherNotAssigned is the last category in UnicodeCategory.
		    // If that changes, change the following assertion as well.
		    //
		    //Contract.Assert(uc >= 0 && uc <= UnicodeCategory.OtherNotAssigned, "Table returns incorrect Unicode category");
		    return (UnicodeCategory) (uc);
		}
	}

	// NOTE: It's important to specify pack size here, since the size of the structure is 2 bytes.  Otherwise,
	// the default pack size will be 4.
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct DigitValues
	{
		internal sbyte decimalDigit;
		internal sbyte digit;
	}

	[StructLayout(LayoutKind.Explicit)]
	struct UnicodeDataHeader
	{
		// Fields
		[FieldOffset(40)]
		internal uint OffsetToCategoriesIndex;
		[FieldOffset(0x2c)]
		internal uint OffsetToCategoriesValue;
		[FieldOffset(0x34)]
		internal uint OffsetToDigitValue;
		[FieldOffset(0x30)]
		internal uint OffsetToNumbericIndex;
		[FieldOffset(0x38)]
		internal uint OffsetToNumbericValue;
		[FieldOffset(0)]
		internal char TableName;
		[FieldOffset(0x20)]
		internal ushort version;
	}
}
