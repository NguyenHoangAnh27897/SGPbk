using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGP.Models;
using System.Data.SqlClient;
using PagedList;
using System.IO;
using OfficeOpenXml;

namespace SGP.Controllers
{
    [Authorize]
    public class ExportController : Controller
    {
        //
        // GET: /Export/
        SGPAPIEntities db = new SGPAPIEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotMailerDetails()
        {
            return View();
        }
        //[Authorize(Roles="Reporter")]
        //Xuat du lieu nhung CG chưa lên sổ phát
        public ActionResult ExcelNotMailerDetails(string FromDate, string ToDate, string ZoneID = "")
        {
           

            string pathRoot = Server.MapPath("~/Report/notmailerdetails.xlsx");
            string name = "notdelivery_details" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }
               
                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                var zoneid = new SqlParameter("@ZoneID", ZoneID);
                List<ResponeNotMailerDetail> list = new List<ResponeNotMailerDetail>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeNotMailerDetail>("SGP_WEB_CGChuaXuLy_ChiTiet @FromDate,@ToDate,@ZoneID", parafrom, parato, zoneid).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeNotMailerDetail()
                        {
                            PostOfficeName = item.PostOfficeName,
                            MailerID = item.MailerID,
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            ServiceTypeID = item.ServiceTypeID,
                            ChenhLech = item.ChenhLech

                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = list[i].PostOfficeName;
                            worksheet.Cells[i + 2, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 2, 4].Value = list[i].ServiceTypeID;
                            worksheet.Cells[i + 2, 5].Value = list[i].ChenhLech;

                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("CGchuaxuly" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        //so lieu tho
        public ActionResult MailerByDate()
        {
            return View();
        }
        //[Authorize(Roles = "Reporter")]
        public ActionResult ExcelMailerByDate(string FromDate, string ToDate, int opt = 0, int zone = 0)
        {
            ViewBag.OPT = opt;
            ViewBag.ZONE = zone;
            string pathRoot = Server.MapPath("~/Report/mailerbydate.xlsx");
            string name = "mailerbydate" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);
            string KV1 = "KV1";
            string KV2 = "KV2";
            string KV3 = "KV3";
            string KV4 = "KV4";

            System.IO.File.Copy(pathRoot, pathTo);

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                SqlParameter parazone = new SqlParameter("@ZoneID", KV1); 
                if(zone == 0){
                    parazone = new SqlParameter("@ZoneID", KV1);
                }else if(zone == 1){
                    parazone = new SqlParameter("@ZoneID", KV2);
                }else if(zone == 2){
                    parazone = new SqlParameter("@ZoneID", KV3);
                }else if(zone == 3){
                    parazone = new SqlParameter("@ZoneID", KV4);
                }
                List<ResponseMailerByDate> list = new List<ResponseMailerByDate>();
                if (FromDate != "" && ToDate != "")
                {
                    if (opt == 1)
                    {
                        var result = db.Database.SqlQuery<ResponseMailerByDate>("SGP_WEB_MailerBySaleDate @FromDate, @ToDate, @ZoneID", parafrom, parato, parazone).ToList();
                        foreach (var item in result)
                        {
                            list.Add(new ResponseMailerByDate()
                            {
                                AcceptDate = item.AcceptDate,
                                MailerID = item.MailerID,
                                SenderID = item.SenderID,
                                SenderName = item.SenderName,
                                SenderProvinceID = item.SenderProvinceID,
                                ReceiveProvinceID = item.ReceiveProvinceID,
                                RecieverDistrictID = item.RecieverDistrictID,
                                ServiceTypeID = item.ServiceTypeID,
                                MailerTypeID = item.MailerTypeID,
                                Quantity = item.Quantity,
                                RealWeight = item.RealWeight,
                                Weight = item.Weight,
                                Money = item.Money,
                                Price = item.Price,
                                PriceDefault = item.PriceDefault,
                                PriceService = item.PriceService,
                                Discount = item.Discount,
                                BefVATAmount = item.BefVATAmount,
                                VATPercent = item.VATPercent,
                                VATAmount = item.VATAmount,
                                Amount = item.Amount,
                                AmountBefDiscount = item.AmountBefDiscount,
                                PostOfficeAcceptID = item.PostOfficeAcceptID,
                                PaymentMethodID = item.PaymentMethodID,
                                PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                                MailerDescription = item.MailerDescription,
                                ThirdpartyDocID = item.ThirdpartyDocID,
                                ThirdpartyCost = item.ThirdpartyCost,
                                CommissionAmt = item.CommissionAmt,
                                CommissionPercent = item.CommissionPercent,
                                CostAmt = item.CostAmt,
                                SalesClosingDate = item.SalesClosingDate,
                                RecieverProvinceID = item.RecieverProvinceID,
                                DiscountPercent = item.DiscountPercent,
                                PostOfficeID = item.PostOfficeID,
                                PostOfficeName = item.PostOfficeName,
                                ZoneID = item.ZoneID,

                            });
                        }
                    }
                    else
                    {
                        var result = db.Database.SqlQuery<ResponseMailerByDate>("SGP_WEB_MailerByDate @FromDate, @ToDate, @ZoneID", parafrom, parato, parazone).ToList();
                        foreach (var item in result)
                        {
                            list.Add(new ResponseMailerByDate()
                            {
                                AcceptDate = item.AcceptDate,
                                MailerID = item.MailerID,
                                SenderID = item.SenderID,
                                SenderName = item.SenderName,
                                SenderProvinceID = item.SenderProvinceID,
                                ReceiveProvinceID = item.ReceiveProvinceID,
                                RecieverDistrictID = item.RecieverDistrictID,
                                ServiceTypeID = item.ServiceTypeID,
                                MailerTypeID = item.MailerTypeID,
                                Quantity = item.Quantity,
                                RealWeight = item.RealWeight,
                                Weight = item.Weight,
                                Money = item.Money,
                                Price = item.Price,
                                PriceDefault = item.PriceDefault,
                                PriceService = item.PriceService,
                                Discount = item.Discount,
                                BefVATAmount = item.BefVATAmount,
                                VATPercent = item.VATPercent,
                                VATAmount = item.VATAmount,
                                Amount = item.Amount,
                                AmountBefDiscount = item.AmountBefDiscount,
                                PostOfficeAcceptID = item.PostOfficeAcceptID,
                                PaymentMethodID = item.PaymentMethodID,
                                PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                                MailerDescription = item.MailerDescription,
                                ThirdpartyDocID = item.ThirdpartyDocID,
                                ThirdpartyCost = item.ThirdpartyCost,
                                CommissionAmt = item.CommissionAmt,
                                CommissionPercent = item.CommissionPercent,
                                CostAmt = item.CostAmt,
                                SalesClosingDate = item.SalesClosingDate,
                                RecieverProvinceID = item.RecieverProvinceID,
                                DiscountPercent = item.DiscountPercent,
                                PostOfficeID = item.PostOfficeID,
                                PostOfficeName = item.PostOfficeName,
                                ZoneID = item.ZoneID,

                            });
                        }
                    } 
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList(); 
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 2, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = list[i].SenderID;
                            worksheet.Cells[i + 2, 4].Value = list[i].SenderName;
                            worksheet.Cells[i + 2, 5].Value = list[i].SenderProvinceID;
                            worksheet.Cells[i + 2, 6].Value = list[i].ReceiveProvinceID;
                            worksheet.Cells[i + 2, 7].Value = list[i].RecieverDistrictID;

                            worksheet.Cells[i + 2, 8].Value = list[i].ServiceTypeID;
                            worksheet.Cells[i + 2, 9].Value = list[i].MailerTypeID;
                            worksheet.Cells[i + 2, 10].Value = list[i].Quantity;
                            worksheet.Cells[i + 2, 11].Value = list[i].RealWeight;
                            worksheet.Cells[i + 2, 12].Value = list[i].Weight;
                            worksheet.Cells[i + 2, 13].Value = list[i].Money;
                            worksheet.Cells[i + 2, 14].Value = list[i].Price;

                            worksheet.Cells[i + 2, 15].Value = list[i].PriceDefault;
                            worksheet.Cells[i + 2, 16].Value = list[i].PriceService;
                            worksheet.Cells[i + 2, 17].Value = list[i].Discount;
                            worksheet.Cells[i + 2, 18].Value = list[i].BefVATAmount;
                            worksheet.Cells[i + 2, 19].Value = list[i].VATPercent;
                            worksheet.Cells[i + 2, 20].Value = list[i].VATAmount;
                            worksheet.Cells[i + 2, 21].Value = list[i].Amount;

                            worksheet.Cells[i + 2, 22].Value = list[i].AmountBefDiscount;
                            worksheet.Cells[i + 2, 23].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 2, 24].Value = list[i].PaymentMethodID;
                            worksheet.Cells[i + 2, 25].Value = list[i].PostOfficeRecieverMoneyID;
                            worksheet.Cells[i + 2, 26].Value = list[i].MailerDescription;
                            worksheet.Cells[i + 2, 27].Value = list[i].ThirdpartyDocID;
                            worksheet.Cells[i + 2, 28].Value = list[i].ThirdpartyCost;

                            worksheet.Cells[i + 2, 29].Value = list[i].CommissionAmt;
                            worksheet.Cells[i + 2, 30].Value = list[i].CommissionPercent;
                            worksheet.Cells[i + 2, 31].Value = list[i].CostAmt;
                            worksheet.Cells[i + 2, 32].Value = list[i].SalesClosingDate;
                            worksheet.Cells[i + 2, 33].Value = list[i].RecieverProvinceID;
                            worksheet.Cells[i + 2, 34].Value = list[i].DiscountPercent;
                            worksheet.Cells[i + 2, 35].Value = list[i].PostOfficeID;

                            worksheet.Cells[i + 2, 36].Value = list[i].PostOfficeName;
                            worksheet.Cells[i + 2, 37].Value = list[i].ZoneID;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("solieutho" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        //san luong dong nhan cac khu vuc
        public ActionResult M_PackingList()
        {
            return View();
        }
        //view chi tiet cac CG chua dc len so phát
        public ActionResult NotDelivery_Details()
        {
            return View();
        }
        public ActionResult ExcelNotDeliveryDetails(string FromDate, string ToDate, string ZoneID = "")
        {

            string pathRoot = Server.MapPath("~/Report/notdelivery_details.xlsx");
            string name = "notdelivery_details" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                var zoneid = new SqlParameter("@ZoneID", ZoneID);
                List<ResponseNotDeliveryDetail> list = new List<ResponseNotDeliveryDetail>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponseNotDeliveryDetail>("SGP_WEB_BaoCaoPhat_ChiTiet @FromDate,@ToDate,@ZoneID", parafrom, parato, zoneid).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponseNotDeliveryDetail()
                        {
                            EmployeeID = item.EmployeeID,
                            EmployeeName = item.EmployeeName,
                            MailerID = item.MailerID,
                            AcceptDate = item.AcceptDate,
                            Weight = item.Weight,
                            PostOfficeName = item.PostOfficeName,
                            StatusID = item.StatusID,
                            StatusName = item.StatusName,
                            ServiceTypeName = item.ServiceTypeName,
                            MailerTypeName = item.MailerTypeName,
                            CurrentPostOffice = item.CurrentPostOffice,
                            ThoiGian = item.ThoiGian

                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].EmployeeID;
                            worksheet.Cells[i + 3, 2].Value = list[i].EmployeeName;
                            worksheet.Cells[i + 3, 3].Value = list[i].MailerID;
                            worksheet.Cells[i + 3, 4].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 3, 5].Value = list[i].Weight;
                            worksheet.Cells[i + 3, 6].Value = list[i].PostOfficeName;

                            worksheet.Cells[i + 3, 7].Value = list[i].StatusID;

                            worksheet.Cells[i + 3, 8].Value = list[i].StatusName;

                            worksheet.Cells[i + 3, 9].Value = list[i].ServiceTypeName;
                            worksheet.Cells[i + 3, 10].Value = list[i].MailerTypeName;
                            worksheet.Cells[i + 3, 11].Value = list[i].CurrentPostOffice;
                            worksheet.Cells[i + 3, 12].Value = list[i].ThoiGian;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("excel" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }             
        public ActionResult M_Mailers28()
        {
            return View();
        }
        //[Authorize(Roles = "Reporter")]
        public ActionResult ExcelMailerDelivery_Master_2kg_8kg(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/mailerdelivery_master_2kg_8kg.xlsx");
            string name = "mailerdelivery_master_2kg_8kg" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                List<ResponeMailerDeliveryMaster2kg8kg> list = new List<ResponeMailerDeliveryMaster2kg8kg>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeMailerDeliveryMaster2kg8kg>("SGP_WEB_MailerDelivery_Master_2kg_8kg @FromDate,@ToDate", parafrom, parato).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeMailerDeliveryMaster2kg8kg()
                        {
                            BC = item.BC,
                            Duoi_2kg_CG = item.Duoi_2kg_CG,
                            Duoi_2kg_SL = item.Duoi_2kg_SL,
                            Duoi_2kg_TL = item.Duoi_2kg_TL,
                            Duoi_2kg_TLK = item.Duoi_2kg_TLK,
                            Tren_2kg_CG = item.Tren_2kg_CG,
                            Tren_2kg_SL = item.Tren_2kg_SL,
                            Tren_2kg_TL = item.Tren_2kg_TL,
                            Tren_2kg_TLK = item.Tren_2kg_TLK,
                            Tren_8kg_CG = item.Tren_8kg_CG,
                            Tren_8kg_SL = item.Tren_8kg_SL,
                            Tren_8kg_TL = item.Tren_8kg_TL,
                            Tren_8kg_TLK = item.Tren_8kg_TLK,
                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].BC;
                            worksheet.Cells[i + 3, 2].Value = list[i].Duoi_2kg_CG;
                            worksheet.Cells[i + 3, 3].Value = list[i].Duoi_2kg_SL;
                            worksheet.Cells[i + 3, 4].Value = list[i].Duoi_2kg_TL;
                            worksheet.Cells[i + 3, 5].Value = list[i].Duoi_2kg_TLK;

                            worksheet.Cells[i + 3, 6].Value = list[i].Tren_2kg_CG;
                            worksheet.Cells[i + 3, 7].Value = list[i].Tren_2kg_SL;
                            worksheet.Cells[i + 3, 8].Value = list[i].Tren_2kg_TL;
                            worksheet.Cells[i + 3, 9].Value = list[i].Tren_2kg_TLK;

                            worksheet.Cells[i + 3, 10].Value = list[i].Tren_8kg_CG;
                            worksheet.Cells[i + 3, 11].Value = list[i].Tren_8kg_SL;
                            worksheet.Cells[i + 3, 12].Value = list[i].Tren_8kg_TL;
                            worksheet.Cells[i + 3, 13].Value = list[i].Tren_8kg_TLK;

                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("mailerdelivery_master_2kg_8kg" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        //[Authorize(Roles = "Reporter")]
        public ActionResult ExcelMailerDelivery_Detail_2kg_8kg(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/mailerdelivery_detail_2kg_8kg.xlsx");
            string name = "mailerdelivery_detail_2kg_8kg" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                List<ResponeMailerDeliveryDetail2kg8kg> list = new List<ResponeMailerDeliveryDetail2kg8kg>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeMailerDeliveryDetail2kg8kg>("SGP_WEB_MailerDelivery_Detail_2kg_8kg @FromDate,@ToDate", parafrom, parato).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeMailerDeliveryDetail2kg8kg()
                        {
                            BC = item.BC,
                            EmployeeID = item.EmployeeID,
                            EmployeeName = item.EmployeeName,
                            Duoi_2kg_SL = item.Duoi_2kg_SL,
                            Duoi_2kg_TL = item.Duoi_2kg_TL,
                            Tren_2kg_SL = item.Tren_2kg_SL,
                            Tren_2kg_TL = item.Tren_2kg_TL,
                            Tren_8kg_SL = item.Tren_8kg_SL,
                            Tren_8kg_TL = item.Tren_8kg_TL,
                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].BC;
                            worksheet.Cells[i + 3, 2].Value = list[i].EmployeeID;
                            worksheet.Cells[i + 3, 3].Value = list[i].EmployeeName;
                            worksheet.Cells[i + 3, 4].Value = list[i].Duoi_2kg_SL;
                            worksheet.Cells[i + 3, 5].Value = list[i].Duoi_2kg_TL;
                            worksheet.Cells[i + 3, 6].Value = list[i].Tren_2kg_SL;

                            worksheet.Cells[i + 3, 7].Value = list[i].Tren_2kg_TL;

                            worksheet.Cells[i + 3, 8].Value = list[i].Tren_8kg_SL;

                            worksheet.Cells[i + 3, 9].Value = list[i].Tren_8kg_TL;

                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("mailerdelivery_detail_2kg_8kg" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        public  ActionResult MailerDelivery_Employee_Zone1()
        {
            return View();
        }
        public ActionResult ExcelMailerDelivery_Employee_Zone1(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/hb_ctv_kv1.xlsx");
            string name = "hb_ctv_kv1" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                List<ResponeDeliveryEmployeeZone1> list = new List<ResponeDeliveryEmployeeZone1>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeDeliveryEmployeeZone1>("SGP_WEB_HoiBao_CTV_KV1 @FromDate,@ToDate", parafrom, parato).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeDeliveryEmployeeZone1()
                        {
                            BCGoc= item.BCGoc,
                            MailerID= item.MailerID,
                            ChuyenThu = item.ChuyenThu,
                            Quantity = item.Quantity,
                            Weight = item.Weight,
                            RealWeight= item.RealWeight,
                            ServiceTypeID = item.ServiceTypeID,
                            PostOfficeID = item.PostOfficeID,
                            EmployeeID= item.EmployeeID,
                            NgayGui = item.NgayGui,
                            DeliveryTo = item.DeliveryTo,
                            NgayNhan = item.NgayNhan,
                            GioNhan= item.GioNhan,
                            MailerTypeID = item.MailerTypeID,
                            MailerDescription = item.MailerDescription
                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].BCGoc;
                            worksheet.Cells[i + 3, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 3, 3].Value = list[i].ChuyenThu;
                            worksheet.Cells[i + 3, 4].Value = list[i].Quantity;
                            worksheet.Cells[i + 3, 5].Value = list[i].Weight;
                            worksheet.Cells[i + 3, 6].Value = list[i].RealWeight;
                            worksheet.Cells[i + 3, 7].Value = list[i].ServiceTypeID;
                            worksheet.Cells[i + 3, 8].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 3, 9].Value = list[i].EmployeeID;

                            worksheet.Cells[i + 3, 10].Value = list[i].NgayGui;
                            worksheet.Cells[i + 3, 11].Value = list[i].DeliveryTo;
                            worksheet.Cells[i + 3, 12].Value = list[i].NgayNhan;
                            worksheet.Cells[i + 3, 13].Value = list[i].GioNhan;
                            worksheet.Cells[i + 3, 14].Value = list[i].MailerTypeID;
                            worksheet.Cells[i + 3, 15].Value = list[i].MailerDescription;

                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("hb_ctv_kv1" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        public ActionResult ExcelMailerDelivery_PostOffice_Zone1(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/hb_ctv_kv1.xlsx");
            string name = "hb_bc_kv1" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                List<ResponeDeliveryEmployeeZone1> list = new List<ResponeDeliveryEmployeeZone1>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeDeliveryEmployeeZone1>("SGP_WEB_HoiBao_BC_KV1 @FromDate,@ToDate", parafrom, parato).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeDeliveryEmployeeZone1()
                        {
                            BCGoc = item.BCGoc,
                            MailerID = item.MailerID,
                            ChuyenThu = item.ChuyenThu,
                            Quantity = item.Quantity,
                            Weight = item.Weight,
                            RealWeight = item.RealWeight,
                            ServiceTypeID = item.ServiceTypeID,
                            PostOfficeID = item.PostOfficeID,
                            EmployeeID = item.EmployeeID,
                            NgayGui = item.NgayGui,
                            DeliveryTo = item.DeliveryTo,
                            NgayNhan = item.NgayNhan,
                            GioNhan = item.GioNhan,
                            MailerTypeID = item.MailerTypeID,
                            MailerDescription = item.MailerDescription
                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].BCGoc;
                            worksheet.Cells[i + 3, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 3, 3].Value = list[i].ChuyenThu;
                            worksheet.Cells[i + 3, 4].Value = list[i].Quantity;
                            worksheet.Cells[i + 3, 5].Value = list[i].Weight;
                            worksheet.Cells[i + 3, 6].Value = list[i].RealWeight;
                            worksheet.Cells[i + 3, 7].Value = list[i].ServiceTypeID;
                            worksheet.Cells[i + 3, 8].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 3, 9].Value = list[i].EmployeeID;

                            worksheet.Cells[i + 3, 10].Value = list[i].NgayGui;
                            worksheet.Cells[i + 3, 11].Value = list[i].DeliveryTo;
                            worksheet.Cells[i + 3, 12].Value = list[i].NgayNhan;
                            worksheet.Cells[i + 3, 13].Value = list[i].GioNhan;
                            worksheet.Cells[i + 3, 14].Value = list[i].MailerTypeID;
                            worksheet.Cells[i + 3, 15].Value = list[i].MailerDescription;

                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("hb_bc_kv1" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        public ActionResult SGP_WEB_TongCG_BC(string FromDate, string ToDate, string PostOfficeID)
        {

            string pathRoot = Server.MapPath("~/Report/notconfirmsender.xlsx");
            string name = "notconfirmsender" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                var postid = new SqlParameter("@PostOfficeID", PostOfficeID);
                List<ResponeTongCG_BC> list = new List<ResponeTongCG_BC>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponeTongCG_BC>("SGP_WEB_TongCG_BC @FromDate,@ToDate,@PostOfficeID", parafrom, parato, postid).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponeTongCG_BC()
                        {
                            MailerID = item.MailerID,
                            AcceptDate = item.AcceptDate,
                            SenderID = item.SenderID,
                            SenderName = item.SenderName,
                            Quantity = item.Quantity,
                            Weight = item.Weight,
                            ReceiveProvinceID = item.ReceiveProvinceID,
                            ServiceTypeID = item.ServiceTypeID,
                            Price = item.Price,
                            MailerDescription = item.MailerDescription,
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            MailerTypeID = item.MailerTypeID

                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 2].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 2, 3].Value = list[i].SenderID;
                            worksheet.Cells[i + 2, 4].Value = list[i].SenderName;
                            worksheet.Cells[i + 2, 5].Value = list[i].Quantity;
                            worksheet.Cells[i + 2, 6].Value = list[i].Weight;

                            worksheet.Cells[i + 2, 7].Value = list[i].ReceiveProvinceID;

                            worksheet.Cells[i + 2, 8].Value = list[i].ServiceTypeID;

                            worksheet.Cells[i + 2, 9].Value = list[i].Price;
                            worksheet.Cells[i + 2, 10].Value = list[i].MailerDescription;
                            worksheet.Cells[i + 2, 11].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 2, 12].Value = list[i].MailerTypeID;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("notconfirmsender" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }

        public ActionResult MailerDelivery()
        {
            return View();
        }

        public ActionResult ExcelMailerDelivery(string FromDate, string ToDate, int opt = 0, int zone = 0)
        {
            ViewBag.OPT = opt;
            ViewBag.ZONE = zone;
            string pathRoot = Server.MapPath("~/Report/mailerbydatedelivery.xlsx");
            string name = "mailerbydatedelivery" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);
            string KV1 = "KV1";
            string KV2 = "KV2";
            string KV3 = "KV3";
            string KV4 = "KV4";

            System.IO.File.Copy(pathRoot, pathTo);

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string fDate;
                string fTo;

                if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
                {
                    fDate = DateTime.Now.ToString("yyyy-MM-dd");
                    fTo = DateTime.Now.ToString("yyyy-MM-dd");
                    FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                    ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    fDate = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                    fTo = DateTime.ParseExact(Request["ToDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                SqlParameter parazone = new SqlParameter("@ZoneID", KV1);
                if (zone == 0)
                {
                    parazone = new SqlParameter("@ZoneID", KV1);
                }
                else if (zone == 1)
                {
                    parazone = new SqlParameter("@ZoneID", KV2);
                }
                else if (zone == 2)
                {
                    parazone = new SqlParameter("@ZoneID", KV3);
                }
                else if (zone == 3)
                {
                    parazone = new SqlParameter("@ZoneID", KV4);
                }
                List<ResponseMailerByDateDelivery> list = new List<ResponseMailerByDateDelivery>();
                if (FromDate != "" && ToDate != "")
                {
                    if (opt == 1)
                    {
                        var result = db.Database.SqlQuery<ResponseMailerByDateDelivery>("SGP_WEB_MailerBySaleDateDelivery @FromDate, @ToDate, @ZoneID", parafrom, parato, parazone).ToList();
                        foreach (var item in result)
                        {
                            list.Add(new ResponseMailerByDateDelivery()
                            {
                                AcceptDate = item.AcceptDate,
                                MailerID = item.MailerID,
                                SenderID = item.SenderID,
                                SenderName = item.SenderName,
                                SenderProvinceID = item.SenderProvinceID,
                                ReceiveProvinceID = item.ReceiveProvinceID,
                                RecieverDistrictID = item.RecieverDistrictID,
                                ServiceTypeID = item.ServiceTypeID,
                                MailerTypeID = item.MailerTypeID,
                                Quantity = item.Quantity,
                                RealWeight = item.RealWeight,
                                Weight = item.Weight,
                                Money = item.Money,
                                Price = item.Price,
                                PriceDefault = item.PriceDefault,
                                PriceService = item.PriceService,
                                Discount = item.Discount,
                                BefVATAmount = item.BefVATAmount,
                                VATPercent = item.VATPercent,
                                VATAmount = item.VATAmount,
                                Amount = item.Amount,
                                AmountBefDiscount = item.AmountBefDiscount,
                                PostOfficeAcceptID = item.PostOfficeAcceptID,
                                PaymentMethodID = item.PaymentMethodID,
                                PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                                MailerDescription = item.MailerDescription,
                                ThirdpartyDocID = item.ThirdpartyDocID,
                                ThirdpartyCost = item.ThirdpartyCost,
                                CommissionAmt = item.CommissionAmt,
                                CommissionPercent = item.CommissionPercent,
                                CostAmt = item.CostAmt,
                                SalesClosingDate = item.SalesClosingDate,
                                RecieverProvinceID = item.RecieverProvinceID,
                                DiscountPercent = item.DiscountPercent,
                                PostOfficeID = item.PostOfficeID,
                                PostOfficeName = item.PostOfficeName,
                                ZoneID = item.ZoneID,
                                DeliveryPostOfficeID = item.DeliveryPostOfficeID,
                                EmployeeID = item.EmployeeID
                            });
                        }
                    }
                    else
                    {
                        var result = db.Database.SqlQuery<ResponseMailerByDateDelivery>("SGP_WEB_MailerByDateDelivery @FromDate, @ToDate, @ZoneID", parafrom, parato, parazone).ToList();
                        foreach (var item in result)
                        {
                            list.Add(new ResponseMailerByDateDelivery()
                            {
                                AcceptDate = item.AcceptDate,
                                MailerID = item.MailerID,
                                SenderID = item.SenderID,
                                SenderName = item.SenderName,
                                SenderProvinceID = item.SenderProvinceID,
                                ReceiveProvinceID = item.ReceiveProvinceID,
                                RecieverDistrictID = item.RecieverDistrictID,
                                ServiceTypeID = item.ServiceTypeID,
                                MailerTypeID = item.MailerTypeID,
                                Quantity = item.Quantity,
                                RealWeight = item.RealWeight,
                                Weight = item.Weight,
                                Money = item.Money,
                                Price = item.Price,
                                PriceDefault = item.PriceDefault,
                                PriceService = item.PriceService,
                                Discount = item.Discount,
                                BefVATAmount = item.BefVATAmount,
                                VATPercent = item.VATPercent,
                                VATAmount = item.VATAmount,
                                Amount = item.Amount,
                                AmountBefDiscount = item.AmountBefDiscount,
                                PostOfficeAcceptID = item.PostOfficeAcceptID,
                                PaymentMethodID = item.PaymentMethodID,
                                PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                                MailerDescription = item.MailerDescription,
                                ThirdpartyDocID = item.ThirdpartyDocID,
                                ThirdpartyCost = item.ThirdpartyCost,
                                CommissionAmt = item.CommissionAmt,
                                CommissionPercent = item.CommissionPercent,
                                CostAmt = item.CostAmt,
                                SalesClosingDate = item.SalesClosingDate,
                                RecieverProvinceID = item.RecieverProvinceID,
                                DiscountPercent = item.DiscountPercent,
                                PostOfficeID = item.PostOfficeID,
                                PostOfficeName = item.PostOfficeName,
                                ZoneID = item.ZoneID,
                                DeliveryPostOfficeID = item.DeliveryPostOfficeID,
                                EmployeeID = item.EmployeeID
                            });
                        }
                    }
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList(); 
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 2, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = list[i].SenderID;
                            worksheet.Cells[i + 2, 4].Value = list[i].SenderName;
                            worksheet.Cells[i + 2, 5].Value = list[i].SenderProvinceID;
                            worksheet.Cells[i + 2, 6].Value = list[i].ReceiveProvinceID;
                            worksheet.Cells[i + 2, 7].Value = list[i].RecieverDistrictID;

                            worksheet.Cells[i + 2, 8].Value = list[i].ServiceTypeID;
                            worksheet.Cells[i + 2, 9].Value = list[i].MailerTypeID;
                            worksheet.Cells[i + 2, 10].Value = list[i].Quantity;
                            worksheet.Cells[i + 2, 11].Value = list[i].RealWeight;
                            worksheet.Cells[i + 2, 12].Value = list[i].Weight;
                            worksheet.Cells[i + 2, 13].Value = list[i].Money;
                            worksheet.Cells[i + 2, 14].Value = list[i].Price;

                            worksheet.Cells[i + 2, 15].Value = list[i].PriceDefault;
                            worksheet.Cells[i + 2, 16].Value = list[i].PriceService;
                            worksheet.Cells[i + 2, 17].Value = list[i].Discount;
                            worksheet.Cells[i + 2, 18].Value = list[i].BefVATAmount;
                            worksheet.Cells[i + 2, 19].Value = list[i].VATPercent;
                            worksheet.Cells[i + 2, 20].Value = list[i].VATAmount;
                            worksheet.Cells[i + 2, 21].Value = list[i].Amount;

                            worksheet.Cells[i + 2, 22].Value = list[i].AmountBefDiscount;
                            worksheet.Cells[i + 2, 23].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 2, 24].Value = list[i].PaymentMethodID;
                            worksheet.Cells[i + 2, 25].Value = list[i].PostOfficeRecieverMoneyID;
                            worksheet.Cells[i + 2, 26].Value = list[i].MailerDescription;
                            worksheet.Cells[i + 2, 27].Value = list[i].ThirdpartyDocID;
                            worksheet.Cells[i + 2, 28].Value = list[i].ThirdpartyCost;

                            worksheet.Cells[i + 2, 29].Value = list[i].CommissionAmt;
                            worksheet.Cells[i + 2, 30].Value = list[i].CommissionPercent;
                            worksheet.Cells[i + 2, 31].Value = list[i].CostAmt;
                            worksheet.Cells[i + 2, 32].Value = list[i].SalesClosingDate;
                            worksheet.Cells[i + 2, 33].Value = list[i].RecieverProvinceID;
                            worksheet.Cells[i + 2, 34].Value = list[i].DiscountPercent;
                            worksheet.Cells[i + 2, 35].Value = list[i].PostOfficeID;

                            worksheet.Cells[i + 2, 36].Value = list[i].PostOfficeName;
                            worksheet.Cells[i + 2, 37].Value = list[i].ZoneID;
                            worksheet.Cells[i + 2, 38].Value = list[i].DeliveryPostOfficeID;
                            worksheet.Cells[i + 2, 39].Value = list[i].EmployeeID;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }

            }
            catch
            {
                return RedirectToAction("error", "home");
            }


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("solieuthophat" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
	} 
}