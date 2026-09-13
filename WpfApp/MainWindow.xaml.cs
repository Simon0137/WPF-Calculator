using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Calculator.Core;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public uint? CurrentAge { get; private set; }
        public uint? TargetAge { get; private set; }
        public double? StartCapital { get; private set; }
        public double? MonthlyPay { get; private set; }
        public double? YearProfit { get; private set; }

        private static readonly Color ForegroundColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
        private static readonly Color DefaultForegroundColor = Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB);
        private static readonly Color ErrorColor = Color.FromArgb(0xFF, 0xD8, 0x34, 0x34);
        private static readonly Color BorderColor = Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3);

        private bool _isCurrentAgeBoxEmpty = true;
        private bool _isTargetAgeBoxEmpty = true;
        private bool _isStartCapitalBoxEmpty = true;
        private bool _isMonthlyPayBoxEmpty = true;
        private bool _isYearProfitBoxEmpty = true;

        public MainWindow()
        {
            InitializeComponent();

            SetDefault(CurrentAgeBox, "0");
            SetDefault(TargetAgeBox, "0");
            SetDefault(StartCapitalBox, "0 €");
            SetDefault(MonthlyPayBox, "0 €");
            SetDefault(YearProfitBox, "0 %");

            CurrentAgeErrorLabel.Visibility = Visibility.Hidden;
            TargetAgeErrorLabel.Visibility = Visibility.Hidden;
            StartCapitalErrorLabel.Visibility = Visibility.Hidden;
            MonthlyPayErrorLabel.Visibility = Visibility.Hidden;
            YearProfitErrorLabel.Visibility = Visibility.Hidden;

            ResultAgeLabel.Visibility = Visibility.Hidden;
            ResultAgeBlock.Visibility = Visibility.Hidden;

            OwnAmountLabel.Visibility = Visibility.Hidden;
            OwnAmountBlock.Visibility = Visibility.Hidden;

            InvestmentsLabel.Visibility = Visibility.Hidden;
            InvestmentsBlock.Visibility = Visibility.Hidden;

            TotalCapitalLabel.Visibility = Visibility.Hidden;
            TotalCapitalBlock.Visibility = Visibility.Hidden;
        }

        private void CurrentAgeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentAgeBox.Text))
            {
                SetDefault(CurrentAgeBox, "0");
                _isCurrentAgeBoxEmpty = true;
            }
            else
            {
                _isCurrentAgeBoxEmpty = false;
                CurrentAge = uint.Parse(CurrentAgeBox.Text);
            }
        }

        private void TargetAgeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TargetAgeBox.Text))
            {
                SetDefault(TargetAgeBox, "0");
                _isTargetAgeBoxEmpty = true;
            }
            else
            {
                _isTargetAgeBoxEmpty = false;
                var targetAge = uint.Parse(TargetAgeBox.Text);
                if (targetAge > CurrentAge) TargetAge = uint.Parse(TargetAgeBox.Text);
                else SetError(TargetAgeBox, TargetAgeErrorLabel, "Целевой возраст должен превышать ваш текущий");
            }
        }

        private void StartCapitalBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StartCapitalBox.Text))
            {
                SetDefault(StartCapitalBox, "0 €");
                _isStartCapitalBoxEmpty = true;
            }
            else
            {
                _isStartCapitalBoxEmpty = false;
                StartCapital = double.Parse(StartCapitalBox.Text);
                StartCapitalBox.Text += " €";
            }
        }

        private void MonthlyPayBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MonthlyPayBox.Text))
            {
                SetDefault(MonthlyPayBox, "0 €");
                _isMonthlyPayBoxEmpty = true;
            }
            else
            {
                _isMonthlyPayBoxEmpty = false;
                MonthlyPay = double.Parse(MonthlyPayBox.Text);
                MonthlyPayBox.Text += " €";
            }
        }

        private void YearProfitBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(YearProfitBox.Text))
            {
                SetDefault(YearProfitBox, "0 %");
                _isYearProfitBoxEmpty = true;
            }
            else
            {
                _isYearProfitBoxEmpty = false;
                YearProfit = double.Parse(YearProfitBox.Text);
                YearProfitBox.Text += " %";
            }
        }

        private static void SetError(TextBox textBox, Label errorLabel, string errorMessage)
        {
            textBox.BorderBrush = new SolidColorBrush(ErrorColor);
            errorLabel.Content = errorMessage;
            errorLabel.Foreground = new SolidColorBrush(ErrorColor);
            errorLabel.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var hasNull = false;

            if (CurrentAge == null)
            {
                hasNull = true;
                SetError(CurrentAgeBox, CurrentAgeErrorLabel, "Это обязательное поле");
            }
            if (TargetAge == null)
            {
                hasNull = true;
                SetError(TargetAgeBox, TargetAgeErrorLabel, "Это обязательное поле");
            }
            if (StartCapital == null)
            {
                hasNull = true;
                SetError(StartCapitalBox, StartCapitalErrorLabel, "Это обязательное поле");
            }
            if (MonthlyPay == null)
            {
                hasNull = true;
                SetError(MonthlyPayBox, MonthlyPayErrorLabel, "Это обязательное поле");
            }
            if (YearProfit == null)
            {
                hasNull = true;
                SetError(YearProfitBox, YearProfitErrorLabel, "Это обязательное поле");
            }

            if (hasNull) return;

            var ageDelta = PensionCalculator.CalcAgeDelta((uint)CurrentAge!, (uint)TargetAge!);
            var ownAmount = PensionCalculator.CalcOwnAmount((double)StartCapital!, ageDelta ?? 0, (double)MonthlyPay!);
            var totalAmount = PensionCalculator.CalcTotalAmount((double)StartCapital, ageDelta ?? 0, (double)MonthlyPay, (double)YearProfit!);
            var investmentsAmount = totalAmount - ownAmount;

            ChangeOutput(ResultAgeLabel, ResultAgeBlock, Visibility.Visible, ageDelta.ToString() ?? "");
            ChangeOutput(OwnAmountLabel, OwnAmountBlock, Visibility.Visible,$"{Math.Round(ownAmount, 2).ToString(CultureInfo.InvariantCulture)} €");
            ChangeOutput(InvestmentsLabel, InvestmentsBlock, Visibility.Visible, $"{Math.Round(investmentsAmount, 2).ToString(CultureInfo.InvariantCulture)} €");
            ChangeOutput(TotalCapitalLabel, TotalCapitalBlock, Visibility.Visible, $"{Math.Round(totalAmount, 2).ToString(CultureInfo.InvariantCulture)} €");
        }

        private static void ChangeOutput(Label label, TextBlock textBlock, Visibility visibility, string blockText = "")
        {
            label.Visibility = visibility;
            textBlock.Text = blockText;
            textBlock.Visibility = visibility;
        }

        private static bool ValidateUint(string input, ref string currentNumber)
        {
            if (!byte.TryParse(input, out var digit)) return false;
            if (digit == 0 && string.IsNullOrEmpty(currentNumber)) return false;

            currentNumber += input;
            return true;

        }

        private static bool ValidateDouble(string input, ref string currentNumber, uint maxDigitsBefore = 6)
        {
            if (input[0] is ',' && !currentNumber.Contains(','))
            {
                currentNumber += input;
                return true;
            }

            if (!byte.TryParse(input, out _)) return false;
            if ((!currentNumber.Contains(',') || currentNumber[(currentNumber.IndexOf(',') + 1)..].Length >= 2)
                && (currentNumber.Contains(',') || currentNumber.Length >= maxDigitsBefore)) return false;

            currentNumber += input;
            return true;

        }

        private static void SetDefault(TextBox textBox, string defaultText)
        {
            textBox.Text = defaultText;
            textBox.Foreground = new SolidColorBrush(DefaultForegroundColor);
        }

        private void CurrentAgeBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CurrentAgeBox.Foreground = new SolidColorBrush(ForegroundColor);
            if (_isCurrentAgeBoxEmpty) CurrentAgeBox.Text = string.Empty;
        }

        private void TargetAgeBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TargetAgeBox.Foreground = new SolidColorBrush(ForegroundColor);
            if (_isTargetAgeBoxEmpty) TargetAgeBox.Text = string.Empty;
        }

        private void StartCapitalBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StartCapitalBox.Foreground = new SolidColorBrush(ForegroundColor);
            StartCapitalBox.Text = _isStartCapitalBoxEmpty ? string.Empty : StartCapitalBox.Text[..^2];
        }

        private void MonthlyPayBox_GotFocus(object sender, RoutedEventArgs e)
        {
            MonthlyPayBox.Foreground = new SolidColorBrush(ForegroundColor);
            MonthlyPayBox.Text = _isMonthlyPayBoxEmpty ? string.Empty : MonthlyPayBox.Text[..^2];
        }

        private void YearProfitBox_GotFocus(object sender, RoutedEventArgs e)
        {
            YearProfitBox.Foreground = new SolidColorBrush(ForegroundColor);
            YearProfitBox.Text = _isYearProfitBoxEmpty ? string.Empty : YearProfitBox.Text[..^2];
        }

        private void CurrentAgeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var boxText = CurrentAgeBox.Text;

            if (ValidateUint(e.Text, ref boxText) && boxText.Length < 3)
            {
                CurrentAgeBox.Text = boxText;
                CurrentAgeBox.CaretIndex = CurrentAgeBox.Text.Length;
            }

            e.Handled = true;
        }

        private void TargetAgeBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var boxText = TargetAgeBox.Text;

            if (ValidateUint(e.Text, ref boxText) && boxText.Length < 3)
            {
                TargetAgeBox.Text = boxText;
                TargetAgeBox.CaretIndex = TargetAgeBox.Text.Length;
            }

            e.Handled = true;
        }

        private void StartCapitalBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var boxText = StartCapitalBox.Text;

            var input = e.Text.Replace('.', ',');
            var isValid = ValidateDouble(input, ref boxText);
            if (isValid)
            {
                if (input[0] is not ',')
                {
                    e.Handled = !isValid;
                    return;
                }

                StartCapitalBox.Text = boxText;
                StartCapitalBox.CaretIndex = StartCapitalBox.Text.Length;
            }
            e.Handled = true;
        }

        private void MonthlyPayBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var boxText = MonthlyPayBox.Text;

            var input = e.Text.Replace('.', ',');
            var isValid = ValidateDouble(input, ref boxText);
            if (isValid)
            {
                if (input[0] is not ',')
                {
                    e.Handled = !isValid;
                    return;
                }

                MonthlyPayBox.Text = boxText;
                MonthlyPayBox.CaretIndex = MonthlyPayBox.Text.Length;
            }
            e.Handled = true;
        }

        private void YearProfitBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var boxText = YearProfitBox.Text;

            var input = e.Text.Replace('.', ',');
            var isValid = ValidateDouble(input, ref boxText, 2);
            if (isValid)
            {
                if (input[0] is not ',')
                {
                    e.Handled = !isValid;
                    return;
                }

                YearProfitBox.Text = boxText;
                YearProfitBox.CaretIndex = YearProfitBox.Text.Length;
            }
            e.Handled = true;
        }

        private static void ResetError(TextBox textBox, Label errorLabel)
        {
            textBox.BorderBrush = new SolidColorBrush(BorderColor);
            errorLabel.Visibility = Visibility.Hidden;
            errorLabel.Foreground = new SolidColorBrush(ForegroundColor);
        }

        private void CurrentAgeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!CurrentAgeErrorLabel.IsVisible) return;
            ResetError(CurrentAgeBox, CurrentAgeErrorLabel);
        }

        private void TargetAgeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!TargetAgeErrorLabel.IsVisible) return;
            ResetError(TargetAgeBox, TargetAgeErrorLabel);
        }

        private void StartCapitalBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!StartCapitalErrorLabel.IsVisible) return;
            ResetError(StartCapitalBox, StartCapitalErrorLabel);
        }

        private void MonthlyPayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!MonthlyPayErrorLabel.IsVisible) return;
            ResetError(MonthlyPayBox, MonthlyPayErrorLabel);
        }

        private void YearProfitBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!YearProfitErrorLabel.IsVisible) return;
            ResetError(YearProfitBox, YearProfitErrorLabel);
        }
    }
}