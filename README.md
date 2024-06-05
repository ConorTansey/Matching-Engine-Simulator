# Order_Book_Simulator

This is a program that is used to read in the contents of a csv file containing market data about trades that have been made by you as well as other participants throughout a trading day and will calculate your profit as well as your short and long exposure once trading has stopped. The logic simulates a matching engine that is price, time priority and takes larger sized orders for orders with the same price.

CSV is in the following format for each entry

{Security} {OrderType} {Quantity} {Price}

There can be multiple orders for the same instrument at the same time 

BUY/SELL indicates that you places the order and BID/OFFER indicates that someone else is placing orders. 

Example: APPLE BUY 10 20 represents us purchasing 10 shares of APPLE at a price of 20.

Refer to SampleTradeData.csv to see the expected format

To run your project pass in the path to your csv file as shown 

dotnet run path


