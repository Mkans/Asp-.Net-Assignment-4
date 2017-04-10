using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKBusService.Models;
using System.Text.RegularExpressions;

namespace MKBusService.Controllers
{
    public class MKDriverController : Controller
    {
        private readonly BusServiceContext _context;

        public MKDriverController(BusServiceContext context)
        {
            _context = context;    
        }

        // GET: MKDriver
        public async Task<IActionResult> Index()
        {
            var busServiceContext = _context.Driver.Include(d => d.ProvinceCodeNavigation);
            return View(await busServiceContext.ToListAsync());
        }
        //Method no:1 
        public JsonResult ProvinceCodeValidation(String provinceCode)
        {
            Regex pattern = new Regex(@"[a-z][a-z]", RegexOptions.IgnoreCase);
            if (pattern.IsMatch(provinceCode) && provinceCode.Length <= 2)
            {
                try
                {
                    var select = _context.Province.Where(x => x.ProvinceCode == provinceCode);

                    if (select.Any())
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json("not a province");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"error  {ex.GetBaseException().Message}");
                }
            }
            return Json("Input too long/short");
        }

        // GET: MKDriver/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: MKDriver/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: MKDriver/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Driver " + driver.LastName + " created";
                return RedirectToAction("Index");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            return View(driver);
        }

        // GET: MKDriver/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            return View(driver);
        }

        // POST: MKDriver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (id != driver.DriverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.DriverId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"error  {ex.GetBaseException().Message}");
                }
                return RedirectToAction("Index");
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            return View(driver);
        }

        // GET: MKDriver/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: MKDriver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
                _context.Driver.Remove(driver);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            
            return RedirectToAction("Index");
        }

        private bool DriverExists(int id)
        {
            return _context.Driver.Any(e => e.DriverId == id);
        }
    }
}
