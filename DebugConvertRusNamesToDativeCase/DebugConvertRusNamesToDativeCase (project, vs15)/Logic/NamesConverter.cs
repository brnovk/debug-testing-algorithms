using System;
using DebugConvertRusNamesToDativeCase.Util;

namespace DebugConvertRusNamesToDativeCase.Logic
{
	/// <summary>
	/// Utility class conversion name
	/// (dative case, formation of the surname with the initials of the full name)
	/// </summary>
	public static class NamesConverter
	{
		// ReSharper disable StringLiteralTypo
		/// <summary>
		/// Getting the user's full name in the dative case from the nominative.
		/// In case of conversion errors, the warnings text is written out to the warning text,
		/// and the full name remains in the nominative case.
		/// </summary>
		public static string FullnameInDativeCase(string lastName, string firstName, string middleName,
			Sex? nullableSex, out string warnings)
		{
			const string separator = " ";
			const string isEmptyMessage = "Фамилия, имя или отчество не указаны, или состоят из одних пробелов, " +
			    "или состоят из 1 символа, что недопустимо для алгоритма преобразования в дательный падеж, " +
			    "и остаются без изменений";
			const string isSexNotSpecified = "Пол сотрудника/курьера не указан, что недопустимо для алгоритма " +
			    "преобразования в дательный падеж, и ФИО остаются без изменений";

			var isEmptyLastName = string.IsNullOrWhiteSpace(lastName) || lastName.Length <= 1;
			var isEmptyFirstName = string.IsNullOrWhiteSpace(firstName) || firstName.Length <= 1;
			var isEmptyMiddleName = string.IsNullOrWhiteSpace(middleName) || middleName.Length <= 1;

			// One of the full name values ​​is empty or consists of 1 character - the full
			// name does not change and a warning is written
			if (isEmptyLastName || isEmptyFirstName || isEmptyMiddleName)
			{
				warnings = isEmptyMessage;
				return (isEmptyLastName ? string.Empty : lastName.Trim() + separator) +
					   (isEmptyFirstName ? string.Empty : firstName.Trim() + separator) +
					   (isEmptyMiddleName ? string.Empty : middleName.Trim() + separator).Trim();
			}
			// Gender is not specified - the full name does not change and a warning is written
			if (nullableSex == null)
			{
				warnings = isSexNotSpecified;
				return lastName.Trim() + separator + firstName.Trim() + separator
					   + (middleName.Trim() + separator).Trim();
			}

			var sex = (Sex)nullableSex;
			var dativeLastName = LastNameInDative(lastName, sex);       // Last name in the dative case
			var dativeFirstName = FirstNameInDative(firstName, sex);    // First name in the dative case
			var dativeMiddleName = MiddleNameInDative(middleName, sex); // Middle name in the dative case

			warnings = string.Empty;
			return string.Format(Constants.EmployeeFullNamePattern, dativeLastName, dativeFirstName, dativeMiddleName);
		}

