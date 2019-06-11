using System;
using System.Linq;
using System.Reflection;

namespace DebugConvertRusNamesToDativeCase.Util
{
	/// <summary>
	/// Sex of the person.
	/// </summary>
	public enum Sex
	{
		// ReSharper disable StringLiteralTypo
		[Sex(DisplayedShort = "м", DisplayedFull = "Мужской", Canonical = "Male")]
		Male,
		[Sex(DisplayedShort = "ж", DisplayedFull = "Женский", Canonical = "Female")]
		Female
		// ReSharper restore StringLiteralTypo
	}

	/// <inheritdoc />
	/// <summary>
	/// Additional attributes of the sex enumeration [Sex]
	/// </summary>
	internal class SexAttribute : Attribute
	{
		public string DisplayedShort { get; set; }
		public string DisplayedFull { get; set; }
		public string Canonical { get; set; }
	}

	/// <summary>
	/// Utility class of enumeration of the floor, which includes the extension methods of
	/// obtaining attributes and a static parser.
	/// </summary>
	public static class SexUtil
	{
		// ReSharper disable UnusedMember.Global
		// ReSharper disable MemberCanBePrivate.Global

		public static string DisplayedShort(this Sex sex) { return GetAttribute(sex).DisplayedShort; }
		public static string DisplayedShort(this Sex? sex) { return sex == null ? null : ((Sex)sex).DisplayedShort(); }

		public static string DisplayedFull(this Sex sex) { return GetAttribute(sex).DisplayedFull; }
		public static string DisplayedFull(this Sex? sex) { return sex == null ? null : ((Sex)sex).DisplayedFull(); }

		public static string Canonical(this Sex sex) { return GetAttribute(sex).Canonical; }
		public static string Canonical(this Sex? sex) { return sex == null ? null : ((Sex)sex).Canonical(); }

		// ReSharper restore MemberCanBePrivate.Global
		// ReSharper restore UnusedMember.Global

		/// <summary>
		/// Getting a person's gender from a string representation (abbreviated, complete and canonical)
		/// </summary>
		public static Sex? GetFromString(string rawValue)
		{
			const StringComparison ignoreCaseComparison = StringComparison.OrdinalIgnoreCase;
			string[] maleNames = { Sex.Male.Canonical(), Sex.Male.DisplayedShort(), Sex.Male.DisplayedFull() };
			string[] femaleNames = { Sex.Female.Canonical(), Sex.Female.DisplayedShort(), Sex.Female.DisplayedFull() };
			if (string.IsNullOrWhiteSpace(rawValue))
			{
				return null;
			}
			var trimmedValue = rawValue.Trim();
			if (maleNames.Any(maleName => string.Equals(maleName, trimmedValue, ignoreCaseComparison)))
			{
				return Sex.Male;
			}
			if (femaleNames.Any(femaleName => string.Equals(femaleName, trimmedValue, ignoreCaseComparison)))
			{
				return Sex.Female;
			}
			return null;
		}

		private static SexAttribute GetAttribute(Sex sex)
		{
			return (SexAttribute)Attribute.GetCustomAttribute(GetMemberInfo(sex), typeof(SexAttribute));
		}

		private static MemberInfo GetMemberInfo(Sex sex)
		{
			return typeof(Sex).GetField(Enum.GetName(typeof(Sex), sex));
		}
	}
}
