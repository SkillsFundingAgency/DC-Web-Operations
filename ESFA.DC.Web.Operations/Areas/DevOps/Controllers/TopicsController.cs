﻿using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Topics.Data;
using ESFA.DC.Web.Operations.Topics.Data.Entities;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Controllers
{
    [Area(AreaNames.DevOps)]
    public class TopicsController : BaseControllerWithDevOpsPolicy
    {
        private readonly JobQueueDataContext _context;
        private readonly TelemetryClient _telemetryClient;

        public TopicsController(JobQueueDataContext context, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _context = context;
            _telemetryClient = telemetryClient;
        }

        // GET: JobTopicSubscriptions
        public async Task<IActionResult> Index(string searchString)
        {
            var jobQueueDataContext = _context.JobTopicSubscription.Include(j => j.Collection)
                .Where(x => string.IsNullOrEmpty(searchString) || x.Collection.Name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Collection.Name).ThenBy(x => x.TopicOrder);

            ViewData["currentFilter"] = searchString;
            return View(await jobQueueDataContext.ToListAsync());
        }

        // GET: JobTopicSubscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _telemetryClient.TrackEvent($"Edit Task by {User.Identity.Name}");
            var jobTopicSubscription = await _context.JobTopicSubscription
                .Include(j => j.Collection)
                .FirstOrDefaultAsync(m => m.JobTopicId == id);
            if (jobTopicSubscription == null)
            {
                return NotFound();
            }

            return View(jobTopicSubscription);
        }

        // GET: JobTopicSubscriptions/Create
        public IActionResult Create()
        {
            ViewData["CollectionId"] = new SelectList(_context.Collection, "CollectionId", "Name");
            return View();
        }

        // POST: JobTopicSubscriptions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobTopicId,CollectionId,TopicName,SubscriptionName,TopicOrder,IsFirstStage,Enabled")] JobTopicSubscription jobTopicSubscription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobTopicSubscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CollectionId"] = new SelectList(_context.Collection, "CollectionId", "Name", jobTopicSubscription.CollectionId);
            return View(jobTopicSubscription);
        }

        // GET: JobTopicSubscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobTopicSubscription = await _context.JobTopicSubscription.FindAsync(id);
            if (jobTopicSubscription == null)
            {
                return NotFound();
            }

            ViewData["CollectionId"] = new SelectList(_context.Collection, "CollectionId", "Name", jobTopicSubscription.CollectionId);
            return View(jobTopicSubscription);
        }

        // POST: JobTopicSubscriptions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobTopicId,CollectionId,TopicName,SubscriptionName,TopicOrder,IsFirstStage,Enabled")] JobTopicSubscription jobTopicSubscription)
        {
            if (id != jobTopicSubscription.JobTopicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobTopicSubscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobTopicSubscriptionExists(jobTopicSubscription.JobTopicId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CollectionId"] = new SelectList(_context.Collection, "CollectionId", "Name", jobTopicSubscription.CollectionId);
            return View(jobTopicSubscription);
        }

        // GET: JobTopicSubscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobTopicSubscription = await _context.JobTopicSubscription
                .Include(j => j.Collection)
                .FirstOrDefaultAsync(m => m.JobTopicId == id);
            if (jobTopicSubscription == null)
            {
                return NotFound();
            }

            return View(jobTopicSubscription);
        }

        // POST: JobTopicSubscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobTopicSubscription = await _context.JobTopicSubscription.FindAsync(id);
            _context.JobTopicSubscription.Remove(jobTopicSubscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobTopicSubscriptionExists(int id)
        {
            return _context.JobTopicSubscription.Any(e => e.JobTopicId == id);
        }
    }
}
