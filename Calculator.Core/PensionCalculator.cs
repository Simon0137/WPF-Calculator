namespace Calculator.Core;

public static class PensionCalculator
{
    public static uint? CalcAgeDelta(uint currentAge, uint targetAge)
    {
        var delta = targetAge - currentAge;
        return delta > 0 ? delta : null;
    }

    public static double CalcOwnAmount(double startCapital, uint ageDelta, double monthlyPayment)
    {
        var monthDelta = ageDelta * 12;
        return startCapital + monthDelta * monthlyPayment;
    }

    public static double CalcTotalAmount(double startCapital, uint ageDelta, double monthlyPayment, double yearlyReturn)
    {
        var monthDelta = ageDelta * 12;
        var monthlyReturn = yearlyReturn / 12;

        var capital = startCapital;
        for (var i = 0; i < monthDelta; i++)
        {
            
            var profit = capital * monthlyReturn * 0.01;
            capital += profit;
            capital += monthlyPayment;
        }

        return capital;
    }
}