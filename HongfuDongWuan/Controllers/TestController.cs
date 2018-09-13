using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using EntityData;
using HongfuDongWuan.Models;

namespace HongfuDongWuan.Controllers
{
    public class TestController : Controller
    {
        const double baseRate = 0.0435; //基准利率 %(一年内) 
        const double floatRate = 0.04; //上浮利率 %(一年内)
        const int deliveryToReceivePayPeriod = 180; //发货日至回款日的期间长度: 180天
        const double AverageStockWF = 0.3; //正权重因子
        const double TurnoverRateWF = 0.7; //正权重因子
        const double NumOfOverdueWF = -0.1; //负权重因子
        const double SlowSalesRateWF = -0.2; //负权重因子
        const double ReturnRateWF = -0.05; //负权重因子
        const double BreakageRateWF = -0.05; //负权重因子

        private List<CustomerModel> cusList;        
        // GET: Test
        public ActionResult Customer()
        {
            cusList = new List<CustomerModel>();
            using (var db = new hfShopEntities())
            {
                var query = from b in db.TEST_Customer
                            orderby b.ID
                            select b;

                foreach (var item in query)
                {
                    CustomerModel cm = new CustomerModel();
                    cm.Month = item.Month;
                    cm.EndUser = item.EndUser;
                    cm.AverageStock = Math.Round((double)item.AverageStock, 0);
                    cm.TurnoverRate = item.TurnoverRate;
                    cm.NumOfOverdue = item.NumOfOverdue;
                    cm.SlowSalesRate = item.SlowSalesRate;
                    cm.ReturnRate = item.ReturnRate;
                    cm.BreakageRate = item.BreakageRate;
                    cm.MonCumlShipQty = Math.Round((double)item.MonCumlShipQty, 0);
                    cm.UnitPrice = item.UnitPrice;
                    cm.MonCumlShipAmt = Math.Round((decimal)item.MonCumlShipAmt, 0);
                    cm.CreditRating = item.CreditRating;
                    cm.CanRaiseCapital = item.CanRaiseCapital;
                    cm.ID = item.ID;
                    cusList.Add(cm);
                }

                ViewBag.CreditRating = (from c in db.TEST_Customer
                                        orderby c.CreditRating
                                        select c.CreditRating).Distinct().ToList();
            }

            return View(cusList);
        }

        [HttpGet]
        public ActionResult FilterCreditRating(string creditRating)
        {
            cusList = new List<CustomerModel>();
            using (var db = new hfShopEntities())
            {
                var query = from b in db.TEST_Customer
                            orderby b.ID
                            where b.CreditRating == creditRating //|| creditRt == null || creditRt == String.Empty
                            select b;

                foreach (var item in query)
                {
                    CustomerModel cm = new CustomerModel();
                    cm.Month = item.Month;
                    cm.EndUser = item.EndUser;
                    cm.AverageStock = Math.Round((double)item.AverageStock, 0);
                    cm.TurnoverRate = item.TurnoverRate;
                    cm.NumOfOverdue = item.NumOfOverdue;
                    cm.SlowSalesRate = item.SlowSalesRate;
                    cm.ReturnRate = item.ReturnRate;
                    cm.BreakageRate = item.BreakageRate;
                    cm.MonCumlShipQty = Math.Round((double)item.MonCumlShipQty, 0);
                    cm.UnitPrice = item.UnitPrice;
                    cm.MonCumlShipAmt = Math.Round((decimal)item.MonCumlShipAmt, 0);
                    cm.CreditRating = item.CreditRating;
                    cm.CanRaiseCapital = item.CanRaiseCapital;
                    cm.CreditScore = cm.CanRaiseCapital.Equals("Y") ? Math.Round((((double)(GetAvgStockScore((double)cm.AverageStock / 10000) + GetTurnoverRateScore((double)cm.TurnoverRate) +
                                GetNumOfOverdueScore((double)cm.NumOfOverdue) + GetSlowSalesRateScore((double)cm.SlowSalesRate) +
                                GetReturnRateScore((double)cm.ReturnRate) + GetBreakageRateScore((double)cm.BreakageRate))) + GetThirdPartyCreditScore(cm.CreditRating)) / 2, 2) : 0;
                    cm.CapitalAmt = cm.CanRaiseCapital.Equals("Y") ? Math.Round(((decimal)cm.MonCumlShipAmt * (decimal)cm.CreditScore) / 100, 0) : 0;
                    cm.CapExpRate = cm.CanRaiseCapital.Equals("Y") ? Math.Round((double)((baseRate + floatRate) / (cm.CreditScore / 100)), 4) : 0;
                    cm.ID = item.ID;
                    cusList.Add(cm);
                }

                ViewBag.CreditRating = (from c in db.TEST_Customer
                                        orderby c.CreditRating
                                        select c.CreditRating).Distinct().ToList();
            }

            return View("Customer", cusList);
        }

