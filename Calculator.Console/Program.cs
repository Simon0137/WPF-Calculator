using System.Globalization;
using System.Numerics;
using Calculator.Core;

namespace Calculator.ConsoleApp;

internal class Program
{
    private static void Main(string[] args)
    {
        var currentAge = EnterField<uint>("Введите свой текущий возраст: ", "Введите свой возраст в виде положительного числа: ");
        var targetAge = EnterField<uint>("Введите целевой возраст: ", "Введите целевой возраст в виде положительного числа: ");
        var ageDelta = PensionCalculator.CalcAgeDelta(currentAge, targetAge);
        while (ageDelta == null)
        {
            targetAge = EnterField<uint>("Целевой возраст должен быть больше текущего: ", "Введите целевой возраст в виде положительного числа: ");
            ageDelta = PensionCalculator.CalcAgeDelta(currentAge, targetAge);
        }

        var startCapital = EnterField<double>("Введите свои текущие сбережения: ", "Введите свои сбережения в виде положительного числа: ");
        var monthlyPayment = EnterField<double>("Введите свой ежемесячный взнос: ", "Ваш ежемесячный взнос должен быть положительным числом: ");

        var yearlyReturn = EnterField<double>("Введите ожидаемую годовую доходность (в процентах): ", "Годовая доходность должна быть положительным числом: ");
        Console.WriteLine();

        
        Console.WriteLine($"До пенсии: {ageDelta} лет");
        Console.WriteLine();

        var ownAmount = PensionCalculator.CalcOwnAmount(startCapital, (uint)ageDelta, monthlyPayment);
        Console.WriteLine($"Вы внесёте: {ownAmount} €");

        var totalAmount = PensionCalculator.CalcTotalAmount(startCapital, (uint)ageDelta, monthlyPayment, yearlyReturn);
        totalAmount = Math.Round(totalAmount, 2);
        Console.WriteLine($"Доход от инвестиций: {totalAmount - ownAmount} €");

        Console.WriteLine();
        Console.WriteLine($"Итоговый капитал: {totalAmount} €");
    }

    private static T EnterField<T>(string fieldLabel, string errorLabel = "") where T : IParsable<T>, INumber<T>
    {
        Console.Write(fieldLabel);

        var fieldValue = Console.ReadLine();
        T? result;
        while (fieldValue == null || !T.TryParse(fieldValue, CultureInfo.InvariantCulture, out result) || result == T.Zero)
        {
            Console.Write(errorLabel);
            fieldValue = Console.ReadLine();
        }

        return result;
    }
}