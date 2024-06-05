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
            
            List<string> trades = ReadCSV(filePath);
            
            if(trades.Count() == 0){
                Console.WriteLine($"Empty csv file");
                return;
            }

            int profit, longExposure, shortExposure = 0;
            (profit, longExposure, shortExposure) = Exchange.Trade(trades);
            Console.WriteLine($"Profit : {profit}");
            Console.WriteLine($"ShortExposure: {shortExposure}");
            Console.WriteLine($"LongExposure : {longExposure}");
        }

        private static List<string> ReadCSV(string filePath){
            List<string> trades = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                var line = reader.ReadLine();

                if (line != null)
                {
                    trades = line.Split(',').ToList<string>();
                }
            }
            return trades;
        }
    } 
}