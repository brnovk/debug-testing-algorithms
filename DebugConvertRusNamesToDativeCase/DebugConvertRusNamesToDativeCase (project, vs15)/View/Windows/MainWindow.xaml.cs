using System.Windows;
using System.Reflection;
using System.Windows.Controls;

using DebugConvertRusNamesToDativeCase.Util;
using DebugConvertRusNamesToDativeCase.Logic;
using DebugConvertRusNamesToDativeCase.View.Util;

namespace DebugConvertRusNamesToDativeCase.View.Windows
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
			AdditionalInitializeComponent();
			VisualInitializeComponent();
			FillingLastNames();
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

		private void FillingLastNames()
		{
			var lastUsedLastName = Properties.Settings.Default.LastUsedLastName;
			var lastUsedFirstName = Properties.Settings.Default.LastUsedFirstName;
			var lastUsedPatronymic = Properties.Settings.Default.LastUsedPatronymic;
			var nullableLastUsedSex =
				SexUtil.GetFromString(Properties.Settings.Default.LastUsedSex);
			if (string.IsNullOrWhiteSpace(lastUsedLastName) ||
				string.IsNullOrWhiteSpace(lastUsedFirstName) ||
				string.IsNullOrWhiteSpace(lastUsedPatronymic) ||
				nullableLastUsedSex == null)
			{
				return;
			}
			var sex = (Sex)nullableLastUsedSex;
			LastNameTextBox.Text = lastUsedLastName;
			FirstNameTextBox.Text = lastUsedFirstName;
			PatronymicTextBox.Text = lastUsedPatronymic;
			if (sex == Sex.Male)
			{
				MaleRadioButton.IsChecked = true;
			}
			else
			{
				FemaleRadioButton.IsChecked = true;
			}
		}

		private void TextBox_OnTextChanged(object senderIsTextBox, TextChangedEventArgs eventArgs)
		{
			ResultTextBox.Clear();
		}

		private void RadioButton_OnChecked(object senderIsRadioButton, RoutedEventArgs eventArgs)
		{
			ResultTextBox.Clear();
		}

		private void ConvertButton_OnClick(object senderIsButton, RoutedEventArgs eventArgs)
		{
			var lastName = LastNameTextBox.Text;
			var firstName = FirstNameTextBox.Text;
			var patronymic = PatronymicTextBox.Text;
			var nullableSex = MaleRadioButton.IsChecked == true ? Sex.Male
				: FemaleRadioButton.IsChecked == true ? Sex.Female : (Sex?)null;
			if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) ||
				string.IsNullOrWhiteSpace(patronymic))
			{
				MessageBox.Show(PageLiterals.ValidationFieldsNotSpecified, 
					PageLiterals.HeaderValidation, MessageBoxButton.OK, 
					MessageBoxImage.Exclamation, MessageBoxResult.OK);
				ResultTextBox.Clear();
				return;
			}
			if (nullableSex == null)
			{
				MessageBox.Show(PageLiterals.ValidationGenderNotSpecified, 
					PageLiterals.HeaderValidation, MessageBoxButton.OK, 
					MessageBoxImage.Exclamation, MessageBoxResult.OK);
				ResultTextBox.Clear();
				return;
			}
			var sex = (Sex)nullableSex;
			string warnings;
			ResultTextBox.Text = NamesConverter.FullnameInDativeCase(lastName, firstName,
				patronymic, sex, out warnings);

			Properties.Settings.Default.LastUsedLastName = lastName.Trim();
			Properties.Settings.Default.LastUsedFirstName = firstName.Trim();
			Properties.Settings.Default.LastUsedPatronymic = patronymic.Trim();
			Properties.Settings.Default.LastUsedSex = sex.ToString();
			Properties.Settings.Default.Save();

			if (!string.IsNullOrWhiteSpace(warnings))
			{
				MessageBox.Show(warnings, PageLiterals.HeaderInformationOrWarning,
					MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
			}
		}
	}
}