        [HttpPost]
        public ActionResult CalcCapital()
        {
            cusList = new List<CustomerModel>();
            using (var db = new hfShopEntities())
            {
                var query = from b in db.TEST_Customer
                            orderby b.ID
                            select b;

                foreach (var item in query)
                {
                    CustomerModel cm = new CustomerModel();
                    cm.Month = item.Month;
                    cm.EndUser = item.EndUser;
                    cm.AverageStock = Math.Round((double)item.AverageStock,0);
                    cm.TurnoverRate = item.TurnoverRate;
                    cm.NumOfOverdue = item.NumOfOverdue;
                    cm.SlowSalesRate = item.SlowSalesRate;
                    cm.ReturnRate = item.ReturnRate;
                    cm.BreakageRate = item.BreakageRate;
                    cm.MonCumlShipQty = Math.Round((double)item.MonCumlShipQty, 0);
                    cm.UnitPrice = item.UnitPrice;
                    cm.MonCumlShipAmt = Math.Round((decimal)item.MonCumlShipAmt,0);
                    cm.CreditRating = item.CreditRating;
                    cm.CanRaiseCapital = item.CanRaiseCapital;
                    cm.CreditScore = cm.CanRaiseCapital.Equals("Y") ? Math.Round((((double)(GetAvgStockScore((double)cm.AverageStock/10000) + GetTurnoverRateScore((double)cm.TurnoverRate) +
                                GetNumOfOverdueScore((double)cm.NumOfOverdue) + GetSlowSalesRateScore((double)cm.SlowSalesRate) +
                                GetReturnRateScore((double)cm.ReturnRate) + GetBreakageRateScore((double)cm.BreakageRate))) + GetThirdPartyCreditScore(cm.CreditRating)) / 2, 2) : 0;
                    cm.CapitalAmt = cm.CanRaiseCapital.Equals("Y") ? Math.Round(((decimal)cm.MonCumlShipAmt * (decimal)cm.CreditScore) / 100, 0) : 0;
                    cm.CapExpRate = cm.CanRaiseCapital.Equals("Y") ? Math.Round((double)((baseRate + floatRate) / (cm.CreditScore / 100)), 4) : 0;
                    cm.ID = item.ID;
                    cusList.Add(cm);
                }

                ViewBag.CreditRating = (from c in db.TEST_Customer
                                        orderby c.CreditRating
                                        select c.CreditRating).Distinct().ToList();
            }

            return View("Customer", cusList);
        }

        public int GetThirdPartyCreditScore(string creditRating)
        {
            int retVal;
            if (creditRating.Equals("AA-"))
                retVal = 80;
            else if (creditRating.Equals("AA"))
                retVal = 85;
            else if (creditRating.Equals("AA+"))
                retVal = 90;
            else if (creditRating.Equals("AAA"))
                retVal = 95;
            else
                retVal = 0;


            return retVal;
        }

        public double GetAvgStockScore(double AverageStock)
        {
            double retVal;
            if (AverageStock < 100)
                retVal = 50;
            else if ((AverageStock >= 100) && (AverageStock < 200))
                retVal = 60;
            else if ((AverageStock >= 200) && (AverageStock < 300))
                retVal = 70;
            else if ((AverageStock >= 300) && (AverageStock < 400))
                retVal = 80;
            else if ((AverageStock >= 400) && (AverageStock < 500))
                retVal = 90;
            else
                retVal = 100;

            return retVal * AverageStockWF;
        }

        public double GetTurnoverRateScore(double TurnoverRate)
        {
            double retVal;
            if (TurnoverRate < 12)
                retVal = 50;
            else if ((TurnoverRate >= 12) && (TurnoverRate < 15))
                retVal = 60;
            else if ((TurnoverRate >= 15) && (TurnoverRate < 18))
                retVal = 70;
            else if ((TurnoverRate >= 18) && (TurnoverRate < 20))
                retVal = 80;
            else if ((TurnoverRate >= 20) && (TurnoverRate < 24))
                retVal = 90;
            else
                retVal = 100;

            return retVal * TurnoverRateWF;
        }

        public double GetNumOfOverdueScore(double NumOfOverdue)
        {
            double retVal;
            if (NumOfOverdue == 0)
                retVal = 100;
            else if (NumOfOverdue == 1)
                retVal = 90;
            else if (NumOfOverdue == 2)
                retVal = 80;
            else if (NumOfOverdue == 3)
                retVal = 70;
            else if (NumOfOverdue == 4)
                retVal = 60;
            else
                retVal = 50;

            return retVal * NumOfOverdueWF;
        }

        public double GetSlowSalesRateScore(double SlowSalesRate)
        {
            double retVal;
            if (SlowSalesRate == 0)
                retVal = 100;
            else if (SlowSalesRate == 0.05)
                retVal = 90;
            else if (SlowSalesRate == 0.1)
                retVal = 80;
            else if (SlowSalesRate == 0.15)
                retVal = 70;
            else if (SlowSalesRate == 0.2)
                retVal = 60;
            else
                retVal = 50;

            return retVal * SlowSalesRateWF;
        }

        public double GetReturnRateScore(double ReturnRate)
        {
            double retVal;
            if (ReturnRate == 0)
                retVal = 0;
            else if (ReturnRate >= 1 && ReturnRate < 5)
                retVal = 50;
            else if (ReturnRate >= 5 && ReturnRate < 10)
                retVal = 60;
            else if (ReturnRate >= 10 && ReturnRate < 15)
                retVal = 70;
            else if (ReturnRate >= 15 && ReturnRate < 20)
                retVal = 80;
            else if (ReturnRate >= 20 & ReturnRate < 25)
                retVal = 90;
            else
                retVal = 100;

            return retVal * ReturnRateWF;
        }

        public double GetBreakageRateScore(double BreakageRate)
        {
            double retVal;
            if (BreakageRate == 0)
                retVal = 100;
            else if (BreakageRate == 0.05)
                retVal = 90;
            else if (BreakageRate == 0.1)
                retVal = 80;
            else if (BreakageRate == 0.15)
                retVal = 70;
            else if (BreakageRate == 0.2)
                retVal = 60;
            else
                retVal = 50;

            return retVal * BreakageRateWF;
        }

    }
}