# BitFinex Auctions

Due to time restrictions this is not entirely finished in various aspects - functional and code syntax.
Application should be compiled and published /or bin copied/ to 3 different folders representign 3 separate CLients.
In DataContext.InitializePeers() one can set the Peers config /IPs, ports/. 
This needs to be inline with each .env file contents per each directory representing each CLient.

Now run each Client.

Sample commmands:

- Create an Auction for Pic1 for 75 -> A#Pic1#75
- Create an Auction for Pic2 for 60 -> A#Pic2#60
- Bid Pic1 for 75 -> B#Pic1#75
- Bid Pic1 for 76 -> B#Pic1#76
- Finalize Auction -> C#Pic1
- List Auctions -> LAU
- List Accounts -> LAC
- List Bids -> LB


Application uses SQLLite DB that get's created upon start. It however doesn't clear itself so multiple runs can lead to repeated rows, auctions etc.



