using System;
using System.Collections.Generic;

namespace DebugConvertRusMoneyFromDigitToString.Logic
{
	// ReSharper disable StringLiteralTypo
	// ReSharper disable CommentTypo
	// ReSharper disable IdentifierTypo

	public enum TextCase
	{
		Nominative,   /* Кто? Что? */
		Genitive,     /* Кого? Чего? */
		Dative,       /* Кому? Чему? */
		Accusative,   /* Кого? Что? */
		Instrumental, /* Кем? Чем? */
		Prepositional /* О ком? О чём? */
	}

	/// <summary>
	/// Redesigned algorithm from article (refactoring only):
	/// https://notesatprograming.blogspot.com.by/2011/10/public-enum-textcase-nominative.html
	/// </summary>
	public static class RuDateMoneyConverter
	{
		private static readonly string[] MonthNamesGenitive =
		{
			"",
			"января",
			"февраля",
			"марта",
			"апреля",
			"мая",
			"июня",
			"июля",
			"августа",
			"сентября",
			"октября",
			"ноября",
			"декабря"
		};

		private const string Zero = "ноль";
		private const string FirstMale = "один";
		private const string FirstFemale = "одна";
		private const string FirstFemaleAccusative = "одну";
		private const string FirstMaleGenetive = "одно";
		private const string SecondMale = "два";
		private const string SecondFemale = "две";
		private const string SecondMaleGenetive = "двух";
		private const string SecondFemaleGenetive = "двух";

		private static readonly string[] From3Till19 =
		{
			"", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять",
			"одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать",
			"семнадцать", "восемнадцать", "девятнадцать"
		};

		private static readonly string[] From3Till19Genetive =
		{
			"", "трех", "четырех", "пяти", "шести", "семи", "восеми", "девяти", "десяти",
			"одиннадцати", "двенадцати", "тринадцати", "четырнадцати", "пятнадцати", "шестнадцати",
			"семнадцати", "восемнадцати", "девятнадцати"
		};

		private static readonly string[] Tens =
		{
			"", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят",
			"восемьдесят", "девяносто"
		};

		private static readonly string[] TensGenetive =
		{
			"", "двадцати", "тридцати", "сорока", "пятидесяти", "шестидесяти", "семидесяти",
			"восьмидесяти", "девяноста"
		};

		private static readonly string[] Hundreds =
		{
			"", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот",
			"восемьсот", "девятьсот"
		};

		private static readonly string[] HundredsGenetive =
			{
			"", "ста", "двухсот", "трехсот", "четырехсот", "пятисот", "шестисот", "семисот",
			"восемисот", "девятисот"
		};

		private static readonly string[] Thousands =
		{
			"", "тысяча", "тысячи", "тысяч"
		};

		private static readonly string[] ThousandsAccusative =
		{
			"", "тысячу", "тысячи", "тысяч"
		};

		private static readonly string[] Millions =
		{
			"", "миллион", "миллиона", "миллионов"
		};

		private static readonly string[] Billions =
		{
			"", "миллиард", "миллиарда", "миллиардов"
		};

		private static readonly string[] Trillions =
		{
			"", "трилион", "трилиона", "триллионов"
		};

		private static readonly string[] Rubles =
		{
			"", "рубль", "рубля", "рублей"
		};

		private static readonly string[] Kopecks =
		{
			"", "копейка", "копейки", "копеек"
		};

		/// <summary>
		/// «07» января 2004
		/// </summary>
		public static string DateToTextLong(DateTime date)
		{
			return string.Format("«{0:D2}» {1} {2}", date.Day,
				MonthName(date.Month, TextCase.Genitive), date.Year.ToString());
		}

		/// <summary>
		/// II квартал 2004
		/// </summary>
		public static string DateToTextQuarter(DateTime date)
		{
			return NumeralsRoman(DateQuarter(date)) + " квартал " + date.Year;
		}
		
		public static int DateQuarter(DateTime date)
		{
			return (date.Month - 1) / 3 + 1;
		}

		private static bool IsPluralGenitive(int digits)
		{
			return digits >= 5 || digits == 0;
		}

		private static bool IsSingularGenitive(int digits)
		{
			return digits >= 2 && digits <= 4;
		}

		private static int LastDigit(long amount)
		{
			if (amount >= 100)
			{
				amount = amount % 100;
			}
			if (amount >= 20)
			{
				amount = amount % 10;
			}
			return (int)amount;
		}

