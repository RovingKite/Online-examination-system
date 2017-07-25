using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FY_Project.Models;
using System.Data.Entity;

namespace FY_Project.Controllers
{
    public class TeacherController : Controller
    {
        List<int> randomNumber = new List<int>();
        List<Question> question_mcq = new List<Question>();
        List<Question> question_tf = new List<Question>();
        Random rand = new Random();
        //
        // GET: /Teacher/
        FYPDBEntities obj = new FYPDBEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Subjects()
        {
            try
            {
                var user = (string)Session["User_Id"];
                User u = obj.Users.First(a => a.User_Id == user);
                List<Subject> subject = u.Subjects.ToList();
                return View(subject);
            }
            catch
            {
                return RedirectToAction("Add_Question", "Teacher");
            }
        }
        public ActionResult Exam_Status()
        {
            var user = (string)Session["User_Id"];
            User u = obj.Users.First(a => a.User_Id == user);
            List<Subject> subjects = u.Subjects.ToList();
            ViewBag.total_subjects = subjects;
            return View();
        }
        [HttpPost]
        public JsonResult Exam_Status(int?id)
        {
            Exam ex = new Exam();
            List<Exam> sub = new List<Exam>();
            var user = (string)Session["User_Id"];
            User u = obj.Users.First(a => a.User_Id == user);
            List<Subject> subjects = u.Subjects.ToList();
            foreach (var s in subjects)
            {
                ex = obj.Exams.First(a => a.Subject_Id == s.Subject_Id);
                sub.Add(ex);
            }
            var subject = sub.Select(a => new { a.Subject_Id, a.Status });
            return Json(subject);
        }
        public ActionResult Create_Criteria()
        {
            List<Subject> sub = Create_Criteria_Method();
            var easymcq = obj.Questions.Where(a => a.Type.Equals("MCQ-E"));
            var mediummcq = obj.Questions.Where(a => a.Type.Equals("MCQ-M"));
            var difficultmcq = obj.Questions.Where(a => a.Type.Equals("MCQ-D"));
            var easytf = obj.Questions.Where(a => a.Type.Equals("True/False-E"));
            var mediumtf = obj.Questions.Where(a => a.Type.Equals("True/False-M"));
            var difficulttf = obj.Questions.Where(a => a.Type.Equals("True/False-D"));
            var totalmcq = obj.Questions.Where(a => a.Type.Equals("MCQ-E") || a.Type.Equals("MCQ-M") || a.Type.Equals("MCQ-D"));
            var totaltf = obj.Questions.Where(a => a.Type.Equals("True/False-E") || a.Type.Equals("True/False-M") || a.Type.Equals("True/False-D"));
            ViewBag.easymcq = easymcq.Count();
            ViewBag.mediummcq = mediummcq.Count();
            ViewBag.difficultmcq = difficultmcq.Count();
            ViewBag.easytf = easytf.Count();
            ViewBag.mediumtf = mediumtf.Count();
            ViewBag.difficulttf = difficulttf.Count();
            ViewBag.totalmcq = totalmcq.Count();
            ViewBag.totaltf = totaltf.Count();
            return View(sub);
        }

