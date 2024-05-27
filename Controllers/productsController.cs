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
    public class productsController : Controller
    {
        private artevolEntities db = new artevolEntities();

      
		public ActionResult About()
		{
			return View();
		}
		// GET: products
		public ActionResult Index()
        {
            //var products = db.products.Include(p => p.art_type1).Include(p => p.sale_type1).Include(p => p.user).Where(p => p.isActive=="1").Include(p => p.images).SingleOrDefault(p => p.id == id);
            var products = db.products
				 .Where(p => p.isActive.Equals("1") ) // Filter by isActive == 1
								.Include(p => p.art_type1)
                                .Include(p => p.sale_type1)
                                .Include(p => p.user)
								.Select(p => new
                                {
                                    Product = p,
                                    Images = db.images.Where(i => i.prd_id == p.id).ToList()
                                })
                                .ToList()
                                .Select(p => new ProductImg
                                {
                                    Product = p.Product,
                                    Images = p.Images
                                });
            return View(products.ToList());
        }

		// GET: products/Details/5
		[Authorize]
		public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //product product = db.products.Find(id);
            var product = db.products.Include(p => p.images).SingleOrDefault(p => p.id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
			//var bids = db.bids.Where(p => p.prd_id == id).ToList();
			var bids = db.bids.Where(p => p.prd_id == id).OrderByDescending(p => p.amount).ToList();

			var viewModel = new prd_bids
            {
                Product = product,
                Bids = bids
            };
            return View(viewModel);
        }

		// GET: products/Create
		[Authorize(Roles = "admin,user2")]
		public ActionResult Create()
        {
            ViewBag.art_type = new SelectList(db.art_type, "id", "type");
            ViewBag.sale_type = new SelectList(db.sale_type, "id", "sale");
            ViewBag.addedBy = new SelectList(db.users, "id", "name");
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "admin,user2")]
		public ActionResult Create([Bind(Include = "id,name,description,sale_type,art_type,price,addedBy")] product product, HttpPostedFileBase[] productImages)
        {
            if (ModelState.IsValid)
            {
                product.addedBy = (int)Session["uId"];
                product.isActive = "1";
                db.products.Add(product);
                db.SaveChanges();
                foreach (var image in productImages)
                {
                    if (image != null && image.ContentLength > 0)
                    {
                        var imageName = Path.GetFileName(image.FileName);
                        var imagePath = Path.Combine(Server.MapPath("~/Content/images/products/"), imageName);
                        image.SaveAs(imagePath);
                        image dbImage = new image
                        {
                            prd_id = product.id,
                            url = "/Images/Products/" + imageName
                        };
                        db.images.Add(dbImage);
                        db.SaveChanges();

                    }
                }
                return RedirectToAction("AdminList", "products");

            }

            ViewBag.art_type = new SelectList(db.art_type, "id", "type", product.art_type);
            ViewBag.sale_type = new SelectList(db.sale_type, "id", "sale", product.sale_type);
            ViewBag.addedBy = new SelectList(db.users, "id", "name", product.addedBy);
            return View(product);
        }

		// GET: products/Edit/5
		[Authorize(Roles = "admin")]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.art_type = new SelectList(db.art_type, "id", "type", product.art_type);
            ViewBag.sale_type = new SelectList(db.sale_type, "id", "sale", product.sale_type);
            ViewBag.addedBy = new SelectList(db.users, "id", "name", product.addedBy);
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,sale_type,art_type,price,addedBy,isActive")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                product.isActive = "1";
                string ab = product.isActive;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.art_type = new SelectList(db.art_type, "id", "type", product.art_type);
            ViewBag.sale_type = new SelectList(db.sale_type, "id", "sale", product.sale_type);
            ViewBag.addedBy = new SelectList(db.users, "id", "name", product.addedBy);
            return View(product);
        }

		// GET: products/Delete/5
		[Authorize(Roles = "admin")]
		public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //var image = db.images.Where(p=>p.prd_id==id);
            //         foreach (var item in image)
            //         {
            //             db.images.Remove(item);
            //         }
            //db.SaveChanges();
            product product = db.products.Find(id);
            if (product != null)
            {
                product.isActive = "0";

                db.SaveChanges();
            }

            return RedirectToAction("AdminList");
        }

		public ActionResult Activate(int id)
		{
			
			product product = db.products.Find(id);
			if (product != null)
			{
				product.isActive = "1";

				db.SaveChanges();
			}

			return RedirectToAction("AdminList");
		}
		public ActionResult addToCart(product product) {
            var cart = Session["cart"] as List<cart>;
            if (Session["uId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            if (cart == null)
            {
                cart = new List<cart>();
            }
            cart cartobj = new cart();
            cartobj.id = product.id;
            cartobj.name = product.name;
            cartobj.price = product.price;
            cartobj.quantity = 1;
            //cartobj.total = cartobj.quantity * cartobj.price
            cart.Add(cartobj);
            Session["cart"] = cart;

            return RedirectToAction("Cart", "Products");
        }
        public ActionResult Cart()
        {
            var cart = Session["cart"] as List<cart>;

            if (cart != null && cart.Count() > 0)
            {
                return View(cart);
            }
            else
            {
                
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles ="user1")]
        public ActionResult PlaceBid(int Prd_Id, decimal bidAmount)
        {
            if (Session["uId"] == null || (int)Session["uId"] == 0)
            //if(false)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var bid = new bid
                {
                    prd_id = Prd_Id,
                    amount = (int)bidAmount,
                    user_id = (int)Session["uId"],
                    //user_id = 2,
				};
                
                db.bids.Add(bid);
                db.SaveChanges();
				return RedirectToAction("Details", new { id = Prd_Id });
			}

        }
		[Authorize(Roles = "admin")]
		public ActionResult AdminList()
        {
            return View(db.products.ToList());
        }

		public ActionResult List(int? category)
        {
            if (category == null)
            {
                var products = db.products
					.Where(p => p.isActive == "1") // Filter by isActive == 1
								 .Include(p => p.art_type1)
                                 .Include(p => p.sale_type1)
                                 .Include(p => p.user)
                                 .Select(p => new
                                 {
                                     Product = p,
                                     Images = db.images.Where(i => i.prd_id == p.id).ToList()
                                 })
                                 .ToList()
                                 .Select(p => new ProductImg
                                 {
                                     Product = p.Product,
                                     Images = p.Images
                                 });
				return View(products.ToList());
			}
            else
            {
				var products = db.products
					.Where(p => p.isActive == "1") // Filter by isActive == 1
								.Include(p => p.art_type1)
								.Include(p => p.sale_type1)
								.Include(p => p.user)
								.Select(p => new
								{
									Product = p,
									Images = db.images.Where(i => i.prd_id == p.id).ToList()
								})
								.ToList()
								.Select(p => new ProductImg
								{
									Product = p.Product,
									Images = p.Images
								})
                                .Where(p => p.Product.art_type == category);
			return View(products.ToList());
			}
			
        }
        public ActionResult Remove(cart cartdata)
        {
            List<cart> cartlist = (List<cart>)Session["cart"];
            cartlist.RemoveAll(p => p.id == cartdata.id);
            Session["cart"] = cartlist;

			return RedirectToAction("cart","Products");
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
