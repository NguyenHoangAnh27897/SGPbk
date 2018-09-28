using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGP.Models;
using System.Web.Script.Serialization;
using System.Net;
using System.Data.SqlClient;
namespace SGP.Controllers
{
    public class HomeController : Controller
    {
        SGPAPIEntities db = new SGPAPIEntities();

        [Authorize]
        public ActionResult Index()
        {
           
            //List<ResponseIndex> list = new List<ResponseIndex>();
            //var result = db.Database.SqlQuery<ResponseIndex>("SGP_WEB_Index1").FirstOrDefault();
            //ViewBag.DoanhThu = result.DoanhThu;
            //ViewBag.SoPhieu = result.PhieuGui;
            //ViewBag.SoLuong = result.SoLuong;
            //ViewBag.TrongLuong = result.TrongLuong;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetAmountByMonth()
        {
            List<ResponseAmountByMonth> list = new List<ResponseAmountByMonth>();
                var result = db.SGP_WEB_AmountByMonth().ToList();
                foreach (var item in result)
                {
                    list.Add(new ResponseAmountByMonth()
                    {
                        Thang = item.Thang,
                        DoanhThu = item.DoanhThu
                    });
                }
            return Json(list.Select(p => new { Thang = p.Thang, DoanhThu = p.DoanhThu}), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetServiceAmount()
        {
            List<ResponseServiceMonth> list = new List<ResponseServiceMonth>();
            //var result = db.SGP_WEB_AmountByMonth().ToList();
            var result = db.Database.SqlQuery<ResponseServiceMonth>("SGP_WEB_ServiceAmount").ToList();
            foreach (var item in result)
            {
                list.Add(new ResponseServiceMonth()
                {
                    DV = item.DV,
                    SL = item.SL,
                    TL = item.TL,
                    DoanhThu = item.DoanhThu,
                    PhanTram = item.PhanTram
                });
            }
            return Json(list.Select(p => new { DV = p.DV,SL = p.SL,TL= p.TL, DoanhThu = p.DoanhThu,PhanTram= p.PhanTram }), JsonRequestBehavior.AllowGet);
        }
       
    }
}