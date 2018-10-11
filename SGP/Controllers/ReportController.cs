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
using CrystalDecisions.CrystalReports.Engine;

namespace SGP.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        PMSSGP_200911Entities pms = new PMSSGP_200911Entities();
        SGPAPIEntities db = new SGPAPIEntities();
        PMS_TESTEntities1 test = new PMS_TESTEntities1();

        public ActionResult Index(string FromDate, string ToDate)
        {

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
                fDate = DateTime.Parse(Request["FromDate"]).ToString("yyyy-MM-dd");
                fTo = DateTime.Parse(Request["ToDate"]).ToString("yyyy-MM-dd");
            }

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            List<ResponeMailers> list = new List<ResponeMailers>();
            if (FromDate != "" && ToDate != "")
            {

                //var result = db.Database.SqlQuery<ResponeMailers>("SGP_WEB_Mailer @Fromdate,@Todate", Fromdate, Todate).ToList();
                var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeMailers()
                    {
                        PostOfficeID = item.PostOfficeAcceptID,
                        TongCG = item.TongCG,
                        TongSL = item.TongSL,
                        TongTL = item.TongTL,
                    });
                }
            }

            return View(list);
        }
        [Authorize]
        public ActionResult GetMailer(string FromDate, string ToDate)
        {
            if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate))
            {
                FromDate = DateTime.Now.ToString("yyyy-MM-dd");
                ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                FromDate = DateTime.Parse(Request["FromDate"]).ToString("yyyy-MM-dd");
                ToDate = DateTime.Parse(Request["ToDate"]).ToString("yyyy-MM-dd");
            }

            List<ResponeMailers> list = new List<ResponeMailers>();
            if (FromDate != "" && ToDate != "")
            {
                var Fromdate = new SqlParameter("@Fromdate", FromDate);
                var Todate = new SqlParameter("@Todate", ToDate);
                //var result = db.Database.SqlQuery<ResponeMailers>("SGP_WEB_Mailer @Fromdate,@Todate", Fromdate, Todate).ToList();
                var result = db.SGP_WEB_Mailer(FromDate, ToDate).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeMailers()
                    {
                        PostOfficeID = item.PostOfficeAcceptID,
                        TongCG = item.TongCG,
                        TongSL = item.TongSL,
                        TongTL = item.TongTL,
                    });
                }
            }
            return Json(list.Select(p => new { PostOfficeID = p.PostOfficeID, TongCG = p.TongCG, TongSL = p.TongSL, TongTL = p.TongTL }), JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult PostOffice(string FromDate, string ToDate, string ZoneID = "")
        {

            string fDate;
            string fTo;
            string fZone;

            if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate) || String.IsNullOrEmpty(ZoneID))
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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ZoneID = ZoneID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazoneid = new SqlParameter("@ZoneID", ZoneID);
            List<ResponsePostOffice> list = new List<ResponsePostOffice>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponsePostOffice>("SGP_WEB_PostOffice @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponsePostOffice()
                    {
                        MBC = item.MBC,
                        BC = item.BC,
                        SL = item.SL,
                        TL = item.TL,
                    });
                }
            }

            return View(list);
        }
        public ActionResult PostOfficeAmount(string FromDate, string ToDate, int? page)
        {

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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            List<ResponePostOfficeAmount> list = new List<ResponePostOfficeAmount>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponePostOfficeAmount>("SGP_WEB_PostOfficeAmount @FromDate,@ToDate", parafrom, parato).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponePostOfficeAmount()
                    {
                        STT = item.STT,
                        PostOfficeID = item.PostOfficeID,
                        PostOfficeName = item.PostOfficeName,
                        TotalMailer = item.TotalMailer,
                        TotalQuantity = item.TotalQuantity,
                        TotalWeight = item.TotalWeight,
                        BefVATAmount = item.BefVATAmount,
                        Amount = item.Amount,
                    });
                }
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));

            // return View(list);
        }
        [Authorize(Roles = "Reporter")]
        public ActionResult ExcelPostOfficeAmount(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/postofficeamount.xlsx");
            string name = "postofficeamount" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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
                List<ResponePostOfficeAmount> list = new List<ResponePostOfficeAmount>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponePostOfficeAmount>("SGP_WEB_PostOfficeAmount @FromDate,@ToDate", parafrom, parato).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponePostOfficeAmount()
                        {
                            STT = item.STT,
                            PostOfficeID = item.PostOfficeID,
                            PostOfficeName = item.PostOfficeName,
                            TotalMailer = item.TotalMailer,
                            TotalQuantity = item.TotalQuantity,
                            TotalWeight = item.TotalWeight,
                            BefVATAmount = item.BefVATAmount,
                            Amount = item.Amount
                          
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
                            worksheet.Cells[i + 2, 1].Value = list[i].STT;
                            worksheet.Cells[i + 2, 2].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 2, 3].Value = list[i].PostOfficeName;
                            worksheet.Cells[i + 2, 4].Value = list[i].TotalMailer;

                            worksheet.Cells[i + 2, 5].Value = list[i].TotalQuantity;

                            worksheet.Cells[i + 2, 6].Value = list[i].TotalWeight;

                            worksheet.Cells[i + 2, 7].Value = list[i].BefVATAmount;
                            worksheet.Cells[i + 2, 8].Value = list[i].Amount;
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
        [Authorize]
        public ActionResult MailerDeliveryTime(string FromDate, string ToDate, int? page, string ZoneID = "")
        {

            string fDate;
            string fTo;
            string fZone;

            if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate) || String.IsNullOrEmpty(ZoneID))
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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ZoneID = ZoneID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazoneid = new SqlParameter("@ZoneID", ZoneID);
            List<ResponseDeliveryTime> list = new List<ResponseDeliveryTime>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponseDeliveryTime>("SGP_WEB_MailerDeliveryTime @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponseDeliveryTime()
                    {
                        DocumentID = item.DocumentID,
                        DocumentTime = item.DocumentTime,
                        PostOfficeID = item.PostOfficeID,
                        Quantity = item.Quantity,
                        Weight = item.Weight,
                        EmployeeID = item.EmployeeID,
                        Time = item.Time
                    });
                }
            }
            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
            //return View(list);
        }
        public ActionResult MailerDelivery(string FromDate, string ToDate, string ZoneID = "")
        {

            string fDate;
            string fTo;
            string fZone;

            if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate) || String.IsNullOrEmpty(ZoneID))
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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ZoneID = ZoneID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazoneid = new SqlParameter("@ZoneID", ZoneID);
            List<ResponseDelivery> list = new List<ResponseDelivery>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponseDelivery>("SGP_WEB_BaoCaoPhat_Tong @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponseDelivery()
                    {
                        BC = item.BC,
                        DaNhan = item.DaNhan,
                        DaPhat = item.DaPhat,
                        ChuyenHoan = item.ChuyenHoan,
                        Khac = item.Khac,
                        ChuaPhat = item.ChuaPhat
                    });
                }
            }

            return View(list);
        }
        public ActionResult TotalMailer(string FromDate, string ToDate, int? page)
        {

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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var zoneid = new SqlParameter("@ZoneID", "");
            List<ResponeTongCG> list = new List<ResponeTongCG>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponeTongCG>("SGP_WEB_TongCG @FromDate,@ToDate,@ZoneID", parafrom, parato, zoneid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeTongCG()
                    {
                        ZoneID = item.ZoneID,
                        MaBC = item.MaBC,
                        BC = item.BC,
                        TongCG = item.TongCG,
                        ChuaNhapDT = item.ChuaNhapDT,
                    });
                }
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult ThuHoKT(string FromDate, string ToDate, int? page, string ZoneID = "", string Type = "")
        {

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
            ViewBag.ZoneID = ZoneID;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.Type = Type;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazone = new SqlParameter("@ZoneID", ZoneID);
            var paratype = new SqlParameter("@Type", Type);
            List<ResponseThuHo> list = new List<ResponseThuHo>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponseThuHo>("SGP_WEB_THUHOKT @FromDate,@ToDate,@ZoneID,@Type", parafrom, parato, parazone, paratype).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponseThuHo()
                    {
                        AcceptDate = item.AcceptDate,
                        MailerID = item.MailerID,
                        SenderID = item.SenderID,
                        SenderName = item.SenderName,
                        PostOfficeAcceptID = item.PostOfficeAcceptID,
                        PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                        Amount = item.Amount,
                        DocID = item.DocID,
                        Invoice = item.Invoice,
                        Description = item.Description,
                        PostOfficeID = item.PostOfficeID
                    });
                }
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult ThuHoKTList(string FromDate, string ToDate, int? page)
        {
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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parauser = new SqlParameter("@User", User.Identity.Name);
            List<ResponseTHKT> list = new List<ResponseTHKT>();

            var result = db.Database.SqlQuery<ResponseTHKT>("SGP_WEB_THUHOKTList @FromDate,@ToDate,@User", parafrom, parato, parauser).ToList();
            foreach (var item in result)
            {
                list.Add(new ResponseTHKT()
                {
                    MailerID = item.MailerID,
                    Amount = item.Amount,
                    DocID = item.DocID,
                    Invoice = item.Invoice,
                    Description = item.Description,
                    CreateDate = item.CreateDate,
                });
            }
            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExcelKeToanTH(string FromDate, string ToDate)
        {

            string pathRoot = Server.MapPath("~/Report/thuhokt.xlsx");
            string name = "thuhokt" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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
                var parauser = new SqlParameter("@User", User.Identity.Name);
                List<ResponseTHKT> list = new List<ResponseTHKT>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponseTHKT>("SGP_WEB_THUHOKTList @FromDate,@ToDate,@User", parafrom, parato, parauser).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponseTHKT()
                        {
                            MailerID = item.MailerID,
                            Amount = item.Amount,
                            DocID = item.DocID,
                            Invoice = item.Invoice,
                            Description = item.Description,
                            CreateDate = item.CreateDate,
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
                            worksheet.Cells[i + 2, 2].Value = list[i].Amount;
                            worksheet.Cells[i + 2, 3].Value = list[i].DocID;
                            worksheet.Cells[i + 2, 4].Value = list[i].Invoice;
                            worksheet.Cells[i + 2, 5].Value = list[i].Description;
                            worksheet.Cells[i + 2, 6].Value = list[i].CreateDate;
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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("thuhokt" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        [Authorize]
        public ActionResult PackingListbyZone(string FromDate, string ToDate, int? page, string ZoneID = "")
        {

            string fDate;
            string fTo;
            string fZone;

            if (String.IsNullOrEmpty(FromDate) || String.IsNullOrEmpty(ToDate) || String.IsNullOrEmpty(ZoneID))
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

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.ZoneID = ZoneID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazoneid = new SqlParameter("@ZoneID", ZoneID);
            List<ResponesePackingList> list = new List<ResponesePackingList>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponesePackingList>("SGP_WEB_PackingListbyZone @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponesePackingList()
                    {
                        AcceptDate = item.AcceptDate,
                        MailerID = item.MailerID,
                        PostOfficeID = item.PostOfficeID,
                        PostOfficeIDAccept = item.PostOfficeIDAccept,
                        Weight = item.Weight,
                        PostOfficeDeliveryID = item.PostOfficeDeliveryID,
                        DeliveryDate = item.DeliveryDate
                    });
                }
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult ExcelPackingListbyZone(string FromDate, string ToDate, string ZoneID = "")
        {

            string pathRoot = Server.MapPath("~/Report/packinglistbyzone.xlsx");
            string name = "packinglistbyzone" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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

                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                // ViewBag.ZoneID = ZoneID;

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                var parazoneid = new SqlParameter("@ZoneID", ZoneID);
                List<ResponesePackingList> list = new List<ResponesePackingList>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponesePackingList>("SGP_WEB_PackingListbyZone @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponesePackingList()
                        {
                            AcceptDate = item.AcceptDate,
                            MailerID = item.MailerID,
                            PostOfficeID = item.PostOfficeID,
                            PostOfficeIDAccept = item.PostOfficeIDAccept,
                            Weight = item.Weight,
                            PostOfficeDeliveryID = item.PostOfficeDeliveryID,
                            DeliveryDate = item.DeliveryDate
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
                            worksheet.Cells[i + 2, 1].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 2, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 2, 4].Value = list[i].PostOfficeIDAccept;

                            worksheet.Cells[i + 2, 5].Value = list[i].Weight;

                            worksheet.Cells[i + 2, 6].Value = list[i].PostOfficeDeliveryID;

                            worksheet.Cells[i + 2, 7].Value = list[i].DeliveryDate;

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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("packinglistbyzone" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        public ActionResult CheckMailerIDScan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CheckMailerIDScan(string FromDate, string ToDate, string ZoneID = "")
        {

            string pathRoot = Server.MapPath("~/Report/checkscan.xlsx");
            string name = "checkscan" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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

                ViewBag.FromDate = FromDate;
                ViewBag.ToDate = ToDate;
                // ViewBag.ZoneID = ZoneID;

                var parafrom = new SqlParameter("@FromDate", fDate);
                var parato = new SqlParameter("@ToDate", fTo);
                var parazoneid = new SqlParameter("@ZoneID", ZoneID);
                List<string> mailers = new List<string>();

                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponesePackingList>("SGP_WEB_CheckMailerIDScan @FromDate,@ToDate,@ZoneID", parafrom, parato, parazoneid).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        mailers.Add(item.MailerID);


                    }
                }

                ServiceReference1.Imailerscan scan = new ServiceReference1.ImailerscanClient();
                ServiceReference1.DocumentResult[] mailerResult = scan.DocumentScan(mailers.ToArray());

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < mailerResult.Count(); i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = mailerResult[i].id;

                            worksheet.Cells[i + 2, 2].Value = mailerResult[i].status == 1 ? "co" : "khong";

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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("checkscan" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }

        public ActionResult ReportKeToanTH(string FromDate, string ToDate)
        {

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
                var parauser = new SqlParameter("@User", User.Identity.Name);
                Reports.dsThuHo dsth = new Reports.dsThuHo();
                int i = 1;
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponseTHKT>("SGP_WEB_ReportTHUHOKT @FromDate,@ToDate,@User", parafrom, parato, parauser).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        dsth.Tables["ThuHo"].Rows.Add(
                            i,
                            item.AcceptDate,
                            item.MailerID,
                            item.Amount
                            );
                        i += 1;
                    }
                }
                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/rptThuHo.rpt")));

                rd.SetDataSource(dsth);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();


                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "ListThuHo.pdf");  
            //return View();
        }
        public ActionResult CheckPackingList(string FromDate, string ToDate, int? page, string PostOfficeID = "", string TripNumber = "")
        {

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
            ViewBag.PostOfficeID = PostOfficeID;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.TripNumber = TripNumber;

            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parazone = new SqlParameter("@PostOfficeID", PostOfficeID);
            var paratype = new SqlParameter("@TripNumber", TripNumber);
            List<ResponeCheckPackingList> list = new List<ResponeCheckPackingList>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<ResponeCheckPackingList>("SGP_WEB_CheckPackingList @FromDate,@ToDate,@PostOfficeID,@TripNumber", parafrom, parato, parazone, paratype).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeCheckPackingList()
                    {
                        DocumentID = item.DocumentID,
                        DocumentDate = item.DocumentDate,
                        PostOfficeID = item.PostOfficeID,
                        PostOfficeIDAccept = item.PostOfficeIDAccept,
                        TripNumber = item.TripNumber,
                        Weight = item.Weight,
                        NumberOfPackage = item.NumberOfPackage,
                        TransportObjectID = item.TransportObjectID,
                        SendDescription = item.SendDescription

                    });
                }
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult ExcelKeToanTH1(string FromDate, string ToDate, string ZoneID = "", string Type = "")
        {

            string pathRoot = Server.MapPath("~/Report/ketoanth.xlsx");
            string name = "thuhokt" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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
                var parazone = new SqlParameter("@ZoneID", ZoneID);
                var paratype = new SqlParameter("@Type", Type);
                List<ResponseThuHo> list = new List<ResponseThuHo>();
                if (FromDate != "" && ToDate != "")
                {

                    var result = db.Database.SqlQuery<ResponseThuHo>("SGP_WEB_THUHOKT @FromDate,@ToDate,@ZoneID,@Type", parafrom, parato, parazone, paratype).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new ResponseThuHo()
                        {
                            AcceptDate = item.AcceptDate,
                            MailerID = item.MailerID,
                            SenderID = item.SenderID,
                            SenderName = item.SenderName,
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            PostOfficeRecieverMoneyID = item.PostOfficeRecieverMoneyID,
                            Amount = item.Amount,
                            DocID = item.DocID,
                            Invoice = item.Invoice,
                            Description = item.Description,
                            PostOfficeID = item.PostOfficeID
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
                            worksheet.Cells[i + 2, 1].Value = list[i].AcceptDate;
                            worksheet.Cells[i + 2, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = list[i].SenderID;
                            worksheet.Cells[i + 2, 4].Value = list[i].SenderName;
                            worksheet.Cells[i + 2, 5].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 2, 6].Value = list[i].PostOfficeRecieverMoneyID;
                            worksheet.Cells[i + 2, 7].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 2, 8].Value = list[i].Amount;
                            worksheet.Cells[i + 2, 9].Value = list[i].DocID;
                            worksheet.Cells[i + 2, 10].Value = list[i].Invoice;
                            worksheet.Cells[i + 2, 11].Value = list[i].Description;
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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("thuhokt" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
        public ActionResult ReportKeToanTHbyDocID(string DocID)
        {

            var paraDocID = new SqlParameter("@DocID", DocID);
            Reports.dsThuHo dsth = new Reports.dsThuHo();
            int i = 1;


                var result = db.Database.SqlQuery<ResponseTHKT>("SGP_WEB_ReportTHUHOKTbyDocID @DocID", paraDocID).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    dsth.Tables["ThuHo"].Rows.Add(
                        i,
                        item.AcceptDate,
                        item.MailerID,
                        item.Amount
                        );
                    i += 1;
                }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/rptThuHo.rpt")));

            rd.SetDataSource(dsth);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ListThuHo.pdf");
            //return View();
        }
        public ActionResult ReportCNKHByID(string CustomerID)
        {

            var paraDocID = new SqlParameter("@CustomerID", CustomerID);
            Reports.dsThuHo dsth = new Reports.dsThuHo();
            int i = 1;


            var result = db.Database.SqlQuery<ResponseTHKT>("SGP_WEB_ReportCNKHByID @CustomerID", paraDocID).ToList();
            // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
            foreach (var item in result)
            {
                dsth.Tables["ThuHo"].Rows.Add(
                    i,
                    item.AcceptDate,
                    item.MailerID,
                    item.Amount
                    );
                i += 1;
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports/rptThuHo.rpt")));

            rd.SetDataSource(dsth);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();


            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "ListThuHo.pdf");
            //return View();
        }

        public ActionResult MBVPackingList(string FromDate, string ToDate, int? page)
        {
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
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            List<ResponeCheckingPacking> list = new List<ResponeCheckingPacking>();
            if (FromDate != "" && ToDate != "")
            {

                var result = pms.Database.SqlQuery<ResponeCheckingPacking>("SGP_viewPackingList @FromDate,@ToDate", parafrom, parato).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeCheckingPacking()
                    {
                        DocumentID = item.DocumentID,
                        DocumentDate = item.DocumentDate,
                        PostOfficeIDAccept = item.PostOfficeIDAccept,
                        NumberOfPackage = item.NumberOfPackage,
                        TripNumber = item.TripNumber,
                        Weight = item.Weight,
                        Description = item.Description,
                        DocumentOrder = item.DocumentOrder,
                        Tranport = item.Tranport,
                        StartDate = item.StartDate,
                        EndDate =item.EndDate,
                        RecieveDate = item.RecieveDate,
                        RecieveDescription = item.RecieveDescription
                    });
                }
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult AmountKT()
        {
            return View();
        }

        public ActionResult AmountKTExcel(string Date)
        {
            string pathRoot = Server.MapPath("~/Report/baocaosanluong.xlsx");
            string name = "baocaosanluong" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);

            System.IO.File.Copy(pathRoot, pathTo);

            ViewBag.FromDate = Date;

            try
            {
                FileInfo newFile = new FileInfo(pathTo);

                string date;

                if (String.IsNullOrEmpty(Date))
                {
                    date = DateTime.Now.ToString("yyyy-MM-dd");
                    Date = DateTime.Now.ToString("dd/MM/yyyy");

                }
                else
                {
                    date = DateTime.ParseExact(Request["Date"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
                }

                var paradate = new SqlParameter("@Date", date);
                var paradate1 = new SqlParameter("@Date", date);
                var paradate2 = new SqlParameter("@Date", date);
                var paradate3 = new SqlParameter("@Date", date);
                List<BCSanluongNhanhDuoi2> list = new List<BCSanluongNhanhDuoi2>();
                List<BCSanluongNhanhTren2> list1 = new List<BCSanluongNhanhTren2>();
                List<BCSanluongThuongDuoi2> list2 = new List<BCSanluongThuongDuoi2>();
                List<BCSanluongThuongTren2> list3 = new List<BCSanluongThuongTren2>();
                pms.Database.CommandTimeout = 0;
                if (Date != "")
                {
                   
                    var result = pms.Database.SqlQuery<BCSanluongNhanhDuoi2>("SGP_BCSanLuongNhanhDuoi2 @Date", paradate).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new BCSanluongNhanhDuoi2()
                        {
                            KV = item.KV,
                            SoLuong = item.SoLuong,
                            TrongLuong = item.TrongLuong,
                            TrongLuongKhoi = item.TrongLuongKhoi
                        });
                    }
                }
                if (Date != "")
                {

                    var result = pms.Database.SqlQuery<BCSanluongNhanhTren2>("SGP_BCSanLuongNhanhTren2 @Date", paradate1).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list1.Add(new BCSanluongNhanhTren2()
                        {
                            KV = item.KV,
                            SoLuong = item.SoLuong,
                            TrongLuong = item.TrongLuong,
                            TrongLuongKhoi = item.TrongLuongKhoi
                        });
                    }
                }

                if (Date != "")
                {

                    var result = pms.Database.SqlQuery<BCSanluongThuongDuoi2>("SGP_BCSanLuongThuongDuoi2 @Date", paradate2).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list2.Add(new BCSanluongThuongDuoi2()
                        {
                            KV = item.KV,
                            SoLuong = item.SoLuong,
                            TrongLuong = item.TrongLuong,
                            TrongLuongKhoi = item.TrongLuongKhoi
                        });
                    }
                }

                if (Date != "")
                {

                    var result = pms.Database.SqlQuery<BCSanluongThuongTren2>("SGP_BCSanLuongThuongTren2 @Date", paradate3).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list3.Add(new BCSanluongThuongTren2()
                        {
                            KV = item.KV,
                            SoLuong = item.SoLuong,
                            TrongLuong = item.TrongLuong,
                            TrongLuongKhoi = item.TrongLuongKhoi
                        });
                    }
                }
                
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    date = DateTime.ParseExact(Request["Date"], "dd/MM/yyyy", null).ToString("MM");
                    string thang = date;
                    date = DateTime.ParseExact(Request["Date"], "dd/MM/yyyy", null).ToString("yyyy");
                    string nam = date;
                    worksheet.Cells[1, 2].Value = "Tháng " +  thang +"/"+ nam ;
                    int count = 1;
                    int dong = 1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {  
                            if(list[i].KV != null)
                            {
                                if (list[i].KV.Equals("1A.HNI->HNI"))
                                {
                                    worksheet.Cells[6, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[6, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[6, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1B.HNI->KV1"))
                                {
                                    worksheet.Cells[7, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[7, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[7, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1C.HNI->HCM"))
                                {
                                    worksheet.Cells[8, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[8, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[8, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1D.HNI->KV2"))
                                {
                                    worksheet.Cells[9, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[9, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[9, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1E.HNI->DNG"))
                                {
                                    worksheet.Cells[10, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[10, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[10, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1F.HNI->KV3"))
                                {
                                    worksheet.Cells[11, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[11, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[11, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1G.HNI->CTO"))
                                {
                                    worksheet.Cells[12, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[12, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[12, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("1H.HNI->KV4"))
                                {
                                    worksheet.Cells[13, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[13, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[13, 4].Value = list[i].TrongLuongKhoi;
                                }


                                //
                                if (list[i].KV.Equals("2A.HCM->HNI"))
                                {
                                    worksheet.Cells[15, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[15, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[15, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2B.HCM->KV1"))
                                {
                                    worksheet.Cells[16, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[16, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[16, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2C.HCM->HCM"))
                                {
                                    worksheet.Cells[17, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[17, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[17, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2D.HCM->KV2"))
                                {
                                    worksheet.Cells[18, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[18, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[18, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2E.HCM->DNG"))
                                {
                                    worksheet.Cells[19, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[19, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[19, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2F.HCM->KV3"))
                                {
                                    worksheet.Cells[20, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[20, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[20, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2G.HCM->CTO"))
                                {
                                    worksheet.Cells[21, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[21, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[21, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("2H.HCM->KV4"))
                                {
                                    worksheet.Cells[22, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[22, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[22, 4].Value = list[i].TrongLuongKhoi;
                                }


                                //
                                if (list[i].KV.Equals("3A.DNG->HNI"))
                                {
                                    worksheet.Cells[24, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[24, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[24, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3B.DNG->KV1"))
                                {
                                    worksheet.Cells[25, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[25, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[25, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3C.DNG->HCM"))
                                {
                                    worksheet.Cells[26, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[26, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[26, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3D.DNG->KV2"))
                                {
                                    worksheet.Cells[27, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[27, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[27, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3E.DNG->DNG"))
                                {
                                    worksheet.Cells[28, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[28, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[28, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3F.DNG->KV3"))
                                {
                                    worksheet.Cells[29, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[29, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[29, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3G.DNG->CTO"))
                                {
                                    worksheet.Cells[30, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[30, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[30, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("3H.DNG->KV4"))
                                {
                                    worksheet.Cells[31, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[31, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[31, 4].Value = list[i].TrongLuongKhoi;
                                }


                                //
                                if (list[i].KV.Equals("4A.CTO->HNI"))
                                {
                                    worksheet.Cells[33, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[33, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[33, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4B.CTO->KV1"))
                                {
                                    worksheet.Cells[34, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[34, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[34, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4C.CTO->HCM"))
                                {
                                    worksheet.Cells[35, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[35, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[35, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4D.CTO->KV2"))
                                {
                                    worksheet.Cells[36, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[36, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[36, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4E.CTO->DNG"))
                                {
                                    worksheet.Cells[37, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[37, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[37, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4F.CTO->KV3"))
                                {
                                    worksheet.Cells[38, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[38, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[38, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4G.CTO->CTO"))
                                {
                                    worksheet.Cells[39, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[39, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[39, 4].Value = list[i].TrongLuongKhoi;
                                }
                                if (list[i].KV.Equals("4H.CTO->KV4"))
                                {
                                    worksheet.Cells[40, 2].Value = list[i].SoLuong;
                                    worksheet.Cells[40, 3].Value = list[i].TrongLuong;
                                    worksheet.Cells[40, 4].Value = list[i].TrongLuongKhoi;
                                }
                            }
                         
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    if(list1[0].KV != null)
                    {
                        count = 0;
                        dong = 0;
                    }else
                    {
                        count = 1;
                        dong = 1;
                    }              
                    for (int i = 0; i < list1.Count; i++)
                    {

                        try
                        {

                            if (list1[i].KV != null)
                            {
                                if (list1[i].KV.Equals("1A.HNI->HNI"))
                                {
                                    worksheet.Cells[6, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[6, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[6, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1B.HNI->KV1"))
                                {
                                    worksheet.Cells[7, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[7, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[7, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1C.HNI->HCM"))
                                {
                                    worksheet.Cells[8, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[8, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[8, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1D.HNI->KV2"))
                                {
                                    worksheet.Cells[9, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[9, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[9, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1E.HNI->DNG"))
                                {
                                    worksheet.Cells[10, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[10, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[10, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1F.HNI->KV3"))
                                {
                                    worksheet.Cells[11, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[11, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[11, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1G.HNI->CTO"))
                                {
                                    worksheet.Cells[12, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[12, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[12, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("1H.HNI->KV4"))
                                {
                                    worksheet.Cells[13, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[13, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[13, 7].Value = list1[i].TrongLuongKhoi;
                                }


                                //
                                if (list1[i].KV.Equals("2A.HCM->HNI"))
                                {
                                    worksheet.Cells[15, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[15, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[15, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2B.HCM->KV1"))
                                {
                                    worksheet.Cells[16, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[16, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[16, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2C.HCM->HCM"))
                                {
                                    worksheet.Cells[17, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[17, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[17, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2D.HCM->KV2"))
                                {
                                    worksheet.Cells[18, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[18, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[18, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2E.HCM->DNG"))
                                {
                                    worksheet.Cells[19, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[19, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[19, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2F.HCM->KV3"))
                                {
                                    worksheet.Cells[20, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[20, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[20, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2G.HCM->CTO"))
                                {
                                    worksheet.Cells[21, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[21, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[21, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("2H.HCM->KV4"))
                                {
                                    worksheet.Cells[22, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[22, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[22, 7].Value = list1[i].TrongLuongKhoi;
                                }


                                //
                                if (list1[i].KV.Equals("3A.DNG->HNI"))
                                {
                                    worksheet.Cells[24, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[24, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[24, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3B.DNG->KV1"))
                                {
                                    worksheet.Cells[25, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[25, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[25, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3C.DNG->HCM"))
                                {
                                    worksheet.Cells[26, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[26, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[26, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3D.DNG->KV2"))
                                {
                                    worksheet.Cells[27, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[27, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[27, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3E.DNG->DNG"))
                                {
                                    worksheet.Cells[28, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[28, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[28, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3F.DNG->KV3"))
                                {
                                    worksheet.Cells[29, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[29, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[29, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3G.DNG->CTO"))
                                {
                                    worksheet.Cells[30, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[30, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[30, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("3H.DNG->KV4"))
                                {
                                    worksheet.Cells[31, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[31, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[31, 7].Value = list1[i].TrongLuongKhoi;
                                }


                                //
                                if (list1[i].KV.Equals("4A.CTO->HNI"))
                                {
                                    worksheet.Cells[33, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[33, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[33, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4B.CTO->KV1"))
                                {
                                    worksheet.Cells[34, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[34, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[34, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4C.CTO->HCM"))
                                {
                                    worksheet.Cells[35, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[35, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[35, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4D.CTO->KV2"))
                                {
                                    worksheet.Cells[36, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[36, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[36, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4E.CTO->DNG"))
                                {
                                    worksheet.Cells[37, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[37, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[37, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4F.CTO->KV3"))
                                {
                                    worksheet.Cells[38, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[38, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[38, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4G.CTO->CTO"))
                                {
                                    worksheet.Cells[39, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[39, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[39, 7].Value = list1[i].TrongLuongKhoi;
                                }
                                if (list1[i].KV.Equals("4H.CTO->KV4"))
                                {
                                    worksheet.Cells[40, 5].Value = list1[i].SoLuong;
                                    worksheet.Cells[40, 6].Value = list1[i].TrongLuong;
                                    worksheet.Cells[40, 7].Value = list1[i].TrongLuongKhoi;
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    if (list2[0].KV != null)
                    {
                        count = 0;
                        dong = 0;
                    }
                    else
                    {
                        count = 1;
                        dong = 1;
                    }
                    for (int i = 0; i < list2.Count; i++)
                    {

                        try
                        {

                            if (list2[i].KV != null)
                            {
                                if (list2[i].KV.Equals("1A.HNI->HNI"))
                                {
                                    worksheet.Cells[6, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[6, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[6, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1B.HNI->KV1"))
                                {
                                    worksheet.Cells[7, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[7, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[7, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1C.HNI->HCM"))
                                {
                                    worksheet.Cells[8, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[8, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[8, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1D.HNI->KV2"))
                                {
                                    worksheet.Cells[9, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[9, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[9, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1E.HNI->DNG"))
                                {
                                    worksheet.Cells[10, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[10, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[10, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1F.HNI->KV3"))
                                {
                                    worksheet.Cells[11, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[11, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[11, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1G.HNI->CTO"))
                                {
                                    worksheet.Cells[12, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[12, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[12, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("1H.HNI->KV4"))
                                {
                                    worksheet.Cells[13, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[13, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[13, 10].Value = list2[i].TrongLuongKhoi;
                                }


                                //
                                if (list2[i].KV.Equals("2A.HCM->HNI"))
                                {
                                    worksheet.Cells[15, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[15, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[15, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2B.HCM->KV1"))
                                {
                                    worksheet.Cells[16, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[16, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[16, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2C.HCM->HCM"))
                                {
                                    worksheet.Cells[17, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[17, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[17, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2D.HCM->KV2"))
                                {
                                    worksheet.Cells[18, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[18, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[18, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2E.HCM->DNG"))
                                {
                                    worksheet.Cells[19, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[19, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[19, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2F.HCM->KV3"))
                                {
                                    worksheet.Cells[20, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[20, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[20, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2G.HCM->CTO"))
                                {
                                    worksheet.Cells[21, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[21, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[21, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("2H.HCM->KV4"))
                                {
                                    worksheet.Cells[22, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[22, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[22, 10].Value = list2[i].TrongLuongKhoi;
                                }


                                //
                                if (list2[i].KV.Equals("3A.DNG->HNI"))
                                {
                                    worksheet.Cells[24, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[24, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[24, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3B.DNG->KV1"))
                                {
                                    worksheet.Cells[25, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[25, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[25, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3C.DNG->HCM"))
                                {
                                    worksheet.Cells[26, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[26, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[26, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3D.DNG->KV2"))
                                {
                                    worksheet.Cells[27, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[27, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[27, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3E.DNG->DNG"))
                                {
                                    worksheet.Cells[28, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[28, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[28, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3F.DNG->KV3"))
                                {
                                    worksheet.Cells[29, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[29, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[29, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3G.DNG->CTO"))
                                {
                                    worksheet.Cells[30, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[30, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[30, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("3H.DNG->KV4"))
                                {
                                    worksheet.Cells[31, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[31, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[31, 10].Value = list2[i].TrongLuongKhoi;
                                }


                                //
                                if (list2[i].KV.Equals("4A.CTO->HNI"))
                                {
                                    worksheet.Cells[33, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[33, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[33, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4B.CTO->KV1"))
                                {
                                    worksheet.Cells[34, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[34, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[34, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4C.CTO->HCM"))
                                {
                                    worksheet.Cells[35, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[35, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[35, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4D.CTO->KV2"))
                                {
                                    worksheet.Cells[36, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[36, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[36, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4E.CTO->DNG"))
                                {
                                    worksheet.Cells[37, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[37, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[37, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4F.CTO->KV3"))
                                {
                                    worksheet.Cells[38, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[38, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[38, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4G.CTO->CTO"))
                                {
                                    worksheet.Cells[39, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[39, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[39, 10].Value = list2[i].TrongLuongKhoi;
                                }
                                if (list2[i].KV.Equals("4H.CTO->KV4"))
                                {
                                    worksheet.Cells[40, 8].Value = list2[i].SoLuong;
                                    worksheet.Cells[40, 9].Value = list2[i].TrongLuong;
                                    worksheet.Cells[40, 10].Value = list2[i].TrongLuongKhoi;
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    if (list3[0].KV != null)
                    {
                        count = 0;
                        dong = 0;
                    }
                    else
                    {
                        count = 1;
                        dong = 1;
                    }
                    for (int i = 0; i < list3.Count; i++)
                    {

                        try
                        {

                            if (list3[i].KV != null)
                            {
                                if (list3[i].KV.Equals("1A.HNI->HNI"))
                                {
                                    worksheet.Cells[6, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[6, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[6, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1B.HNI->KV1"))
                                {
                                    worksheet.Cells[7, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[7, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[7, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1C.HNI->HCM"))
                                {
                                    worksheet.Cells[8, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[8, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[8, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1D.HNI->KV2"))
                                {
                                    worksheet.Cells[9, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[9, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[9, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1E.HNI->DNG"))
                                {
                                    worksheet.Cells[10, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[10, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[10, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1F.HNI->KV3"))
                                {
                                    worksheet.Cells[11, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[11, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[11, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1G.HNI->CTO"))
                                {
                                    worksheet.Cells[12, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[12, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[12, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("1H.HNI->KV4"))
                                {
                                    worksheet.Cells[13, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[13, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[13, 13].Value = list3[i].TrongLuongKhoi;
                                }


                                //
                                if (list3[i].KV.Equals("2A.HCM->HNI"))
                                {
                                    worksheet.Cells[15, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[15, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[15, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2B.HCM->KV1"))
                                {
                                    worksheet.Cells[16, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[16, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[16, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2C.HCM->HCM"))
                                {
                                    worksheet.Cells[17, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[17, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[17, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2D.HCM->KV2"))
                                {
                                    worksheet.Cells[18, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[18, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[18, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2E.HCM->DNG"))
                                {
                                    worksheet.Cells[19, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[19, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[19, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2F.HCM->KV3"))
                                {
                                    worksheet.Cells[20, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[20, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[20, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2G.HCM->CTO"))
                                {
                                    worksheet.Cells[21, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[21, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[21, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("2H.HCM->KV4"))
                                {
                                    worksheet.Cells[22, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[22, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[22, 13].Value = list3[i].TrongLuongKhoi;
                                }


                                //
                                if (list3[i].KV.Equals("3A.DNG->HNI"))
                                {
                                    worksheet.Cells[24, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[24, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[24, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3B.DNG->KV1"))
                                {
                                    worksheet.Cells[25, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[25, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[25, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3C.DNG->HCM"))
                                {
                                    worksheet.Cells[26, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[26, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[26, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3D.DNG->KV2"))
                                {
                                    worksheet.Cells[27, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[27, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[27, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3E.DNG->DNG"))
                                {
                                    worksheet.Cells[28, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[28, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[28, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3F.DNG->KV3"))
                                {
                                    worksheet.Cells[29, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[29, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[29, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3G.DNG->CTO"))
                                {
                                    worksheet.Cells[30, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[30, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[30, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("3H.DNG->KV4"))
                                {
                                    worksheet.Cells[31, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[31, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[31, 13].Value = list3[i].TrongLuongKhoi;
                                }


                                //
                                if (list3[i].KV.Equals("4A.CTO->HNI"))
                                {
                                    worksheet.Cells[33, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[33, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[33, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4B.CTO->KV1"))
                                {
                                    worksheet.Cells[34, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[34, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[34, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4C.CTO->HCM"))
                                {
                                    worksheet.Cells[35, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[35, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[35, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4D.CTO->KV2"))
                                {
                                    worksheet.Cells[36, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[36, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[36, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4E.CTO->DNG"))
                                {
                                    worksheet.Cells[37, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[37, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[37, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4F.CTO->KV3"))
                                {
                                    worksheet.Cells[38, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[38, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[38, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4G.CTO->CTO"))
                                {
                                    worksheet.Cells[39, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[39, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[39, 13].Value = list3[i].TrongLuongKhoi;
                                }
                                if (list3[i].KV.Equals("4H.CTO->KV4"))
                                {
                                    worksheet.Cells[40, 11].Value = list3[i].SoLuong;
                                    worksheet.Cells[40, 12].Value = list3[i].TrongLuong;
                                    worksheet.Cells[40, 13].Value = list3[i].TrongLuongKhoi;
                                }
                            }

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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("baocaosanluong" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }
    }
}