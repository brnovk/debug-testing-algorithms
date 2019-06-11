using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;

using DebugConvertRusMoneyFromDigitToString.Util;
using DebugConvertRusMoneyFromDigitToString.Logic;

namespace DebugConvertRusMoneyFromDigitToString.View.Windows
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			InitializeComboBoxes();
			InitializeStartedValues();
		}

		private void AdditionalInitializeComponent()
		{
			Title = Common.GetApplicationTitle(Assembly.GetExecutingAssembly());
		}

		private void VisualInitializeComponent()
		{
			Background = Constants.BackColor2_Botticelli;
			Foreground = Constants.ForeColor2_PapayaWhip;
			FontSize = Constants.FontSize;
		}

		private void InitializeComboBoxes()
		{
			var comboBoxItems = new List<ComboBoxItem>();
			foreach (var textCase in Enum.GetValues(typeof(TextCase)).Cast<TextCase>().ToList())
			{

				var comboBoxItem = new ComboBoxItem
				{
					Content = textCase,
					IsEnabled = textCase == TextCase.Nominative || textCase == TextCase.Genitive || 
					            textCase == TextCase.Accusative
				};
				comboBoxItems.Add(comboBoxItem);
			}
			Val1CasesComboBox.ItemsSource = comboBoxItems;
			Val1CasesComboBox.SelectedIndex = 0;

			Val2CasesComboBox.ItemsSource = comboBoxItems;
			Val2CasesComboBox.SelectedIndex = 0;
		}

		private void InitializeStartedValues()
		{
			Val1LongUpDown.Value = RandomLong();
			Val2DoubleUpDown.Value = RandomDouble();
			Val3DoubleUpDown.Value = RandomDouble();
			Val4DoubleUpDown.Value = RandomDouble();
			Val5DoubleUpDown.Value = RandomDouble();
			Val6DatePicker.SelectedDate = DateTime.Today;
			Val7DatePicker.SelectedDate = DateTime.Today;
		}

		private static long RandomLong()
		{
			var random = new Random();
			const long min = 0L;
			const ulong uRange = 999999999999999UL;
			ulong ulongRand;
			do
			{
				var buf = new byte[8];
				random.NextBytes(buf);
				ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
			} while (ulongRand > ulong.MaxValue - (ulong.MaxValue % uRange + 1) % uRange);
			return (long)(ulongRand % uRange) + min;
		}

		private static double RandomDouble()
		{
			const double minimum = 0.0;
			const double maximum = 99999999999.999;
			return new Random().NextDouble() * (maximum - minimum) + minimum;
		}

		private static int GetPrecision(double number)
		{
			var precision = 0;
			while (Math.Abs(number * Math.Pow(10, precision) -
			    Math.Round(number * Math.Pow(10, precision))) > 0.000000000000000000001)
			{
				precision++;
			}
			return precision;
		}

		private void Val1LongUpDown_OnValueChanged(object senderIsLongUpDown, 
			RoutedPropertyChangedEventArgs<object> eventArgs)
		{
			RecalculateVal1();
		}

		private void Val1CasesComboBox_OnSelectionChanged(object senderIsComboBox, 
			SelectionChangedEventArgs eventArgs)
		{
			RecalculateVal1();
		}
		
		private void Val1CheckBox_OnCheckedChanged(object senderIsCheckBox, 
			RoutedEventArgs eventArgs)
		{
			RecalculateVal1();
		}

		private void RecalculateVal1()
		{
			if (Val1LongUpDown == null || Val1IsMaleCheckBox == null || 
			    Val1IsFirstCapitalCheckBox == null || Val1CasesComboBox == null || 
			    Val1LongUpDown.Value == null)
			{
				return;
			}
			var comboBoxItem = Val1CasesComboBox.SelectedItem as ComboBoxItem;
			if (comboBoxItem == null)
			{
				Result1TextBox.Text = string.Empty;
				return;
			}
			var nullableTextCase = comboBoxItem.Content as TextCase?;
			if (nullableTextCase == null || Val1IsFirstCapitalCheckBox.IsChecked == null ||
			    Val1IsMaleCheckBox.IsChecked == null)
			{
				Result1TextBox.Text = string.Empty;
				return;
			}
			var value = (long)Val1LongUpDown.Value;
			var textCase = (TextCase)nullableTextCase;
			var isMale = (bool)Val1IsMaleCheckBox.IsChecked;
			var isFirstCapital = (bool)Val1IsFirstCapitalCheckBox.IsChecked;
			Result1TextBox.Text = 
				RuDateMoneyConverter.NumeralsToTxt(value, textCase, isMale, isFirstCapital);
		}

		private void Val2DoubleUpDown_OnValueChanged(object senderIsDoubleUpDown, 
			RoutedPropertyChangedEventArgs<object> eventArgs)
		{
			RecalculateVal2();
		}

		private void Val2CasesComboBox_OnSelectionChanged(object senderIsComboBox, 
			SelectionChangedEventArgs eventArgs)
		{
			RecalculateVal2();
		}

		private void Val2CheckBox_OnCheckedChanged(object senderIsCheckBox,
			RoutedEventArgs eventArgs)
		{
			RecalculateVal2();
		}

		private void RecalculateVal2()
		{
			if (Val2DoubleUpDown == null || 
			    Val2IsFirstCapitalCheckBox == null || Val2CasesComboBox == null ||
			    Val2DoubleUpDown.Value == null)
			{
				return;
			}
			var comboBoxItem = Val2CasesComboBox.SelectedItem as ComboBoxItem;
			if (comboBoxItem == null)
			{
				Result2TextBox.Text = string.Empty;
				return;
			}
			var nullableTextCase = comboBoxItem.Content as TextCase?;
			if (nullableTextCase == null || Val2IsFirstCapitalCheckBox.IsChecked == null)
			{
				Result2TextBox.Text = string.Empty;
				return;
			}
			var value = (double)Val2DoubleUpDown.Value;
			var precision = GetPrecision(value);
			var textCase = (TextCase)nullableTextCase;
			var isFirstCapital = (bool)Val2IsFirstCapitalCheckBox.IsChecked;
			Result2TextBox.Text = 
				RuDateMoneyConverter.NumeralsDoubleToTxt(value, precision, textCase, isFirstCapital);
		}

		private void Val3DoubleUpDown_OnValueChanged(object senderIsDoubleUpDown,
			RoutedPropertyChangedEventArgs<object> eventArgs)
		{
			RecalculateVal3();
		}

		private void Val3CheckBox_OnCheckedChanged(object senderIsCheckBox,
			RoutedEventArgs eventArgs)
		{
			RecalculateVal3();
		}

		private void RecalculateVal3()
		{
			if (Val3DoubleUpDown == null || Val3IsFirstCapitalCheckBox == null || 
			    Val3DoubleUpDown.Value == null || Val3IsFirstCapitalCheckBox.IsChecked == null)
			{
				return;
			}
			var value = (double)Val3DoubleUpDown.Value;
			var isFirstCapital = (bool)Val3IsFirstCapitalCheckBox.IsChecked;
			Result3TextBox.Text = RuDateMoneyConverter.CurrencyToTxt(value, isFirstCapital);
		}

		private void Val4DoubleUpDown_OnValueChanged(object senderIsDoubleUpDown,
			RoutedPropertyChangedEventArgs<object> eventArgs)
		{
			RecalculateVal4();
		}

		private void Val4CheckBox_OnCheckedChanged(object senderIsCheckBox,
			RoutedEventArgs eventArgs)
		{
			RecalculateVal4();
		}

		private void RecalculateVal4()
		{
			if (Val4DoubleUpDown == null || Val4IsFirstCapitalCheckBox == null ||
			    Val4DoubleUpDown.Value == null || Val4IsFirstCapitalCheckBox.IsChecked == null)
			{
				return;
			}
			var value = (double)Val4DoubleUpDown.Value;
			var isFirstCapital = (bool)Val4IsFirstCapitalCheckBox.IsChecked;
			Result4TextBox.Text = RuDateMoneyConverter.CurrencyToTxtFull(value, isFirstCapital);
		}

		private void Val5DoubleUpDown_OnValueChanged(object senderIsDoubleUpDown,
			RoutedPropertyChangedEventArgs<object> eventArgs)
		{
			RecalculateVal5();
		}

		private void Val5CheckBox_OnCheckedChanged(object senderIsCheckBox,
			RoutedEventArgs eventArgs)
		{
			RecalculateVal5();
		}

		private void RecalculateVal5()
		{
			if (Val5DoubleUpDown == null || Val5IsFirstCapitalCheckBox == null ||
			    Val5DoubleUpDown.Value == null || Val5IsFirstCapitalCheckBox.IsChecked == null)
			{
				return;
			}
			var value = (double)Val5DoubleUpDown.Value;
			var isFirstCapital = (bool)Val5IsFirstCapitalCheckBox.IsChecked;
			Result5TextBox.Text = RuDateMoneyConverter.CurrencyToTxtShort(value, isFirstCapital);
		}

		private void Val6DatePicker_OnSelectedDateChanged(object senderIsDatePicker, 
			SelectionChangedEventArgs eventArgs)
		{
			var nullableDateTime = Val6DatePicker.SelectedDate;
			if (nullableDateTime == null)
			{
				Result6TextBox.Text = string.Empty;
				return;
			}
			Result6TextBox.Text = RuDateMoneyConverter.DateToTextLong(nullableDateTime.Value);
		}

		private void Val7DatePicker_OnSelectedDateChanged(object senderIsDatePicker, 
			SelectionChangedEventArgs eventArgs)
		{
			var nullableDateTime = Val7DatePicker.SelectedDate;
			if (nullableDateTime == null)
			{
				Result7TextBox.Text = string.Empty;
				return;
			}
			Result7TextBox.Text = RuDateMoneyConverter.DateToTextQuarter(nullableDateTime.Value);
		}
	}
}
