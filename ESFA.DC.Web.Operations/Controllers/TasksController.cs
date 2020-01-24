using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Topics.Data;
using ESFA.DC.Web.Operations.Topics.Data.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Controllers
{
    public class TasksController : BaseControllerWithDevOpsPolicy
    {
        private readonly JobQueueDataContext _context;

        public TasksController(JobQueueDataContext context, ILogger logger, TelemetryClient telemetryClient)
            : base(logger, telemetryClient)
        {
            _context = context;
        }

        // GET: JobSubscriptionTasks
        public async Task<IActionResult> Index(string searchString)
        {
            var jobQueueDataContext = _context.JobSubscriptionTask.Include(j => j.JobTopic)
                .ThenInclude(x => x.Collection)
                .Where(x => string.IsNullOrEmpty(searchString) || x.JobTopic.Collection.Name.StartsWith(searchString, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.JobTopic.Collection.Name)
                .ThenBy(x => x.JobTopic.IsFirstStage.GetValueOrDefault())
                .ThenBy(x => x.JobTopic.TopicOrder)
                .ThenBy(x => x.TaskOrder);

            ViewData["currentFilter"] = searchString;
            return View(await jobQueueDataContext.ToListAsync());
        }

        // GET: JobSubscriptionTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobSubscriptionTask = await _context.JobSubscriptionTask
                .Include(j => j.JobTopic)
                .FirstOrDefaultAsync(m => m.JobTopicTaskId == id);
            if (jobSubscriptionTask == null)
            {
                return NotFound();
            }

            return View(jobSubscriptionTask);
        }

        // GET: JobSubscriptionTasks/Create
        public IActionResult Create()
        {
            ViewData["JobTopicId"] = new SelectList(_context.JobTopicSubscription, "JobTopicId", "SubscriptionName");
            return View();
        }

        // POST: JobSubscriptionTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobTopicTaskId,JobTopicId,TaskName,TaskOrder,Enabled")] JobSubscriptionTask jobSubscriptionTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobSubscriptionTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["JobTopicId"] = new SelectList(_context.JobTopicSubscription, "JobTopicId", "SubscriptionName", jobSubscriptionTask.JobTopicId);
            return View(jobSubscriptionTask);
        }

        // GET: JobSubscriptionTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobSubscriptionTask = await _context.JobSubscriptionTask.Include(x => x.JobTopic).ThenInclude(y => y.Collection).SingleOrDefaultAsync(x => x.JobTopicTaskId == id);
            if (jobSubscriptionTask == null)
            {
                return NotFound();
            }

            ViewData["JobTopicId"] = new SelectList(_context.JobTopicSubscription.Where(x => x.JobTopicId == jobSubscriptionTask.JobTopicId), "JobTopicId", "SubscriptionName", jobSubscriptionTask.JobTopicId);
            return View(jobSubscriptionTask);
        }

        // POST: JobSubscriptionTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobTopicTaskId,JobTopicId,TaskName,TaskOrder,Enabled")] JobSubscriptionTask jobSubscriptionTask)
        {
            if (id != jobSubscriptionTask.JobTopicTaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobSubscriptionTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobSubscriptionTaskExists(jobSubscriptionTask.JobTopicTaskId))
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

            ViewData["JobTopicId"] = new SelectList(_context.JobTopicSubscription, "JobTopicId", "SubscriptionName", jobSubscriptionTask.JobTopicId);
            return View(jobSubscriptionTask);
        }

        // GET: JobSubscriptionTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobSubscriptionTask = await _context.JobSubscriptionTask
                .Include(j => j.JobTopic)
                .FirstOrDefaultAsync(m => m.JobTopicTaskId == id);
            if (jobSubscriptionTask == null)
            {
                return NotFound();
            }

            return View(jobSubscriptionTask);
        }

        // POST: JobSubscriptionTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobSubscriptionTask = await _context.JobSubscriptionTask.FindAsync(id);
            _context.JobSubscriptionTask.Remove(jobSubscriptionTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobSubscriptionTaskExists(int id)
        {
            return _context.JobSubscriptionTask.Any(e => e.JobTopicTaskId == id);
        }
    }
}
