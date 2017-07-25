using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FY_Project.Models;
using System.Data.Entity;

namespace FY_Project.Controllers
{
    public class StudentController : Controller
    {
        public static List<int> rand_question = new List<int>();
        public static List<int> marks = new List<int>();
        Random random = new Random();
        FYPDBEntities obj = new FYPDBEntities();
        //
        // GET: /Student/

        public ActionResult Index(int? id)
        {
            ViewBag.message = id;
            return View();
        }
        [HttpPost]
        public ActionResult Index()
        {
            var exams = obj.Exams.First(a => a.Exam_Id == 1);
            if(exams.Status.Equals("Active"))
            {
                return Json("active");
            }
            else
            {
                return Json("Inactive");
            }
        }
        public ActionResult Instructions()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Start_Exam(int id=1)
        {
            marks.Clear();
            rand_question.Clear();
            var total_questions = obj.Papers.Where(a => a.Exam_Id == id);
            var count_total_questions = total_questions.Count();
            int number;
            do
            {
                number = random.Next(1, 3);
            } while (rand_question.Contains(number));
            rand_question.Add(number);
            var question = obj.Papers.OrderBy(a => a.Question_Id).Where(b => b.Exam_Id == id).Skip(number - 1).Take(1).ToList();
            foreach (var s in question)
            {
                var pick_question = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == s.Question_Id).ToList();
                ViewBag.question1 = pick_question;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Start_Exam(string selected_answer, string correct_answer)
        {
            var total_questions = obj.Papers.Where(a => a.Exam_Id == 1);
            var count_total_questions = total_questions.Count();
            int count_questions = rand_question.Count();
            if (count_questions < count_total_questions)
            {
                int number;
                do
                {
                    number = random.Next(1, 3);
                } while (rand_question.Contains(number));
                rand_question.Add(number);
                var question = obj.Papers.OrderBy(a => a.Question_Id).Where(b => b.Exam_Id == 1).Skip(number - 1).Take(1).ToList();
                List<Question> pick_question = new List<Question>();
                foreach (var s in question)
                {
                    pick_question = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == s.Question_Id).ToList();
                }
                return Json(pick_question);
            }
            else
            {
                int a = 0;
                foreach (var i in marks)
                {
                    a = a + i;
                }
                return RedirectToAction("Index", "Student", new { @id = a });
            }
        }
        public ActionResult Logout()
        {
            if(Session["User_Id"] != null)
            {
                Session["User_Id"] = null;
                RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}
