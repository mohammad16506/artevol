using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using artevol.Models;

namespace artevol.Controllers
{
    public class usersController : Controller
    {
        private artevolEntities db = new artevolEntities();

		// GET: users
		[Authorize(Roles = "admin")]
		public ActionResult Index()
        {
            var users = db.users.Include(u => u.role1).Where(p=>p.isActive == true);
            return View(users.ToList());
        }

		// GET: users/Details/5
		[Authorize(Roles = "admin")]
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            ViewBag.role = new SelectList(db.roles, "id", "role1");
            return View();
        }

        // POST: users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,gender,age,email,password,role,isActive")] user user)
        {
            if (ModelState.IsValid)
            {
                user.role = 2;
                user.isActive = true;
                db.users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.role = new SelectList(db.roles, "id", "role1", user.role);
            return View(user);
        }

		// GET: users/Edit/5
		[Authorize(Roles = "admin")]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.role = new SelectList(db.roles, "id", "role1", user.role);
            return View(user);
        }

        // POST: users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,gender,age,email,password,role,isActive")] user user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.role = new SelectList(db.roles, "id", "role1", user.role);
            return View(user);
        }

		// GET: users/Delete/5
		[Authorize(Roles = "admin")]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            user user = db.users.Find(id);
            //db.users.Remove(user);
            user.isActive = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            //ModelState.AddModelError("", "INVALID CREDENTIALS");
            
            return View();

        }

        [HttpPost]
        public ActionResult Login(user user)
        {
			//bool isContain = db.users.Any( x => x.email==user.email && x.password==user.password );
			var loggedInUser = db.users.FirstOrDefault(x => x.email == user.email && x.password == user.password);
			if (loggedInUser != null)
            {
                bool isContain;
                Session["uId"] = loggedInUser.id;
				Session["role"] = loggedInUser.role;
                int rs = (int)Session["role"];

				//            FormsAuthentication.SetAuthCookie(loggedInUser.name,false);
				//            int role = (int)loggedInUser.role;
				//string[] userRoles = Roles.GetRolesForUser(loggedInUser.name);

				//if (role == 1) { Roles.AddUserToRole(loggedInUser.name, "admin"); }
				//            else if (role == 3) { Roles.AddUserToRole(loggedInUser.name, "user2"); }
				//else { Roles.AddUserToRole(loggedInUser.name, "user1"); }
				// Set authentication cookie
				FormsAuthentication.SetAuthCookie(loggedInUser.name, false);

				// Get role ID from the logged in user
				int roleId = (int)loggedInUser.role;

				// Map role ID to role name
				string roleName = null;
				if (roleId == 1)
				{
					roleName = "admin";
				}
				else if (roleId == 2)
				{
					roleName = "user1";
				}
				else if (roleId == 3)
				{
					roleName = "user2";
				}

				// If a valid role name is found, assign the user to that role
				if (roleName != null)
				{
					// Check if the user is already assigned to the role
					string[] userRoles = Roles.GetRolesForUser(loggedInUser.name);
                    string[] ab = Roles.GetAllRoles();
					if (!userRoles.Contains(roleName))
					{
						Roles.AddUserToRole(loggedInUser.name, roleName);
					}
				}

				return RedirectToAction("Index","Products");
            }
            else
            {
                ModelState.AddModelError("", "WRONG CREDENTIALS ENTERED");
                return View();
            }

		}
        [Authorize]
		public ActionResult Logout()
		{
			Session.Clear(); // Clear all session variables
			return RedirectToAction("Index", "Products"); // Redirect to the login page
		}
        [Authorize]
		public ActionResult Request()
		{
			return View();
		}

        [HttpPost]
		public ActionResult Request(request request)
        {
            request.status = "pending";
            request.user = (int)Session["uId"];
            db.requests.Add(request);
            db.SaveChanges();
            return RedirectToAction("Index","Products");
        }

		[Authorize(Roles = "admin")]
		public ActionResult ReqAction(int id,String status,int uid)
        {
            var request = db.requests.Find(id);
				request.status = status;
            db.SaveChanges();

            if (status == "accept") {
                var user = db.users.Find(uid);
                user.role = (user.role==1) ? 1 : 3;
                int chk = (int)user.role;
                db.SaveChanges();
                    }
            return RedirectToAction("RequestShow","Users");
        }

		[Authorize(Roles = "admin")] 
		public ActionResult RequestShow()
        {
            var request = db.requests.Include(p => p.user1).Include(p=>p.reqType1);
            return View(request.ToList());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
