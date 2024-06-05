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