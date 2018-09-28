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
                    for (int i = 0; i < list.Count+2; i++)
                    {

                        try
                        {
                            
                            if (dong % 9 == 0)
                            {
                                worksheet.Cells[i + 6, 2].Value = "";
                                worksheet.Cells[i + 6, 3].Value = "";
                                dong++;
                            }
                            else
                            {
                                worksheet.Cells[i + 6, 2].Value = list[count].SoLuong;
                                worksheet.Cells[i + 6, 3].Value = list[count].TrongLuong;
                                worksheet.Cells[i + 6, 4].Value = list[count].TrongLuongKhoi;
                                count++;
                                dong++;
                            }                        
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 1;
                    dong = 1;
                    for (int i = 0; i < list1.Count +3; i++)
                    {

                        try
                        {

                            if (dong % 9 == 0)
                            {
                                worksheet.Cells[i + 6, 5].Value = "";
                                worksheet.Cells[i + 6, 6].Value = "";
                                dong++;
                            }
                            else
                            {
                                    worksheet.Cells[i + 6, 5].Value = list1[count].SoLuong;
                                    worksheet.Cells[i + 6, 6].Value = list1[count].TrongLuong;
                                    worksheet.Cells[i + 6, 7].Value = list1[count].TrongLuongKhoi;
                                    count++;
                                    dong++;
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 1;
                    dong = 1;
                    for (int i = 0; i < list2.Count +6; i++)
                    {

                        try
                        {

                            if (dong % 9 == 0 || dong == 20 || dong == 25 || dong == 26)
                            {
                                worksheet.Cells[i + 6, 8].Value = "";
                                worksheet.Cells[i + 6, 9].Value = "";
                                dong++;
                            }
                            else
                            {
                                worksheet.Cells[i + 6, 8].Value = list2[count].SoLuong;
                                worksheet.Cells[i + 6, 9].Value = list2[count].TrongLuong;
                                worksheet.Cells[i + 6, 10].Value = list2[count].TrongLuongKhoi;
                                count++;
                                dong++;
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 1;
                    dong = 1;
                    for (int i = 0; i < list3.Count +7; i++)
                    {

                        try
                        {

                            if (dong % 9 == 0 || dong == 20 || dong == 23 || dong == 25 || dong == 26 || dong == 33)
                            {
                                worksheet.Cells[i + 6, 11].Value = "";
                                worksheet.Cells[i + 6, 12].Value = "";
                                dong++;
                            }
                            else
                            {
                                worksheet.Cells[i + 6, 11].Value = list3[count].SoLuong;
                                worksheet.Cells[i + 6, 12].Value = list3[count].TrongLuong;
                                worksheet.Cells[i + 6, 13].Value = list3[count].TrongLuongKhoi;
                                count++;
                                dong++;
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