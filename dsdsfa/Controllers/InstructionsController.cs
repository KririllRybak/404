using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using dsdsfa.Data;
using dsdsfa.Models;
using Microsoft.AspNetCore.Identity;

namespace dsdsfa.Controllers
{
    public class InstructionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Instructions
        public async Task<IActionResult> Index()
        {

            var applicationDbContext = _context.Instructions.Include(i => i.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Instructions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instructions
                .Include(i => i.ApplicationUser)
                .SingleOrDefaultAsync(m => m.InstructionId == id);
            if (instruction == null)
            {
                return NotFound();
            }

            return View(instruction);
        }

        // GET: Instructions/Create
        public IActionResult Create()
        {
            SelectList category = new SelectList(_context.Categories, "CategoryId", "NameCategory");
            ViewBag.Categories = category;
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Instructions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InstructionId,ApplicationUserId,NameInstruction,Decription,Img")] Instruction instruction)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _userManager.GetUserAsync(HttpContext.User).Result;
                instruction.ApplicationUser = currentUser;
                _context.Add(instruction);
                await _context.SaveChangesAsync();
                TempData["Instruction"] = instruction.InstructionId.ToString();
                return RedirectToAction("AddStep");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", instruction.ApplicationUserId);
            return View("AddStep");
        }

        [HttpPost]
        public IActionResult SaveChanges(InstructionStep instStep)
        {
            var insrtcId = Convert.ToInt32((string)TempData["Instruction"]);
            var instruction = _context.Instructions.Include(i => i.instructionSteps).Single(i => i.InstructionId == insrtcId);
            instruction.instructionSteps.Add(instStep);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult AddStep()
        {
            return View("AddStep");
        }

        [HttpPost]
        public IActionResult AddStep(InstructionStep instStep)
        {
            var insrtcId = Convert.ToInt32((string)TempData["Instruction"]);
            var instruction = _context.Instructions.Include(i => i.instructionSteps).Single(i => i.InstructionId == insrtcId);
            instruction.instructionSteps.Add(instStep);
            _context.SaveChanges();
            return RedirectToAction("AddStep");
        }

        // GET: Instructions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instructions.SingleOrDefaultAsync(m => m.InstructionId == id);
            if (instruction == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", instruction.ApplicationUserId);
            return View(instruction);
        }

        // POST: Instructions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InstructionId,ApplicationUserId,NameInstruction,Decription,Img")] Instruction instruction)
        {
            if (id != instruction.InstructionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(instruction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InstructionExists(instruction.InstructionId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", instruction.ApplicationUserId);
            return View(instruction);
        }

        // GET: Instructions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instruction = await _context.Instructions
                .Include(i => i.ApplicationUser)
                .SingleOrDefaultAsync(m => m.InstructionId == id);
            if (instruction == null)
            {
                return NotFound();
            }

            return View(instruction);
        }

        // POST: Instructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instruction = await _context.Instructions.SingleOrDefaultAsync(m => m.InstructionId == id);
            _context.Instructions.Remove(instruction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructionExists(int id)
        {
            return _context.Instructions.Any(e => e.InstructionId == id);
        }
    }
}
