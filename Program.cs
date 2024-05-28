namespace app
{
    class Program
    {
        static void Main(string[] args){
            if (args.Length != 1)
            {
                Console.WriteLine("Must pass a path to a file containing trade data");
                return;
            }

            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"The file '{filePath}' does not exist.");
                return;
            }

            List<string> records = new List<string>(){"MAVEN BUY 10 20 SELL 5 25 OFFER 10 18 BID 5 28"};
            int profit, longExposure, shortExposure = 0;
            (profit, longExposure, shortExposure) = Exchange.Trade(records);
            Console.WriteLine($"Profit : {profit}");
            Console.WriteLine($"ShortExposure: {shortExposure}");
            Console.WriteLine($"LongExposure : {longExposure}");
        }
    }
}