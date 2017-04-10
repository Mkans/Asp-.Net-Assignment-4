using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MKBusService.Models;
using Microsoft.AspNetCore.Http;

namespace MKBusService.Controllers
{
    public class MKRouteScheduleController : Controller
    {
        private readonly BusServiceContext _context;

        public MKRouteScheduleController(BusServiceContext context)
        {
            _context = context;    
        }

        // GET: MKRouteSchedule
        public async Task<IActionResult> Index(string busRouteCode, string routeName)
        {
            if (busRouteCode != null)
            {
                HttpContext.Session.SetString("BusRouteCode", busRouteCode);
                HttpContext.Session.SetString("RouteName", routeName);
            }
            else if (HttpContext.Session.GetString("BusRouteCode") != null)
            {
                busRouteCode = HttpContext.Session.GetString("BusRouteCode");
                routeName = HttpContext.Session.GetString("RouteName");
            }
            else
            {
                TempData["message"] = "Please select a bus route to view its stops";
                return RedirectToAction(actionName: "Index", controllerName: "MKBusRoute");
            }
            var routes = _context.RouteSchedule.Where(a => a.BusRouteCode == busRouteCode);
            return View(await routes.OrderByDescending(r => r.IsWeekDay).ThenBy(r => r.StartTime).ToListAsync());
        }

        // GET: MKRouteSchedule/RouteStopsSchedule
        public async Task<IActionResult> RouteStopSchedule(string RouteStopId)
        {
            if (RouteStopId != null)
            {
                HttpContext.Session.SetString("RouteStopId", RouteStopId);
            }
            else if (HttpContext.Session.GetString("RouteStopId") != null)
            {
                RouteStopId = HttpContext.Session.GetString("RouteStopId");
            }
            else
            {
                TempData["message"] = "Please select a bus route to view its stops";
                return RedirectToAction(actionName: "Index", controllerName: "MKRouteStop");
            }
            int routeNo = int.Parse(RouteStopId);
            var busRouteCode = HttpContext.Session.GetString("BusRouteCode");
            var busServiceContext = _context.RouteSchedule.Where(a => a.BusRouteCode == busRouteCode).Include(r => r.BusRouteCodeNavigation);
            var offsetMins = from x in _context.RouteStop
                             where x.RouteStopId == int.Parse(RouteStopId)
                             select x.OffsetMinutes;
            var routes = from x in _context.RouteSchedule
                         where x.BusRouteCode == busRouteCode
                         select new RouteSchedule
                         {
                             StartTime = x.StartTime.Add(TimeSpan.FromMinutes((double)offsetMins.Single())),
                             IsWeekDay = x.IsWeekDay
                         };

            if (routes.Count() == 0)
            {
                TempData["message"] = "No schedule records";
                return RedirectToAction(actionName: "Index", controllerName: "MKRouteStop");
            }
            return View(await routes.OrderByDescending(r => r.IsWeekDay).ThenBy(r => r.StartTime).ToListAsync());
        }
        public async Task<IActionResult> RouteStopSchedule(string BusRouteCode ,string RouteName)
        {
            var routeStopId = (from p in _context.RouteStop
                              where p.BusRouteCode == BusRouteCode 
                               select p.RouteStopId).FirstOrDefault();
            var RouteStopId = routeStopId.ToString();
            if (RouteStopId != null)
            {
                HttpContext.Session.SetString("RouteStopId", RouteStopId);
            }
            else if (HttpContext.Session.GetString("RouteStopId") != null)
            {
                RouteStopId = HttpContext.Session.GetString("RouteStopId");
            }
            else
            {
                TempData["message"] = "Please select a bus route to view its stops";
                return RedirectToAction(actionName: "Index", controllerName: "MKRouteStop");
            }
            int routeNo = int.Parse(RouteStopId);
            var busRouteCode = HttpContext.Session.GetString("BusRouteCode");
            var busServiceContext = _context.RouteSchedule.Where(a => a.BusRouteCode == busRouteCode).Include(r => r.BusRouteCodeNavigation);
            var offsetMins = from x in _context.RouteStop
                             where x.RouteStopId == int.Parse(RouteStopId)
                             select x.OffsetMinutes;
            var routes = from x in _context.RouteSchedule
                         where x.BusRouteCode == busRouteCode
                         select new RouteSchedule
                         {
                             StartTime = x.StartTime.Add(TimeSpan.FromMinutes((double)offsetMins.Single())),
                             IsWeekDay = x.IsWeekDay
                         };

            if (routes.Count() == 0)
            {
                TempData["message"] = "No schedule records";
                return RedirectToAction(actionName: "Index", controllerName: "MKRouteStop");
            }
            return View(await routes.OrderByDescending(r => r.IsWeekDay).ThenBy(r => r.StartTime).ToListAsync());
        }

        // GET: MKRouteSchedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        // GET: MKRouteSchedule/Create
        public IActionResult Create()
        {
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode");
            return View();
        }

        // POST: MKRouteSchedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // GET: MKRouteSchedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // POST: MKRouteSchedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (id != routeSchedule.RouteScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteScheduleExists(routeSchedule.RouteScheduleId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // GET: MKRouteSchedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        // POST: MKRouteSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            _context.RouteSchedule.Remove(routeSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool RouteScheduleExists(int id)
        {
            return _context.RouteSchedule.Any(e => e.RouteScheduleId == id);
        }
    }
}
