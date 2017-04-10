using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKBusService.Models;
// Muthukumar Kandasamy
// 7723794
namespace MKBusService.Controllers
{
    public class MKBusRouteController : Controller
    {
        private readonly BusServiceContext _context;

        public MKBusRouteController(BusServiceContext context)
        {
            _context = context;    
        }

        // The index function will be the default view option for the controller unless
        // certain method is specified
        public async Task<IActionResult> Index()
        {
            //Added a function to sort the busRouteCode if the code is of length 1, (0 to 9) a zero (0) will be added in the front to sort it 
            //return View(await _context.BusRoute.OrderBy(a => a.BusRouteCode).ToListAsync());
            TempData["message"] = "From the Bus Route controller. Holla Amigo !!!";
            return View(await _context.BusRoute.OrderBy(a => (a.BusRouteCode.Length == 1) ? "0" + a.BusRouteCode : a.BusRouteCode).ToListAsync());
        }

        //details method will fetch all the columns with matching id that is busRouteCode
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        // the create button in the create window, will take us back to the main window,
        // where the recend busStop will be displayed
        public IActionResult Create()
        {
            return View();
        }

        //The Create method, to create new BusRouteCode and RouteName and will return the busRoute VIEW
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(busRoute);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // Edit method to edit the data matching the id, that is busRouteCode
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }
            return View(busRoute);
        }

        //This edit methid actually checks the busRouteCode is in the table or not
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (id != busRoute.BusRouteCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusRouteExists(busRoute.BusRouteCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // Initially checks whether the data is in the table and retun notFound() function if not.
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        //Method to delete the record from the table and redirect the action to the Index method
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            _context.BusRoute.Remove(busRoute);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //return the BusRouteCode for the ID
        private bool BusRouteExists(string id)
        {
            return _context.BusRoute.Any(e => e.BusRouteCode == id);
        }
    }
}
