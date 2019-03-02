using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ContractorCore
{
    public class Class1
    {
        public Class1()
        {
            var generalcollection = DataProvider.GetDatabase().GetCollection<General>(typeof(General).ToString());
            var general = generalcollection.Find(new BsonDocument()).FirstOrDefault();
            if(general == null)
            {
                general = new General()
                {
                    Initialized = false
                };
                generalcollection.InsertOne(general);
            }
            if (!general.Initialized)
            {


                var location = new Location()
                {
                    Name = "Europe",
                    LocationType = LocationType.Continent
                };
                var locationcollection = DataProvider.GetDatabase().GetCollection<Location>(typeof(Location).ToString());
                var t = locationcollection.Find(f => f.Name == "Europe").FirstOrDefault();
                if (t == null)
                {
                    locationcollection.InsertOne(location);
                    t = location;
                }
                var governmentcollection = DataProvider.GetDatabase().GetCollection<Government>(typeof(Government).ToString());
                var government = new Government()
                {
                    Location = t._id,
                    Population = 100000
                };
                governmentcollection.InsertOne(government);

                var contractorcollection = DataProvider.GetDatabase().GetCollection<Contractor>(typeof(Contractor).ToString());
                var contractor = new Contractor() { Name = "Test", Location = t._id, Money = 100000, ProfitPercentage = 1.02M };
                contractorcollection.InsertOne(contractor);

                var commoditycollection = DataProvider.GetDatabase().GetCollection<Commodity>(typeof(Commodity).ToString());
                var commodity = new Commodity()
                {
                    Name = "Food"
                };
                commoditycollection.InsertOne(commodity);
                var commodityWithAmountcollection = DataProvider.GetDatabase().GetCollection<CommodityWithAmount>(typeof(CommodityWithAmount).ToString());
                var commodityWithAmount = new CommodityWithAmount()
                {
                    Commodity = commodity._id,
                    Amount = 100
                };
                commodityWithAmountcollection.InsertOne(commodityWithAmount);

                var activitycollection = DataProvider.GetDatabase().GetCollection<Activity>(typeof(CommodityWithAmount).ToString());
                var activity = new Activity()
                {
                    Name = "Basic farm",
                    Input = new List<ObjectId>(),
                    Effectiveness = 1,
                    Labor = 40,
                    Result = commodityWithAmount._id,
                };
                activitycollection.InsertOne(activity);
                var contractoractivitycollection = DataProvider.GetDatabase().GetCollection<ContractorActivity>(typeof(ContractorActivity).ToString());
                for (int y = 0; y < 1000; y++)
                {
                    var contractoractivity = new ContractorActivity()
                    {
                        Activity = activity._id,
                        Contractor = contractor._id,
                        LaborWages = 1,
                        ResultPrice = 1
                    };
                    contractoractivitycollection.InsertOne(contractoractivity);
                }
                general.Initialized = true;
                generalcollection.UpdateOne(Builders<General>.Filter.Eq(f => f._id, general._id), Builders<General>.Update.Set(u => u.Initialized, general.Initialized));
            }
            for (int x = 0; x < 10; x++)
            {
                UpdateGovernment();
                UpdateActivities();
            }
        }

        public void UpdateGovernment()
        {
            Console.WriteLine("Updating Government");
            //Each population needs 1 unit of food
            var governmentcollection = DataProvider.GetDatabase().GetCollection<Government>(typeof(Government).ToString());
            var locationcollection = DataProvider.GetDatabase().GetCollection<Location>(typeof(Location).ToString());
            var stockpilecollection = DataProvider.GetDatabase().GetCollection<Stockpile>(typeof(Stockpile).ToString());
            var commoditycollection = DataProvider.GetDatabase().GetCollection<Commodity>(typeof(Commodity).ToString());
            var contractorcollection = DataProvider.GetDatabase().GetCollection<Contractor>(typeof(Contractor).ToString());
            foreach (var government in governmentcollection.Find(new BsonDocument()).ToList())
            {
                var foodneeded = government.Population;
                var location = locationcollection.Find(f => f._id == government.Location).Single();
                foreach (var stockpile in stockpilecollection.Find(f => f.Location == location._id).ToList().OrderBy(o => o.CurrentPrice))
                {
                    if (commoditycollection.Find(f => f._id == stockpile.Commodity).Single().Name != "Food")
                        continue;
                    var canpayfor = (int)(government.Money / stockpile.CurrentPrice);
                    if (canpayfor < 0) canpayfor = 0;
                    var amount = Math.Min(Math.Min(foodneeded, stockpile.Amount), canpayfor);
                    foodneeded -= amount;
                    stockpile.Amount -= amount;
                    government.Money -= amount * stockpile.CurrentPrice;
                    var contractor = contractorcollection.Find(a => a._id == stockpile.Contractor).Single();
                    contractor.Money += amount * stockpile.CurrentPrice;

                    stockpilecollection.UpdateOne(Builders<Stockpile>.Filter.Eq(f => f._id, stockpile._id), Builders<Stockpile>.Update.Set(u => u.Amount, stockpile.Amount));
                    governmentcollection.UpdateOne(Builders<Government>.Filter.Eq(f => f._id, government._id), Builders<Government>.Update.Set(u => u.Money, government.Money));
                    contractorcollection.UpdateOne(Builders<Contractor>.Filter.Eq(f => f._id, contractor._id), Builders<Contractor>.Update.Set(u => u.Money, contractor.Money));
                }
                Console.WriteLine("Food needed {0}", foodneeded);
                Console.WriteLine("Money {0}", government.Money);
            }
        }

        public void UpdateActivities()
        {
            Console.WriteLine("Updating Activities");
            var governmentcollection = DataProvider.GetDatabase().GetCollection<Government>(typeof(Government).ToString());
            var locationcollection = DataProvider.GetDatabase().GetCollection<Location>(typeof(Location).ToString());
            var stockpilecollection = DataProvider.GetDatabase().GetCollection<Stockpile>(typeof(Stockpile).ToString());
            var commoditycollection = DataProvider.GetDatabase().GetCollection<Commodity>(typeof(Commodity).ToString());
            var contractorcollection = DataProvider.GetDatabase().GetCollection<Contractor>(typeof(Contractor).ToString());
            var contractoractivitycollection = DataProvider.GetDatabase().GetCollection<ContractorActivity>(typeof(ContractorActivity).ToString());
            var activitycollection = DataProvider.GetDatabase().GetCollection<Activity>(typeof(CommodityWithAmount).ToString());
            var commodityWithAmountcollection = DataProvider.GetDatabase().GetCollection<CommodityWithAmount>(typeof(CommodityWithAmount).ToString());
            foreach (var location in locationcollection.Find(new BsonDocument()).ToList())
            {
                var government = governmentcollection.Find(g => g.Location == location._id).ToList().Single();
                var availablePeople = government.Population;
                foreach (var contractorActivity in contractoractivitycollection.Find(new BsonDocument()).ToList().OrderByDescending(o => o.LaborWages))
                {
                    var activity = activitycollection.Find(a => a._id == contractorActivity.Activity).Single();
                    var contractor = contractorcollection.Find(a => a._id == contractorActivity.Contractor).Single();
                    bool canDoActivity = true;
                    if (availablePeople < activity.Labor)
                    {
                        canDoActivity = false;
                        continue;
                    }
                    var amount = Math.Max(activity.Labor, availablePeople);
                    foreach(var input in activity.Input)
                    {
                        var inputCommodity = commodityWithAmountcollection.Find(c => c._id == input).Single();
                        var stockpile = stockpilecollection.Find(s => s.Commodity == inputCommodity.Commodity && s.Amount >= inputCommodity.Amount).ToList().OrderBy(o => o.CurrentPrice).First();
                        if (stockpile.Amount < inputCommodity.Amount)
                        {
                            canDoActivity = false;
                            continue;
                        }
                        if(contractor.Money < stockpile.CurrentPrice * inputCommodity.Amount)
                        {
                            canDoActivity = false;
                            continue;

                        }
                    }
                    if(canDoActivity)
                    {
                        foreach (var input in activity.Input)
                        {
                            var inputCommodity = commodityWithAmountcollection.Find(c => c._id == input).Single();
                            var stockpile = stockpilecollection.Find(s => s.Commodity == inputCommodity.Commodity && s.Amount >= inputCommodity.Amount).ToList().OrderBy(o => o.CurrentPrice).First();
                            stockpile.Amount -= inputCommodity.Amount;
                            contractor.Money -= stockpile.CurrentPrice;
                        }
                        availablePeople -= activity.Labor;
                        government.Money += activity.Labor * contractorActivity.LaborWages;
                        contractor.Money -= activity.Labor * contractorActivity.LaborWages;

                        var commoditywithamount = commodityWithAmountcollection.Find(f => f._id == activity.Result).Single();
                        var resultstockpile = stockpilecollection.Find(f => f.Commodity == commoditywithamount.Commodity && f.Contractor == contractor._id).FirstOrDefault();
                        if(resultstockpile == null)
                        {
                            resultstockpile = new Stockpile()
                            {
                                Commodity = commoditywithamount.Commodity,
                                Contractor = contractor._id,
                                Amount = 0,
                                Location = location._id,
                                ManufactoryPrice = 0,
                                CurrentPrice = 0
                            };
                            stockpilecollection.InsertOne(resultstockpile);
                        }
                        resultstockpile.Amount += commoditywithamount.Amount;
                        resultstockpile.ManufactoryPrice = (activity.Labor * contractorActivity.LaborWages) / commoditywithamount.Amount;
                        resultstockpile.CurrentPrice = resultstockpile.ManufactoryPrice * contractor.ProfitPercentage;

                        governmentcollection.UpdateOne(Builders<Government>.Filter.Eq(f => f._id, government._id), Builders<Government>.Update.Set(u => u.Money, government.Money));
                        stockpilecollection.UpdateOne(Builders<Stockpile>.Filter.Eq(f => f._id, resultstockpile._id), Builders<Stockpile>.Update.Set(u => u.Amount, resultstockpile.Amount));
                        stockpilecollection.UpdateOne(Builders<Stockpile>.Filter.Eq(f => f._id, resultstockpile._id), Builders<Stockpile>.Update.Set(u => u.ManufactoryPrice, resultstockpile.ManufactoryPrice));
                        stockpilecollection.UpdateOne(Builders<Stockpile>.Filter.Eq(f => f._id, resultstockpile._id), Builders<Stockpile>.Update.Set(u => u.CurrentPrice, resultstockpile.CurrentPrice));
                        contractorcollection.UpdateOne(Builders<Contractor>.Filter.Eq(f => f._id, contractor._id), Builders<Contractor>.Update.Set(u => u.Money, contractor.Money));
                    }
                }
            }
            foreach (var document in contractorcollection.Find(new BsonDocument()).ToList())
            {
                Console.WriteLine(document.ToBsonDocument().ToString());

            }
            foreach (var document in stockpilecollection.Find(new BsonDocument()).ToList())
            {
                Console.WriteLine(document.ToBsonDocument().ToString());
            }
        }
    }
}
