using Microsoft.AspNetCore.Mvc;
using vCore_MVC.Models;


namespace vCore_MVC.Controllers
{
    public class RotController : Controller
    {

        private readonly AppDbContext _context;

        public RotController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Create()
        {
            ViewBag.CurrentDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

            return View();
        }
        [HttpPost]
        public IActionResult Create(Rot model, List<int> SelectedSurcharges)
        {
            if (SelectedSurcharges != null)
            {
                foreach (var id in SelectedSurcharges)
                {
                    // Save mapping here
                }
            }

            return RedirectToAction("Create"); // ✅ important
        }

        // 1st one for GET CoreCommodity data for autocomplete
        [HttpGet]
        public JsonResult SearchCommodity(string term)
        {
            var data = _context.CoreCommodities
                .Where(c => c.Name != null &&
                            (string.IsNullOrEmpty(term) || c.Name.Contains(term)))
                .Select(c => new
                {
                    id = c.Id,
                    label = c.Name
                })
                .Take(10)
                .ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetSurchargeList()
        {
            var count = _context.CoreSurchargeMasters.Count();
            var data = _context.CoreSurchargeMasters
                .Select(s => new
                {
                    id = s.Id,
                    text = s.ShortCode + " - " + s.Description
                })
                .ToList();

            return Json(data);
        }


        //Service Type  
        [HttpGet]
        public JsonResult GetServiceTypes()
        {
            var data = _context.CoreServiceTypes
                .Where(x => x.Name != null)
                .Select(x => new
                {
                    value = x.Name,   // or x.Id if needed later
                    text = x.Name
                })
                .Distinct()
                .ToList();

            return Json(data);
        }
    }
}
