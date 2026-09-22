using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using vCore_MVC.Models;

namespace vCore_MVC.Controllers
{
    public class OrganisationMasterController : Controller
    {
        private readonly AppDbContext _context;

        public OrganisationMasterController(AppDbContext context)
        {
            _context = context;
        }
        #region Branch

        // =========================
        // BRANCH LIST
        // =========================
        public async Task<IActionResult> Branch()
        {
            int tenantId = Convert.ToInt32(
                User.FindFirst("TenantId")?.Value
            );

            var branches = await _context.CoreBranches
                .Where(x => x.Status == 1
                         && x.IsDeleted == 0
                         && x.TenantId == tenantId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(branches);
        }

        // =========================
        // UPDATE BRANCH STATUS
        // =========================
        [HttpPost]
        public async Task<IActionResult> UpdateBranchStatus(long id, int status)
        {
            var branch = await _context.CoreBranches
                .FirstOrDefaultAsync(x => x.Id == id);

            if (branch == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            branch.Status = status;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        // =========================
        // DELETE BRANCH
        // =========================

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(long id)
        {
            var branch = await _context.CoreBranches
                .FirstOrDefaultAsync(x => x.Id == id);

            if (branch == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            branch.IsDeleted = 1;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }



        // =========================
        // SAVE BRANCH
        // =========================
        [HttpPost]
        public async Task<IActionResult> SaveBranch(
    [FromBody] BranchWithSubUnits model)
        {
            try
            {
                // =====================================
                // VALIDATION
                // =====================================
                if (model == null || model.Branch == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid request data."
                    });
                }

                int tenantId = Convert.ToInt32(
                    User.FindFirst("TenantId")?.Value
                );

                string username =
                    User.Identity?.Name ?? "System";

                CoreBranch branch;

                // =====================================
                // INSERT NEW BRANCH
                // =====================================
                if (model.Branch.Id == 0)
                {
                    branch = new CoreBranch
                    {
                        Name = model.Branch.Name,

                        ShortCode = model.Branch.ShortCode,

                        Status = 1,

                        IsDeleted = 0,

                        TenantId = tenantId,

                        CreatedBy = username,

                        CreationDate = DateTime.Now
                    };

                    _context.CoreBranches.Add(branch);

                    await _context.SaveChangesAsync();

                    // =====================================
                    // SAVE SUB UNITS FOR NEW BRANCH
                    // =====================================
                    if (model.SubUnits != null &&
                        model.SubUnits.Any())
                    {
                        foreach (var item in model.SubUnits)
                        {
                            // Skip empty rows
                            if (string.IsNullOrWhiteSpace(item.Name) &&
                                string.IsNullOrWhiteSpace(item.ShortCode))
                            {
                                continue;
                            }

                            item.Id = 0;

                            item.BranchId = branch.Id;

                            item.TenantId = tenantId;

                            item.Status = 1;

                            item.IsDeleted = 0;

                            item.CreatedBy = username;

                            item.CreationDate = DateTime.Now;

                            _context.CoreBranchSubUnits.Add(item);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                // =====================================
                // UPDATE EXISTING BRANCH
                // =====================================
                else
                {
                    branch = await _context.CoreBranches
                        .FirstOrDefaultAsync(x =>
                            x.Id == model.Branch.Id);

                    if (branch == null)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "Branch not found."
                        });
                    }

                    // =====================================
                    // UPDATE MAIN BRANCH
                    // =====================================
                    branch.Name = model.Branch.Name;

                    branch.ShortCode = model.Branch.ShortCode;

                    branch.ModifiedBy = username;

                    branch.ModifiedDate = DateTime.Now;

                    await _context.SaveChangesAsync();

                    // =====================================
                    // ADD NEW SUB UNITS ONLY
                    // =====================================
                    if (model.SubUnits != null &&
                        model.SubUnits.Any())
                    {
                        foreach (var item in model.SubUnits)
                        {
                            // Skip empty rows
                            if (string.IsNullOrWhiteSpace(item.Name) &&
                                string.IsNullOrWhiteSpace(item.ShortCode))
                            {
                                continue;
                            }

                            // Prevent duplicate subunits
                            bool alreadyExists = await _context
                                .CoreBranchSubUnits
                                .AnyAsync(x =>
                                    x.BranchId == branch.Id &&
                                    x.TenantId == tenantId &&
                                    x.Name == item.Name &&
                                    x.IsDeleted == 0);

                            if (alreadyExists)
                            {
                                continue;
                            }

                            item.Id = 0;

                            item.BranchId = branch.Id;

                            item.TenantId = tenantId;

                            item.Status = 1;

                            item.IsDeleted = 0;

                            item.CreatedBy = username;

                            item.CreationDate = DateTime.Now;

                            _context.CoreBranchSubUnits.Add(item);
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                return Json(new
                {
                    success = true,
                    message = "Branch saved successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        #endregion


        #region SubBranch

        public async Task<IActionResult> BranchSubUnit()
        {
            int tenantId = Convert.ToInt32(
                User.FindFirst("TenantId")?.Value
            );

            ViewBag.Branches = await _context.CoreBranches
                .Where(x =>
                    x.Status == 1 &&
                    x.IsDeleted == 0 &&
                    x.TenantId == tenantId)
                .OrderBy(x => x.Name)
                .ToListAsync();

            var data = await (
                from s in _context.CoreBranchSubUnits
                join b in _context.CoreBranches
                    on s.BranchId equals b.Id
                where s.Status == 1
                   && s.IsDeleted == 0
                   && s.TenantId == tenantId
                orderby s.Id descending
                select new BranchSubUnitVM
                {
                    Id = s.Id,

                    Name = s.Name,

                    ShortCode = s.ShortCode,

                    BranchId = s.BranchId,

                    BranchName = b.Name,

                    Status = s.Status
                }
            ).ToListAsync();

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBranchSubUnit(
    CoreBranchSubUnit model)
        {
            try
            {
                int tenantId = Convert.ToInt32(
                    User.FindFirst("TenantId")?.Value
                );

                string username =
                    User.Identity?.Name ?? "System";

                // =========================
                // INSERT
                // =========================
                if (model.Id == 0)
                {
                    model.Status = 1;

                    model.IsDeleted = 0;

                    model.TenantId = tenantId;

                    model.CreatedBy = username;

                    model.CreationDate = DateTime.Now;

                    _context.CoreBranchSubUnits.Add(model);
                }

                // =========================
                // UPDATE
                // =========================
                else
                {
                    var data = await _context
                        .CoreBranchSubUnits
                        .FirstOrDefaultAsync(x =>
                            x.Id == model.Id);

                    if (data == null)
                    {
                        return Json(new
                        {
                            success = false
                        });
                    }

                    data.Name = model.Name;

                    data.ShortCode = model.ShortCode;

                    data.BranchId = model.BranchId;

                    data.ModifiedBy = username;

                    data.ModifiedDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBranchSubUnit(long id)
        {
            var data = await _context
                .CoreBranchSubUnits
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return Json(new
                {
                    success = false
                });
            }

            data.IsDeleted = 1;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true
            });
        }

        #endregion
    }
}