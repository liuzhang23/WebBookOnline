using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using WebBookOnline.Models;

namespace WebBookOnline.Controllers
{
    public class HomeController : Controller
    {
        private BookReadingDataBaseEntities db = new BookReadingDataBaseEntities();
        // GET: Home
        public ActionResult Index(int ? page)
        {
            ViewBag.listCategories = db.Categories.ToList();
            ViewBag.listBooks = db.Books.ToList();
            ViewBag.listChapters = db.Chapters.ToList();

            var listChapterOfBook = db.Chapters
                    .GroupBy(c => c.book_id)
                    .Select(g => g.OrderByDescending(c => c.created_at).FirstOrDefault())
                    .ToList();
            var ListLastChapters = listChapterOfBook.OrderByDescending(c => c.created_at).ToList();
            ViewBag.ListLastChapters = ListLastChapters;

            // Start Phan trang
            int pageSize = 16;
            if (page == null) { page = 1; }
            var books = db.Books.OrderByDescending(c => c.time_update).ToList();
            // End Phan trang
            return View(books.ToPagedList((int)page, (int)pageSize));
        }


        static string RemoveDiacritics(string input)
        {
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public ActionResult Search(string searchString)
        {
            var lb = db.Books.ToList();
            List<Book> listBook = new List<Book>();
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = RemoveDiacritics(searchString.ToLower());
                foreach(var item in lb)
                {
                    if (RemoveDiacritics(item.title.ToLower()).Contains(searchString))
                    {
                        listBook.Add(item);
                    }
                }
            }
            List<Chapter> listLastChapters = new List<Chapter>();
            foreach (var item in db.Books.ToList())
            {
                var listChapterOfBook = db.Chapters
                    .Where(c => c.book_id == item.book_id)
                    .OrderBy(c => c.chapter_number)
                    .ToList();

                var lastChapter = listChapterOfBook.LastOrDefault();
                listLastChapters.Add(lastChapter);
            }
            ViewBag.listBooks = listBook;

            ViewBag.ListLastChapters = listLastChapters;
            ViewBag.listCategories = db.Categories.ToList();
            return View();
        }

        public ActionResult Details(int? id)
        {
            ViewBag.listCategories = db.Categories.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            
            ViewBag.listChapters = db.Chapters.Where(c => c.book_id == book.book_id).OrderByDescending(c => c.chapter_number).ToList();

            // First Chapter, End Chapter
            ViewBag.FirstChapter = db.Chapters.Where(c => c.book_id == book.book_id).OrderBy(c => c.chapter_number).First();
            ViewBag.LastChapter = db.Chapters.Where(c => c.book_id == book.book_id).OrderByDescending(c => c.chapter_number).First();


            return View(book);
        }

        public ActionResult Reading(int? id)
        {
            ViewBag.listCategories = db.Categories.ToList();
             if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chapter chapter = db.Chapters.Find(id);
            Book book = db.Books.Find(chapter.book_id); ViewBag.book = book;

            var listChapters = db.Chapters.Where(c => c.book_id == book.book_id).OrderBy(c => c.chapter_number).ToList();

            var previousChapter = listChapters.FirstOrDefault(c => c.chapter_number == chapter.chapter_number - 1);
            if (listChapters.FirstOrDefault() == chapter)
            {
                ViewBag.previousChapter = chapter;
            }
            else
            {
                ViewBag.previousChapter = previousChapter;
            }
            var nextChapter = listChapters.FirstOrDefault(c => c.chapter_number == chapter.chapter_number + 1);
            if (listChapters.LastOrDefault() == chapter)
            {
                ViewBag.nextChapter = chapter;
            }
            else
            {
                ViewBag.nextChapter = nextChapter;
            }
            

            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }
    }
}