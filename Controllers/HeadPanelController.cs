using Budget_Manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Budget_Manager.Controllers
{
    public class HeadPanelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HeadPanelController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            //Last 30 Days
            DateTime StartDate = DateTime.Today.AddDays(-29);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date>= StartDate && y.Date<= EndDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Przychody")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Total Expense
            int TotalExpense  = SelectedTransactions
                .Where(i => i.Category.Type == "Wydatki")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pl-PLN");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

            //Donut Chart - Expense by Category
            ViewBag.DonutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Wydatki")
                .GroupBy(j => j.Category.CategoryID)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l=>l.amount)
                .ToList();

            //Spline Chart - Incomes and Expenses
            //Income
            List<SplineChartData> IncomesSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Przychody")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    income = k.Sum(l => l.Amount)
                })
                .ToList();

            //Expense
            List<SplineChartData> ExpensesSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Wydatki")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    expense = k.Sum(l => l.Amount)
                })
                .ToList();

            //Combine Incomes and Expenses
            string[] Last30Days = Enumerable.Range(0,30)
                .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            // Using left join
            ViewBag.SplineChartData = from day in Last30Days
                                      join income in IncomesSummary on day equals income.day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in ExpensesSummary on day equals expense.day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,  // day = day from Last30Days
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense,
                                      };

            // Recent Transactions
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(5)
                .ToListAsync();

            // Recent Categories
            ViewBag.RecentCategories = await _context.Categories
                .OrderByDescending(i => i.CategoryID)
                .Take(5)
                .ToListAsync();

            return View();

        }
    }

    public class SplineChartData
    {
        public string day;
        public int income;
        public int expense;
    }
    
}
