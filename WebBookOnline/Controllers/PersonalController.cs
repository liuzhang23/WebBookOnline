using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBookOnline.Models;

namespace WebBookOnline.Controllers
{
    public class PersonalController : Controller
    {
        private BookReadingDataBaseEntities db = new BookReadingDataBaseEntities();
        // GET: Personal
        public ActionResult Index()
        {
            ViewBag.listCategories = db.Categories.ToList();
            ViewBag.books = db.Books.ToList();
            return View();
        }


        public ActionResult CreateBook()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateBook([Bind(Include = "book_id,title,author,description,cover_image,chapter_count,view_count,favorite_count,comment_count,created_at,time_update, user_id")] Book book)
        {
            if (ModelState.IsValid)
            {
                book.chapter_count = 0;
                book.view_count = 0;
                book.favorite_count = 0;
                book.comment_count = 0;
                book.created_at = DateTime.Now;
                book.time_update = DateTime.Now;
                book.user_id = 6;

                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Chapters/Create
        public ActionResult CreateChapter()
        {
            ViewBag.book_id = new SelectList(db.Books, "book_id", "title");
            return View();
        }

        // POST: Chapters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateChapter([Bind(Include = "chapter_id,book_id,chapter_number,title,content,created_at,view")] Chapter chapter)
        {
            if (ModelState.IsValid)
            {
                if (chapter.created_at == null)
                {
                    chapter.created_at = DateTime.Now; // Gán giá trị mặc định là thời gian hiện tại
                }

                if (chapter.view == null)
                {
                    chapter.view = 0; // Gán giá trị mặc định là thời gian hiện tại
                }
                db.Chapters.Add(chapter);
                db.SaveChanges();

                // Cập nhật trường "TimeUpdate" trong bảng "Books"
                var book = db.Books.FirstOrDefault(b => b.book_id == chapter.book_id);
                if (book != null)
                {
                    book.time_update = DateTime.Now;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            

            ViewBag.book_id = new SelectList(db.Books, "book_id", "title", chapter.book_id);
            return View(chapter);
        }
    }
}