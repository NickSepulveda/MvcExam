using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MvcExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace MvcExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            _context = context;
        }
     
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("register")]     //associated route string (exclude the leading /)
            public IActionResult RegisterUser(User user)
            {
                if(ModelState.IsValid)
                {
                    // do somethng!  maybe insert into db?  then we will redirect
                    // If a User exists with provided email
                    if(_context.Users.Any(u => u.Username == user.Username))
                    {
                        //UNIQUE USERNAME VALIDATION
                        // Manually add a ModelState error to the Email field, with provided
                        // error message
                        ModelState.AddModelError("Username", "Username already in use!");
                        return View("Index");
                        // You may consider returning to the View at this point
                    }
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.Password = Hasher.HashPassword(user, user.Password);
                    _context.Add(user);
                    _context.SaveChanges();
                    ViewBag.UserFirstName = user.FirstName;
                    ViewBag.UserLastName = user.LastName;
                    ViewBag.ThisUser = user;
                    ViewBag.UserFirstName = user.FirstName;
                    ViewBag.UserLastName = user.LastName;
                    //Input all Hobby info for dashboard here
                    ViewBag.AllHobbies = _context.Hobbies.Include(s => s.UsersForThisHobby);
                    var test = _context.Hobbies.Count();
                    ViewBag.AllHobbiesCount = test;
                    var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby);
                    var enthusiastsCount = hobbyenthusiasts.Count();
                    ViewBag.EnthusiastsCount = enthusiastsCount;
                    ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
                    var onthelist = _context.Hobbies.Include(a => a.UsersForThisHobby).Where(a => a.UserId == user.UserId);
                    var hello = _context.Associations.Any(r => r.UserId == user.UserId);
                    ViewBag.context = _context;
                    ViewBag.onthelisttest = hello;
                    Console.WriteLine(hello);
                    return View("UserDashboard");
                }
                else
                {
                    // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                    return View("Index");
                } 
            }
        [HttpPost("login/user")]
            public IActionResult LoginUser(LoginUser user)
            {
                if(ModelState.IsValid)
                {
                    var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                    // If no user exists with provided email
                    if(userInDb == null)
                    {
                        // Add an error to ModelState and return to View!
                        ModelState.AddModelError("Email", "Email not in Database");
                        return View("Index");
                    }
                    // Initialize hasher object
                    var hasher = new PasswordHasher<LoginUser>();
                    // verify provided password against hash stored in db
                    var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.Password);
                    // result can be compared to 0 for failure
                    if(result == 0)
                    {
                        // handle failure (this should be similar to how "existing email" is handled)
                        ModelState.AddModelError("Password", "Invalid Password");
                        return View("Index");
                    }
                    ViewBag.ThisUser = userInDb;
                    //Input all Hobby info for dashboard here
                    ViewBag.AllHobbies = _context.Hobbies.Include(s => s.UsersForThisHobby);
                    var test = _context.Hobbies.Count();
                    ViewBag.AllHobbiesCount = test;
                    var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby);
                    var enthusiastsCount = hobbyenthusiasts.Count();
                    ViewBag.EnthusiastsCount = enthusiastsCount;
                    ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
                    var onthelist = _context.Hobbies.Include(a => a.UsersForThisHobby).Where(a => a.UserId == userInDb.UserId);
                    var hello = _context.Associations.Any(r => r.UserId == userInDb.UserId);
                    ViewBag.context = _context;
                    ViewBag.onthelisttest = hello;
                    return View("UserDashboard");
                }
                else
                {
                    // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                    return View("Index");
                }
            }
        [HttpGet("userdashboard/{userId}")]
        public IActionResult UserDashboard(LoginUser user)
        { 
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == user.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Email not in Database");
                    return View();
                }
            }
            return View("Unauthorized");
        }
        
        [HttpGet("/planhobby/{userId}")]
        public IActionResult PlanHobby(int userId)
        {
            var ThisUser = _context.Users.FirstOrDefault(b => b.UserId == userId);
            ViewBag.ThisUser = ThisUser;
            return View("CreateHobby");
        }
        [HttpPost("createhobby")]     //associated route string (exclude the leading /)
            public IActionResult CreateHobby(Hobby hobby, int userId)
            {
                var ThisUser = _context.Users.FirstOrDefault(b => b.UserId == userId);
                ViewBag.ThisUser = ThisUser;
                if(ModelState.IsValid)
                {
                    if(_context.Hobbies.Any(u => u.Name == hobby.Name))
                    {
                        //UNIQUE Hobby Name VALIDATION
                        // Manually add a ModelState error to the Email field, with provided
                        // error message
                        ModelState.AddModelError("Name", "Hobby Name already added!");
                        return View("CreateHobby");
                        // You may consider returning to the View at this point
                    }
                    var userInDb = _context.Users.FirstOrDefault(b => b.UserId == userId);
                    ViewBag.ThisUser = userInDb;
                    var thishobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobby.HobbyId);
                    ViewBag.Hobby = thishobby;
                    var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby).Where(b => b.HobbyId == hobby.HobbyId);
                    var enthusiastCount = hobbyenthusiasts.Count();
                    ViewBag.EnthusiastsCount = enthusiastCount;
                    ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
                    _context.Add(hobby);
                    _context.SaveChanges();
                    ViewBag.ThisHobby = hobby;
                    return View("ViewHobby");
                }
                else
                {
                    // Oh no!  We need to return a ViewResponse to preserve the ModelState, and the errors it now contains!
                    return View("CreateHobby");
                } 
            }
        [HttpGet("/dashboardredirect/{userId}")]
        public IActionResult DashBoardRedirect(int hobbyId, int userId)
        {
            var userInDb = _context.Users.FirstOrDefault(b => b.UserId == userId);
            ViewBag.ThisUser = userInDb;
            //Input all hobby info for dashboard here
            var thishobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            ViewBag.Hobby = thishobby;
            var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby).Where(b => b.HobbyId == hobbyId);
            var enthusiastCount = hobbyenthusiasts.Count();
            ViewBag.EnthusiastsCount = enthusiastCount;
            ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
            ViewBag.AllHobbies = _context.Hobbies.Include(s => s.UsersForThisHobby);
            var test = _context.Hobbies.Count();
            ViewBag.AllHobbiesCount = test;
            return View("UserDashboard", userInDb);
        }
        [HttpGet("viewhobby/{hobbyId}/{userId}")]
        public IActionResult ViewHobby(int hobbyId, int userId)
        {
            var userInDb = _context.Users.FirstOrDefault(b => b.UserId == userId);
            ViewBag.ThisUser = userInDb;
            var hobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            ViewBag.Hobby = hobby;
            var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby).Where(b => b.HobbyId == hobbyId);
            var enthusiastCount = hobbyenthusiasts.Count();
            ViewBag.EnthusiastsCount = enthusiastCount;
            ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
            var thishobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobby.HobbyId);
            ViewBag.ThisHobby = thishobby;
            return View();
        }
        [HttpPost("updatehobby/{hobbyId}/{userId}")]
         public IActionResult UpdateHobby(Association newAssociation, int hobbyId, int userId)
        {
            var userInDb = _context.Users.FirstOrDefault(b => b.UserId == userId);
            ViewBag.ThisUser = userInDb;
            var hobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            ViewBag.Hobby = hobby;
            var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby).Where(b => b.HobbyId == hobbyId);
            var enthusiastCount = hobbyenthusiasts.Count();
            ViewBag.EnthusiastsCount = enthusiastCount;
            ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
            var thishobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobby.HobbyId);
            ViewBag.ThisHobby = thishobby;
            var ThisHobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            ViewBag.ThisHobby = ThisHobby;
            if(ModelState.IsValid)
            {
                var unrelatedHobbies = _context.Hobbies
                .Include(c => c.UsersForThisHobby)
                .Where(c => c.UsersForThisHobby.All(a => a.UserId != userId));
                if(!(unrelatedHobbies.Contains(thishobby)))
                {
                    //MULTIPLE ADDS VALIDATION
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("HobbyId", "You've already added this hobby!");
                    return View("ViewHobby");
                    // You may consider returning to the View at this point
                }
                _context.Add(newAssociation);
                _context.SaveChanges();
                return View("ViewHobby");
            }
            else 
            {
                return View("ViewHobby");
            }
        }
        [HttpGet("editthishobby/{hobbyId}/{userId}")]
        public IActionResult EditHobby(int hobbyId, int userId)
        {
            ViewBag.UserId = userId;
            Hobby RetrievedHobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            ViewBag.HobbyName = RetrievedHobby.Name;
            ViewBag.HobbyId = RetrievedHobby.HobbyId;
            ViewBag.HobbyDescription = RetrievedHobby.Description;
            return View();
        }
        [HttpPost("updatethathobby/{hobbyId}/{userId}")]
        public IActionResult UpdateUser(Hobby updatedhobby, int hobbyId, int userId)
        {
            Hobby UpdatedHobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
            if(ModelState.IsValid)
            {
                UpdatedHobby.Name = updatedhobby.Name;
                UpdatedHobby.Description = updatedhobby.Description;
                UpdatedHobby.UpdatedAt = DateTime.Now;
                var userInDb = _context.Users.FirstOrDefault(b => b.UserId == userId);
                ViewBag.ThisUser = userInDb;
                var hobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
                ViewBag.Hobby = hobby;
                var hobbyenthusiasts = _context.Associations.Include(a => a.User).Include(b => b.Hobby).Where(b => b.HobbyId == hobbyId);
                var enthusiastCount = hobbyenthusiasts.Count();
                ViewBag.EnthusiastsCount = enthusiastCount;
                ViewBag.HobbyEnthusiasts = hobbyenthusiasts;
                var thishobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobby.HobbyId);
                ViewBag.ThisHobby = thishobby;
                var ThisHobby = _context.Hobbies.FirstOrDefault(b => b.HobbyId == hobbyId);
                ViewBag.ThisHobby = ThisHobby;
                _context.SaveChanges();
                return View("ViewHobby");
            }
            else return View("EditHobby");
        }
    }

}
