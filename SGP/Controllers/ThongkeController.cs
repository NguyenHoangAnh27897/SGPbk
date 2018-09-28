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
    public class ThongkeController : Controller
    {
        //
        // GET: /Thongke/
        //Test1Entities db = new Test1Entities();
        SGPAPIEntities db = new SGPAPIEntities();
        public ActionResult ThongKe(string FromDate, string ToDate, int? page, string SenderID = "")
        {
            string fDate;
            string fTo;
            //string fSender;
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
            ViewBag.SenderID = SenderID;
            var parafrom = new SqlParameter("@FromDate", fDate);
            var parato = new SqlParameter("@ToDate", fTo);
            var parasender = new SqlParameter("@SenderID", SenderID);
            List<PostOffice> list = new List<PostOffice>();
            if (FromDate != "" && ToDate != "")
            {

                var result = db.Database.SqlQuery<PostOffice>("SGP_WEB_ThongKe @FromDate,@ToDate,@SenderID", parafrom, parato, parasender).ToList();
                foreach (var item in result)
                {
                    list.Add(new PostOffice()
                    {
                        ZoneID = item.ZoneID,
                        PostOfficeName = item.PostOfficeName,
                        TongSoPhat= item.TongSoPhat,
                        DaPhat = item.DaPhat,
                        TiLe = item.TiLe,
                    });
                }
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            return View(list.ToPagedList(pageNumber, pageSize));
        }

	
    }
}