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