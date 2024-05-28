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