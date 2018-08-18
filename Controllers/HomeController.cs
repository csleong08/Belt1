using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Belt1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Belt1.Controllers
{
    public class HomeController : Controller
    {
        private Belt1Context _context;
        public HomeController(Belt1Context context)
        {
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Index (Users myUser)
        {
            return View("Index");
        }
        [HttpPost("AddUser")]
        public IActionResult AddUser(UsersValidate uservalidator)
        {
            if(ModelState.IsValid)
            {
                var emailvalidation = _context.users.SingleOrDefault(p => p.email == uservalidator.email);
                if(emailvalidation == null)
                {
                    PasswordHasher<UsersValidate> Hasher = new PasswordHasher<UsersValidate>();
                    uservalidator.password = Hasher.HashPassword(uservalidator, uservalidator.password);
                    Users myUser = new Users();
                    myUser.first_name = uservalidator.first_name;
                    myUser.last_name = uservalidator.last_name;
                    myUser.email = uservalidator.email;
                    myUser.password = uservalidator.password;
                    myUser.created_at = DateTime.Now;
                    myUser.updated_at = DateTime.Now;
                    _context.Add(myUser);
                    _context.SaveChanges();

                    HttpContext.Session.SetInt32("UserID", myUser.id);
                    int? UserID = HttpContext.Session.GetInt32("UserID");
                    ViewBag.UserID = UserID;
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["uniqueemail"] = "This email belongs to a registered user. Please use another email address";
                    return View("Index");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("LoginProcess")]
        public IActionResult LoginProcess(LoginValidate myLogin)
        {
            if(ModelState.IsValid)
            {
                Users loginData = _context.users.SingleOrDefault(p => p.email == myLogin.login_email);
                if(loginData == null)
                {
                    ModelState.AddModelError("login_email", "Email Address is not registered");
                }
                else if(loginData != null && myLogin.login_password != null)
                {
                    var Hasher = new PasswordHasher<Users>();
                    // Pass the user object, the hashed password, and the PasswordToCheck
                    if(0 != Hasher.VerifyHashedPassword(loginData, loginData.password, myLogin.login_password))
                    {
                        HttpContext.Session.SetInt32("UserID", loginData.id);
                        int? UserID = HttpContext.Session.GetInt32("UserID");
                        ViewBag.UserID = UserID;
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ModelState.AddModelError("login_password", "Incorrect password");
                        return View("Index");
                    }
                }
                return View("Index");
            }
            // ViewBag.error = "LOL, Nice try!";
            // TempData["error"] = "LOL, try again!";
            return View("Index");
        }
        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            if(UserID == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.userInfo = _context.users.Where(p => p.id == UserID).SingleOrDefault();
            var allActivities = _context.activities.OrderBy(p => p.datetime).Include(p => p.Participant).ToList();
            ViewBag.allActivities = allActivities;
            // //  Bonus: Activities expire when the scheduled date passes, and are removed from the database.
            // foreach (var item in allActivities)
            // {
            //     if(item.date < DateTime.Today)
            //     {
            //         foreach (var join in item.Participant)
            //         {
            //             _context.participants.Remove(join);
            //         }
            //         _context.activities.Remove(item);
            //         _context.SaveChanges();
            //         return RedirectToAction("Dashboard");
            //     }
            // }
            return View("Dashboard");
        }
        [HttpPost("AddActivity")]
        public IActionResult AddActivity(ActivitiesValidate activityvalidator, int duration, DateTime datetime, DateTime date, string duration2)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            var userInfo = _context.users.Where(p => p.id == UserID).SingleOrDefault();
            if(ModelState.IsValid)
            {
                Activities myActivity = new Activities();
                myActivity.title = activityvalidator.title;
                myActivity.datetime = datetime;
                myActivity.date = date;
                myActivity.duration = duration;
                myActivity.duration2 = duration2;
                myActivity.coordinator = userInfo.first_name;
                myActivity.description = activityvalidator.description;
                myActivity.created_at = DateTime.Now;
                myActivity.updated_at = DateTime.Now;
                myActivity.usersid = (int)UserID;
                if(date < DateTime.Today)
                {
                    ModelState.AddModelError("date", "Activity creation has to be in the future, nice try!");
                    // ViewBag.error = "LOL, Nice try!";
                    return View("PlanActivity");
                };
                // if(myActivity.time < DateTime.Now)
                // {
                //     ModelState.AddModelError("time", "Activity creation has to be in the future, nice try!");
                //     // ViewBag.error = "LOL, Nice try!";
                //     return View("PlanActivity");
                // };
                _context.Add(myActivity);
                _context.SaveChanges();
                ViewBag.activityInfo = myActivity;
                var activityid = myActivity.id;
                // var activityid = _context.activities.Where(p => p.id).Last();
                // ViewBag.activityInfo = _context.activities.Where(p => p.id == activityid).Include(p => p.Participant)
                // .ThenInclude(p => p.Users).FirstOrDefault();
                return RedirectToAction("Activity", new{activityid=activityid});
                // return RedirectToAction("Activity");
            }
            else
            {
                return View("PlanActivity");
            }
        }
        [HttpGet("PlanActivity")]
        public IActionResult PlanActivity()
        {
            return View("PlanActivity");
        }
        [HttpGet("/Activity/{activityid}")]
        public IActionResult Activity(int activityid)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            ViewBag.userInfo = _context.users.Where(p => p.id == UserID).SingleOrDefault();
            var activityInfo = _context.activities.Where(p => p.id == activityid).Include(p => p.Participant)
            .ThenInclude(p => p.Users).FirstOrDefault();
            ViewBag.activityInfo = activityInfo;
            var allActivities = _context.activities.Include(p => p.Participant).ToList();
            ViewBag.allActivities = allActivities;
            // var participantID = _context.activities.Where(p )
            // var activityID = activityid;
            // ViewBag.activityID = activityID;

            return View("Activity");
        }
        [HttpPost("DeleteActivity")]
        public IActionResult DeleteActivity(int activityID)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            var DelParticipant = _context.participants.Where(p => p.activitiesid == activityID).ToList();
            var DelActivity = _context.activities.Where(p => p.id == activityID).SingleOrDefault();
            foreach (var join in DelParticipant)
            {
                _context.participants.Remove(join);
            }
            _context.activities.Remove(DelActivity);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost("DeleteJoin")]
        public IActionResult DeleteJoin(int activityID)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            var DelParticipant = _context.participants.Where(p => p.activitiesid == activityID).Where(p => p.usersid == UserID).SingleOrDefault();
            _context.participants.Remove(DelParticipant);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpPost("AddJoin")]
        public IActionResult AddJoin(int activityID)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            // var DelReservation = _context.reservations.Where(p => p.weddingsid == weddingID).ToList();
            // var DelReservation = _context.reservations.Where(p => p.id == reservationID).SingleOrDefault();
            Participants myParticipant = new Participants();
            myParticipant.created_at = DateTime.Now;
            myParticipant.updated_at = DateTime.Now;
            myParticipant.usersid = (int)UserID;
            myParticipant.activitiesid = activityID;
            _context.participants.Add(myParticipant);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        // [HttpPost("UpdateActivity")]
        // public IActionResult UpdateActivity(ActivitiesValidate activityvalidator, int activityid)
        // {
        //     int? UserID = HttpContext.Session.GetInt32("UserID");

        //     Activities activityUpdate = _context.activities.Where(p => p.id == activityid).FirstOrDefault();

        //     if(ModelState.IsValid)
        //     {
        //         if(activityUpdate.date < DateTime.Today)
        //         {
        //             ModelState.AddModelError("date", "Activity creation has to be in the future, nice try!");
        //             // ViewBag.error = "LOL, Nice try!";
        //             ViewBag.activityInfo = activityUpdate;
        //             return View("Activity");
        //         };
        //         activityUpdate.divorcee_one = activityvalidator.divorcee_one;
        //         activityUpdate.divorcee_two = activityvalidator.divorcee_two;
        //         activityUpdate.date = activityvalidator.date;
        //         activityUpdate.updated_at = DateTime.Now;
        //         activityUpdate.courthouse = activityvalidator.courthouse;

        //         _context.SaveChanges();
        //         return RedirectToAction("activity", new{activityid=activityid});
        //     }
        //     ViewBag.activityInfo = activityUpdate;
        //     return View("Activity");
        // }
    }
}