		/// <summary>
		/// Десять тысяч рублей 67 копеек
		/// </summary>
		public static string CurrencyToTxt(double amount, bool firstCapital)
		{
			var rublesAmount = (long)Math.Floor(amount);
			var kopecksAmount = (long)Math.Round(amount * 100) % 100;
			var lastRublesDigit = LastDigit(rublesAmount);
			var lastKopecksDigit = LastDigit(kopecksAmount);
			var s = NumeralsToTxt(rublesAmount, TextCase.Nominative, true, firstCapital) + " ";
			if (IsPluralGenitive(lastRublesDigit))
			{
				s += Rubles[3] + " ";
			}
			else if (IsSingularGenitive(lastRublesDigit))
			{
				s += Rubles[2] + " ";
			}
			else
			{
				s += Rubles[1] + " ";
			}
			s += string.Format("{0:00} ", kopecksAmount);
			if (IsPluralGenitive(lastKopecksDigit))
			{
				s += Kopecks[3] + " ";
			}
			else if (IsSingularGenitive(lastKopecksDigit))
			{
				s += Kopecks[2] + " ";
			}
			else
			{
				s += Kopecks[1] + " ";
			}
			return s.Trim();
		}

		/// <summary>
		/// 10 000 (Десять тысяч) рублей 67 копеек
		/// </summary>
		public static string CurrencyToTxtFull(double amount, bool firstCapital)
		{
			var rublesAmount = (long)Math.Floor(amount);
			var kopecksAmount = (long)Math.Round(amount * 100) % 100;
			var lastRublesDigit = LastDigit(rublesAmount);
			var lastKopecksDigit = LastDigit(kopecksAmount);
			var s = string.Format("{0:N0} ({1}) ", rublesAmount, 
				NumeralsToTxt(rublesAmount, TextCase.Nominative, true, firstCapital));
			if (IsPluralGenitive(lastRublesDigit))
			{
				s += Rubles[3] + " ";
			}
			else if (IsSingularGenitive(lastRublesDigit))
			{
				s += Rubles[2] + " ";
			}
			else
			{
				s += Rubles[1] + " ";
			}
			s += string.Format("{0:00} ", kopecksAmount);
			if (IsPluralGenitive(lastKopecksDigit))
			{
				s += Kopecks[3] + " ";
			}
			else if (IsSingularGenitive(lastKopecksDigit))
			{
				s += Kopecks[2] + " ";
			}
			else
			{
				s += Kopecks[1] + " ";
			}
			return s.Trim();
		}

		/// <summary>
		/// 10 000 рублей 67 копеек  
		/// </summary>
		public static string CurrencyToTxtShort(double amount, bool firstCapital)
		{
			var rublesAmount = (long)Math.Floor(amount);
			var kopecksAmount = (long)Math.Round(amount * 100) % 100;
			var lastRublesDigit = LastDigit(rublesAmount);
			var lastKopecksDigit = LastDigit(kopecksAmount);
			var s = string.Format("{0:N0} ", rublesAmount);
			if (IsPluralGenitive(lastRublesDigit))
			{
				s += Rubles[3] + " ";
			}
			else if (IsSingularGenitive(lastRublesDigit))
			{
				s += Rubles[2] + " ";
			}
			else
			{
				s += Rubles[1] + " ";
			}
			s += string.Format("{0:00} ", kopecksAmount);
			if (IsPluralGenitive(lastKopecksDigit))
			{
				s += Kopecks[3] + " ";
			}
			else if (IsSingularGenitive(lastKopecksDigit))
			{
				s += Kopecks[2] + " ";
			}
			else
			{
				s += Kopecks[1] + " ";
			}
			return s.Trim();
		}

		private static string MakeText(int digits, IList<string> hundreds, IList<string> tens, 
			IList<string> from3Till19, string second, string first, IList<string> power)
		{
			var s = "";
			var bufferDigits = digits;
			if (bufferDigits >= 100)
			{
				s += hundreds[bufferDigits / 100] + " ";
				bufferDigits = bufferDigits % 100;
			}
			if (bufferDigits >= 20)
			{
				s += tens[bufferDigits / 10 - 1] + " ";
				bufferDigits = bufferDigits % 10;
			}
			if (bufferDigits >= 3)
			{
				s += from3Till19[bufferDigits - 2] + " ";
			}
			else if (bufferDigits == 2)
			{
				s += second + " ";
			}
			else if (bufferDigits == 1)
			{
				s += first + " ";
			}
			if (digits == 0 || power.Count <= 0)
			{
				return s;
			}
			bufferDigits = LastDigit(digits);
			if (IsPluralGenitive(bufferDigits))
			{
				s += power[3] + " ";
			}
			else if (IsSingularGenitive(bufferDigits))
			{
				s += power[2] + " ";
			}
			else
			{
				s += power[1] + " ";
			}
			return s;
		}

