using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using HR_System.Data;
using PagedList;
using HR_System.Web.Data;


namespace HR_Test.Controllers
{
    public class PeopleController : Controller
    {
        private HR_SystemWebContext db = new HR_SystemWebContext();

        // GET: People
        public async Task<ActionResult> Index(string sortOrder, string currentFilterDept, string currentFilterDStatus, string searchStringDept, string searchStringStatus, string currentPageSize, int? page)
        {
            ViewBag.currentPageSize = currentPageSize ?? "10";
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "LastName" : "";
            ViewBag.DepSortParm = sortOrder == "Department" ? "DepartmentDesc" : "Department";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstNameDesc" : "FirstName";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "StatusDesc" : "Status";
            ViewBag.DateOfBirthSortParm = sortOrder == "DOB" ? "DOBDesc" : "DOB";

            string pageSizeStr = currentPageSize ?? "10";
            int pageSize = int.Parse(pageSizeStr);
            if (searchStringDept != null || searchStringStatus != null)
            {
                page = 1;
            }
            else
            {
                searchStringDept = currentFilterDept;
                searchStringStatus = currentFilterDStatus;
            }

            ViewBag.CurrentFilterDept = searchStringDept;
            ViewBag.CurrentFilterStatus = searchStringStatus;

            IQueryable<Person> people;
            if (!String.IsNullOrEmpty(searchStringDept) && !String.IsNullOrEmpty(searchStringStatus))
            {
                people = db.People.Include(p => p.Company).Include(p => p.Department).Include(p => p.Status).Where(p =>
                     p.Department.DepartmentName == searchStringDept && p.Status.Status1 == searchStringStatus);
            }
            else if (!String.IsNullOrEmpty(searchStringDept))
            {
                people = db.People.Include(p => p.Company).Include(p => p.Department)
                   .Where(p => p.Department.DepartmentName == searchStringDept);
            }
            else
            {
                if (!String.IsNullOrEmpty(searchStringStatus))
                {
                    people = db.People.Include(p => p.Company).Include(p => p.Department)
                       .Where(p => p.Status.Status1 == searchStringStatus);
                }
                else
                {
                    people = db.People.Include(p => p.Company).Include(p => p.Department);
                }
            }

            //int pageSize = 3;
            int pageNumber = (page ?? 1);

            List<Person> subPeople;
            switch (sortOrder)
            {
                case "LastName":
                    subPeople = await people.OrderBy(x => x.LastName).ToListAsync();
                    break;
                //return View(await people.OrderBy(x=>x.LastName).ToListAsync());
                case "Department":
                    subPeople = await people.OrderBy(x => x.Department.DepartmentName).ToListAsync();
                    break;
                case "DepartmentDesc":
                    subPeople = await people.OrderByDescending(x => x.Department.DepartmentName).ToListAsync();
                    break;
                case "FirstName":
                    subPeople = await people.OrderBy(x => x.FirstName).ToListAsync();
                    break;
                case "FirstNameDesc":
                    subPeople = await people.OrderByDescending(x => x.FirstName).ToListAsync();
                    break;
                case "Status":
                    subPeople = await people.OrderBy(x => x.Status).ToListAsync();
                    break;
                case "StatusDesc":
                    subPeople = await people.OrderByDescending(x => x.StatusId).ToListAsync();
                    break;
                case "DOB":
                    subPeople = await people.OrderBy(x => x.DateOfBirth).ToListAsync();
                    break;
                case "DOBDesc":
                    subPeople = await people.OrderByDescending(x => x.DateOfBirth).ToListAsync();
                    break;
                default:
                    subPeople = await people.OrderByDescending(x => x.LastName).ToListAsync();
                    break;

            }
            return View(subPeople.ToPagedList(pageNumber, pageSize));


        }

        // GET: People/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PersonId,FirstName,LastName,DateOfBirth,StatusId,DepartmentId,CompanyId,EmployeeNumber,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1");
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1");
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PersonId,FirstName,LastName,DateOfBirth,StatusId,DepartmentId,CompanyId,EmployeeNumber,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1");

            return View(person);
        }

        // GET: People/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Person person = await db.People.FindAsync(id);
            db.People.Remove(person);
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

/*
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HR_System.Data;
using HR_System.Web.Data;

namespace HR_System.Web
{
    public class PeopleController : Controller
    {
        private HR_SystemWebContext db = new HR_SystemWebContext();

        // GET: People
        public async Task<ActionResult> Index()
        {
            var people = db.People.Include(p => p.Company).Include(p => p.Department).Include(p => p.Status);
            return View(await people.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: People/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1");
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PersonId,FirstName,LastName,DateOfBirth,StatusId,DepartmentId,CompanyId,EmployeeNumber,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1", person.StatusId);
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1", person.StatusId);
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PersonId,FirstName,LastName,DateOfBirth,StatusId,DepartmentId,CompanyId,EmployeeNumber,Email")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", person.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", person.DepartmentId);
            ViewBag.StatusId = new SelectList(db.Status, "StatusId", "Status1", person.StatusId);
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = await db.People.FindAsync(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Person person = await db.People.FindAsync(id);
            db.People.Remove(person);
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
*/