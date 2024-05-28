using System;
using System.Data.Common;
using System.Reflection;
using System.Reflection.PortableExecutable;

//2 -> MAVEN BUY 10 20 SELL 5 25 OFFER 10 18 BID 5 28
//4 -> 
// 5 -> "MAVEN BUY 3 20 BID 5 20 BUY 2 20","MAVEN SELL 9 18"
// 6 -> "TINYCORP SELL 27 1", "MAVEN BID 5 20 OFFER 5 25", "MEDPHARMA BID 3 120 OFFER 7 150", "NEWFIRM BID 10 140 BID 7 150 OFFER 14 180","TINYCORP BID 25 3 OFFER 25 6", "FASTAIR BID 21 65 OFFER 35 85","FLYCARS BID 50 80 OFFER 100 90", "BIGBANK BID 200 13 OFFER 100 19", "REDCHIP BID 55 25 OFFER 80 30", "FASTAIR BUY 50 100", "CHEMCO SELL 100 67", "MAVEN BUY 5 30","REDCHIP SELL 5 30","NEWFIRM BUY 2 200", "MEDPHARMA BUY 2 150", "BIGBANK SELL 50 11","FLYCARS BUY 200 100", "CHEMCO BID 1000 77 OFFER 500 88"

//Test Case 7 -> "A BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25", "A SELL 4 19 BUY 3 26","A BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25","A SELL 4 19 BUY 3 26", "B BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25","B SELL 4 19 BUY 3 26","B BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25","B SELL 4 19","C BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25","C SELL 3 19 BUY 4 26","C BID 1 20 BID 1 21 BID 1 22 OFFER 1 23 OFFER 1 24 OFFER 1 25","C BUY 3 26","D BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","D OFFER 4 19 BID 3 26","D BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","D OFFER 4 19 BID 3 26","E BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","E OFFER 4 19 BID 3 26","E BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","E OFFER 4 19","F BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","F OFFER 3 19 BID 4 26","F BUY 1 20 BUY 1 21 BUY 1 22 SELL 1 23 SELL 1 24 SELL 1 25","F BID 3 26"

namespace app{

    class Program
    {
        static void Main(string[] args){
            List<string> records = new List<string>(){"MAVEN BUY 10 20 SELL 5 25 OFFER 10 18 BID 5 28"};
            Result.Trade(records);
        }
    }

    class Result
{

    /*
     * Complete the 'Trade' function below.
     */
     

    enum Action{
        BUY,
        SELL,
        BID,
        OFFER
    }
    
    class Order{
        private Action _action;
        private int _size;
        private int _price;

        public Action Action{
            get => _action;
            set => _action = value;
        }

        public int Size{
            get => _size;
            set => _size = value;
        }
        
        public int Price{
            get => _price;
            set => _price = value;
        }

        public Order(string share, string action, string size, string price){
            if(!Enum.TryParse(action, out Action parsedAction)){
                Console.WriteLine("Invalid action in order");
            };
            if(!int.TryParse(size, out int parsedSize)){
                Console.WriteLine("Invalid size in order");
            }
            if(!int.TryParse(price, out int parsedPrice)){
                Console.WriteLine("Invalid price in order");
            }
            Action = parsedAction;
            Size = parsedSize;
            Price = parsedPrice;
        }
    }

    class PriceLevel{ 
        private LinkedList<Order> _orders = new();

        public LinkedList<Order> Orders{
            get => _orders;
        }
    }
    
    class Book{
        private Dictionary<int, PriceLevel> _priceLevels = new Dictionary<int, PriceLevel>();
        private SortedSet<int> _prices = [];

        public Dictionary<int, PriceLevel> PriceLevels {get => _priceLevels;}
        public SortedSet<int> Prices {get => _prices;}

        public void AddOrder(Order order){
            if(!PriceLevels.ContainsKey(order.Price)){
                PriceLevels[order.Price] = new PriceLevel();
                PriceLevels[order.Price].Orders.AddFirst(new LinkedListNode<Order>(order));
                Prices.Add(order.Price);
            }　else {
                PriceLevels[order.Price].Orders.AddFirst(new LinkedListNode<Order>(order));
            }
        } 
    }
    
    class OrderBook{
        public Book buyBook;
        public Book sellBook;
        public int bestBid = -1;
        public int bestOffer = -1;
        
        public OrderBook(string shareName){
            buyBook = new Book();
            sellBook = new Book();
            bestBid = -1;
            bestOffer = -1;
        }

