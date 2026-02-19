using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Solvo.Data;
using Solvo.Models;
using System.Security.Claims;

namespace Solvo.Controllers
{
    [Authorize]
    public class SavingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SavingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goals = await _context.SavingsGoals
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(goals);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ✅ FIXED CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SavingsGoal goal)
        {
            // Assign server values BEFORE validation
            goal.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            goal.CreatedAt = DateTime.UtcNow;
            goal.UpdatedAt = DateTime.UtcNow;

            // Remove validation error for UserId
            ModelState.Remove(nameof(SavingsGoal.UserId));

            if (ModelState.IsValid)
            {
                _context.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(goal);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (goal == null) return NotFound();
            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SavingsGoal goal)
        {
            if (id != goal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                goal.UserId = userId!;
                goal.UpdatedAt = DateTime.UtcNow;

                _context.Update(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (goal != null)
            {
                _context.SavingsGoals.Remove(goal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCurrent(int id, decimal currentAmount)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var goal = await _context.SavingsGoals
                .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (goal != null)
            {
                goal.CurrentAmount = currentAmount;
                goal.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}
