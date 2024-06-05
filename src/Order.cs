class Order{
        private OrderType _orderType;
        private int _size;
        private int _price;

        public OrderType OrderType{
            get => _orderType;
            set => _orderType = value;
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
            if(!Enum.TryParse(action, out OrderType parsedAction)){
                Console.WriteLine("Invalid action in order");
            };
            if(!int.TryParse(size, out int parsedSize)){
                Console.WriteLine("Invalid size in order");
            }
            if(!int.TryParse(price, out int parsedPrice)){
                Console.WriteLine("Invalid price in order");
            }
            OrderType = parsedAction;
            Size = parsedSize;
            Price = parsedPrice;
        }
    }