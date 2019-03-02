using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;
using System;
using System.Diagnostics;

namespace ContractorCore
{
    public class Class1
    {
        public Class1()
        {
            var db = DataProvider.GetDatabase();
            var location = new Location()
            {
                Name = "Europe",
                LocationType = LocationType.Continent
            };
            var test = new MongoDbRef<Location>();
            var t = Location.Get(f => f.Name == "Europe");
            if (t != null)
            {
                test.Set(t._id);
            }
            else
            {
                test.SetAndInsert(location);
            }

            var government = new Government()
            {
                Location = test,
                Population = 100000
            };
            Government.Set(government);

            var contractor = new Contractor() { Name = "Test", Location = test };
            Contractor.Set(contractor);

            var commodity = new Commodity()
            {
                Name = "Food"
            };
            var commodityWithAmount = new CommodityWithAmount()
            {
                Commodity = new MongoDbRef<Commodity>(commodity),
                Amount = 1000
            };
            var activity = new Activity()
            {
                Name = "Basic farm",
                Input = new System.Collections.Generic.List<MongoDbRef<CommodityWithAmount>>(),
                Effectiveness = 1,
                Labor = 40,
                Result = new MongoDbRef<CommodityWithAmount>(commodityWithAmount),
            };
            for (int y = 0; y < 100; y++)
            {
                var contractoractivity = new ContractorActivity()
                {
                    Activity = new MongoDbRef<Activity>(activity),
                    Contractor = new MongoDbRef<Contractor>(contractor),
                    LaborWages = 1,
                    ResultPrice = 1
                };
                ContractorActivity.Set(contractoractivity);
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
            foreach (var government in Government.List())
            {
                var foodneeded = government.Population;
                var location = government.Location.Get();
                foreach (var stockpile in Stockpile.List().OrderBy(o => o.CurrentPrice))
                {
                    if (stockpile.Commodity.Get().Name != "Food")
                        continue;
                    var amount = Math.Max(foodneeded, stockpile.Amount);
                    foodneeded -= amount;
                    stockpile.Amount -= amount;
                    government.Money -= amount * stockpile.CurrentPrice;
                }
                Console.WriteLine("Food needed {0}", foodneeded);
                Console.WriteLine("Money {0}", government.Money);
            }
        }

        public void UpdateActivities()
        {
            Console.WriteLine("Updating Activities");
            foreach (var location in Location.List())
            {
                var government = Government.Get(g => g.Location.ID == location._id);
                var availablePeople = government.Population;
                foreach (var contractorActivity in ContractorActivity.List().OrderByDescending(o => o.LaborWages))
                {
                    var activity = contractorActivity.Activity.Get();
                    var contractor = contractorActivity.Contractor.Get();
                    bool canDoActivity = true;
                    if (availablePeople < activity.Labor)
                    {
                        canDoActivity = false;
                        continue;
                    }
                    var amount = Math.Max(activity.Labor, availablePeople);
                    foreach(var input in activity.Input)
                    {
                        var stockpile = Stockpile.List(s => s.Commodity.Get() == input.Get().Commodity.Get() && s.Amount >= input.Get().Amount).OrderBy(o => o.CurrentPrice).First();
                        if (stockpile.Amount < input.Get().Amount)
                        {
                            canDoActivity = false;
                            continue;
                        }
                    }
                    if(canDoActivity)
                    {
                        foreach (var input in activity.Input)
                        {
                            var stockpile = Stockpile.List(s => s.Commodity.Get() == input.Get().Commodity.Get() && s.Amount >= input.Get().Amount).OrderBy(o => o.CurrentPrice).First();
                            stockpile.Amount -= input.Get().Amount;
                            contractor.Money -= stockpile.CurrentPrice;
                        }
                        availablePeople -= activity.Labor;
                        government.Money += activity.Labor * contractorActivity.LaborWages;
                        var commodity = activity.Result.Get().Commodity.Get();
                        var resultstockpile = Stockpile.List().FirstOrDefault(s => s.Commodity.Get() == commodity);
                        if(resultstockpile == null)
                        {
                            resultstockpile = new Stockpile()
                            {
                                Commodity = activity.Result.Get().Commodity,
                                Amount = 0,
                                Location = new MongoDbRef<Location>(location._id),
                                ManufactoryPrice = 0,
                                CurrentPrice = 0
                            };
                        }
                        resultstockpile.Amount += activity.Result.Get().Amount;
                        resultstockpile.ManufactoryPrice = (activity.Labor * contractorActivity.LaborWages) / activity.Result.Get().Amount;
                        resultstockpile.CurrentPrice = contractorActivity.ResultPrice;
                    }
                }
            }
            foreach (var document in Contractor.List())
            {
                Console.WriteLine(document.ToBsonDocument().ToString());

            }
        }
    }
}
