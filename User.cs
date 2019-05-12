using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class User
    {
        public string Name { get; set; }
        public DateTime PaidDate { get; set; }
        public int Involve { get; set; }
        public string Item { get; set; }

        public int Amount { get; set; }
        public bool Aashish { get; set; }
        public bool Bharat { get; set; }
        public bool Nishant { get; set; }
        public bool Shailendra { get; set; }
        public string Case { get; set; }


        public List<CalculativeModel> CalculateSheet()
        {
            var models = new List<User>();
            var allInvolvedModel = new List<CalculativeModel>();
            System.Diagnostics.Debug.WriteLine("app is ready to read file");
            using (var reader = new StreamReader(@"D:\test3.csv"))
            {

                while (!reader.EndOfStream)
                {
                    var model = new User();
                    var line = reader.ReadLine();
                    var d = line.Split(",");
                    if (d[0] == "Items")                    
                        continue;
                    else
                        { 
                        model.Item = d[0];
                        model.Name = d[1];
                        model.Amount = Convert.ToInt32(d[2]);
                        model.Involve = Convert.ToInt32(d[4]);
                        if (d[5] == "1")
                            model.Nishant = true;
                        else
                            model.Nishant = false;
                        if (d[6] == "1")
                            model.Shailendra = true;
                        else
                            model.Shailendra = false;
                        if (d[7] == "1")
                            model.Aashish = true;
                        else
                            model.Aashish = false;
                        if (d[8] == "1")
                            model.Bharat = true;
                        else
                            model.Bharat = false;

                        //when 3 involved there are four cases
                        if (d[5] == "1" && d[6] == "1" && d[7] == "1" && d[8] == "1")
                            model.Case = "ABNS";
                        if (d[5] == "1" && d[6] == "1" && d[7] == "1" && d[8] == "0")
                            model.Case = "ANS";
                        if (d[5] == "1" && d[6] == "1" && d[7] == "0" && d[8] == "1")
                            model.Case = "BNS";
                        if (d[5] == "1" && d[6] == "0" && d[7] == "1" && d[8] == "1")
                            model.Case = "ABN";
                        if (d[5] == "0" && d[6] == "1" && d[7] == "1" && d[8] == "1")
                            model.Case = "ABS";

                        //when 3 involve there are 6 cases
                        if (d[5] == "0" && d[6] == "0" && d[7] == "1" && d[8] == "1")
                            model.Case = "AB";
                        if (d[5] == "1" && d[6] == "0" && d[7] == "0" && d[8] == "1")
                            model.Case = "BN";
                        if (d[5] == "1" && d[6] == "1" && d[7] == "0" && d[8] == "0")
                            model.Case = "NS";
                        if (d[5] == "0" && d[6] == "1" && d[7] == "0" && d[8] == "1")
                            model.Case = "BS";
                        if (d[5] == "0" && d[6] == "1" && d[7] == "1" && d[8] == "0")
                            model.Case = "AS";
                        if (d[5] == "1" && d[6] == "0" && d[7] == "1" && d[8] == "0")
                            model.Case = "AN";
                        if (d[5] == "1" && d[6] == "0" && d[7] == "0" && d[8] == "0")
                            model.Case = "N";
                        if (d[5] == "0" && d[6] == "1" && d[7] == "0" && d[8] == "0")
                            model.Case = "S";
                        if (d[5] == "0" && d[6] == "0" && d[7] == "1" && d[8] == "0")
                            model.Case = "A";
                        if (d[5] == "0" && d[6] == "0" && d[7] == "0" && d[8] == "1")
                            model.Case = "B";
                    }


                    models.Add(model);
                }
                Console.WriteLine("File has been read successfully");
                Console.WriteLine("Calculation has started");

                var dataToCalculate = models.GroupBy(m => m.Case).ToList();
                var dataModel = new List<CalculativeModel>();
                dataToCalculate.ForEach(d =>
                {
                    d.ToList().ForEach(e => {
                        var data = new CalculativeModel();
                        data.Name = e.Name;
                        data.TotalAmount = d.Sum(s => s.Amount);
                        data.Case = e.Case;
                        data.Paid = d.Where(n => n.Name == e.Name).Sum(s => s.Amount);
                        data.PerPersonNeedToPay = data.TotalAmount / e.Case.Length;
                        data.NeedToPay = data.PerPersonNeedToPay - data.Paid;
                        dataModel.Add(data);
                    });
                    
                });

                Console.WriteLine(dataModel);
                var distinctData = dataModel.GroupBy(g => new { g.Case, g.Name}).Distinct().ToList();
                var distinctFinalCalculationData = new List<CalculativeModel>();
                distinctData.ForEach(d =>
                {
                    var data = d.FirstOrDefault();
                    distinctFinalCalculationData.Add(data);
                });

                Console.WriteLine(distinctFinalCalculationData);
                var finalCalculation = distinctFinalCalculationData.GroupBy(g => g.Case).ToList();
                var lastDataModel1 = new List<FinalCalculation>();
                finalCalculation.ForEach(f =>
                {
                    f.ToList().ForEach(e => {
                        if (e.Paid < e.PerPersonNeedToPay)
                        {
                            var lastData = new FinalCalculation();
                            var maxAmount = f.Max(m => m.Paid);
                            var maxPaid = f.Where(s => s.Paid == maxAmount).FirstOrDefault();
                            var amountNeedToCheck = (maxAmount - e.PerPersonNeedToPay) - e.NeedToPay;
                            if (amountNeedToCheck > 0)
                            {
                                lastData.PayTo = maxPaid.Name;
                                lastData.PayFrom = e.Name;
                                lastData.Amount = e.NeedToPay;
                                lastDataModel1.Add(lastData);
                                maxPaid.NeedToPay = maxPaid.NeedToPay + e.NeedToPay;
                                e.NeedToPay = e.NeedToPay - lastData.Amount;
                               

                            }
                            else
                            {
                                var groupedPeople = f.Where(s => s.Name != e.Name).ToList();
                                groupedPeople.ForEach(gp =>
                                {
                                    if (gp.Paid > gp.PerPersonNeedToPay)
                                    {
                                        lastData.PayFrom = e.Name;
                                        lastData.PayTo = gp.Name;
                                        lastData.Amount = -(gp.NeedToPay);
                                        e.NeedToPay = e.NeedToPay - lastData.Amount;
                                        lastDataModel1.Add(lastData);
                                        gp.NeedToPay = gp.NeedToPay - lastData.Amount;
                                    }
                                });


                            }

                        }
                    });
                });

                var showDetails = lastDataModel1.GroupBy(g => new { g.PayFrom, g.PayTo }).ToList();
                var calculateAmount = new List<FinalCalculation>();
                showDetails.ForEach(d =>
                {
                    var newData = new FinalCalculation();
                    newData.PayFrom = d.Select(n => n.PayFrom).FirstOrDefault();
                    newData.PayTo = d.Select(n => n.PayTo).FirstOrDefault();
                    newData.Amount = d.Sum(s => s.Amount);
                    calculateAmount.Add(newData);
                });

                Console.WriteLine(calculateAmount);
                var whoPaidToWhom = calculateAmount.OrderBy(o => o.PayFrom).ToList();
                whoPaidToWhom.ForEach(d =>
                {
                    Console.WriteLine(d.PayFrom + " need to pay " + d.Amount + " to " + d.PayTo);
                });
                Console.WriteLine("Calculation has been done successfully");


                //    Console.WriteLine("First its to goind to calculate when all were involved");
                //    var allInvolved = models.Where(d => d.Involve == 3).ToList();
                //    var allInvolvdAmount = allInvolved.Sum(s => s.Amount);
                //    var allInvolvedGroupedByName = allInvolved.GroupBy(g => g.Name).ToList();


                //    allInvolvedGroupedByName.ForEach(s =>
                //    {
                //        var allInvolvedM = new CalculativeModel();
                //        allInvolvedM.Name = s.Key;
                //        allInvolvedM.Paid = s.Sum(d => d.Amount);
                //        allInvolvedM.TotalAmount = allInvolvdAmount;
                //        Console.WriteLine(" Total Amount is " + allInvolvedM.TotalAmount);
                //        allInvolvedM.NeedToPay = allInvolvedM.TotalAmount / 3 - allInvolvedM.Paid;
                //        Console.WriteLine(allInvolvedM.Name + " has paid " + allInvolvedM.Paid + " and he need to pay  " + allInvolvedM.NeedToPay);
                //        allInvolvedModel.Add(allInvolvedM);

                //    });

                //}
                ////case AAshish, Nishant
                //Console.WriteLine("Now the app is going to calculate When 2 were involved");
                //Console.WriteLine("1st its going to calculate when Aashish and Nishant were involved");
                //var aashishNishantData = models.Where(m => m.Involve == 2 && m.Nishant == true && m.Aashish == true).ToList();
                //var aashishNishantGroup = aashishNishantData.GroupBy(d => d.Name).ToList();
                var threeInvolved = new List<CalculativeModel>();
                //aashishNishantGroup.ForEach(an =>
                //{
                //    var aNModel = new CalculativeModel();
                //    aNModel.Name = an.Key;
                //    aNModel.TotalAmount = aashishNishantData.Sum(d => d.Amount);
                //    Console.WriteLine("when Aashish and Nishant were involved the Total Amount is " + aNModel.TotalAmount);
                //    aNModel.Paid = an.Sum(d => d.Amount);
                //    aNModel.NeedToPay = aNModel.TotalAmount / 2 - aNModel.Paid;
                //    Console.WriteLine(aNModel.Name + " has paid " + aNModel.Paid + " and he need to pay  " + aNModel.NeedToPay);
                //    aNModel.Nishant = true;
                //    aNModel.Aashish = true;
                //    aNModel.Shailendra = false;
                //    threeInvolved.Add(aNModel);
                //});

                ////case Aashish, Shailendra
                //Console.WriteLine("2nd its going to calculate when Aashish and Shailendra were involved");
                //var aashishShailendraData = models.Where(m => m.Involve == 2 && m.Aashish == true && m.Shailendra == true && m.Nishant == false).ToList();
                //var aashishShailendraGroup = aashishShailendraData.GroupBy(d => d.Name).ToList();
                //aashishShailendraGroup.ForEach(aS =>
                //{
                //    var aSModel = new CalculativeModel();
                //    aSModel.Name = aS.Key;
                //    aSModel.TotalAmount = aashishShailendraData.Sum(d => d.Amount);
                //    Console.WriteLine("when Aashish and Shailendra were involved the Total Amount is " + aSModel.TotalAmount);
                //    aSModel.Paid = aS.Sum(d => d.Amount);
                //    aSModel.NeedToPay = aSModel.TotalAmount / 2 - aSModel.Paid;
                //    Console.WriteLine(aSModel.Name + " has paid " + aSModel.Paid + " and he need to pay  " + aSModel.NeedToPay);
                //    aSModel.Nishant = false;
                //    aSModel.Aashish = true;
                //    aSModel.Shailendra = true;
                //    threeInvolved.Add(aSModel);
                //});

                ////case Nishant, Shailendra
                //Console.WriteLine("3rd its going to calculate when Nishant and Shailendra were involved");
                //var nishantShailendraData = models.Where(m => m.Involve == 2 && m.Nishant == true && m.Shailendra == true && m.Aashish == false).ToList();
                //var nishantShailendraGroup = nishantShailendraData.GroupBy(d => d.Name).ToList();
                //nishantShailendraGroup.ForEach(nS =>
                //{
                //    var nSModel = new CalculativeModel();
                //    nSModel.Name = nS.Key;
                //    nSModel.TotalAmount = nishantShailendraData.Sum(d => d.Amount);
                //    Console.WriteLine("when Nishant and Shailendra were involved the Total Amount is " + nSModel.TotalAmount);
                //    nSModel.Paid = nS.Sum(d => d.Amount);
                //    nSModel.NeedToPay = nSModel.TotalAmount / 2 - nSModel.Paid;
                //    Console.WriteLine(nSModel.Name + " has paid " + nSModel.Paid + " and he need to pay  " + nSModel.NeedToPay);
                //    nSModel.Nishant = true;
                //    nSModel.Aashish = false;
                //    nSModel.Shailendra = true;
                //    threeInvolved.Add(nSModel);

                //});

                threeInvolved.ForEach(d =>
                {


                });

                return allInvolvedModel;
            }
        }

        public class CalculateModel
        {
            public string Name { get; set; }
            public int TotalAmount { get; set; }
            public int Paid { get; set; }


        }

        public class CalculativeModel
        {
            public string Name { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal Paid { get; set; }
            public decimal PerPersonNeedToPay { get; set; }
            public decimal NeedToPay { get; set; }
            public string Case { get; set; }

            public static implicit operator CalculativeModel(decimal v)
            {
                throw new NotImplementedException();
            }
        }

        public class FinalCalculation
        {
            public string PayFrom { get; set; }
            public string PayTo { get; set; }
            public decimal Amount { get; set; }

        }

    }
}
