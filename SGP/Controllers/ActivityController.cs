using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using PagedList;
using System.Threading;
using System.IO;
using SGP.Models;

namespace SGP.Controllers
{
    public class ActivityController : Controller
    {
        SGPAPIEntities db = new SGPAPIEntities();
        //
        // GET: /Activity/
        public ActionResult Index(string searchString, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);


            var user = User.Identity.Name;
            if (String.IsNullOrEmpty(searchString))
            {
                searchString = "";
            }

            ViewBag.SearchString = searchString;
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var post = prinicpal.Claims.Where(c => c.Type == ClaimTypes.PostalCode).Select(c => c.Value).SingleOrDefault();
            ViewBag.AllZones = db.ZoneInfoes.ToList();
            if (!String.IsNullOrEmpty(post))
            {
                var zone = db.WK_PostOffice.Find(post.ToUpper());

                if (zone != null)
                {
                    ViewBag.Zone = zone.ZoneId;
                    var data = db.ZoneInfoes.Where(p => p.ZoneId == zone.ZoneId).Select(p => p.ActivityInfoes).FirstOrDefault();

                    if (data != null)
                    {
                        return View(data.Where(p => p.IsClock == 0).OrderByDescending(p => p.CreateTime).ToPagedList(pageNumber, pageSize));
                    }
                }
            }



            return View(new List<SGP.Models.ActivityInfo>().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DownloadDocument(string Id)
        {
            var document = db.FileAttaches.Find(Id);

            if (document != null)
            {
                string fullPath = Path.Combine(Server.MapPath("~" + document.DocumentPath));
                return File(fullPath, document.DoucumentType);
            }

            return RedirectToAction("error", "home");
        }


        [HttpPost]
        public ActionResult UpdateFeed(List<HttpPostedFileBase> files, string content, List<string> zone)
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var post = prinicpal.Claims.Where(c => c.Type == ClaimTypes.PostalCode).Select(c => c.Value).SingleOrDefault();
            var postCheck = db.WK_PostOffice.Find(post.ToUpper());
            if (postCheck != null)
            {


                var activity = new ActivityInfo()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = content,
                    CreateTime = DateTime.Now,
                    UserPost = User.Identity.Name,
                    PostOfficeId = postCheck.Id,
                    IsClock = 0
                };

                var zoneCheck = db.ZoneInfoes.Find(postCheck.ZoneId);

                if (zoneCheck != null)
                {
                    activity.ZoneInfoes.Add(zoneCheck);


                    if (zone != null && zone.Count() > 0)
                    {
                        foreach (var item in zone)
                        {
                            var check = db.ZoneInfoes.Find(item);
                            if (check != null)
                            {
                                activity.ZoneInfoes.Add(check);
                            }
                        }


                    }
                    db.ActivityInfoes.Add(activity);

                    db.SaveChanges();

                    if (files != null && files.Count() > 0)
                    {
                        string fsave = "/SGPRes/";
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~" + fsave));
                        if (!exists)
                            System.IO.Directory.CreateDirectory(Server.MapPath(fsave));


                        foreach (var item in files)
                        {

                            if (item != null)
                            {
                                var fileName = fsave + item.FileName + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                                item.SaveAs(Server.MapPath("~" + fileName));
                                string extension = Path.GetExtension(item.FileName);

                                if (extension != ".exe" && extension != ".js")
                                {
                                    var doucment = new FileAttach()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ActivityId = activity.Id,
                                        DocumentName = item.FileName,
                                        DocumentPath = fileName,
                                        DoucumentType = item.ContentType
                                    };

                                    db.FileAttaches.Add(doucment);
                                }

                            }
                        }
                        db.SaveChanges();
                    }

                }

            }

