using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using artevol.Models;

namespace artevol.Controllers
{
    [Authorize]
    public class ordersController : Controller
    {
        private artevolEntities db = new artevolEntities();

        public ActionResult accept(int id)
        {
            order order = db.orders.Find(id);
            order.status = "accepted";
            db.SaveChanges();

            return RedirectToAction("Index");
        }
		public ActionResult decline(int id)
		{
			order order = db.orders.Find(id);
			order.status = "declined";
			db.SaveChanges();

			return RedirectToAction("Index");
		}
        public ActionResult uOrder(int uid)
        {
            var orders  = db.orders.Where( o => o.user == uid);
            return View(orders.ToList());
        }

		// GET: orders
		[Authorize(Roles = "admin")]
		public ActionResult Index()
        {
            var orders = db.orders.Include(o => o.user1);
            return View(orders.ToList());
        }

		// GET: orders/Details/5
		[Authorize(Roles = "admin")]
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: orders/Create
        [Authorize]
        public ActionResult Create()
        {

            ViewBag.prd_id = new SelectList(db.products, "id", "name");
            ViewBag.user = new SelectList(db.users, "id", "name");
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( order order,HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
				var imageName = Path.GetFileName(image.FileName);
				var imagePath = Path.Combine(Server.MapPath("~/Content/images/tr/"), imageName);
				image.SaveAs(imagePath);
				//order.tr_img = imagePath;
				order.tr_img = "/Images/tr/" + imageName;

				int total = 0;
				var cart = Session["cart"] as List<cart>;
                foreach (var prd in cart)
                {
                    var ordPr = new order_prd
                    {
                        product = prd.id,
                        quantity = 1
                    };
                    total += (int)prd.price;
                    order.amount = total;
					db.orders.Add(order);

                    ordPr.order = order.id;
					db.order_prd.Add(ordPr);
                    db.SaveChanges();
                }

                
                db.SaveChanges();
                Session["cart"] = null;
                return RedirectToAction("Index","Products");
            }

        
            return View(order);
        }

		// GET: orders/Edit/5
		[Authorize(Roles = "admin")]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            
            
            return View(order);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user,prd_id,amount,date,status,payment_type,tr_id,tr_img")] order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(order);
        }

		// GET: orders/Delete/5
		[Authorize(Roles = "admin")]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            order order = db.orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            order order = db.orders.Find(id);
            order.status = "delete";
            db.SaveChanges();
            return RedirectToAction("Index");
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
