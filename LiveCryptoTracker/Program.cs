using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly string apiUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin,ethereum,dogecoin,solana,cardano,ripple,shiba-inu&vs_currencies=usd";
    private static readonly string historyApiUrl = "https://api.coingecko.com/api/v3/coins/";

    static async Task Main()
    {
        Console.Clear();
        Console.WriteLine("📢 Live Crypto Tracker (C#) - Preise in USD");
        Console.WriteLine(new string('-', 50));

        await FetchCryptoPrices();

        Console.WriteLine("\nDrücke eine beliebige Taste, um das Programm zu beenden...");
        Console.ReadKey();
    }

    private static async Task FetchCryptoPrices()
    {
        try
        {
            string response = await client.GetStringAsync(apiUrl);
            JObject json = JObject.Parse(response);

            decimal btcPrice = (decimal)json["bitcoin"]["usd"];
            decimal ethPrice = (decimal)json["ethereum"]["usd"];
            decimal dogePrice = (decimal)json["dogecoin"]["usd"];
            decimal solPrice = (decimal)json["solana"]["usd"];
            decimal adaPrice = (decimal)json["cardano"]["usd"];
            decimal xrpPrice = (decimal)json["ripple"]["usd"];
            decimal shibaPrice = (decimal)json["shiba-inu"]["usd"];

            // Preise vom Monatsanfang holen
            decimal startOfMonthBTC = await GetHistoricalPrice("bitcoin");
            decimal startOfMonthETH = await GetHistoricalPrice("ethereum");
            decimal startOfMonthDOGE = await GetHistoricalPrice("dogecoin");
            decimal startOfMonthSOL = await GetHistoricalPrice("solana");
            decimal startOfMonthADA = await GetHistoricalPrice("cardano");
            decimal startOfMonthXRP = await GetHistoricalPrice("ripple");
            decimal startOfMonthSHIBA = await GetHistoricalPrice("shiba-inu");

            // Ergebnisse ausgeben
            Console.WriteLine($"💰 Bitcoin (BTC): ${btcPrice} {ComparePrice(btcPrice, startOfMonthBTC)}");
            Console.WriteLine($"💰 Ethereum (ETH): ${ethPrice} {ComparePrice(ethPrice, startOfMonthETH)}");
            Console.WriteLine($"💰 Dogecoin (DOGE): ${dogePrice} {ComparePrice(dogePrice, startOfMonthDOGE)}");
            Console.WriteLine($"💰 Solana (SOL): ${solPrice} {ComparePrice(solPrice, startOfMonthSOL)}");
            Console.WriteLine($"💰 Cardano (ADA): ${adaPrice} {ComparePrice(adaPrice, startOfMonthADA)}");
            Console.WriteLine($"💰 XRP: ${xrpPrice} {ComparePrice(xrpPrice, startOfMonthXRP)}");
            Console.WriteLine($"💰 Shiba Inu (SHIBA): ${shibaPrice} {ComparePrice(shibaPrice, startOfMonthSHIBA)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fehler beim Abrufen der Daten: {ex.Message}");
        }
    }

    private static async Task<decimal> GetHistoricalPrice(string coinId)
    {
        try
        {
            string historyUrl = $"{historyApiUrl}{coinId}/market_chart?vs_currency=usd&days=30";
            string response = await client.GetStringAsync(historyUrl);
            JObject json = JObject.Parse(response);

            // Der erste Wert ist der Preis vom Monatsanfang
            return json["prices"][0][1].Value<decimal>();
        }
        catch
        {
            return 0; // Falls ein Fehler auftritt, gib 0 zurück
        }
    }

    private static string ComparePrice(decimal currentPrice, decimal oldPrice)
    {
        if (oldPrice == 0) return "(Keine Daten)";

        decimal diff = currentPrice - oldPrice;
        decimal percentChange = (diff / oldPrice) * 100;

        return diff >= 0
            ? $"📈 +${diff:F2} (+{percentChange:F2}%)"
            : $"📉 -${Math.Abs(diff):F2} (-{Math.Abs(percentChange):F2}%)";
    }
}