            return RedirectToAction("index", "activity");
        }



        public ActionResult Comment(int? page, string activityId)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var user = User.Identity.Name;

            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var post = prinicpal.Claims.Where(c => c.Type == ClaimTypes.PostalCode).Select(c => c.Value).SingleOrDefault();

            var checkActivity = db.ActivityInfoes.Find(activityId);

            if (checkActivity == null)
            {
                return RedirectToAction("error", "home");
            }

            ViewBag.Activity = checkActivity;
            ViewBag.Zone = checkActivity.ZoneInfoes;
            ViewBag.FileAttach = checkActivity.FileAttaches;
            var comment = checkActivity.Comments.OrderByDescending(p => p.CreateTime).ToPagedList(pageNumber, pageSize);

            return View(comment);
        }


        [HttpPost]
        public ActionResult UpdateComment(List<HttpPostedFileBase> files, string content, string activityId)
        {

            var checkActivity = db.ActivityInfoes.Find(activityId);

            if (checkActivity == null)
            {
                return RedirectToAction("error", "home");
            }

            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var post = prinicpal.Claims.Where(c => c.Type == ClaimTypes.PostalCode).Select(c => c.Value).SingleOrDefault();
            var postCheck = db.WK_PostOffice.Find(post.ToUpper());
            if (postCheck != null)
            {

                var comment = new Comment()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = content,
                    CreateTime = DateTime.Now,
                    UserPost = User.Identity.Name,
                    PostOfficeId = postCheck.Id,
                    ActivityId = activityId
                };

                db.Comments.Add(comment);
                db.SaveChanges();

                if (files != null && files.Count() > 0)
                {
                    string fsave = "/SGPRes/";
                    bool exists = System.IO.Directory.Exists(Server.MapPath("~" + fsave));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(fsave));


                    foreach (var item in files)
                    {

                        if (item != null)
                        {
                            var fileName = fsave + item.FileName + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";
                            item.SaveAs(Server.MapPath("~" + fileName));
                            string extension = Path.GetExtension(item.FileName);

                            if (extension != ".exe" && extension != ".js")
                            {
                                var doucment = new FileAttachComment()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    CommentId = comment.Id,
                                    DocumentName = item.FileName,
                                    DocumentPath = fileName,
                                    DoucumentType = item.ContentType
                                };

                                db.FileAttachComments.Add(doucment);
                            }

                        }
                    }
                    db.SaveChanges();
                }

            }


            return RedirectToAction("comment", "activity", new { activityId = activityId });
        }

        //wiki
        [Authorize]
        public ActionResult IndexWiki(string id = null)
        {
            var menues = GetMenu("-1");


            if (id != null)
            {
                var data = db.SGPWikis.Find(id);
                ViewBag.CurrentData = data;
            }
            else
            {
                ViewBag.CurrentData = new SGPWiki()
                {
                    Id = null,
                    Title = "TRANG THÔNG TIN NỘI BỘ SGP",
                    Content = "Trang tổng hợp các thông tin nghiệp vụ, quy trình, giới thiệu của công ty BƯU CHÍNH SÀI GÒN - SGP",
                    LastTime = DateTime.Now,
                    LastUser = User.Identity.Name
                };
            }

            return View(menues);
        }

        //[Authorize(Roles = "Administrator")]
        public ActionResult Manage(string id = null)
        {

            var menues = GetMenu("-1");


            if (id != null)
            {
                var data = db.SGPWikis.Find(id);
                ViewBag.CurrentData = data;
            }
            else
            {
                ViewBag.CurrentData = new SGPWiki()
                {
                    Id = null,
                    Title = "TRANG THÔNG TIN NỘI BỘ SGP",
                    Content = "Trang tổng hợp các thông tin nghiệp vụ, quy trình, giới thiệu của công ty BƯU CHÍNH SÀI GÒN - SGP",
                    LastTime = DateTime.Now,
                    LastUser = User.Identity.Name
                };
            }

            return View(menues);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult AddNode(string title, string parentId)
        {

            if (title != null)
            {
                if (parentId == "-1")
                {
                    var data = new SGPWiki()
                    {
                        Id = Guid.NewGuid().ToString(),
                        LastTime = DateTime.Now,
                        LastUser = User.Identity.Name,
                        Title = title,
                        ParentId = "-1",
                        SortNumber = 1,
                        Content = ""
                    };

                    db.SGPWikis.Add(data);
                    db.SaveChanges();

                    return RedirectToAction("manage", "activity", new { id = data.Id });

                }
                else
                {
                    var check = db.SGPWikis.Find(parentId);
                    if (check != null && check.ParentId == "-1")
                    {
                        var data = new SGPWiki()
                        {
                            Id = Guid.NewGuid().ToString(),
                            LastTime = DateTime.Now,
                            LastUser = User.Identity.Name,
                            Title = title,
                            ParentId = check.Id,
                            SortNumber = 1,
                            Content = ""
                        };

                        db.SGPWikis.Add(data);
                        db.SaveChanges();

                        return RedirectToAction("manage", "activity", new { id = data.Id });
                    }
                }
            }


            return RedirectToAction("manage", "activity", new { id = parentId });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateContent(string title, string content, string id)
        {

            var check = db.SGPWikis.Find(id);

            if (check != null)
            {
                check.Title = title;
                check.Content = content;
                check.LastUser = User.Identity.Name;
                check.LastTime = DateTime.Now;

                db.Entry(check).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("manage", "activity", new { id = check.Id });

            }
            else
            {
                return RedirectToAction("manage", "activity");

            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var check = db.SGPWikis.Find(id);

            if (check != null)
            {
                var findChilds = db.SGPWikis.Where(p => p.ParentId == check.Id).ToList();

                if (findChilds != null && findChilds.Count() > 0)
                {
                    return RedirectToAction("manage", "activity", new { id = id });

                }
                else
                {
                    db.SGPWikis.Remove(check);
                    db.SaveChanges();
                    if (check.ParentId == "-1")
                    {
                        return RedirectToAction("manage", "activity");
                    }
                    else
                    {

                        var checkChild = db.SGPWikis.Where(p => p.ParentId == check.ParentId).ToList();

                        if (checkChild == null || checkChild.Count() == 0)
                        {
                            var parent = db.SGPWikis.Find(check.ParentId);
                            if (parent != null)
                            {
                                parent.ParentId = "-1";
                                db.Entry(parent).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }

                        return RedirectToAction("manage", "activity", new { id = check.ParentId });
                    }
                }


            }
            else
                return RedirectToAction("manage", "activity");
        }



        private List<WikiInfo> GetMenu(string parentId)
        {

            List<WikiInfo> menues = new List<WikiInfo>();

            var dataParent = db.SGPWikis.Where(p => p.ParentId == parentId).OrderBy(p => p.SortNumber).ToList();

            if (dataParent != null && dataParent.Count() > 0)
            {
                foreach (var item in dataParent)
                {
                    var info = new WikiInfo()
                    {
                        Id = item.Id,
                        Title = item.Title
                    };

                    info.Childes = GetMenu(item.Id);
                    menues.Add(info);
                }
            }


            return menues;

        }
	}
}