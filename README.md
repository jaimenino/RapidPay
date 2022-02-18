# RapidPay
RapidPay is an ASP.NET Core Web API which implements bearer token authentication to perform the following operations:
* Create credir card
* Get the card balance
* Register a payment

The solution has 3 projects:
* RapidPay.DataAccess: Perform the acces to the database. Uses Entity Framework Core and framework .NET 6.0
* RapidPay.Application: Has the business logic and is the layer to comunicate the api with the database. Uses framework .NET 6.0
* RapidPay.Api: Has the api methods wich are exposed to the user. Uses framework .NET 6.0

### How to run the solution?
* Run the scripts located on RapidPay/Documentation/SQL/ in Sql server
* Open solution with Visual Studio 2022
* Set RapidPay.Api as a startup project
* Compile and run solution. The browser will load the Swagger specification

If you wanto to use Postman, import the profile located in RapidPay/Documentation/Postman.
