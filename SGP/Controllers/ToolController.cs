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
using System.Data;
using GoogleMaps.LocationServices;

namespace SGP.Controllers
{
    public class ToolController : Controller
    {
        //
        // GET: /Tool/
        PMSSGP_200911Entities pms = new PMSSGP_200911Entities();
        SGPAPIEntities sgp = new SGPAPIEntities();
        DBLISTEntities dbl = new DBLISTEntities();
        PMS_TESTEntities1 test = new PMS_TESTEntities1();
        public ActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Accounting")]
        public ActionResult ThuHoKT()
        {
            return View();
        }

        //[Authorize(Roles = "Accounting1")]
        public ActionResult PackingListInsert()
        {
            return View();
        }
        public ActionResult PackingListUpdate()
        {
            return View();
        }

        public ActionResult OnlineReport()
        {
            return View();
        }


        //[Authorize(Roles = "Accounting")]
        [HttpPost]
        public ActionResult ThuHoKT(string MailerID = "", string Amount = "", string DocID = "", string Invoice = "", string Description = "")
        {
            decimal TongTien = 0;
            if (Amount != "")
            {

                TongTien = decimal.Parse(Amount.Replace(",", ""));
            }
            var model = pms.MM_Mailers.Where(d => d.MailerID == MailerID).FirstOrDefault();

            if (model != null)
            {
                try
                {
                    var check = pms.SGP_KT_THUHO.Where(d => d.MailerID == MailerID).FirstOrDefault();
                    if (check == null)
                    {
                        var kt = new SGP_KT_THUHO()
                        {
                            MailerID = MailerID,
                            DocID = DocID,
                            Amount = TongTien,
                            CreateDate = DateTime.Now,
                            Invoice = Invoice,
                            Description = Description,
                            UserID = User.Identity.Name
                        };

                        pms.SGP_KT_THUHO.Add(kt);
                        pms.SaveChanges();
                    }
                    else
                    {
                        SGP_KT_THUHO bp = pms.SGP_KT_THUHO.Single(d => d.MailerID == MailerID && d.UserID == User.Identity.Name);
                        bp.DocID = DocID;
                        bp.Amount = TongTien;
                        bp.CreateDate = DateTime.Now;
                        bp.Invoice = Invoice;
                        bp.Description = Description;
                        pms.SaveChanges();
                    }

                }
                catch
                {

                }


            }
            else
            {

            }

            return RedirectToAction("ThuHoKT");
        }
        public ActionResult getAmount(string MailerID = "")
        {
            var thuho = pms.SGP_KT_THUHO.Where(d => d.MailerID == MailerID).FirstOrDefault();

            if (thuho != null)
            {
                return Json(new { stt = 0, tt = thuho.Amount, pt = thuho.DocID, hd = thuho.Invoice, gc = thuho.Description }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var model = pms.MM_Mailers.Where(d => d.MailerID == MailerID).FirstOrDefault();

                if (model != null)
                    return Json(new { stt = 0, tt = model.Amount, pt = "", hd = "", gc = "" }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { stt = 1, msg = "wrong mailerid" }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult getPackingListInfo(string DocumentID)
        {
            //List<ResponeCheckingPacking> list = new List<ResponeCheckingPacking>();
            var DocID = new SqlParameter("DocumentID", DocumentID);
            var result = pms.SGP_insertPackingList(DocumentID).FirstOrDefault();
            return Json(new { stt = 0, documentid = result.DocumentID, documentdate = result.DocumentDate.ToString(), post = result.PostOfficeIDAccept, package = result.NumberOfPackage, weight = result.Weight, trip = result.TripNumber }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Tracking(int? page, string MailerID = "")
        {

            List<ResponseTracking> list = new List<ResponseTracking>();
            if (MailerID != "")
            {

                ViewBag.MailerID = MailerID;
                var mailerid = new SqlParameter("@MailerID", MailerID);
                var result = sgp.Database.SqlQuery<ResponseTracking>("SGP_WEB_TraCuu @MailerID", mailerid).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponseTracking()
                    {
                        MailerID = item.MailerID,
                        StatusName = item.StatusName,
                        PostOfficeName = item.PostOfficeName,
                        DocumentID = item.DocumentID,
                        UserGroupID = item.UserGroupID,
                        ID = item.ID,
                        CreationDate = item.CreationDate

                    });
                }
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Tracking(HttpPostedFileBase files)
        {
            // kiem tra file co tai len khong
            if (files != null && files.ContentLength > 0)
            {
                // kiem tra co file là excel ko
                string extension = System.IO.Path.GetExtension(files.FileName);
                if (extension.Equals(".xlsx") || extension.Equals(".xls"))
                {
                    // list chua toan bo danh sach
                    List<ResponseTracking> list = new List<ResponseTracking>();


                    // luu cai file mới tai lên vào thư mục Temp
                    string fileSave = "tracking_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + extension;
                    string path = Server.MapPath("~/Temp/" + fileSave);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    files.SaveAs(path);
                    FileInfo newFile = new FileInfo(path);

                    // doc file excel
                    var package = new ExcelPackage(newFile);

                    // doc tu sheet dau tien
                    ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                    int totalRows = sheet.Dimension.End.Row;

                    // duyet toan bo dong
                    for (int i = 1; i <= totalRows; i++)
                    {
                        // mailerId lay ra tu cot dau tien moi dòng
                        string MailerID = Convert.ToString(sheet.Cells[i, 1].Value);
                        if (MailerID != "")
                        {
                            var mailerid = new SqlParameter("@MailerID", MailerID);
                            var result = sgp.Database.SqlQuery<ResponseTracking>("SGP_WEB_TraCuu @MailerID", mailerid).ToList();
                            // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                            foreach (var item in result)
                            {
                                list.Add(new ResponseTracking()
                                {
                                    MailerID = item.MailerID,
                                    StatusName = item.StatusName,
                                    PostOfficeName = item.PostOfficeName,
                                    DocumentID = item.DocumentID,
                                    UserGroupID = item.UserGroupID,
                                    ID = item.ID,
                                    CreationDate = item.CreationDate

                                });
                            }
                        }
                    }


                    // xuat file excel
                    string pathRootExport = Server.MapPath("~/Report/trackingreport.xlsx");
                    string nameExport = "report-tracking-" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
                    string pathToExport = Server.MapPath("~/Temp/" + nameExport);
                    FileInfo fileExport = new FileInfo(pathToExport);
                    System.IO.File.Copy(pathRootExport, pathToExport);
                    using (ExcelPackage packageExport = new ExcelPackage(fileExport))
                    {
                        ExcelWorksheet worksheet = packageExport.Workbook.Worksheets[1];

                        for (int i = 1; i < list.Count; i++)
                        {
                            worksheet.Cells[i + 3, 1].Value = list[i].ID;
                            worksheet.Cells[i + 3, 2].Value = list[i].MailerID;
                            worksheet.Cells[i + 3, 3].Value = list[i].DocumentID;
                            worksheet.Cells[i + 3, 4].Value = list[i].PostOfficeName;
                            worksheet.Cells[i + 3, 5].Value = list[i].UserGroupID;
                            worksheet.Cells[i + 3, 6].Value = list[i].StatusName;
                            worksheet.Cells[i + 3, 7].Value = list[i].CreationDate;
                        }

                        packageExport.Save();

                    }

                    return File(pathToExport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("excel" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
                }

            }

            return RedirectToAction("Tracking", "Tool");
        }
        public ActionResult showMapSony(string FromDate, string ToDate)
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
                fTo = DateTime.ParseExact(Request["FromDate"], "dd/MM/yyyy", null).ToString("yyyy-MM-dd");
            }

            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;

            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);

            List<ResponseSonyMap> list = new List<ResponseSonyMap>();
            //var result = db.SGP_WEB_AmountByMonth().ToList();
            var result = sgp.Database.SqlQuery<ResponseSonyMap>("SGP_WEB_getSonyMap @FromDate,@ToDate", parafrom, parato).ToList();
            foreach (var item in result)
            {
                list.Add(new ResponseSonyMap()
                {
                    Address = item.Address,
                    Lang = item.Lang,
                    Long = item.Long
                });
            }
            return View(list);
        }
        [HttpGet]
        public ActionResult CheckMailerCG3(int? page, string MailerID = "", string TypeID = "", string DB = "")
        {

            List<ResponeCheckPackingList> list = new List<ResponeCheckPackingList>();
            if (MailerID != "")
            {

                ViewBag.MailerID = MailerID;
                var mailerid = new SqlParameter("@MailerID", MailerID);
                var typeid = new SqlParameter("@TypeID", TypeID);
                var db = new SqlParameter("@DB", DB);
                var result = sgp.Database.SqlQuery<ResponeCheckPackingList>("SGP_WEB_CheckTripCG3 @MailerID,@TypeID,@DB", mailerid, typeid, db).ToList();
                // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponeCheckPackingList()
                    {
                        DocumentDate = item.DocumentDate,
                        DocumentID = item.DocumentID,
                        PostOfficeID = item.PostOfficeID,
                        PostOfficeIDAccept = item.PostOfficeIDAccept,
                        TripNumber = item.TripNumber
                    });
                }
            }

            int pageSize = 30;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult TrackingEMS(string ems)
        {
            var model = pms.MM_Mailers.Where(d => d.MailerDescription == ems || d.MailerID == ems).FirstOrDefault();

            return View(model);
        }

        public ActionResult SendMail(string ems)
        {

            if (!String.IsNullOrEmpty(ems))
            {
                var model = pms.MM_Mailers.Where(d => d.MailerDescription == ems || d.MailerID == ems).FirstOrDefault();
                var email = pms.SGP_Email.Where(p => p.PostOfficeID == model.PostOfficeAcceptID).FirstOrDefault();
                try
                {
                    XMail.Send("loivv201@gmail.com", email.Email, "", " Thư hoàn EMS ", "Số phiếu : " + model.MailerID + " Số EMS : " + model.MailerDescription + " Ngày gửi :" + model.AcceptDate);
                }
                catch (Exception ex)
                {

                }

            }


            return RedirectToAction("TrackingEMS", "Tool", new { ems = ems });
        }
        //[Authorize(Roles = "Accounting1")]
        [HttpPost]
        public ActionResult PackingListInsert(string DocID, DateTime? DocumentDate, string PostOfficeIDAccept, int? NumberOfPackage, string TripNumber, double? Weight, string Description, string OrderDocument, string Tranport, DateTime? StartDate, DateTime? EndDate)
        {
            var kt = new SGP_PackingList()
            {
                DocumentID = DocID,
                DocumentDate = DocumentDate,
                PostOfficeIDAccept = PostOfficeIDAccept,
                NumberOfPackage = NumberOfPackage,
                TripNumber = TripNumber,
                Weight = Weight,
                Description = Description,
                DocumentOrder = OrderDocument,
                Tranport = Tranport,
                StartDate = StartDate,
                EndDate = EndDate
            };
            pms.SGP_PackingList.Add(kt);
            pms.SaveChanges();
            return RedirectToAction("PackingListInsert");
            //return View();
        }
        [HttpPost]
        public ActionResult PackingListUpdate(string DocID, DateTime? DocumentDate, string PostOfficeIDAccept, string TripNumber, DateTime? RecieveDate, string RecieveDescription)
        {
            var check = pms.SGP_PackingList.Where(p => p.DocumentID == DocID).FirstOrDefault();
            check.DocumentID = DocID;
            check.DocumentDate = DocumentDate;
            check.PostOfficeIDAccept = PostOfficeIDAccept;
            check.TripNumber = TripNumber;
            check.RecieveDate = RecieveDate;
            check.RecieveDescription = RecieveDescription;
            pms.SaveChanges();
            return RedirectToAction("PackingListUpdate");
        }

        [HttpPost]
        public ActionResult OnlineReport(string Title, string IDReceipt, string DetailContent, string CreateName, DateTime? CreateDate, string Status, string Fault, bool CheckFault)
        {
            //string CheckFault = Request.Form["selec"];
            DateTime? dt;
            if (CreateDate != null)
            {
                dt = CreateDate;
            }
            else
            {
                dt = DateTime.Now;
            }
            if (CheckFault == true)
            {
                var kt1 = new Fault()
                {
                    FaultName = Fault,
                    Point = 0
                };
                sgp.Faults.Add(kt1);
                sgp.SaveChanges();
                var rs = sgp.Faults.Where(p => p.FaultName == Fault).FirstOrDefault();
                int Ft = rs.IDFault;
                var kt = new ReportOnline()
                {
                    Title = Title,
                    IDReceipt = IDReceipt,
                    DetailContent = DetailContent,
                    CreateName = CreateName,
                    CreateDate = dt,
                    Status = Status,
                    IDFault = Ft,
                    //ID = "1"
                };
                sgp.ReportOnlines.Add(kt);
                sgp.SaveChanges();

            }
            else
            {
                var kt = new ReportOnline()
                {
                    Title = Title,
                    IDReceipt = IDReceipt,
                    DetailContent = DetailContent,
                    CreateName = CreateName,
                    CreateDate = dt,
                    Status = Status,
                    IDFault = int.Parse(Request.Form["select"]),
                    //ID = "1"
                };
                sgp.ReportOnlines.Add(kt);
                sgp.SaveChanges();
            }
            return RedirectToAction("OnlineReport");
            //return View();
        }

        public ActionResult ViewReport(int? page = 1)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<CreateTable> list = new List<CreateTable>();
            var result = sgp.Database.SqlQuery<CreateTable>("SGP_WEB_ReportOnline").ToList();
            foreach (var item in result)
            {
                list.Add(new CreateTable()
                {
                    ID = item.ID,
                    Title = item.Title,
                    IDReceipt = item.IDReceipt,
                    CreateName = item.CreateName,
                    CreateDate = item.CreateDate,
                });
            }
            return View(list.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ReviewReport(int id)
        {
            //CrystalReport1 cr = new CrystalReport1();
            var rs = sgp.ReportOnlines.Find(id);
            //TextObject _txtTitle = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["txtTitle"];
            //_txtTitle.Text = "HA";
            //TextObject _txtID = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["txtID"];
            //_txtID.Text = "HA";
            //TextObject _txtDetail = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["txtDetail"];
            //_txtDetail.Text = rs.DetailContent;
            //TextObject _txtCreateName = (TextObject)cr.ReportDefinition.Sections["Section3"].ReportObjects["txtCreateName"];
            //_txtCreateName.Text = rs.CreateName;
            return View(sgp.ReportOnlines.Find(id));
        }

        public ActionResult In(int id, string Title = "IN")
        {
            var rs = sgp.ReportOnlines.Find(id);
            ReportDocument rd = new ReportDocument();
            rd.Load(Server.MapPath("~/CrystalReport1.rpt"));
            TextObject _txtTitle = (TextObject)rd.ReportDefinition.Sections["Section3"].ReportObjects["txtTitle"];
            _txtTitle.Text = rs.Title;
            TextObject _txtID = (TextObject)rd.ReportDefinition.Sections["Section3"].ReportObjects["txtID"];
            _txtID.Text = rs.IDReceipt;
            TextObject _txtDetail = (TextObject)rd.ReportDefinition.Sections["Section3"].ReportObjects["txtDetail"];
            _txtDetail.Text = rs.DetailContent;
            TextObject _txtCreateName = (TextObject)rd.ReportDefinition.Sections["Section3"].ReportObjects["txtCreateName"];
            _txtCreateName.Text = rs.CreateName;
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf");
        }

        public ActionResult SpecialCustomer(string FromDate, string ToDate, int? page, int opt = 0, string CustomerGroupID = "", string CustomerID = "")
        {
            string fDate;
            string fTo;
            ViewBag.OPT = opt;
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
            ViewBag.CustomerGroupID = CustomerGroupID;
            ViewBag.CustomerID = CustomerID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var paracusgroupid = new SqlParameter("@CustomerGroupID", CustomerGroupID);
            var paracusid = new SqlParameter("@CustomerID", CustomerID);
            List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
            if (FromDate != "" && ToDate != "" && CustomerGroupID != null && CustomerID == "")
            {
                if (opt == 1)
                {
                    paracusid.Value = DBNull.Value;
                    var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoChiTiet @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                    //List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
                    foreach (var item in result)
                    {
                        lst.Add(new BaoCaoTongHop()
                        {
                            AcceptDates = item.AcceptDates,
                            MailerID = item.MailerID,
                            Quantity = item.Quantity,
                            Weight = item.Weight,
                            RecieverProvince = item.RecieverProvince,
                            Notes = item.Notes,
                            RecieverName = item.RecieverName,
                            DeliveryDate = item.DeliveryDate,
                            CustomerName = item.CustomerName,
                            DeliveryStatus = item.DeliveryStatus
                        });
                    }
                }
                else
                {
                    paracusid.Value = DBNull.Value;
                    var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoTongHop @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                    foreach (var item in result)
                    {
                        lst.Add(new BaoCaoTongHop()
                        {
                            FromDate = FromDate,
                            ToDate = ToDate,
                            CustomerGroupID = CustomerGroupID,
                            CustomerID = CustomerID,
                            TongCG = item.TongCG,
                            TongPhat = item.TongPhat,
                            ChuaPhat = item.ChuaPhat
                        });
                    }
                }
            }
            if (FromDate != "" && ToDate != "" && CustomerGroupID != null && CustomerID != "")
            {
                if (opt == 1)
                {
                    var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoChiTiet @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                    //List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
                    foreach (var item in result)
                    {
                        lst.Add(new BaoCaoTongHop()
                        {
                            AcceptDates = item.AcceptDates,
                            MailerID = item.MailerID,
                            Quantity = item.Quantity,
                            Weight = item.Weight,
                            RecieverProvince = item.RecieverProvince,
                            Notes = item.Notes,
                            RecieverName = item.RecieverName,
                            DeliveryDate = item.DeliveryDate,
                            CustomerName = item.CustomerName,
                            DeliveryStatus = item.DeliveryStatus
                        });
                    }
                }
                else
                {
                    var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoTongHop @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                    foreach (var item in result)
                    {
                        lst.Add(new BaoCaoTongHop()
                        {
                            FromDate = FromDate,
                            ToDate = ToDate,
                            CustomerGroupID = CustomerGroupID,
                            CustomerID = CustomerID,
                            TongCG = item.TongCG,
                            TongPhat = item.TongPhat,
                            ChuaPhat = item.ChuaPhat
                        });
                    }
                }
            }
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            return View(lst.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public JsonResult GetData()
        {
            List<CustomerGroup> lstCus = new List<CustomerGroup>();
            var result = pms.Database.SqlQuery<CustomerGroup>("MM_CustomerGroup").ToList();
            foreach (var item in result)
            {
                lstCus.Add(new CustomerGroup()
                {
                    CustomerGroupID = item.CustomerGroupID,
                });
            }
            return Json(lstCus, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCustomerData(string CustomerGroupID = "")
        {
            List<Customer> lstCus = new List<Customer>();
            var customergroupid = new SqlParameter("@CustomerGroupID", CustomerGroupID);
            var result = pms.Database.SqlQuery<Customer>("MM_FindCustomerID @CustomerGroupID", customergroupid).ToList();
            foreach (var item in result)
            {
                lstCus.Add(new Customer()
                {
                    CustomerID = item.CustomerID,
                });
            }
            return Json(lstCus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrackingSLDT(string FromDate, string ToDate, int? page, int opt = 0)
        {
            string fDate;
            string fTo;
            ViewBag.OPT = opt;
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
            List<DuongTruc_KTNhan> lst = new List<DuongTruc_KTNhan>();
            if (FromDate != "" && ToDate != "")
            {
                if (opt == 0)
                {
                    var result = sgp.Database.SqlQuery<DuongTruc_KTNhan>("SGP_WEB_DuongTruc_BCKT @FromDate,@ToDate", parafrom, parato).ToList();
                    foreach (var item in result)
                    {
                        lst.Add(new DuongTruc_KTNhan()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            MailerID = item.MailerID,
                            RecieverProvinceID = item.RecieverProvinceID,
                            Quantity = item.Quantity,
                            ZoneID = item.ZoneID,
                            ServiceTypeID = item.ServiceTypeID,
                            Weight = item.Weight
                        });
                    }
                }
                else if (opt == 1)
                {
                    var result = sgp.Database.SqlQuery<DuongTruc_KTNhan>("SGP_WEB_DuongTruc_KTNhan @FromDate,@ToDate", parafrom, parato).ToList();
                    foreach (var item in result)
                    {
                        lst.Add(new DuongTruc_KTNhan()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            MailerID = item.MailerID,
                            RecieverProvinceID = item.RecieverProvinceID,
                            Quantity = item.Quantity,
                            ZoneID = item.ZoneID,
                            ServiceTypeID = item.ServiceTypeID,
                            Weight = item.Weight
                        });
                    }
                }
                else if (opt == 2)
                {

                }
            }
            int pageSize = 25;
            int pageNumber = (page ?? 1);
            //return View(lst.ToPagedList(pageNumber, pageSize));
            return View(lst.ToPagedList(pageNumber, pageSize));
        }
        //[Authorize(Roles = "Accounting")]
        public ActionResult DHLplan()
        {
            return View();
        }

        public ActionResult ExcelDHLplan(string FromDate, string ToDate)
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
            string pathRoot = Server.MapPath("~/Report/luyke.xlsx");
            string name = "luyke" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);
            System.IO.File.Copy(pathRoot, pathTo);
            List<DHLPlan> lst = new List<DHLPlan>();
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            if (FromDate != "" && ToDate != "")
            {
                var data = dbl.Database.SqlQuery<DHLPlan>("DHL_Plan @FromDate,@ToDate", parafrom, parato).ToList();
                foreach (var item in data)
                {
                    lst.Add(new DHLPlan()
                    {
                        ID = item.ID,
                        CG = item.CG,
                        D_O = item.D_O,
                        Contact1 = item.Contact1,
                        Contact2 = item.Contact2,
                        Contact3 = item.Contact3,
                        DeliveryDate = item.DeliveryDate,
                        Employee = item.Employee,
                        KH = item.KH,
                        PGI = item.PGI,
                        Quantity = item.Quantity,
                        SenderAddress = item.SenderAddress,
                        SenderName = item.SenderName,
                        ShiptoAddress = item.ShiptoAddress,
                        ShiptoNM = item.ShiptoNM,
                        SL = item.SL,
                        Subcon = item.Subcon,
                        TL = item.TL,
                        TongSL = item.TongSL,
                        ToNodeCode = item.ToNodeCode,
                        ToZone = item.ToZone,
                        TP = item.TP,
                        Unit1 = item.Unit1,
                        Unit2 = item.Unit2,
                        Unit3 = item.Unit3,
                        Weight = item.Weight,
                        Zone = item.Zone,
                        ZoneDesc = item.ZoneDesc
                    });
                }
                FileInfo newFile = new FileInfo(pathTo);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < lst.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = lst[i].CG;
                            worksheet.Cells[i + 2, 2].Value = lst[i].Contact1;
                            worksheet.Cells[i + 2, 3].Value = lst[i].Contact2;
                            worksheet.Cells[i + 2, 4].Value = lst[i].Contact3;
                            worksheet.Cells[i + 2, 5].Value = lst[i].D_O;
                            worksheet.Cells[i + 2, 6].Value = lst[i].DeliveryDate;
                            worksheet.Cells[i + 2, 7].Value = lst[i].Employee;

                            worksheet.Cells[i + 2, 8].Value = lst[i].ID;
                            worksheet.Cells[i + 2, 9].Value = lst[i].KH;
                            worksheet.Cells[i + 2, 10].Value = lst[i].PGI;
                            worksheet.Cells[i + 2, 11].Value = lst[i].Quantity;
                            worksheet.Cells[i + 2, 12].Value = lst[i].SenderAddress;
                            worksheet.Cells[i + 2, 13].Value = lst[i].SenderName;
                            worksheet.Cells[i + 2, 14].Value = lst[i].ShiptoAddress;

                            worksheet.Cells[i + 2, 15].Value = lst[i].ShiptoNM;
                            worksheet.Cells[i + 2, 16].Value = lst[i].SL;
                            worksheet.Cells[i + 2, 17].Value = lst[i].Subcon;
                            worksheet.Cells[i + 2, 18].Value = lst[i].TL;
                            worksheet.Cells[i + 2, 19].Value = lst[i].TongSL;
                            worksheet.Cells[i + 2, 20].Value = lst[i].ToNodeCode;
                            worksheet.Cells[i + 2, 21].Value = lst[i].ToZone;

                            worksheet.Cells[i + 2, 22].Value = lst[i].TP;
                            worksheet.Cells[i + 2, 23].Value = lst[i].Unit1;
                            worksheet.Cells[i + 2, 24].Value = lst[i].Unit2;
                            worksheet.Cells[i + 2, 25].Value = lst[i].Unit3;
                            worksheet.Cells[i + 2, 26].Value = lst[i].Weight;
                            worksheet.Cells[i + 2, 27].Value = lst[i].Zone;
                            worksheet.Cells[i + 2, 28].Value = lst[i].ZoneDesc;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }
            }

            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("luyke" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }

        public ActionResult KhaiThac()
        {
            return View();
        }

        public ActionResult ChiPhiNgoaiTuyen(string Address = "", string InputAddress = "", string Fee = "")
        {
            if (Address != "" && InputAddress != "")
            {
                Calculate(Address, InputAddress);
            }
            return View();
        }

        public void Calculate(string Address = "", string InputAddress = "")
        {
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(Address);
            var locationService1 = new GoogleLocationService();
            var point1 = locationService1.GetLatLongFromAddress(InputAddress);
            double circumference = 40000.0; // Earth's circumference at the equator in km
            double distance = 0.0;

            //Calculate radians
            double latitude1Rad = DegreesToRadians(point.Latitude);
            double longitude1Rad = DegreesToRadians(point.Longitude);
            double latititude2Rad = DegreesToRadians(point1.Latitude);
            double longitude2Rad = DegreesToRadians(point1.Longitude);

            double logitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);

            if (logitudeDiff > Math.PI)
            {
                logitudeDiff = 2.0 * Math.PI - logitudeDiff;
            }

            double angleCalculation =
                Math.Acos(
                  Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) +
                  Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff));

            distance = circumference * angleCalculation / (2.0 * Math.PI);
            @ViewBag.Distance = distance;
            int budget = Convert.ToInt32(distance) * 3500;
            @ViewBag.Budget = budget;
        }

        public double DegreesToRadians(double a)
        {
            return a * (Math.PI / 180);
        }

        public ActionResult getAddress(string PostOfficeName = "")
        {
            var result = pms.MM_PostOffices.Where(s => s.PostOfficeName == PostOfficeName).FirstOrDefault();
            return Json(new { address = result.Address }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetPostOffice()
        {
            List<PostOfficeAddress> lstPost = new List<PostOfficeAddress>();
            var result = test.Database.SqlQuery<PostOfficeAddress>("MM_GetPostOffice").ToList();
            foreach (var item in result)
            {
                lstPost.Add(new PostOfficeAddress()
                {
                    PostOfficeName = item.PostOfficeName,
                });
            }
            return Json(lstPost, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeSoLieu()
        {

            return View();
        }

        public ActionResult ExcelThongKeSoLieu(string FromDate, string ToDate)
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
            string pathRoot = Server.MapPath("~/Report/thongkesolieu.xlsx");
            string name = "thongkesolieu" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);
            System.IO.File.Copy(pathRoot, pathTo);
            List<ThongKeSoLieu> lst = new List<ThongKeSoLieu>();
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            if (FromDate != "" && ToDate != "")
            {
                var data = pms.Database.SqlQuery<ThongKeSoLieu>("SGP_WEB_ThongKeSoLieuTrongTungNhom @FromDate,@ToDate", parafrom, parato).ToList();
                foreach (var item in data)
                {
                    lst.Add(new ThongKeSoLieu()
                    {
                        MailerID = item.MailerID,
                        AcceptDate = item.AcceptDate,
                        Quantity = item.Quantity,
                        Weight = item.Weight,
                        RealWeight = item.RealWeight,
                        RecieverProvinceID = item.RecieverProvinceID,
                        ServiceTypeID = item.ServiceTypeID,
                        Price = item.Price,
                        BefVATAmount = item.BefVATAmount
                    });
                }
                FileInfo newFile = new FileInfo(pathTo);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < lst.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = lst[i].MailerID;
                            worksheet.Cells[i + 2, 2].Value = lst[i].AcceptDate;
                            worksheet.Cells[i + 2, 3].Value = lst[i].Quantity;
                            worksheet.Cells[i + 2, 4].Value = lst[i].Weight;
                            worksheet.Cells[i + 2, 5].Value = lst[i].RealWeight;
                            worksheet.Cells[i + 2, 6].Value = lst[i].RecieverProvinceID;
                            worksheet.Cells[i + 2, 7].Value = lst[i].ServiceTypeID;

                            worksheet.Cells[i + 2, 8].Value = lst[i].Price;
                            worksheet.Cells[i + 2, 9].Value = lst[i].BefVATAmount;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }
            }

            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("thongkesolieu" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));

        }
        [HttpPost]
        public ActionResult SpecialExcel(string FromDate, string ToDate, string CustomerGroupID = "", string CustomerID = "")
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
            string pathRoot = Server.MapPath("~/Report/baocaophattheokh.xlsx");
            string name = "baocaophattheokh" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string pathTo = Server.MapPath("~/Temp/" + name);
            System.IO.File.Copy(pathRoot, pathTo);
            List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var paracusgroupid = new SqlParameter("@CustomerGroupID", CustomerGroupID);
            var paracusid = new SqlParameter("@CustomerID", CustomerID);
            if (FromDate != "" && ToDate != "" && CustomerGroupID != null && CustomerID == "")
            {
                paracusid.Value = DBNull.Value;
                var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoChiTiet @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                //List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
                foreach (var item in result)
                {
                    lst.Add(new BaoCaoTongHop()
                    {
                        AcceptDates = item.AcceptDates,
                        MailerID = item.MailerID,
                        Quantity = item.Quantity,
                        Weight = item.Weight,
                        RecieverProvince = item.RecieverProvince,
                        Notes = item.Notes,
                        RecieverName = item.RecieverName,
                        DeliveryDate = item.DeliveryDate,
                        CustomerName = item.CustomerName,
                        DeliveryStatus = item.DeliveryStatus
                    });
                }

                FileInfo newFile = new FileInfo(pathTo);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < lst.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = lst[i].AcceptDates;
                            worksheet.Cells[i + 2, 2].Value = lst[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = lst[i].Quantity;
                            worksheet.Cells[i + 2, 4].Value = lst[i].Weight;
                            worksheet.Cells[i + 2, 5].Value = lst[i].RecieverProvince;
                            worksheet.Cells[i + 2, 6].Value = lst[i].Notes;
                            worksheet.Cells[i + 2, 7].Value = lst[i].RecieverName;

                            worksheet.Cells[i + 2, 8].Value = lst[i].DeliveryDate;
                            worksheet.Cells[i + 2, 9].Value = lst[i].DeliveryStatus;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }
            }
            if (FromDate != "" && ToDate != "" && CustomerGroupID != null && CustomerID != "")
            {
                var result = pms.Database.SqlQuery<BaoCaoTongHop>("SGP_WEB_BaoCaoChiTiet @FromDate,@ToDate,@CustomerGroupID,@CustomerID", parafrom, parato, paracusgroupid, paracusid).ToList();
                //List<BaoCaoTongHop> lst = new List<BaoCaoTongHop>();
                foreach (var item in result)
                {
                    lst.Add(new BaoCaoTongHop()
                    {
                        AcceptDates = item.AcceptDates,
                        MailerID = item.MailerID,
                        Quantity = item.Quantity,
                        Weight = item.Weight,
                        RecieverProvince = item.RecieverProvince,
                        Notes = item.Notes,
                        RecieverName = item.RecieverName,
                        DeliveryDate = item.DeliveryDate,
                        CustomerName = item.CustomerName,
                        DeliveryStatus = item.DeliveryStatus
                    });
                }
                FileInfo newFile = new FileInfo(pathTo);
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];

                    for (int i = 0; i < lst.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 2, 1].Value = lst[i].AcceptDates;
                            worksheet.Cells[i + 2, 2].Value = lst[i].MailerID;
                            worksheet.Cells[i + 2, 3].Value = lst[i].Quantity;
                            worksheet.Cells[i + 2, 4].Value = lst[i].Weight;
                            worksheet.Cells[i + 2, 5].Value = lst[i].RecieverProvince;
                            worksheet.Cells[i + 2, 6].Value = lst[i].Notes;
                            worksheet.Cells[i + 2, 7].Value = lst[i].RecieverName;

                            worksheet.Cells[i + 2, 8].Value = lst[i].DeliveryDate;
                            worksheet.Cells[i + 2, 9].Value = lst[i].DeliveryStatus;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }

                    package.Save();

                }
            }
            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("baocaophattheokh" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }

        public ActionResult SanLuongNhan()
        {
            return View();
        }

        public ActionResult ExcelSLNhan(string Date)
        {
            string pathRoot = Server.MapPath("~/Report/sanluongnhan.xlsx");
            string name = "sanluongnhan" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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
                var paradate4 = new SqlParameter("@Date", date);
                var paradate5 = new SqlParameter("@Date", date);
                var paradate6 = new SqlParameter("@Date", date);
                var paradate7 = new SqlParameter("@Date", date);
                var paradate8 = new SqlParameter("@Date", date);
                var paradate9 = new SqlParameter("@Date", date);
                var paradate10 = new SqlParameter("@Date", date);
                var paradate11 = new SqlParameter("@Date", date);
                var paradate12 = new SqlParameter("@Date", date);
                var paradate13 = new SqlParameter("@Date", date);
                var paradate14 = new SqlParameter("@Date", date);
                var paradate15 = new SqlParameter("@Date", date);

                var paradate16 = new SqlParameter("@Date", date);
                var paradate17 = new SqlParameter("@Date", date);
                var paradate18 = new SqlParameter("@Date", date);
                var paradate19 = new SqlParameter("@Date", date);
                var paradate20 = new SqlParameter("@Date", date);
                var paradate21 = new SqlParameter("@Date", date);
                var paradate22 = new SqlParameter("@Date", date);
                var paradate23 = new SqlParameter("@Date", date);
                var paradate24 = new SqlParameter("@Date", date);
                var paradate25 = new SqlParameter("@Date", date);
                var paradate26 = new SqlParameter("@Date", date);
                var paradate27 = new SqlParameter("@Date", date);
                var paradate28 = new SqlParameter("@Date", date);
                var paradate29 = new SqlParameter("@Date", date);
                var paradate30 = new SqlParameter("@Date", date);
                var paradate31 = new SqlParameter("@Date", date);

                var paradate32 = new SqlParameter("@Date", date);
                var paradate33 = new SqlParameter("@Date", date);
                var paradate34 = new SqlParameter("@Date", date);
                var paradate35 = new SqlParameter("@Date", date);
                var paradate36 = new SqlParameter("@Date", date);
                var paradate37 = new SqlParameter("@Date", date);
                var paradate38 = new SqlParameter("@Date", date);
                var paradate39 = new SqlParameter("@Date", date);
                var paradate40 = new SqlParameter("@Date", date);
                var paradate41 = new SqlParameter("@Date", date);
                var paradate42 = new SqlParameter("@Date", date);
                var paradate43 = new SqlParameter("@Date", date);
                var paradate44 = new SqlParameter("@Date", date);
                var paradate45 = new SqlParameter("@Date", date);
                var paradate46 = new SqlParameter("@Date", date);
                var paradate47 = new SqlParameter("@Date", date);

                List<SanLuongNhanDuoi2CPN> list = new List<SanLuongNhanDuoi2CPN>();
                List<SanLuongNhanTren2CPN> list1 = new List<SanLuongNhanTren2CPN>();
                List<SanLuongNhanTren8CPN> list2 = new List<SanLuongNhanTren8CPN>();
                List<SanLuongNhanTren50CPN> list3 = new List<SanLuongNhanTren50CPN>();
                List<SanLuongNhanDuoi2CPT> list4 = new List<SanLuongNhanDuoi2CPT>();
                List<SanLuongNhanTren2CPT> list5 = new List<SanLuongNhanTren2CPT>();
                List<SanLuongNhanTren8CPT> list6 = new List<SanLuongNhanTren8CPT>();
                List<SanLuongNhanTren50CPT> list7 = new List<SanLuongNhanTren50CPT>();
                List<SanLuongNhanDuoi2EMS> list8 = new List<SanLuongNhanDuoi2EMS>();
                List<SanLuongNhanTren2EMS> list9 = new List<SanLuongNhanTren2EMS>();
                List<SanLuongNhanTren8EMS> list10 = new List<SanLuongNhanTren8EMS>();
                List<SanLuongNhanTren50EMS> list11 = new List<SanLuongNhanTren50EMS>();
                List<SanLuongNhanDuoi2QT> list12 = new List<SanLuongNhanDuoi2QT>();
                List<SanLuongNhanTren2QT> list13 = new List<SanLuongNhanTren2QT>();
                List<SanLuongNhanTren8QT> list14 = new List<SanLuongNhanTren8QT>();
                List<SanLuongNhanTren50QT> list15 = new List<SanLuongNhanTren50QT>();

                List<SanLuongNhanDuoi2CPNKV3> list16 = new List<SanLuongNhanDuoi2CPNKV3>();
                List<SanLuongNhanTren2CPNKV3> list17 = new List<SanLuongNhanTren2CPNKV3>();
                List<SanLuongNhanTren8CPNKV3> list18 = new List<SanLuongNhanTren8CPNKV3>();
                List<SanLuongNhanTren50CPNKV3> list19 = new List<SanLuongNhanTren50CPNKV3>();
                List<SanLuongNhanDuoi2CPTKV3> list20 = new List<SanLuongNhanDuoi2CPTKV3>();
                List<SanLuongNhanTren2CPTKV3> list21 = new List<SanLuongNhanTren2CPTKV3>();
                List<SanLuongNhanTren8CPTKV3> list22 = new List<SanLuongNhanTren8CPTKV3>();
                List<SanLuongNhanTren50CPTKV3> list23 = new List<SanLuongNhanTren50CPTKV3>();
                List<SanLuongNhanDuoi2EMSKV3> list24 = new List<SanLuongNhanDuoi2EMSKV3>();
                List<SanLuongNhanTren2EMSKV3> list25 = new List<SanLuongNhanTren2EMSKV3>();
                List<SanLuongNhanTren8EMSKV3> list26 = new List<SanLuongNhanTren8EMSKV3>();
                List<SanLuongNhanTren50EMSKV3> list27 = new List<SanLuongNhanTren50EMSKV3>();
                List<SanLuongNhanDuoi2QTKV3> list28 = new List<SanLuongNhanDuoi2QTKV3>();
                List<SanLuongNhanTren2QTKV3> list29 = new List<SanLuongNhanTren2QTKV3>();
                List<SanLuongNhanTren8QTKV3> list30 = new List<SanLuongNhanTren8QTKV3>();
                List<SanLuongNhanTren50QTKV3> list31 = new List<SanLuongNhanTren50QTKV3>();

                List<SanLuongNhanDuoi2CPNKV4> list32 = new List<SanLuongNhanDuoi2CPNKV4>();
                List<SanLuongNhanTren2CPNKV4> list33 = new List<SanLuongNhanTren2CPNKV4>();
                List<SanLuongNhanTren8CPNKV4> list34 = new List<SanLuongNhanTren8CPNKV4>();
                List<SanLuongNhanTren50CPNKV4> list35 = new List<SanLuongNhanTren50CPNKV4>();
                List<SanLuongNhanDuoi2CPTKV4> list36 = new List<SanLuongNhanDuoi2CPTKV4>();
                List<SanLuongNhanTren2CPTKV4> list37 = new List<SanLuongNhanTren2CPTKV4>();
                List<SanLuongNhanTren8CPTKV4> list38 = new List<SanLuongNhanTren8CPTKV4>();
                List<SanLuongNhanTren50CPTKV4> list39 = new List<SanLuongNhanTren50CPTKV4>();
                List<SanLuongNhanDuoi2EMSKV4> list40 = new List<SanLuongNhanDuoi2EMSKV4>();
                List<SanLuongNhanTren2EMSKV4> list41 = new List<SanLuongNhanTren2EMSKV4>();
                List<SanLuongNhanTren8EMSKV4> list42 = new List<SanLuongNhanTren8EMSKV4>();
                List<SanLuongNhanTren50EMSKV4> list43 = new List<SanLuongNhanTren50EMSKV4>();
                List<SanLuongNhanDuoi2QTKV4> list44 = new List<SanLuongNhanDuoi2QTKV4>();
                List<SanLuongNhanTren2QTKV4> list45 = new List<SanLuongNhanTren2QTKV4>();
                List<SanLuongNhanTren8QTKV4> list46 = new List<SanLuongNhanTren8QTKV4>();
                List<SanLuongNhanTren50QTKV4> list47 = new List<SanLuongNhanTren50QTKV4>();
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPN>("SGP_SanLuongNhanDuoi2CPNKV2 @Date", paradate).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new SanLuongNhanDuoi2CPN()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPN>("SGP_SanLuongNhanTren2CPNKV2 @Date", paradate1).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list1.Add(new SanLuongNhanTren2CPN()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPN>("SGP_SanLuongNhanTren8CPNKV2 @Date", paradate2).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list2.Add(new SanLuongNhanTren8CPN()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPN>("SGP_SanLuongNhanTren50CPNKV2 @Date", paradate3).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list3.Add(new SanLuongNhanTren50CPN()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPT>("SGP_SanLuongNhanDuoi2CPTKV2 @Date", paradate4).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list4.Add(new SanLuongNhanDuoi2CPT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPT>("SGP_SanLuongNhanTren2CPTKV2 @Date", paradate5).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list5.Add(new SanLuongNhanTren2CPT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPT>("SGP_SanLuongNhanTren8CPTKV2 @Date", paradate6).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list6.Add(new SanLuongNhanTren8CPT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPT>("SGP_SanLuongNhanTren50CPTKV2 @Date", paradate7).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list7.Add(new SanLuongNhanTren50CPT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2EMS>("SGP_SanLuongNhanDuoi2EMSKV2 @Date", paradate8).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list8.Add(new SanLuongNhanDuoi2EMS()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2EMS>("SGP_SanLuongNhanTren2EMSKV2 @Date", paradate9).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list9.Add(new SanLuongNhanTren2EMS()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8EMS>("SGP_SanLuongNhanTren8EMSKV2 @Date", paradate10).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list10.Add(new SanLuongNhanTren8EMS()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50EMS>("SGP_SanLuongNhanTren50EMSKV2 @Date", paradate11).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list11.Add(new SanLuongNhanTren50EMS()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2QT>("SGP_SanLuongNhanDuoi2QTKV2 @Date", paradate12).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list12.Add(new SanLuongNhanDuoi2QT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2QT>("SGP_SanLuongNhanTren2QTKV2 @Date", paradate13).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list13.Add(new SanLuongNhanTren2QT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8QT>("SGP_SanLuongNhanTren8QTKV2 @Date", paradate14).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list14.Add(new SanLuongNhanTren8QT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50QT>("SGP_SanLuongNhanTren50QTKV2 @Date", paradate15).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list15.Add(new SanLuongNhanTren50QT()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                //////////////////////////
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPNKV3>("SGP_SanLuongNhanDuoi2CPNKV3 @Date", paradate16).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list16.Add(new SanLuongNhanDuoi2CPNKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPNKV3>("SGP_SanLuongNhanTren2CPNKV3 @Date", paradate17).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list17.Add(new SanLuongNhanTren2CPNKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPNKV3>("SGP_SanLuongNhanTren8CPNKV3 @Date", paradate18).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list18.Add(new SanLuongNhanTren8CPNKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPNKV3>("SGP_SanLuongNhanTren50CPNKV3 @Date", paradate19).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list19.Add(new SanLuongNhanTren50CPNKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPTKV3>("SGP_SanLuongNhanDuoi2CPTKV3 @Date", paradate20).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list20.Add(new SanLuongNhanDuoi2CPTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPTKV3>("SGP_SanLuongNhanTren2CPTKV3 @Date", paradate21).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list21.Add(new SanLuongNhanTren2CPTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPTKV3>("SGP_SanLuongNhanTren8CPTKV3 @Date", paradate22).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list22.Add(new SanLuongNhanTren8CPTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPTKV3>("SGP_SanLuongNhanTren50CPTKV3 @Date", paradate23).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list23.Add(new SanLuongNhanTren50CPTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2EMSKV3>("SGP_SanLuongNhanDuoi2EMSKV3 @Date", paradate24).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list24.Add(new SanLuongNhanDuoi2EMSKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2EMSKV3>("SGP_SanLuongNhanTren2EMSKV3 @Date", paradate25).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list25.Add(new SanLuongNhanTren2EMSKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8EMSKV3>("SGP_SanLuongNhanTren8EMSKV3 @Date", paradate26).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list26.Add(new SanLuongNhanTren8EMSKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50EMSKV3>("SGP_SanLuongNhanTren50EMSKV3 @Date", paradate27).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list27.Add(new SanLuongNhanTren50EMSKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2QTKV3>("SGP_SanLuongNhanDuoi2QTKV3 @Date", paradate28).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list28.Add(new SanLuongNhanDuoi2QTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2QTKV3>("SGP_SanLuongNhanTren2QTKV3 @Date", paradate29).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list29.Add(new SanLuongNhanTren2QTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8QTKV3>("SGP_SanLuongNhanTren8QTKV3 @Date", paradate30).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list30.Add(new SanLuongNhanTren8QTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50QTKV3>("SGP_SanLuongNhanTren50QTKV3 @Date", paradate31).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list31.Add(new SanLuongNhanTren50QTKV3()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                //////////////////////////
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPNKV4>("SGP_SanLuongNhanDuoi2CPNKV4 @Date", paradate32).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list32.Add(new SanLuongNhanDuoi2CPNKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPNKV4>("SGP_SanLuongNhanTren2CPNKV4 @Date", paradate33).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list33.Add(new SanLuongNhanTren2CPNKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPNKV4>("SGP_SanLuongNhanTren8CPNKV4 @Date", paradate34).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list34.Add(new SanLuongNhanTren8CPNKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPNKV4>("SGP_SanLuongNhanTren50CPNKV4 @Date", paradate35).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list35.Add(new SanLuongNhanTren50CPNKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2CPTKV4>("SGP_SanLuongNhanDuoi2CPTKV4 @Date", paradate36).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list36.Add(new SanLuongNhanDuoi2CPTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2CPTKV4>("SGP_SanLuongNhanTren2CPTKV4 @Date", paradate37).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list37.Add(new SanLuongNhanTren2CPTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8CPTKV4>("SGP_SanLuongNhanTren8CPTKV4 @Date", paradate38).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list38.Add(new SanLuongNhanTren8CPTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50CPTKV4>("SGP_SanLuongNhanTren50CPTKV4 @Date", paradate39).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list39.Add(new SanLuongNhanTren50CPTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2EMSKV4>("SGP_SanLuongNhanDuoi2EMSKV4 @Date", paradate40).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list40.Add(new SanLuongNhanDuoi2EMSKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2EMSKV4>("SGP_SanLuongNhanTren2EMSKV4 @Date", paradate41).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list41.Add(new SanLuongNhanTren2EMSKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8EMSKV4>("SGP_SanLuongNhanTren8EMSKV4 @Date", paradate42).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list42.Add(new SanLuongNhanTren8EMSKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50EMSKV4>("SGP_SanLuongNhanTren50EMSKV4 @Date", paradate43).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list43.Add(new SanLuongNhanTren50EMSKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanDuoi2QTKV4>("SGP_SanLuongNhanDuoi2QTKV4 @Date", paradate44).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list44.Add(new SanLuongNhanDuoi2QTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren2QTKV4>("SGP_SanLuongNhanTren2QTKV4 @Date", paradate45).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list45.Add(new SanLuongNhanTren2QTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren8QTKV4>("SGP_SanLuongNhanTren8QTKV4 @Date", paradate46).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list46.Add(new SanLuongNhanTren8QTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongNhanTren50QTKV4>("SGP_SanLuongNhanTren50QTKV4 @Date", paradate47).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list47.Add(new SanLuongNhanTren50QTKV4()
                        {
                            PostOfficeAcceptID = item.PostOfficeAcceptID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    //date = DateTime.ParseExact(Request["Date"], "dd/MM/yyyy", null).ToString("MM");
                    //string thang = date;
                    //date = DateTime.ParseExact(Request["Date"], "dd/MM/yyyy", null).ToString("yyyy");
                    //string nam = date;
                    //worksheet.Cells[1, 2].Value = "Tháng " + thang + "/" + nam;
                    for (int i = 0; i < list.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 12, 1].Value = list[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 12, 5].Value = list[i].SL;
                            worksheet.Cells[i + 12, 6].Value = list[i].TL;
                            worksheet.Cells[i + 12, 7].Value = list[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    int count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list1.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list1[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 8].Value = list1[count].SL;
                                    worksheet.Cells[i + 12, 9].Value = list1[count].TL;
                                    worksheet.Cells[i + 12, 10].Value = list1[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list1[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 8].Value = "";
                                    worksheet.Cells[i + 12, 9].Value = "";
                                    worksheet.Cells[i + 12, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list2.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list2[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 11].Value = list2[count].SL;
                                    worksheet.Cells[i + 12, 12].Value = list2[count].TL;
                                    worksheet.Cells[i + 12, 13].Value = list2[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list2[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 11].Value = "";
                                    worksheet.Cells[i + 12, 12].Value = "";
                                    worksheet.Cells[i + 12, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list3.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list3[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 14].Value = list3[count].SL;
                                    worksheet.Cells[i + 12, 15].Value = list3[count].TL;
                                    worksheet.Cells[i + 12, 16].Value = list3[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list3[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 14].Value = "";
                                    worksheet.Cells[i + 12, 15].Value = "";
                                    worksheet.Cells[i + 12, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list4.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list4[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 17].Value = list4[count].SL;
                                    worksheet.Cells[i + 12, 18].Value = list4[count].TL;
                                    worksheet.Cells[i + 12, 19].Value = list4[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list4[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 17].Value = "";
                                    worksheet.Cells[i + 12, 18].Value = "";
                                    worksheet.Cells[i + 12, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list5.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list5[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 20].Value = list5[count].SL;
                                    worksheet.Cells[i + 12, 21].Value = list5[count].TL;
                                    worksheet.Cells[i + 12, 22].Value = list5[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list5[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 20].Value = "";
                                    worksheet.Cells[i + 12, 21].Value = "";
                                    worksheet.Cells[i + 12, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list6.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list6[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 23].Value = list6[count].SL;
                                    worksheet.Cells[i + 12, 24].Value = list6[count].TL;
                                    worksheet.Cells[i + 12, 25].Value = list6[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list6[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 23].Value = "";
                                    worksheet.Cells[i + 12, 24].Value = "";
                                    worksheet.Cells[i + 12, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list7.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list7[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 26].Value = list7[count].SL;
                                    worksheet.Cells[i + 12, 27].Value = list7[count].TL;
                                    worksheet.Cells[i + 12, 28].Value = list7[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list7[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 26].Value = "";
                                    worksheet.Cells[i + 12, 27].Value = "";
                                    worksheet.Cells[i + 12, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list8.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list8[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 29].Value = list8[count].SL;
                                    worksheet.Cells[i + 12, 30].Value = list8[count].TL;
                                    worksheet.Cells[i + 12, 31].Value = list8[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list8[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 29].Value = "";
                                    worksheet.Cells[i + 12, 30].Value = "";
                                    worksheet.Cells[i + 12, 31].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list9.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list9[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 32].Value = list9[count].SL;
                                    worksheet.Cells[i + 12, 33].Value = list9[count].TL;
                                    worksheet.Cells[i + 12, 34].Value = list9[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list9[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 32].Value = "";
                                    worksheet.Cells[i + 12, 33].Value = "";
                                    worksheet.Cells[i + 12, 34].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list10.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list10[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 35].Value = list10[count].SL;
                                    worksheet.Cells[i + 12, 36].Value = list10[count].TL;
                                    worksheet.Cells[i + 12, 37].Value = list10[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list10[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 35].Value = "";
                                    worksheet.Cells[i + 12, 36].Value = "";
                                    worksheet.Cells[i + 12, 37].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list11.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list11[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 38].Value = list11[count].SL;
                                    worksheet.Cells[i + 12, 39].Value = list11[count].TL;
                                    worksheet.Cells[i + 12, 40].Value = list11[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list11[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 38].Value = "";
                                    worksheet.Cells[i + 12, 39].Value = "";
                                    worksheet.Cells[i + 12, 40].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list12.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list12[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 41].Value = list12[count].SL;
                                    worksheet.Cells[i + 12, 42].Value = list12[count].TL;
                                    worksheet.Cells[i + 12, 43].Value = list12[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list12[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 41].Value = "";
                                    worksheet.Cells[i + 12, 42].Value = "";
                                    worksheet.Cells[i + 12, 43].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list13.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list13[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 44].Value = list13[count].SL;
                                    worksheet.Cells[i + 12, 45].Value = list13[count].TL;
                                    worksheet.Cells[i + 12, 46].Value = list13[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list13[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 44].Value = "";
                                    worksheet.Cells[i + 12, 45].Value = "";
                                    worksheet.Cells[i + 12, 46].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list14.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list14[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 47].Value = list14[count].SL;
                                    worksheet.Cells[i + 12, 48].Value = list14[count].TL;
                                    worksheet.Cells[i + 12, 49].Value = list14[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list14[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 47].Value = "";
                                    worksheet.Cells[i + 12, 48].Value = "";
                                    worksheet.Cells[i + 12, 49].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list15.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list15[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 50].Value = list15[count].SL;
                                    worksheet.Cells[i + 12, 51].Value = list15[count].TL;
                                    worksheet.Cells[i + 12, 52].Value = list15[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list15[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 50].Value = "";
                                    worksheet.Cells[i + 12, 51].Value = "";
                                    worksheet.Cells[i + 12, 52].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    ////////////////////////
                    for (int i = 0; i < list16.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 108, 1].Value = list16[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 108, 5].Value = list16[i].SL;
                            worksheet.Cells[i + 108, 6].Value = list16[i].TL;
                            worksheet.Cells[i + 108, 7].Value = list16[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list17.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list17[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 8].Value = list17[count].SL;
                                    worksheet.Cells[i + 108, 9].Value = list17[count].TL;
                                    worksheet.Cells[i + 108, 10].Value = list17[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list17[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 8].Value = "";
                                    worksheet.Cells[i + 108, 9].Value = "";
                                    worksheet.Cells[i + 108, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list18.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list18[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 11].Value = list18[count].SL;
                                    worksheet.Cells[i + 108, 12].Value = list18[count].TL;
                                    worksheet.Cells[i + 108, 13].Value = list18[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list18[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 11].Value = "";
                                    worksheet.Cells[i + 108, 12].Value = "";
                                    worksheet.Cells[i + 108, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list19.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list19[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 14].Value = list19[count].SL;
                                    worksheet.Cells[i + 108, 15].Value = list19[count].TL;
                                    worksheet.Cells[i + 108, 16].Value = list19[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list19[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 14].Value = "";
                                    worksheet.Cells[i + 108, 15].Value = "";
                                    worksheet.Cells[i + 108, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list20.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list20[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 17].Value = list20[count].SL;
                                    worksheet.Cells[i + 108, 18].Value = list20[count].TL;
                                    worksheet.Cells[i + 108, 19].Value = list20[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list20[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 17].Value = "";
                                    worksheet.Cells[i + 108, 18].Value = "";
                                    worksheet.Cells[i + 108, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list21.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list21[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 20].Value = list21[count].SL;
                                    worksheet.Cells[i + 108, 21].Value = list21[count].TL;
                                    worksheet.Cells[i + 108, 22].Value = list21[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list21[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 20].Value = "";
                                    worksheet.Cells[i + 108, 21].Value = "";
                                    worksheet.Cells[i + 108, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list22.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list22[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 23].Value = list22[count].SL;
                                    worksheet.Cells[i + 108, 24].Value = list22[count].TL;
                                    worksheet.Cells[i + 108, 25].Value = list22[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list22[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 23].Value = "";
                                    worksheet.Cells[i + 108, 24].Value = "";
                                    worksheet.Cells[i + 108, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list23.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list23[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 26].Value = list23[count].SL;
                                    worksheet.Cells[i + 108, 27].Value = list23[count].TL;
                                    worksheet.Cells[i + 108, 28].Value = list23[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list23[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 26].Value = "";
                                    worksheet.Cells[i + 108, 27].Value = "";
                                    worksheet.Cells[i + 108, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list24.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list24[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 29].Value = list24[count].SL;
                                    worksheet.Cells[i + 108, 30].Value = list24[count].TL;
                                    worksheet.Cells[i + 108, 31].Value = list24[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list24[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 29].Value = "";
                                    worksheet.Cells[i + 108, 30].Value = "";
                                    worksheet.Cells[i + 108, 31].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list25.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list25[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 32].Value = list25[count].SL;
                                    worksheet.Cells[i + 108, 33].Value = list25[count].TL;
                                    worksheet.Cells[i + 108, 34].Value = list25[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list25[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 32].Value = "";
                                    worksheet.Cells[i + 108, 33].Value = "";
                                    worksheet.Cells[i + 108, 34].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list26.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list26[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 35].Value = list26[count].SL;
                                    worksheet.Cells[i + 108, 36].Value = list26[count].TL;
                                    worksheet.Cells[i + 108, 37].Value = list26[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list26[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 35].Value = "";
                                    worksheet.Cells[i + 108, 36].Value = "";
                                    worksheet.Cells[i + 108, 37].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list27.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list27[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 38].Value = list27[count].SL;
                                    worksheet.Cells[i + 108, 39].Value = list27[count].TL;
                                    worksheet.Cells[i + 108, 40].Value = list27[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list27[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 38].Value = "";
                                    worksheet.Cells[i + 108, 39].Value = "";
                                    worksheet.Cells[i + 108, 40].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list28.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list28[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 41].Value = list28[count].SL;
                                    worksheet.Cells[i + 108, 42].Value = list28[count].TL;
                                    worksheet.Cells[i + 108, 43].Value = list28[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list28[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 41].Value = "";
                                    worksheet.Cells[i + 108, 42].Value = "";
                                    worksheet.Cells[i + 108, 43].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list29.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list29[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 44].Value = list29[count].SL;
                                    worksheet.Cells[i + 108, 45].Value = list29[count].TL;
                                    worksheet.Cells[i + 108, 46].Value = list29[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list29[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 44].Value = "";
                                    worksheet.Cells[i + 108, 45].Value = "";
                                    worksheet.Cells[i + 108, 46].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list30.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list30[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 47].Value = list30[count].SL;
                                    worksheet.Cells[i + 108, 48].Value = list30[count].TL;
                                    worksheet.Cells[i + 108, 49].Value = list30[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list30[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 47].Value = "";
                                    worksheet.Cells[i + 108, 48].Value = "";
                                    worksheet.Cells[i + 108, 49].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list31.Count)
                            {
                                if (worksheet.Cells[i + 108, 1].Value.ToString().Equals(list31[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 50].Value = list31[count].SL;
                                    worksheet.Cells[i + 108, 51].Value = list31[count].TL;
                                    worksheet.Cells[i + 108, 52].Value = list31[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 108, 1].Value.ToString().Equals(list31[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 108, 50].Value = "";
                                    worksheet.Cells[i + 108, 51].Value = "";
                                    worksheet.Cells[i + 108, 52].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    ////////////////////////
                    for (int i = 0; i < list32.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 111, 1].Value = list32[i].PostOfficeAcceptID;
                            worksheet.Cells[i + 111, 5].Value = list32[i].SL;
                            worksheet.Cells[i + 111, 6].Value = list32[i].TL;
                            worksheet.Cells[i + 111, 7].Value = list32[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list33.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list33[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 8].Value = list33[count].SL;
                                    worksheet.Cells[i + 111, 9].Value = list33[count].TL;
                                    worksheet.Cells[i + 111, 10].Value = list33[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list33[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 8].Value = "";
                                    worksheet.Cells[i + 111, 9].Value = "";
                                    worksheet.Cells[i + 111, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list34.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list34[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 11].Value = list34[count].SL;
                                    worksheet.Cells[i + 111, 12].Value = list34[count].TL;
                                    worksheet.Cells[i + 111, 13].Value = list34[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list34[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 11].Value = "";
                                    worksheet.Cells[i + 111, 12].Value = "";
                                    worksheet.Cells[i + 111, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list35.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list35[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 14].Value = list35[count].SL;
                                    worksheet.Cells[i + 111, 15].Value = list35[count].TL;
                                    worksheet.Cells[i + 111, 16].Value = list35[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list35[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 14].Value = "";
                                    worksheet.Cells[i + 111, 15].Value = "";
                                    worksheet.Cells[i + 111, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list36.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list36[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 17].Value = list36[count].SL;
                                    worksheet.Cells[i + 111, 18].Value = list36[count].TL;
                                    worksheet.Cells[i + 111, 19].Value = list36[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list36[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 17].Value = "";
                                    worksheet.Cells[i + 111, 18].Value = "";
                                    worksheet.Cells[i + 111, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list37.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list37[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 20].Value = list37[count].SL;
                                    worksheet.Cells[i + 111, 21].Value = list37[count].TL;
                                    worksheet.Cells[i + 111, 22].Value = list37[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list37[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 20].Value = "";
                                    worksheet.Cells[i + 111, 21].Value = "";
                                    worksheet.Cells[i + 111, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list38.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list38[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 23].Value = list38[count].SL;
                                    worksheet.Cells[i + 111, 24].Value = list38[count].TL;
                                    worksheet.Cells[i + 111, 25].Value = list38[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list38[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 23].Value = "";
                                    worksheet.Cells[i + 111, 24].Value = "";
                                    worksheet.Cells[i + 111, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list39.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list39[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 26].Value = list39[count].SL;
                                    worksheet.Cells[i + 111, 27].Value = list39[count].TL;
                                    worksheet.Cells[i + 111, 28].Value = list39[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list39[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 26].Value = "";
                                    worksheet.Cells[i + 111, 27].Value = "";
                                    worksheet.Cells[i + 111, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list40.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list40[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 29].Value = list40[count].SL;
                                    worksheet.Cells[i + 111, 30].Value = list40[count].TL;
                                    worksheet.Cells[i + 111, 31].Value = list40[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list40[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 29].Value = "";
                                    worksheet.Cells[i + 111, 30].Value = "";
                                    worksheet.Cells[i + 111, 31].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list41.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list41[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 32].Value = list41[count].SL;
                                    worksheet.Cells[i + 111, 33].Value = list41[count].TL;
                                    worksheet.Cells[i + 111, 34].Value = list41[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list41[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 32].Value = "";
                                    worksheet.Cells[i + 111, 33].Value = "";
                                    worksheet.Cells[i + 111, 34].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list42.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list42[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 35].Value = list42[count].SL;
                                    worksheet.Cells[i + 111, 36].Value = list42[count].TL;
                                    worksheet.Cells[i + 111, 37].Value = list42[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list42[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 35].Value = "";
                                    worksheet.Cells[i + 111, 36].Value = "";
                                    worksheet.Cells[i + 111, 37].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list43.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list43[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 38].Value = list43[count].SL;
                                    worksheet.Cells[i + 111, 39].Value = list43[count].TL;
                                    worksheet.Cells[i + 111, 40].Value = list43[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list43[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 38].Value = "";
                                    worksheet.Cells[i + 111, 39].Value = "";
                                    worksheet.Cells[i + 111, 40].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list44.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list44[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 41].Value = list44[count].SL;
                                    worksheet.Cells[i + 111, 42].Value = list44[count].TL;
                                    worksheet.Cells[i + 111, 43].Value = list44[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list44[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 41].Value = "";
                                    worksheet.Cells[i + 111, 42].Value = "";
                                    worksheet.Cells[i + 111, 43].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list45.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list45[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 44].Value = list45[count].SL;
                                    worksheet.Cells[i + 111, 45].Value = list45[count].TL;
                                    worksheet.Cells[i + 111, 46].Value = list45[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list45[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 44].Value = "";
                                    worksheet.Cells[i + 111, 45].Value = "";
                                    worksheet.Cells[i + 111, 46].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list46.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list46[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 47].Value = list46[count].SL;
                                    worksheet.Cells[i + 111, 48].Value = list46[count].TL;
                                    worksheet.Cells[i + 111, 49].Value = list46[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list46[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 47].Value = "";
                                    worksheet.Cells[i + 111, 48].Value = "";
                                    worksheet.Cells[i + 111, 49].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list32.Count; i++)
                    {
                        try
                        {
                            if (count < list47.Count)
                            {
                                if (worksheet.Cells[i + 111, 1].Value.ToString().Equals(list47[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 50].Value = list47[count].SL;
                                    worksheet.Cells[i + 111, 51].Value = list47[count].TL;
                                    worksheet.Cells[i + 111, 52].Value = list47[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 111, 1].Value.ToString().Equals(list47[count].PostOfficeAcceptID.ToString()))
                                {
                                    worksheet.Cells[i + 111, 50].Value = "";
                                    worksheet.Cells[i + 111, 51].Value = "";
                                    worksheet.Cells[i + 111, 52].Value = "";
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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("sanluongnhan" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
        }

        public ActionResult SanLuongPhat()
        {
            return View();
        }

        public ActionResult ExcelSLPhat(string Date)
        {
            string pathRoot = Server.MapPath("~/Report/sanluongphat.xlsx");
            string name = "sanluongphat" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
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
                var paradate4 = new SqlParameter("@Date", date);
                var paradate5 = new SqlParameter("@Date", date);
                var paradate6 = new SqlParameter("@Date", date);
                var paradate7 = new SqlParameter("@Date", date);

                var paradate8 = new SqlParameter("@Date", date);
                var paradate9 = new SqlParameter("@Date", date);
                var paradate10 = new SqlParameter("@Date", date);
                var paradate11 = new SqlParameter("@Date", date);
                var paradate12 = new SqlParameter("@Date", date);
                var paradate13 = new SqlParameter("@Date", date);
                var paradate14 = new SqlParameter("@Date", date);
                var paradate15 = new SqlParameter("@Date", date);

                var paradate16 = new SqlParameter("@Date", date);
                var paradate17 = new SqlParameter("@Date", date);
                var paradate18 = new SqlParameter("@Date", date);
                var paradate19 = new SqlParameter("@Date", date);
                var paradate20 = new SqlParameter("@Date", date);
                var paradate21 = new SqlParameter("@Date", date);
                var paradate22 = new SqlParameter("@Date", date);
                var paradate23 = new SqlParameter("@Date", date);

                var paradate24 = new SqlParameter("@Date", date);
                var paradate25 = new SqlParameter("@Date", date);
                var paradate26 = new SqlParameter("@Date", date);
                var paradate27 = new SqlParameter("@Date", date);
                var paradate28 = new SqlParameter("@Date", date);
                var paradate29 = new SqlParameter("@Date", date);
                var paradate30 = new SqlParameter("@Date", date);
                var paradate31 = new SqlParameter("@Date", date);


                List<SanLuongPhatDuoi2CPN> list = new List<SanLuongPhatDuoi2CPN>();
                List<SanLuongPhatTren2CPN> list1 = new List<SanLuongPhatTren2CPN>();
                List<SanLuongPhatTren8CPN> list2 = new List<SanLuongPhatTren8CPN>();
                List<SanLuongPhatTren50CPN> list3 = new List<SanLuongPhatTren50CPN>();
                List<SanLuongPhatDuoi2CPT> list4 = new List<SanLuongPhatDuoi2CPT>();
                List<SanLuongPhatTren2CPT> list5 = new List<SanLuongPhatTren2CPT>();
                List<SanLuongPhatTren8CPT> list6 = new List<SanLuongPhatTren8CPT>();
                List<SanLuongPhatTren50CPT> list7 = new List<SanLuongPhatTren50CPT>();

                List<SanLuongPhatDuoi2CPNKV3> list8 = new List<SanLuongPhatDuoi2CPNKV3>();
                List<SanLuongPhatTren2CPNKV3> list9 = new List<SanLuongPhatTren2CPNKV3>();
                List<SanLuongPhatTren8CPNKV3> list10 = new List<SanLuongPhatTren8CPNKV3>();
                List<SanLuongPhatTren50CPNKV3> list11 = new List<SanLuongPhatTren50CPNKV3>();
                List<SanLuongPhatDuoi2CPTKV3> list12 = new List<SanLuongPhatDuoi2CPTKV3>();
                List<SanLuongPhatTren2CPTKV3> list13 = new List<SanLuongPhatTren2CPTKV3>();
                List<SanLuongPhatTren8CPTKV3> list14 = new List<SanLuongPhatTren8CPTKV3>();
                List<SanLuongPhatTren50CPTKV3> list15 = new List<SanLuongPhatTren50CPTKV3>();

                List<SanLuongPhatDuoi2CPNKV4> list16 = new List<SanLuongPhatDuoi2CPNKV4>();
                List<SanLuongPhatTren2CPNKV4> list17 = new List<SanLuongPhatTren2CPNKV4>();
                List<SanLuongPhatTren8CPNKV4> list18 = new List<SanLuongPhatTren8CPNKV4>();
                List<SanLuongPhatTren50CPNKV4> list19 = new List<SanLuongPhatTren50CPNKV4>();
                List<SanLuongPhatDuoi2CPTKV4> list20 = new List<SanLuongPhatDuoi2CPTKV4>();
                List<SanLuongPhatTren2CPTKV4> list21 = new List<SanLuongPhatTren2CPTKV4>();
                List<SanLuongPhatTren8CPTKV4> list22 = new List<SanLuongPhatTren8CPTKV4>();
                List<SanLuongPhatTren50CPTKV4> list23 = new List<SanLuongPhatTren50CPTKV4>();

                List<SanLuongPhatDuoi2CPNKV1> list24 = new List<SanLuongPhatDuoi2CPNKV1>();
                List<SanLuongPhatTren2CPNKV1> list25 = new List<SanLuongPhatTren2CPNKV1>();
                List<SanLuongPhatTren8CPNKV1> list26 = new List<SanLuongPhatTren8CPNKV1>();
                List<SanLuongPhatTren50CPNKV1> list27 = new List<SanLuongPhatTren50CPNKV1>();
                List<SanLuongPhatDuoi2CPTKV1> list28 = new List<SanLuongPhatDuoi2CPTKV1>();
                List<SanLuongPhatTren2CPTKV1> list29 = new List<SanLuongPhatTren2CPTKV1>();
                List<SanLuongPhatTren8CPTKV1> list30 = new List<SanLuongPhatTren8CPTKV1>();
                List<SanLuongPhatTren50CPTKV1> list31 = new List<SanLuongPhatTren50CPTKV1>();
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPN>("SGP_SanLuongPhatDuoi2CPNKV2 @Date", paradate).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list.Add(new SanLuongPhatDuoi2CPN()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPN>("SGP_SanLuongPhatTren2CPNKV2 @Date", paradate1).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list1.Add(new SanLuongPhatTren2CPN()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPN>("SGP_SanLuongPhatTren8CPNKV2 @Date", paradate2).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list2.Add(new SanLuongPhatTren8CPN()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPN>("SGP_SanLuongPhatTren50CPNKV2 @Date", paradate3).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list3.Add(new SanLuongPhatTren50CPN()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPT>("SGP_SanLuongPhatDuoi2CPTKV2 @Date", paradate4).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list4.Add(new SanLuongPhatDuoi2CPT()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPT>("SGP_SanLuongPhatTren2CPTKV2 @Date", paradate5).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list5.Add(new SanLuongPhatTren2CPT()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPT>("SGP_SanLuongPhatTren8CPTKV2 @Date", paradate6).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list6.Add(new SanLuongPhatTren8CPT()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPT>("SGP_SanLuongPhatTren50CPTKV2 @Date", paradate7).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list7.Add(new SanLuongPhatTren50CPT()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPNKV3>("SGP_SanLuongPhatDuoi2CPNKV3 @Date", paradate8).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list8.Add(new SanLuongPhatDuoi2CPNKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPNKV3>("SGP_SanLuongPhatTren2CPNKV3 @Date", paradate9).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list9.Add(new SanLuongPhatTren2CPNKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPNKV3>("SGP_SanLuongPhatTren8CPNKV3 @Date", paradate10).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list10.Add(new SanLuongPhatTren8CPNKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPNKV3>("SGP_SanLuongPhatTren50CPNKV3 @Date", paradate11).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list11.Add(new SanLuongPhatTren50CPNKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPTKV3>("SGP_SanLuongPhatDuoi2CPTKV3 @Date", paradate12).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list12.Add(new SanLuongPhatDuoi2CPTKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPTKV3>("SGP_SanLuongPhatTren2CPTKV3 @Date", paradate13).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list13.Add(new SanLuongPhatTren2CPTKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPTKV3>("SGP_SanLuongPhatTren8CPTKV3 @Date", paradate14).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list14.Add(new SanLuongPhatTren8CPTKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPTKV3>("SGP_SanLuongPhatTren50CPTKV3 @Date", paradate15).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list15.Add(new SanLuongPhatTren50CPTKV3()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                //////////////////////////
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPNKV4>("SGP_SanLuongPhatDuoi2CPNKV4 @Date", paradate16).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list16.Add(new SanLuongPhatDuoi2CPNKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPNKV4>("SGP_SanLuongPhatTren2CPNKV4 @Date", paradate17).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list17.Add(new SanLuongPhatTren2CPNKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPNKV4>("SGP_SanLuongPhatTren8CPNKV4 @Date", paradate18).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list18.Add(new SanLuongPhatTren8CPNKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPNKV4>("SGP_SanLuongPhatTren50CPNKV4 @Date", paradate19).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list19.Add(new SanLuongPhatTren50CPNKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPTKV4>("SGP_SanLuongPhatDuoi2CPTKV4 @Date", paradate20).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list20.Add(new SanLuongPhatDuoi2CPTKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPTKV4>("SGP_SanLuongPhatTren2CPTKV4 @Date", paradate21).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list21.Add(new SanLuongPhatTren2CPTKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPTKV4>("SGP_SanLuongPhatTren8CPTKV4 @Date", paradate22).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list22.Add(new SanLuongPhatTren8CPTKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPTKV4>("SGP_SanLuongPhatTren50CPTKV4 @Date", paradate23).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list23.Add(new SanLuongPhatTren50CPTKV4()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPNKV1>("SGP_SanLuongPhatDuoi2CPNKV1 @Date", paradate24).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list24.Add(new SanLuongPhatDuoi2CPNKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPNKV1>("SGP_SanLuongPhatTren2CPNKV1 @Date", paradate25).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list25.Add(new SanLuongPhatTren2CPNKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPNKV1>("SGP_SanLuongPhatTren8CPNKV1 @Date", paradate26).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list26.Add(new SanLuongPhatTren8CPNKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPNKV1>("SGP_SanLuongPhatTren50CPNKV1 @Date", paradate27).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list27.Add(new SanLuongPhatTren50CPNKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatDuoi2CPTKV1>("SGP_SanLuongPhatDuoi2CPTKV1 @Date", paradate28).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list28.Add(new SanLuongPhatDuoi2CPTKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }
                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren2CPTKV1>("SGP_SanLuongPhatTren2CPTKV1 @Date", paradate29).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list29.Add(new SanLuongPhatTren2CPTKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren8CPTKV1>("SGP_SanLuongPhatTren8CPTKV1 @Date", paradate30).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list30.Add(new SanLuongPhatTren8CPTKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
                        });
                    }
                }

                if (Date != "")
                {
                    pms.Database.CommandTimeout = 0;
                    var result = pms.Database.SqlQuery<SanLuongPhatTren50CPTKV1>("SGP_SanLuongPhatTren50CPTKV1 @Date", paradate31).ToList();
                    // var result = db.SGP_WEB_Mailer(fDate, fTo).ToList();
                    foreach (var item in result)
                    {
                        list31.Add(new SanLuongPhatTren50CPTKV1()
                        {
                            PostOfficeID = item.PostOfficeID,
                            SL = item.SL,
                            TL = item.TL,
                            DT = item.DT
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
                            worksheet.Cells[i + 12, 1].Value = list[i].PostOfficeID;
                            worksheet.Cells[i + 12, 5].Value = list[i].SL;
                            worksheet.Cells[i + 12, 6].Value = list[i].TL;
                            worksheet.Cells[i + 12, 7].Value = list[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    int count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list1.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list1[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 8].Value = list1[count].SL;
                                    worksheet.Cells[i + 12, 9].Value = list1[count].TL;
                                    worksheet.Cells[i + 12, 10].Value = list1[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list1[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 8].Value = "";
                                    worksheet.Cells[i + 12, 9].Value = "";
                                    worksheet.Cells[i + 12, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list2.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list2[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 11].Value = list2[count].SL;
                                    worksheet.Cells[i + 12, 12].Value = list2[count].TL;
                                    worksheet.Cells[i + 12, 13].Value = list2[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list2[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 11].Value = "";
                                    worksheet.Cells[i + 12, 12].Value = "";
                                    worksheet.Cells[i + 12, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list3.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list3[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 14].Value = list3[count].SL;
                                    worksheet.Cells[i + 12, 15].Value = list3[count].TL;
                                    worksheet.Cells[i + 12, 16].Value = list3[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list3[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 14].Value = "";
                                    worksheet.Cells[i + 12, 15].Value = "";
                                    worksheet.Cells[i + 12, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list4.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list4[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 17].Value = list4[count].SL;
                                    worksheet.Cells[i + 12, 18].Value = list4[count].TL;
                                    worksheet.Cells[i + 12, 19].Value = list4[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list4[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 17].Value = "";
                                    worksheet.Cells[i + 12, 18].Value = "";
                                    worksheet.Cells[i + 12, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list5.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list5[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 20].Value = list5[count].SL;
                                    worksheet.Cells[i + 12, 21].Value = list5[count].TL;
                                    worksheet.Cells[i + 12, 22].Value = list5[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list5[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 20].Value = "";
                                    worksheet.Cells[i + 12, 21].Value = "";
                                    worksheet.Cells[i + 12, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list6.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list6[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 23].Value = list6[count].SL;
                                    worksheet.Cells[i + 12, 24].Value = list6[count].TL;
                                    worksheet.Cells[i + 12, 25].Value = list6[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list6[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 23].Value = "";
                                    worksheet.Cells[i + 12, 24].Value = "";
                                    worksheet.Cells[i + 12, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        try
                        {
                            if (count < list7.Count)
                            {
                                if (worksheet.Cells[i + 12, 1].Value.ToString().Equals(list7[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 26].Value = list7[count].SL;
                                    worksheet.Cells[i + 12, 27].Value = list7[count].TL;
                                    worksheet.Cells[i + 12, 28].Value = list7[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list7[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 12, 26].Value = "";
                                    worksheet.Cells[i + 12, 27].Value = "";
                                    worksheet.Cells[i + 12, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }
                    }
                    ///////////////
                    for (int i = 0; i < list8.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 35, 1].Value = list8[i].PostOfficeID;
                            worksheet.Cells[i + 35, 5].Value = list8[i].SL;
                            worksheet.Cells[i + 35, 6].Value = list8[i].TL;
                            worksheet.Cells[i + 35, 7].Value = list8[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list9.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list9[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 8].Value = list9[count].SL;
                                    worksheet.Cells[i + 35, 9].Value = list9[count].TL;
                                    worksheet.Cells[i + 35, 10].Value = list9[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list9[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 8].Value = "";
                                    worksheet.Cells[i + 35, 9].Value = "";
                                    worksheet.Cells[i + 35, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list10.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list10[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 11].Value = list10[count].SL;
                                    worksheet.Cells[i + 35, 12].Value = list10[count].TL;
                                    worksheet.Cells[i + 35, 13].Value = list10[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list10[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 11].Value = "";
                                    worksheet.Cells[i + 35, 12].Value = "";
                                    worksheet.Cells[i + 35, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list11.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list11[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 14].Value = list11[count].SL;
                                    worksheet.Cells[i + 35, 15].Value = list11[count].TL;
                                    worksheet.Cells[i + 35, 16].Value = list11[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list11[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 14].Value = "";
                                    worksheet.Cells[i + 35, 15].Value = "";
                                    worksheet.Cells[i + 35, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list12.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list12[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 17].Value = list12[count].SL;
                                    worksheet.Cells[i + 35, 18].Value = list12[count].TL;
                                    worksheet.Cells[i + 35, 19].Value = list12[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list12[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 17].Value = "";
                                    worksheet.Cells[i + 35, 18].Value = "";
                                    worksheet.Cells[i + 35, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list13.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list13[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 20].Value = list13[count].SL;
                                    worksheet.Cells[i + 35, 21].Value = list13[count].TL;
                                    worksheet.Cells[i + 35, 22].Value = list13[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list13[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 20].Value = "";
                                    worksheet.Cells[i + 35, 21].Value = "";
                                    worksheet.Cells[i + 35, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list14.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list14[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 23].Value = list14[count].SL;
                                    worksheet.Cells[i + 35, 24].Value = list14[count].TL;
                                    worksheet.Cells[i + 35, 25].Value = list14[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 35, 1].Value.ToString().Equals(list14[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 23].Value = "";
                                    worksheet.Cells[i + 35, 24].Value = "";
                                    worksheet.Cells[i + 35, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list8.Count; i++)
                    {
                        try
                        {
                            if (count < list15.Count)
                            {
                                if (worksheet.Cells[i + 35, 1].Value.ToString().Equals(list15[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 26].Value = list15[count].SL;
                                    worksheet.Cells[i + 35, 27].Value = list15[count].TL;
                                    worksheet.Cells[i + 35, 28].Value = list15[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 12, 1].Value.ToString().Equals(list15[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 35, 26].Value = "";
                                    worksheet.Cells[i + 35, 27].Value = "";
                                    worksheet.Cells[i + 35, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    ////////////////////////
                    for (int i = 0; i < list16.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 38, 1].Value = list16[i].PostOfficeID;
                            worksheet.Cells[i + 38, 5].Value = list16[i].SL;
                            worksheet.Cells[i + 38, 6].Value = list16[i].TL;
                            worksheet.Cells[i + 38, 7].Value = list16[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list17.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list17[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 8].Value = list17[count].SL;
                                    worksheet.Cells[i + 38, 9].Value = list17[count].TL;
                                    worksheet.Cells[i + 38, 10].Value = list17[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list17[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 8].Value = "";
                                    worksheet.Cells[i + 38, 9].Value = "";
                                    worksheet.Cells[i + 38, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list18.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list18[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 11].Value = list18[count].SL;
                                    worksheet.Cells[i + 38, 12].Value = list18[count].TL;
                                    worksheet.Cells[i + 38, 13].Value = list18[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list18[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 11].Value = "";
                                    worksheet.Cells[i + 38, 12].Value = "";
                                    worksheet.Cells[i + 38, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list19.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list19[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 14].Value = list19[count].SL;
                                    worksheet.Cells[i + 38, 15].Value = list19[count].TL;
                                    worksheet.Cells[i + 38, 16].Value = list19[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list19[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 14].Value = "";
                                    worksheet.Cells[i + 38, 15].Value = "";
                                    worksheet.Cells[i + 38, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list20.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list20[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 17].Value = list20[count].SL;
                                    worksheet.Cells[i + 38, 18].Value = list20[count].TL;
                                    worksheet.Cells[i + 38, 19].Value = list20[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list20[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 17].Value = "";
                                    worksheet.Cells[i + 38, 18].Value = "";
                                    worksheet.Cells[i + 38, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list21.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list21[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 20].Value = list21[count].SL;
                                    worksheet.Cells[i + 38, 21].Value = list21[count].TL;
                                    worksheet.Cells[i + 38, 22].Value = list21[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list21[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 20].Value = "";
                                    worksheet.Cells[i + 38, 21].Value = "";
                                    worksheet.Cells[i + 38, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list22.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list22[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 23].Value = list22[count].SL;
                                    worksheet.Cells[i + 38, 24].Value = list22[count].TL;
                                    worksheet.Cells[i + 38, 25].Value = list22[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list22[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 23].Value = "";
                                    worksheet.Cells[i + 38, 24].Value = "";
                                    worksheet.Cells[i + 38, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list16.Count; i++)
                    {
                        try
                        {
                            if (count < list23.Count)
                            {
                                if (worksheet.Cells[i + 38, 1].Value.ToString().Equals(list23[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 26].Value = list23[count].SL;
                                    worksheet.Cells[i + 38, 27].Value = list23[count].TL;
                                    worksheet.Cells[i + 38, 28].Value = list23[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 38, 1].Value.ToString().Equals(list23[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 38, 26].Value = "";
                                    worksheet.Cells[i + 38, 27].Value = "";
                                    worksheet.Cells[i + 38, 28].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    //////////////
                    for (int i = 0; i < list24.Count; i++)
                    {

                        try
                        {
                            worksheet.Cells[i + 44, 1].Value = list24[i].PostOfficeID;
                            worksheet.Cells[i + 44, 5].Value = list24[i].SL;
                            worksheet.Cells[i + 44, 6].Value = list24[i].TL;
                            worksheet.Cells[i + 44, 7].Value = list24[i].DT;
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list25.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list25[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 8].Value = list25[count].SL;
                                    worksheet.Cells[i + 44, 9].Value = list25[count].TL;
                                    worksheet.Cells[i + 44, 10].Value = list25[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list25[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 8].Value = "";
                                    worksheet.Cells[i + 44, 9].Value = "";
                                    worksheet.Cells[i + 44, 10].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list26.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list26[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 11].Value = list26[count].SL;
                                    worksheet.Cells[i + 44, 12].Value = list26[count].TL;
                                    worksheet.Cells[i + 44, 13].Value = list26[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list26[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 11].Value = "";
                                    worksheet.Cells[i + 44, 12].Value = "";
                                    worksheet.Cells[i + 44, 13].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list27.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list27[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 14].Value = list27[count].SL;
                                    worksheet.Cells[i + 44, 15].Value = list27[count].TL;
                                    worksheet.Cells[i + 44, 16].Value = list27[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list27[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 14].Value = "";
                                    worksheet.Cells[i + 44, 15].Value = "";
                                    worksheet.Cells[i + 44, 16].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list28.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list28[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 17].Value = list28[count].SL;
                                    worksheet.Cells[i + 44, 18].Value = list28[count].TL;
                                    worksheet.Cells[i + 44, 19].Value = list28[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list28[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 17].Value = "";
                                    worksheet.Cells[i + 44, 18].Value = "";
                                    worksheet.Cells[i + 44, 19].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list29.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list29[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 20].Value = list29[count].SL;
                                    worksheet.Cells[i + 44, 21].Value = list29[count].TL;
                                    worksheet.Cells[i + 44, 22].Value = list29[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list29[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 20].Value = "";
                                    worksheet.Cells[i + 44, 21].Value = "";
                                    worksheet.Cells[i + 44, 22].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list30.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list30[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 23].Value = list30[count].SL;
                                    worksheet.Cells[i + 44, 24].Value = list30[count].TL;
                                    worksheet.Cells[i + 44, 25].Value = list30[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list30[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 23].Value = "";
                                    worksheet.Cells[i + 44, 24].Value = "";
                                    worksheet.Cells[i + 44, 25].Value = "";
                                }
                            }
                        }
                        catch
                        {
                            return RedirectToAction("error", "home");
                        }

                    }
                    count = 0;
                    for (int i = 0; i < list24.Count; i++)
                    {
                        try
                        {
                            if (count < list31.Count)
                            {
                                if (worksheet.Cells[i + 44, 1].Value.ToString().Equals(list31[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 26].Value = list31[count].SL;
                                    worksheet.Cells[i + 44, 27].Value = list31[count].TL;
                                    worksheet.Cells[i + 44, 28].Value = list31[count].DT;
                                    count++;
                                }
                                else if (!worksheet.Cells[i + 44, 1].Value.ToString().Equals(list31[count].PostOfficeID.ToString()))
                                {
                                    worksheet.Cells[i + 44, 26].Value = "";
                                    worksheet.Cells[i + 44, 27].Value = "";
                                    worksheet.Cells[i + 44, 28].Value = "";
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


            return File(pathTo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", string.Format("sanluongphat" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".{0}", "xlsx"));
            }

        public List<getPostOffice> getPostOffice(int check)
        {
            if(check == 2 || check == 4)
            {
                var query = (from ds in pms.MM_PostOffices
                             select new getPostOffice()
                             {
                                 PostOfficeID = ds.PostOfficeID,
                                 PostOfficeName = ds.PostOfficeName

                             });
                return query.ToList();
            }else
            {
                return null;
            }
            
        }

        public List<getListSpCustomer> getListSpCustomer(string tungay, string denngay, string customerid, string buucuc, int loai)
        {
            var parafrom = new SqlParameter("@FromDate", tungay);
            var parato = new SqlParameter("@ToDate", denngay);
            var paracustomer = new SqlParameter("@CustomerID", customerid);
            var parabuucuc = new SqlParameter("@PostOfficeID", buucuc);
            var paraloai = new SqlParameter("@Loai", loai);
            List<getListSpCustomer> list = new List<getListSpCustomer>();
            var result = sgp.Database.SqlQuery<getListSpCustomer>("SGP_WEB_SpecialCustomerTab4 @FromDate,@ToDate,@CustomerID,@PostOfficeID,@Loai", parafrom, parato, paracustomer, parabuucuc, paraloai).ToList();
            foreach (var item in result)
            {
                list.Add(new getListSpCustomer()
                {
                    PostOfficeAcceptID = item.PostOfficeAcceptID,
                    ReceiveProvinceID = item.ReceiveProvinceID,
                    RecieverAddress = item.RecieverAddress,
                    SenderName = item.SenderName,
                    Amount = item.Amount,
                    DeliveryDate = item.DeliveryDate,
                    DeliveryTo = item.DeliveryTo,
                    MailerID = item.MailerID,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    SenderID = item.SenderID,
                    ServiceTypeID = item.ServiceTypeID
                });
            }
            return list;
        }

        public List<MM_CustomerGroups> getCustomerGroupPMS()
        {
            List<MM_CustomerGroups> data = pms.MM_CustomerGroups.ToList();
            return data;
        }

        public List<ZoneList> getZone(int check)
        {
            if(check == 1 || check == 3)
            {
                var query = (from ds in pms.MM_Zones
                             select new ZoneList
                             {
                                 ZoneID = ds.ZoneID

                             });
                return query.ToList();
            }
            return null;
        }



        public ActionResult KHDacBiet(string FromDate, string ToDate, int? page = 1)
        {
            int pageSize = 20;
            int pageNumber = (page ?? 1);
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
            return View();
        }
    }
}