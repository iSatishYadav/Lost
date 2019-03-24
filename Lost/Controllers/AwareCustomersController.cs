using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lost.Controllers
{
    public class AwareCustomersController : Controller
    {
        private readonly LostEntities db = new LostEntities();

        // GET: AwareCustomers
        public async Task<ActionResult> Index()
        {
            return View("~/Views/Customers/Index.cshtml", await db.Customers.ToListAsync());
        }

        // GET: AwareCustomers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/Customers/Details.cshtml", customer);
        }

        // GET: AwareCustomers/Create
        public ActionResult Create()
        {
            return View("~/Views/Customers/Create.cshtml");
        }

        // POST: AwareCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,CustomerName,CustomerAddress")] Customer customer)
        {
            customer.UpdatedBy = User.Identity.Name;
            customer.UpdatedOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View("~/Views/Customers/Create.cshtml", customer);
        }

        // GET: AwareCustomers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            //Adding ETags for concurrency control            
            string prop = customer.UpdatedOn.Ticks.ToString();
            SetETagsInResponse(Response, prop);
            return View("~/Views/Customers/Edit.cshtml", customer);
        }

        private void SetETagsInResponse(HttpResponseBase response, string prop)
        {
            var hash = prop.ToMd5Hash();
            string hashString = $"\"{hash}\"";
            response.Headers.Add("ETag", hashString);
            var cookie = new HttpCookie("X-ETag", hash)
            {
                HttpOnly = true,
            };
            response.SetCookie(cookie);
        }

        private bool MatchETag(HttpRequestBase request, string prop)
        {
            var hash = prop.ToMd5Hash();
            string hashString = $"\"{hash}\"";
            var eTagHeader = request.Headers["ETag"];
            if (eTagHeader != null)
            {
                return eTagHeader.Equals(hashString);
            }
            var eTagCookie = request.Cookies["X-ETag"];
            if (eTagCookie != null)
            {
                return eTagCookie.Value.Equals(hash);
            }
            return false;
        }



        // POST: AwareCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,CustomerName,CustomerAddress")] Customer customerViewModel)
        {
            customerViewModel.UpdatedBy = User.Identity.Name;
            customerViewModel.UpdatedOn = DateTime.Now;
            var customerEntity = await db.Customers.FindAsync(customerViewModel.Id);
            if (customerEntity == null)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                if (!MatchETag(Request, customerEntity.UpdatedOn.Ticks.ToString()))
                {
                    string prop = customerEntity.UpdatedOn.Ticks.ToString();
                    SetETagsInResponse(Response, prop);
                    ModelState.AddModelError("", "Somebody has changed the values since you got them, please verify the values again and retry.");
                    ModelState.AddModelError(nameof(customerViewModel.CustomerAddress), $"Current value: {customerEntity.CustomerAddress}");
                    ModelState.AddModelError(nameof(customerViewModel.CustomerName), $"Current value: {customerEntity.CustomerName}");
                    return View("~/Views/Customers/Edit.cshtml", customerEntity);
                    //return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed);
                }
                customerEntity.CustomerName = customerViewModel.CustomerName;
                customerEntity.CustomerAddress = customerViewModel.CustomerAddress;
                customerEntity.UpdatedBy = User.Identity.Name;
                customerEntity.UpdatedOn = DateTime.Now;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("~/Views/Customers/Edit.cshtml", customerViewModel);
        }

        // GET: AwareCustomers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/Customers/Delete.cshtml", customer);
        }

        // POST: AwareCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
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