		/// <summary>
		/// Last name in the dative case.
		/// </summary>
		private static string LastNameInDative(string original, Sex sex)
		{
			const char markerOfCompoundName = '-';
			original = original.Trim();

			// compound last names are divided into an array and each word is converted by a recursive call
			if (original.Contains(markerOfCompoundName.ToString()))
			{
				var parts = original.Split(markerOfCompoundName);
				var resultCompound = string.Empty;
				for (var i = 0; i < parts.Length; i++)
				{
					resultCompound += LastNameInDative(parts[i], sex);
					if (i != parts.Length - 1)
					{
						resultCompound += markerOfCompoundName;
					}
				}
				return resultCompound;
			}
			switch (sex)
			{
				case Sex.Male:
					if (IsWordEndOn(original, "о") || IsWordEndOn(original, "у"))
					{
						return original;
					}
					else if (IsWordEndOn(original, "а") || IsWordEndOn(original, "я"))
					{
						return ReplaceEnd(original, "е");
					}
					else if (IsWordEndOn(original, "ый") || IsWordEndOn(original, "ий") || IsWordEndOn(original, "ой"))
					{
						return ReplaceEnd(original, "ому", 2);
					}
					else if (IsWordEndOn(original, "бей"))
					{
						return ReplaceEnd(original, "бью");
					}
					else if (IsWordEndOn(original, "й"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "ок"))
					{
						return ReplaceEnd(original, "ку");
					}
					else if (IsWordEndOn(original, "лец"))
					{
						return ReplaceEnd(original, "льцу", 3);
					}
					else if (IsWordEndOn(original, "нец"))
					{
						return ReplaceEnd(original, "нецу", 3);
					}
					else if (IsWordEndOn(original, "ец"))
					{
						return ReplaceEnd(original, "цу");
					}
					else
					{
						return ReplaceEnd(original, "у", 0);
					}
				case Sex.Female:
					if (IsWordEndOn(original, "ва"))
					{
						return ReplaceEnd(original, "вой", 2);
					}
					else if (IsWordEndOn(original, "на"))
					{
						return ReplaceEnd(original, "ной", 2);
					}
					else if (IsWordEndOn(original, "ая"))
					{
						return ReplaceEnd(original, "ой");
					}
					else
					{
						return original;
					}
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// First name in the dative case
		/// </summary>
		private static string FirstNameInDative(string original, Sex sex)
		{
			const char markerOfCompoundName = '-';
			original = original.Trim();

			// compound names are divided into an array and a recursive call converts each word separately
			if (original.Contains(markerOfCompoundName.ToString()))
			{
				var parts = original.Split(markerOfCompoundName);
				var resultCompound = string.Empty;
				for (var i = 0; i < parts.Length; i++)
				{
					resultCompound += FirstNameInDative(parts[i], sex);
					if (i != parts.Length - 1)
					{
						resultCompound += markerOfCompoundName;
					}
				}
				return resultCompound;
			}
			switch (sex)
			{
				case Sex.Male:
					if (IsWordEndOn(original, "й"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "ел"))
					{
						return ReplaceEnd(original, "лу");
					}
					else if (IsWordEndOn(original, "ь"))
					{
						return ReplaceEnd(original, "ю");
					}
					else if (IsWordEndOn(original, "я"))
					{
						return ReplaceEnd(original, "е");
					}
					else
					{
						return ReplaceEnd(original, "у", 0);
					}
				case Sex.Female:
					if (IsWordEndOn(original, "ь"))
					{
						return ReplaceEnd(original, "и");
					}
					else
					{
						return ReplaceEnd(original, "е");
					}
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// Middle name in the dative case
		/// </summary>
		private static string MiddleNameInDative(string original, Sex sex)
		{
			original = original.Trim();
			switch (sex)
			{
				case Sex.Male:
					return ReplaceEnd(original, "у", 0);
				case Sex.Female:
					return ReplaceEnd(original, "е");
				default:
					throw new ApplicationException(sex.ToString());
			}
		}

		/// <summary>
		/// Check whether the word ends at the specified ending
		/// </summary>
		private static bool IsWordEndOn(string word, string ended)
		{
			const StringComparison comparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;
			var countEnded = ended.Length;
			var startEndingWordIndex = word.Length - countEnded;
			return word.Substring(startEndingWordIndex).Equals(ended, comparisonIgnoreCase);
		}

		/// <summary>
		/// Replacing the ending of a word with the specified ending.
		/// The case (lower case or upper case) of the ending depends on the case of the last char of the source word.
		/// </summary>
		private static string ReplaceEnd(string word, string ended)
		{
			return ReplaceEnd(word, ended, ended.Length);
		}

		/// <summary>
		/// Replaces the ending of the word with the specified ending, indicating the number of characters of
		/// the old word to be deleted.
		/// The case (lower case or upper case) of the ending depends on the case of the last char of the source word.
		/// </summary>
		private static string ReplaceEnd(string word, string ended, int deletedSymbols)
		{
			var lastCharIndex = word.Length - 1;
			var isWorldIsUpperCase = char.IsUpper(word[lastCharIndex]);
			var caseSensitiveEnding = isWorldIsUpperCase ? ended.ToUpper() : ended.ToLower();
			if (deletedSymbols == 0)
			{
				return word + caseSensitiveEnding;
			}
			return word.Remove(word.Length - deletedSymbols, deletedSymbols) + caseSensitiveEnding;
		}

		/// <summary>
		/// Getting abbreviated form full name (surname and initials with dots)
		/// </summary>
		public static string GetShortName(string fullName)
		{
			const string parsingErrorResult = "<Conversion error>";
			const string formatPattern = Constants.EmployeeShortNamePattern;
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return parsingErrorResult;
			}
			var split = fullName.Split(null);
			return split.Length == 3
				? string.Format(formatPattern, split[0], split[1][0], split[2][0])
				: parsingErrorResult;
		}
		// ReSharper restore StringLiteralTypo
	}
}