		/// <summary>
		/// реализовано для падежей:
		/// именительный (nominative), родительный (Genitive),  винительный (accusative)
		/// </summary>
		public static string NumeralsToTxt(long sourceNumber, TextCase _case, bool isMale, 
			bool firstCapital)
		{
			var s = "";
			var number = sourceNumber;
			var power = 0;
			if (number >= (long)Math.Pow(10, 15) || number < 0)
			{
				return s;
			}
			while (number > 0)
			{
				var remainder = (int)(number % 1000);
				number = number / 1000;
				switch (power)
				{
					case 12:
						s = MakeText(remainder, Hundreds, Tens, From3Till19, SecondMale, FirstMale, 
							    Trillions) + s;
						break;
					case 9:
						s = MakeText(remainder, Hundreds, Tens, From3Till19, SecondMale, FirstMale, 
							    Billions) + s;
						break;
					case 6:
						s = MakeText(remainder, Hundreds, Tens, From3Till19, SecondMale, FirstMale, 
							    Millions) + s;
						break;
					case 3:
						switch (_case)
						{
							case TextCase.Accusative:
								s = MakeText(remainder, Hundreds, Tens, From3Till19, SecondFemale, 
									FirstFemaleAccusative, ThousandsAccusative) + s;
								break;
							default:
								s = MakeText(remainder, Hundreds, Tens, From3Till19, SecondFemale, 
								    FirstFemale, Thousands) + s;
								break;
						}
						break;
					default:
						string[] powerArray = { };
						switch (_case)
						{
							case TextCase.Genitive:
								s = MakeText(remainder, HundredsGenetive, TensGenetive, 
								    From3Till19Genetive, 
								    isMale ? SecondMaleGenetive : SecondFemaleGenetive, 
								    isMale ? FirstMaleGenetive : FirstFemale, powerArray) + s;
								break;
							case TextCase.Accusative:
								s = MakeText(remainder, Hundreds, Tens, From3Till19, 
									isMale ? SecondMale : SecondFemale, 
									isMale ? FirstMale : FirstFemaleAccusative, powerArray) + s;
								break;
							default:
								s = MakeText(remainder, Hundreds, Tens, From3Till19, 
									isMale ? SecondMale : SecondFemale, 
									isMale ? FirstMale : FirstFemale, powerArray) + s;
								break;
						}
						break;
				}
				power += 3;
			}
			if (sourceNumber == 0)
			{
				s = Zero + " ";
			}
			if (s != "" && firstCapital)
			{
				s = s.Substring(0, 1).ToUpper() + s.Substring(1);
			}
			return s.Trim();
		}

		public static string NumeralsDoubleToTxt(double sourceNumber, int _decimal, TextCase _case, 
			bool firstCapital)
		{
			var decNum = (long)Math.Round(sourceNumber * Math.Pow(10, _decimal)) 
			    % (long)Math.Pow(10, _decimal);
			var s = string.Format(" {0} целых {1} сотых", 
				NumeralsToTxt((long)sourceNumber, _case, true, firstCapital), 
				NumeralsToTxt(decNum, _case, true, false));
			return s.Trim();
		}

		/// <summary>
		/// название месяца с единицы
		/// </summary>
		public static string MonthName(int month, TextCase _case)
		{
			var s = "";
			if (month <= 0 || month > 12)
			{
				return s;
			}
			switch (_case)
			{
				case TextCase.Genitive:
					s = MonthNamesGenitive[month];
					break;
			}
			return s;
		}

		public static string NumeralsRoman(int number)
		{
			var s = "";
			switch (number)
			{
				case 1:
					s = "I";
					break;
				case 2:
					s = "II";
					break;
				case 3:
					s = "III";
					break;
				case 4:
					s = "IV";
					break;
			}
			return s;
		}
	}

	// ReSharper restore IdentifierTypo
	// ReSharper restore CommentTypo
	// ReSharper restore StringLiteralTypo
}