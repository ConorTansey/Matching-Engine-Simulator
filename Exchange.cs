class Exchange{

    public static (int profit, int longExposure, int shortExposure) Trade(List<string> records)
        {
            int profit = 0;
            Dictionary<string, OrderBook> orderBooks = new Dictionary<string, OrderBook>();
            
            foreach(string record in records){
                string[] s = record.Split(" ");
                string share = s[0];
                if(!orderBooks.ContainsKey(share)){
                    orderBooks[share] = new OrderBook(share);
                }
                for(int i=1; i < s.Length; i+=3){
                    Order newOrder = new Order(s[0],s[i],s[i+1],s[i+2]);
                    profit += orderBooks[share].TryMatchOrder(newOrder);
                }
                Console.WriteLine(record);
            }

            (int longExposure, int shortExposure) = OrderBook.CalculateExposures(orderBooks);
            return (profit, longExposure, shortExposure);
        }
    
}