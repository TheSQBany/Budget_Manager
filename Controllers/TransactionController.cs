using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Budget_Manager.Models;

namespace Budget_Manager.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/CreateOrEdit
        public IActionResult CreateOrEdit(int id=0)
        {
            PopulateCategories();
            if(id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));

        }

        // POST: Transaction/CreateOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrEdit([Bind("TransactionID,CategoryID,Amount,Date,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionID == 0)
                    _context.Add(transaction);
                else
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }


        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryID = 0, Title = "- wybierz kategorię -" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}