        private List<Subject> Create_Criteria_Method()
        {
            var user = (string)Session["User_Id"];
            User u = obj.Users.First(a => a.User_Id.Equals(user));
            List<Subject> sub = u.Subjects.ToList();
            return sub;
        }
        public ActionResult Create_Exam(FormCollection fc)
        {
            string subject_name = fc["subject"];
            int total = int.Parse(fc["totalmarks"]);
            float mcq = int.Parse(fc["mcqs"]);
            float truefalse = int.Parse(fc["truefalse"]);
            int mcqmark = int.Parse(fc["mcqmark"]);
            float truefalsemark = int.Parse(fc["truefalsemark"]);
            float TotalMcq = (mcq/100)*total;
            float Totaltruefalse = (truefalse / 100) * total;
            float mcqQuestion1 = (TotalMcq / mcqmark);
            float truefalseQuestion1 = (Totaltruefalse / truefalsemark);
            ViewBag.eachmcqmark = mcqmark;
            ViewBag.eachtfmark = truefalsemark;

            int mcqQuestions = (int)Math.Round(mcqQuestion1,MidpointRounding.AwayFromZero);
            int truefalseQuestions = total - ((mcqQuestions)*mcqmark);
            
            var user1 = (string)Session["User_Id"];
            User user = obj.Users.First(a => a.User_Id == user1);
            Subject subject = obj.Subjects.First(a => a.Subject_Name == subject_name && a.User_Id == user.User_Id);
            
            var queryforallmcq = subject.Questions.Where(a => a.Type == "MCQ-E" || a.Type == "MCQ-M" || a.Type == "MCQ-D");
            var queryforeasymcq = subject.Questions.Where(a=>a.Type == "MCQ-E");
            var queryformediummcq = subject.Questions.Where(a=>a.Type == "MCQ-M");
            var queryfordifficultmcq = subject.Questions.Where(a=>a.Type == "MCQ-D");

            var queryforalltf = subject.Questions.Where(a => a.Type == "True/False-E" || a.Type == "True/False-M" || a.Type == "True/False-D");
            var queryforeasytf = subject.Questions.Where(a=>a.Type == "True/False-E");
            var queryformediumtf = subject.Questions.Where(a=>a.Type == "True/False-M");
            var queryfordifficulttf = subject.Questions.Where(a=>a.Type == "True/False-D");
            
            int easymcq_count = queryforeasymcq.Count();
            int mediummcq_count = queryformediummcq.Count();
            int difficultmcq_count = queryfordifficultmcq.Count();
            int easytf_count = queryforeasytf.Count();
            int mediumtf_count = queryformediumtf.Count();
            int difficulttf_count = queryfordifficulttf.Count();
            int allmcq_count = queryforallmcq.Count();
            int alltf_count = queryforalltf.Count();
            
            if(fc["mcqe"] !=null && fc["tfe"] == null)
            {
                float mcqeasy = int.Parse(fc["mcqe"]);
                float mcqmedium = int.Parse(fc["mcqm"]);
                float mcqdifficult = int.Parse(fc["mcqd"]);

                var easymcq_Questions = (mcqeasy / 100) * mcqQuestions;
                var mediummcq_Questions = (mcqmedium / 100) * mcqQuestions;
                var difficultmcq_Questions = (mcqdifficult / 100) * mcqQuestions;

                if (((easymcq_Questions + 1) <= easymcq_count) && ((mediummcq_Questions + 1) <= mediummcq_count) && ((difficultmcq_Questions + 1) <= difficultmcq_count) && (truefalseQuestions <= alltf_count))
                {
                    float a = easymcq_Questions % 1;
                    float b = mediummcq_Questions % 1;
                    float c = difficultmcq_Questions % 1;
                    float d = a + b + c;
                    if (d == 0.0)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(mediummcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficultmcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediummcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediummcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficultmcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                    else if (d == 1)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                    else if (d == 2)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                            randomNumber.Clear();
                            for (int i = 0; i < truefalseQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, alltf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E" || z.Type == "True/False-M" || z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                }
                else
                {
                    if ((easymcq_Questions+1 > easymcq_count) && (mediummcq_Questions+1 > mediummcq_count) && (difficultmcq_Questions+1 > difficultmcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium , Difficult MCQ's and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (mediummcq_Questions+1 > mediummcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium MCQ and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (difficultmcq_Questions+1 > difficultmcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Difficult MCQ and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediummcq_Questions+1 > mediummcq_count) && (difficultmcq_Questions+1 > difficultmcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium , Difficult MCQ and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (mediummcq_Questions+1 > mediummcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy and Medium MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (difficultmcq_Questions+1 > difficultmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy and Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy MCQ's and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediummcq_Questions+1 > mediummcq_count) && (difficultmcq_Questions+1 > difficultmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium and Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediummcq_Questions+1 > mediummcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium MCQ's and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((difficultmcq_Questions+1 > difficultmcq_count) && (truefalseQuestions+1 > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult MCQ's and True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (easymcq_Questions+1 > easymcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mediummcq_Questions+1 > mediummcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (difficultmcq_Questions+1 > difficultmcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (truefalseQuestions+1 > alltf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                }
            }
            else if (fc["mcqe"] == null && fc["tfe"] != null)
            {
                float tfeasy = int.Parse(fc["tfe"]);
                float tfmedium = int.Parse(fc["tfm"]);
                float tfdifficult = int.Parse(fc["tfd"]);

                var easytf_Questions = (tfeasy / 100) * truefalseQuestions;
                var mediumtf_Questions = (tfmedium / 100) * truefalseQuestions;
                var difficulttf_Questions = (tfdifficult / 100) * truefalseQuestions;

                if (((easytf_Questions + 1) <= easymcq_count) && ((mediumtf_Questions + 1) <= mediummcq_count) && ((difficulttf_Questions + 1) <= difficultmcq_count) && (mcqQuestions <= allmcq_count))
                {
                    float a = easytf_Questions % 1;
                    float b = mediumtf_Questions % 1;
                    float c = difficulttf_Questions % 1;
                    float d = a + b + c;
                    if (d == 0.0)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(mediumtf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficulttf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediumtf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediumtf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficulttf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }
                    else if (d == 1)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }
                    else if (d == 2)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficulttf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficulttf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions)+1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                            randomNumber.Clear();
                            for (int i = 0; i < mcqQuestions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, allmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E" || z.Type == "MCQ-M" || z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }   
                }
                else
                {
                    if ((easytf_Questions+1 > easytf_count) && (mediumtf_Questions+1 > mediumtf_count) && (difficulttf_Questions+1 > difficulttf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium , Difficult True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (mediumtf_Questions+1 > mediumtf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (difficulttf_Questions+1 > difficulttf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Difficult True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediumtf_Questions+1 > mediumtf_count) && (difficulttf_Questions+1 > difficulttf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium , Difficult True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (mediumtf_Questions+1 > mediumtf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy and Medium True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (difficulttf_Questions+1 > difficulttf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy and Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediumtf_Questions+1 > mediumtf_count) && (difficulttf_Questions+1 > difficulttf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium and Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediumtf_Questions+1 > mediumtf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((difficulttf_Questions+1 > difficulttf_count) && (mcqQuestions+1 > allmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult True/False and MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (easytf_Questions+1 > easytf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mediumtf_Questions+1 > mediumtf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (difficulttf_Questions+1 > difficulttf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mcqQuestions+1 > allmcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                }
            }
            else if (fc["mcqe"] != null && fc["tfe"] != null)
            {
                float mcqeasy = int.Parse(fc["mcqe"]);
                float mcqmedium = int.Parse(fc["mcqm"]);
                float mcqdifficult = int.Parse(fc["mcqd"]);
                var easymcq_Questions = (mcqeasy / 100) * mcqQuestions;
                var mediummcq_Questions = (mcqmedium / 100) * mcqQuestions;
                var difficultmcq_Questions = (mcqdifficult / 100) * mcqQuestions;

                float tfeasy = int.Parse(fc["tfe"]);
                float tfmedium = int.Parse(fc["tfm"]);
                float tfdifficult = int.Parse(fc["tfd"]);
                var easytf_Questions = (tfeasy / 100) * truefalseQuestions;
                var mediumtf_Questions = (tfmedium / 100) * truefalseQuestions;
                var difficulttf_Questions = (tfdifficult / 100) * truefalseQuestions;

                if (((easymcq_Questions + 1) <= easymcq_count) && ((mediummcq_Questions + 1) <= mediummcq_count) && ((difficultmcq_Questions + 1) <= difficultmcq_count) && ((easytf_Questions + 1) <= easytf_count) && ((mediumtf_Questions + 1) <= mediumtf_count) && ((difficulttf_Questions + 1) <= difficulttf_count))
                {
                    float m1 = easymcq_Questions % 1;
                    float m2 = mediummcq_Questions % 1;
                    float m3 = difficultmcq_Questions % 1;
                    float m = m1 + m2 + m3;
                    
                    if (m == 0.0)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(mediummcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficultmcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediummcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easymcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediummcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficultmcq_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }
                    else if (m == 1)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easymcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }
                    else if (m == 2)
                    {
                        if (easymcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (mediummcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficultmcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else if (difficultmcq_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easymcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easymcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediummcq_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediummcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficultmcq_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficultmcq_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "MCQ-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.MCQ = question_mcq;
                        }
                    }
                    float t1 = easytf_Questions % 1;
                    float t2 = mediumtf_Questions % 1;
                    float t3 = difficulttf_Questions % 1;
                    float t = t1 + t2 + t3;
                    
                    if (t == 0.0)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(mediumtf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficulttf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediumtf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < easytf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < mediumtf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < difficulttf_Questions; i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                    else if (t == 1)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(easytf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                    else if (t == 2)
                    {
                        if (easytf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }

                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (mediumtf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(difficulttf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else if (difficulttf_Questions == 0)
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                        else
                        {
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(easytf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, easytf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-E").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < (Math.Floor(mediumtf_Questions) + 1); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, mediumtf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-M").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            randomNumber.Clear();
                            for (int i = 0; i < Math.Floor(difficulttf_Questions); i++)
                            {
                                int number;
                                do
                                {
                                    number = rand.Next(1, difficulttf_count + 1);
                                } while (randomNumber.Contains(number));
                                randomNumber.Add(number);
                                var qtn = obj.Questions.Where(z => z.Type == "True/False-D").OrderBy(z => z.Question_Id).Skip(number - 1).Take(1).ToList();
                                foreach (var put in qtn)
                                {
                                    question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                                }
                            }
                            ViewBag.TrueFalse = question_tf;
                        }
                    }
                }
                else
                {
                    if (((easymcq_Questions+1) > easymcq_count) && ((mediummcq_Questions+1) > mediummcq_count) && ((difficultmcq_Questions+1) > difficultmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium , Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (((easytf_Questions+1) > easytf_count) && ((mediumtf_Questions+1) > mediumtf_count) && ((difficulttf_Questions+1) > difficulttf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium , Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (mediummcq_Questions+1 > mediummcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easymcq_Questions+1 > easymcq_count) && (difficultmcq_Questions+1 > difficultmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediummcq_Questions+1 > mediummcq_count) && (difficultmcq_Questions+1 > difficultmcq_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium , Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (mediumtf_Questions+1 > mediumtf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Medium True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((easytf_Questions+1 > easytf_count) && (difficulttf_Questions+1 > difficulttf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy , Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if ((mediumtf_Questions+1 > mediumtf_count) && (difficulttf_Questions+1 > difficulttf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium , Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (easymcq_Questions+1 > easymcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mediummcq_Questions+1 > mediummcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (difficultmcq_Questions+1 > difficultmcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult MCQ's to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (easytf_Questions+1 > easytf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Easy True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mediumtf_Questions+1 > mediumtf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Medium True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (difficulttf_Questions+1 > difficulttf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough Difficult True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                }
            }
            else if (fc["mcqe"] == null && fc["tfe"] == null)
            {
                if (mcqQuestions <= allmcq_count && truefalseQuestions <= alltf_count)
                {
                    randomNumber.Clear();
                        for (int i = 0; i < mcqQuestions; i++)
                        {
                            int number;
                            do
                            {
                                number = rand.Next(1, allmcq_count+1);
                            } while (randomNumber.Contains(number));
                            randomNumber.Add(number);
                            var qtn = obj.Questions.Where(a => a.Type == "MCQ-E" || a.Type == "MCQ-M" || a.Type == "MCQ-D").OrderBy(b => b.Question_Id).Skip(number-1).Take(1).ToList();
                            foreach (var put in qtn)
                            {
                                question_mcq.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type});
                            }
                        }
                        ViewBag.MCQ = question_mcq;
                        randomNumber.Clear();
                        for (int i = 0; i < truefalseQuestions; i++)
                        {
                            int number;
                            do
                            {
                                number = rand.Next(1, alltf_count+1);
                            } while (randomNumber.Contains(number));
                            randomNumber.Add(number);

                            var tfs = obj.Questions.Where(a => a.Type == "True/False-E" || a.Type == "True/False-M" || a.Type == "True/False-D").OrderBy(b => b.Question_Id).Skip(number-1).Take(1).ToList();
                            foreach (var put in tfs)
                            {
                                question_tf.Add(new Question() { Question_Id = put.Question_Id, Questions = put.Questions, Type = put.Type });
                            }
                        }
                        ViewBag.TrueFalse = question_tf;
                }
                else
                {
                    if ((mcqQuestions > allmcq_count) && (truefalseQuestions > alltf_count))
                    {
                        ViewBag.ErrorMessage = "You have not enough questons for MCQ'S and True/False to meet the Criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (mcqQuestions > allmcq_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough questons for MCQ'S to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                    else if (truefalseQuestions > alltf_count)
                    {
                        ViewBag.ErrorMessage = "You have not enough questons for True/False to meet the criteria";
                        List<Subject> sub = Create_Criteria_Method();
                        return View("Create_Criteria", sub);
                    }
                }
            }
        return View();
        }
        public ActionResult Send_Exam(FormCollection fc)
        {
            Paper paper = new Paper();
            for (int i = 1; i <= ((fc.Count)-2);i++ )
            {
                paper.Question_Id = int.Parse(fc["question_" + i]);
                paper.Question_Marks = int.Parse(fc["mark_" + i]);
                paper.Exam_Id = 1;
                obj.Papers.Add(paper);
                obj.SaveChanges();
            }
                return View();
        }

        public ActionResult Manage_MCQ(int id)
        {
            Subject sub = obj.Subjects.First(a => a.Subject_Id.Equals(id));
            ViewBag.Subject = sub.Subject_Name;
            ViewBag.subject_id = id;
            var mcqs = sub.Questions.Where(a => a.Type.Equals("MCQ-E") || a.Type.Equals("MCQ-M") || a.Type.Equals("MCQ-D"));
            return View(mcqs);
        }
        public ActionResult View_MCQ_Detail(int id,int sub_id)
        {
            List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == id).ToList();
            Subject sub = obj.Subjects.First(a => a.Subject_Id == sub_id);
            ViewBag.subject = sub.Subject_Name;
            ViewBag.subject_id = sub_id;
            return View(qtn);
        }
        public ActionResult Del_MCQ(int id,int sub_id)
        {
            Question q=obj.Questions.First(a => a.Question_Id.Equals(id));
            obj.Questions.Attach(q);
            obj.Questions.Remove(q);
            obj.SaveChanges();
            return RedirectToAction("Manage_MCQ", "Teacher", new { @id=sub_id});
        }
        public ActionResult Edit_MCQ(int id, int sub_id)
        {
            List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == id).ToList();
            ViewBag.subject_id = sub_id; 
            return View(qtn);
        }
        [HttpPost]
        public ActionResult Edit_MCQ(FormCollection fc , int id , int sub_id)
        {
            obj.Options.Where(a => a.Question_Id == id).ToList().ForEach(a => obj.Options.Remove(a));
            obj.SaveChanges();

            Question qtn = new Question();
            Option optn = new Option();
            qtn = obj.Questions.First(a => a.Question_Id == id);
            qtn.Questions=fc["Questions"];
            qtn.Type = fc["Type"];
            obj.SaveChanges();
            
            int counter = 2;
            int count = fc.Count;
            for (int i = 1; i <= (count - counter) ; i++)
            {
                qtn = obj.Questions.First(a => a.Question_Id == id);
                optn.Question_Id = qtn.Question_Id;
                optn.Options = fc["Option" + i];
                if (fc[""+i] !=null)
                {
                    optn.Correct_Answer = "yes";
                    counter++;
                }
                else
                {
                    optn.Correct_Answer = " ";
                }

                obj.Options.Add(optn);
                obj.SaveChanges();
            }

            return RedirectToAction("Manage_MCQ", "Teacher", new { @id=sub_id});             
        }

        public ActionResult Add_MCQ(int id)
        {
            ViewBag.subject_id = id;
            return View();
        }
        [HttpPost]
        public ActionResult Add_MCQ(FormCollection fc,int id)
        {
            Option opt = new Option();
            Question qtn = new Question();
            int counter = 2;
            string question1 = fc["Questions"];
            string type1 = fc["Type"];
            qtn.Questions = question1;
            qtn.Subject_Id = id;
            qtn.Type = type1;
            obj.Questions.Add(qtn);
            obj.SaveChanges();
            int count=fc.Count;
            for (int i = 1; i <= (count-counter) ; i++)
            {
                qtn = obj.Questions.OrderByDescending(a => a.Question_Id).First(a => a.Questions.Equals(question1));
                    opt.Options = fc["Option" + i];
                    opt.Question_Id = qtn.Question_Id;
                    if (fc["check" + i] !=null)
                    {
                        opt.Correct_Answer = "yes";
                        counter++;
                    }
                    else
                    {
                        opt.Correct_Answer = " ";
                    }


                obj.Options.Add(opt);
                obj.SaveChanges();
            }
            return RedirectToAction("Manage_MCQ","Teacher",new { @id = id});
        }

        public ActionResult Manage_TrueFalse(int id)
        {
            Subject sub = obj.Subjects.First(a => a.Subject_Id.Equals(id));
            ViewBag.Subject = sub.Subject_Name;
            ViewBag.subject_id = id;
            var tf = sub.Questions.Where(a => a.Type.Equals("True/False-E") || a.Type.Equals("True/False-M") || a.Type.Equals("True/False-D"));
            return View(tf);
        }
        public ActionResult View_TrueFalseDetail(int id,int sub_id)
        {
            List<Question> qtn = obj.Questions.Include(a => a.Options).Where(b => b.Question_Id == id).ToList();
            Subject sub = obj.Subjects.First(a => a.Subject_Id == sub_id);
            ViewBag.subject = sub.Subject_Name;
            ViewBag.subject_id = sub_id;
            return View(qtn);
        }
        public ActionResult Del_TrueFalse(int id,int sub_id)
        {
            Question qtn = obj.Questions.First(a => a.Question_Id.Equals(id));
            obj.Questions.Attach(qtn);
            obj.Questions.Remove(qtn);
            obj.SaveChanges();
            return RedirectToAction("Manage_TrueFalse", "Teacher", new { @id=sub_id});
        }
        public ActionResult Edit_TrueFalse(int id,int sub_id)
        {
            List<Question> qtn = obj.Questions.Include(a=>a.Options).Where(b=>b.Question_Id.Equals(id)).ToList();
            ViewBag.subject_id = sub_id;
            return View(qtn);
        }
        [HttpPost]
        public ActionResult Edit_TrueFalse(FormCollection fc,int id,int sub_id)
        {
            Question qtn = new Question();
            Option optn = new Option();
            qtn = obj.Questions.First(a => a.Question_Id.Equals(id));
            qtn.Questions = fc["Questions"];
            qtn.Type = fc["Type"];
            obj.SaveChanges();
            optn = obj.Options.First(a => a.Question_Id==id);
            optn.Options = fc["radio1"];
            obj.SaveChanges();
            return RedirectToAction("Manage_TrueFalse", "Teacher", new { @id = sub_id });
        }
        public ActionResult Add_TrueFalse(int id)
        {
            ViewBag.subject_id = id;
            return View();
        }
        [HttpPost]
        public ActionResult Add_TrueFalse(FormCollection fc, int id, int sub_id)
        {
            Question q = new Question();
            Option o = new Option();
            string question = fc["questions"];
            string option = fc["radio1"];
            string type = fc["Type"];
            q.Questions = question;
            q.Subject_Id = id;
            q.Type = type;
            obj.Questions.Add(q);
            obj.SaveChanges();
            q = obj.Questions.OrderByDescending(a => a.Question_Id).First(a => a.Questions.Equals(question));
            o.Options = option;
            o.Question_Id = q.Question_Id;
            o.Correct_Answer = " ";
            obj.Options.Add(o);
            obj.SaveChanges();
            return RedirectToAction("Manage_TrueFalse", "Teacher", new { @id = sub_id });
        }
        public ActionResult Logout()
        {
            Session["User_Id"] = "";
            return RedirectToAction("Index", "Home");
        }

    }
}
