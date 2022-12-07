using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotLab;
using TelegramBotLab.BotUpdates;
using Xunit.Abstractions;

namespace BotTest;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;
    private static readonly ITelegramBotClient BotClient = new TelegramBotClient(BotInfo.token);

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData("/currencySomething", "Incorrect input")]
    [InlineData("/currency______214Something", "Incorrect input")]
    [InlineData("/currency Sehjdgsj sdghjksd omething", "Such currency does not exist")]
    public async Task TestCurrency(string input, string output)
    {
        try
        {
            var httpClient = new HttpClient();
            var query = await httpClient.GetStringAsync("https://www.cbr.ru/currency_base/daily/");
            var update = new Currency();
            await update.GetInfo(input, query);
        }
        catch (ArgumentException ex)
        {
            Assert.Equal(ex.Message, output);
        }
    } 
    
    [Theory]
    [InlineData('-')]
    [InlineData('+')]
    [InlineData('/')]
    [InlineData('*')]
    public void TestSolve(char operation)
    {
        Func<decimal, decimal, decimal> f = operation switch
        {
            '-' => (decimal a, decimal b) => a - b,
            '+' => (decimal a, decimal b) => a + b,
            '/' => (decimal a, decimal b) => a / b,
            '*' => (decimal a, decimal b) => a * b,
        };

        var solver = new Solve();
        for (var i = 0; i < 1000; i++)
        {
            _testOutputHelper.WriteLine(i.ToString());
            try
            {
                List<decimal> values = new();
                var floatPart1 = i % 100;
                var integerPart1 = i / 100;
                values.Add(integerPart1 + (decimal)floatPart1 / 100);

                for (var j = 0; j < 1000; j++)
                {
                    var floatPart2 = j % 100;
                    var integerPart2 = j / 100;
                    values.Add(integerPart2 + (decimal)floatPart2 / 100);
                    Assert.Equal(
                        solver.Compute(values, $"12{operation}12"),
                        f(values[0], values[1]));
                }
            }
            catch (Exception ex)
            {
                _testOutputHelper.WriteLine(ex.Message);
            }
        }
    }
}