        private int MatchSellOrOffer(Order order){
            int profit = 0;
            int leavesQuantity = order.Size;
                    while(leavesQuantity > 0){
                        LinkedList<Order> restingOrders = buyBook.PriceLevels[bestBid].Orders;
                        while(restingOrders.Last != null && leavesQuantity > 0){
                            LinkedListNode<Order> restingOrder = restingOrders.Last;
                            int matchedQuantity = Math.Min(restingOrder.Value.Size, leavesQuantity);
                            if(order.Action == Action.OFFER && restingOrder.Value.Action == Action.BUY){
                                profit += Math.Abs(restingOrder.Value.Price - order.Price) * matchedQuantity;
                            }
                            else if(order.Action == Action.SELL && restingOrder.Value.Action == Action.BID){
                                profit += Math.Abs(restingOrder.Value.Price - order.Price) * matchedQuantity;
                            }
                            leavesQuantity -= matchedQuantity;
                            if(matchedQuantity == restingOrder.Value.Size){
                                restingOrders.RemoveLast();
                            } else if (matchedQuantity < restingOrder.Value.Size){
                                restingOrder.Value.Size -= matchedQuantity;
                                restingOrders.RemoveLast();
                                restingOrders.AddLast(restingOrder);
                            } 
                        }

                        if(restingOrders.First == null){
                            buyBook.PriceLevels.Remove(buyBook.Prices.Max);
                            buyBook.Prices.Remove(buyBook.Prices.Max);
                            if(buyBook.Prices.Count != 0){
                                bestBid = buyBook.Prices.Max;
                            } else {
                                bestBid = -1;
                                break;
                            }
                            if(bestBid < order.Price){
                                break;
                            }
                        }
                    }
                    
                    if(leavesQuantity > 0){
                        order.Size = leavesQuantity;
                        sellBook.AddOrder(order);
                        if(bestOffer == -1 || bestOffer > order.Price){
                            bestOffer = order.Price;
                        }
                    }
                    return profit;
        }

        private int MatchBuyOrBid(Order order){
            int profit = 0;
            
            int leavesQuantity = order.Size;
            while(leavesQuantity > 0){
                LinkedList<Order> restingOrders = sellBook.PriceLevels[bestOffer].Orders; //bestOffer.orders;
                while(restingOrders.Last != null && leavesQuantity > 0){
                    LinkedListNode<Order> restingOrder = restingOrders.Last;
                    int matchedQuantity = Math.Min(restingOrder.Value.Size, order.Size);
                    if(order.Action == Action.BID && restingOrder.Value.Action == Action.SELL){
                        profit += Math.Abs(restingOrder.Value.Price - order.Price) * matchedQuantity;
                    }
                    else if(order.Action == Action.BUY && restingOrder.Value.Action == Action.OFFER){
                        profit += Math.Abs(restingOrder.Value.Price - order.Price) * matchedQuantity;
                    }
                    leavesQuantity -= matchedQuantity;
                    if(matchedQuantity == restingOrder.Value.Size){
                        restingOrders.RemoveLast();
                    } else if (matchedQuantity < restingOrder.Value.Size){
                        restingOrder.Value.Size -= matchedQuantity;
                        restingOrders.RemoveLast();
                        restingOrders.AddLast(restingOrder);
                    }
                }

                if(restingOrders.First == null){
                    sellBook.PriceLevels.Remove(sellBook.Prices.Min);
                    sellBook.Prices.Remove(sellBook.Prices.Min);
                    if(sellBook.Prices.Count != 0){
                        bestOffer = sellBook.Prices.Min;
                    } else {
                        bestOffer = -1;
                        break;
                    }
                    if(bestOffer > order.Price){
                        break;
                    }
                }
            }
            if(leavesQuantity > 0){
                order.Size = leavesQuantity;
                buyBook.AddOrder(order);
                if(bestBid == -1 || bestBid < order.Price){
                    bestBid = order.Price;
                }
            }
            return profit;
        }
        
        public int TryMatchOrder(Order order){

            int profit = 0;

            if(order.Action == Action.OFFER || order.Action == Action.SELL){
                if(bestBid == -1 || order.Price > bestBid){
                    sellBook.AddOrder(order);
                    if(bestOffer == -1 || bestOffer > order.Price){
                        bestOffer = order.Price;
                    }
                } else {
                    profit += MatchSellOrOffer(order);
                    
                }
            } else {
                if(bestOffer == -1 || order.Price < bestOffer){
                    buyBook.AddOrder(order);
                    if(bestBid == -1 || bestBid < order.Price){
                        bestBid = order.Price;
                    }
                } else {
                    profit += MatchBuyOrBid(order);
                }
            }
            return profit;
        }
    
        public static (int longExposure, int shortExposure) CalculateExposures(Dictionary<string, OrderBook> orderBooks){

            int longExposure = 0, shortExposure = 0;

            //Calculate Long & Short Exposure
            foreach(OrderBook ob in orderBooks.Values){
                foreach(PriceLevel pl in ob.buyBook.PriceLevels.Values){
                    foreach(Order o in pl.Orders){
                        if(o.Action == Action.BUY){
                            longExposure += o.Price * o.Size;
                        }
                    }
                }
                foreach(PriceLevel pl in ob.sellBook.PriceLevels.Values){
                    foreach(Order o in pl.Orders){
                        if(o.Action == Action.SELL){
                            shortExposure += o.Price * o.Size;
                        }
                    }
                }
            }
            return (longExposure, shortExposure);
        }
    }

    
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
}