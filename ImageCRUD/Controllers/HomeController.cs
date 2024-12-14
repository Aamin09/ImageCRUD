using ImageCRUD.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ImageCRUD.Controllers
{
    public class HomeController : Controller
    {
        IceCreamProductsEntities db=new IceCreamProductsEntities();
        // GET: Home
        public ActionResult Index()
        {
            var data = db.Products.ToList();
            return View(data);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Product p)
        {
            if(ModelState.IsValid==true)
            {
                string filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                string extension=Path.GetExtension(p.ImageFile.FileName);
                HttpPostedFileBase posted = p.ImageFile;
                int length = posted.ContentLength;

                if(extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                {
                    if (length <= 1000000)
                    {
                        filename = filename + extension;
                        p.image = "~/images/" + filename;
                        filename = Path.Combine(Server.MapPath("~/images/"), filename);
                        p.ImageFile.SaveAs(filename);
                        db.Products.Add(p);
                        int a=db.SaveChanges();
                        if(a>0)
                        {
                            TempData["insert"] = "<script>alert('Record Inserted!')</script>";
                            ModelState.Clear();
                            return RedirectToAction("Index","Home");
                        }
                        else
                        {
                            ViewBag.insert = "<script>alert('Record Not Inserted!')</script>";
                        }
                    }
                    else
                    {
                        ViewBag.Imagesize = "<script>alert('Size must be or equal to 1 MB!')</script>";
                    }
                }
                else
                {
                    ViewBag.Imageextension = "<script>alert('Image type is not supported!')</script>";
                }
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            var data=db.Products.Where(model=> model.Id == id).FirstOrDefault();
            Session["image"] = data.image;
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Product p)
        {
            if(ModelState.IsValid == true)
            {
                if(p.ImageFile != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(p.ImageFile.FileName);
                    string extension = Path.GetExtension(p.ImageFile.FileName);
                    HttpPostedFileBase posted = p.ImageFile;
                    int length = posted.ContentLength;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (length <= 1000000)
                        {
                            filename = filename + extension;
                            p.image = "~/images/" + filename;
                            filename = Path.Combine(Server.MapPath("~/images/"), filename);
                            p.ImageFile.SaveAs(filename);
                            db.Entry(p).State = EntityState.Modified;
                            int a = db.SaveChanges();
                            if (a > 0)
                            {
                                string ImagePath = Request.MapPath(Session["image"].ToString());
                                if (System.IO.File.Exists(ImagePath))
                                {
                                    System.IO.File.Delete(ImagePath);
                                }
                                TempData["update"] = "<script>alert('Record Updated!')</script>";
                                ModelState.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.update = "<script>alert('Record Not Updated!')</script>";
                            }
                        }
                        else
                        {
                            ViewBag.Imagesize = "<script>alert('Size must be or equal to 1 MB!')</script>";
                        }
                    }
                    else
                    {
                        ViewBag.Imageextension = "<script>alert('Image type is not supported!')</script>";
                    }
                }
                else
                {
                    p.image = Session["image"].ToString();
                    db.Entry(p).State = EntityState.Modified;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["update"] = "<script>alert('Record Updated!')</script>";
                        ModelState.Clear();
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.update = "<script>alert('Record Not Updated!')</script>";
                    }
                }
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            if (id > 0)
            {
                var data = db.Products.Where(model => model.Id == id).FirstOrDefault();
                if(data != null)
                {
                    db.Entry(data).State = EntityState.Deleted;
                    int a = db.SaveChanges();
                    if (a > 0)
                    {
                        TempData["delete"] = "<script>alert('Record Deleted!')</script>";
                        string ImagePath = Request.MapPath(data.image.ToString());
                        if (System.IO.File.Exists(ImagePath))
                        {
                            System.IO.File.Delete(ImagePath);
                        }
                    }
                    else
                    {
                        TempData["delete"] = "<script>alert('Record Not Deleted!')</script>";
                    }
                }
            }
            return RedirectToAction("Index","Home");
        }

        public ActionResult Details(int id)
        {
            var data = db.Products.Where(model => model.Id == id).FirstOrDefault();
            Session["image2"]=data.image.ToString();
            return View(data);
        }
    }
}