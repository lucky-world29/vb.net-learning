using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using vCore_MVC.Models;

namespace vCore_MVC.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly AppDbContext _context;
        private int? GetTenantId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type.Contains("Tenant"));

            if (claim == null)
                return null;

            if (int.TryParse(claim.Value, out int id))
                return id;

            return null;
        }
        private string HashPasswordShort(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes).Substring(0, 25);
        }
        public AdministrationController(AppDbContext context)
        {
            _context = context;
        }

        //====================================== /Administration/UserRole ======================================

        #region Administration/UserRole
        public async Task<IActionResult> UserRole()
        {
            //var roles = await _context.CoreRoles
            //    .Where(r => r.IsDeleted == 0)   // optional but recommended
            //    .ToListAsync();
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var roles = await _context.CoreRoles
                .Where(r => r.IsDeleted == 0 && r.TenantId == tenantId)
                .ToListAsync();

            return View(roles);
            //    var roles = await _context.CoreRoles
            //.Select(r => new
            //{
            //    r.Id,
            //    r.RoleName,
            //    r.Description,
            //    r.Status,
            //    r.CreatedBy,
            //    r.IsDeleted
            //})
            //.ToListAsync();

            //    return Json(roles);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName, string roleDesc, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleName) || string.IsNullOrWhiteSpace(roleDesc))
                    return Json(new { success = false, message = "All fields are required" });

                roleName = roleName.Trim().ToUpper();

                // ✅ GET TENANT ONLY ONCE
                if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
                {
                    await HttpContext.SignOutAsync("MyCookieAuth");
                    return RedirectToAction("Login", "Home");
                }

                // ✅ DUPLICATE CHECK (TENANT SAFE)
                if (await _context.CoreRoles.AnyAsync(r =>
                        r.RoleName == roleName &&
                        r.IsDeleted == 0 &&
                        r.TenantId == tenantId))
                {
                    return Json(new { success = false, message = "Role already exists" });
                }

                // ✅ CREATE ROLE
                var role = new CoreRole
                {
                    RoleName = roleName,
                    Description = roleDesc.Trim(),
                    Status = status == "Active" ? 1.0f : 0.0f,
                    CreatedBy = 1,
                    CreatedDateTime = DateTime.Now,
                    IsDeleted = 0,
                    TenantId = tenantId
                };

                _context.CoreRoles.Add(role);
                await _context.SaveChangesAsync();

                return Json(new { success = true, id = role.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateRole(int id, string roleName, string roleDesc, string status)
        {
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var role = await _context.CoreRoles
                .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);

            if (role == null)
                return Json(new { success = false });

            roleName = roleName.Trim().ToUpper();

            // ✅ ADD THIS CHECK
            if (await _context.CoreRoles.AnyAsync(r =>
                    r.RoleName == roleName &&
                    r.Id != id &&
                    r.IsDeleted == 0 &&
                    r.TenantId == tenantId))
            {
                return Json(new { success = false, message = "Role already exists" });
            }

            role.RoleName = roleName;
            role.Description = roleDesc.Trim();
            role.Status = status == "Active" ? 1.0f : 0.0f;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(int id)
        {
            //var role = await _context.CoreRoles.FindAsync(id);
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var role = await _context.CoreRoles
                .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);
            if (role == null)
                return Json(new { success = false });

            role.IsDeleted = 1;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> BulkUpdateStatus(List<int> ids, string status)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "No roles selected" });

            // ✅ Tenant check (MANDATORY)
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            // ✅ Fetch only valid roles (tenant + not deleted)
            var roles = await _context.CoreRoles
                .Where(r => ids.Contains(r.Id) && r.TenantId == tenantId && r.IsDeleted == 0)
                .ToListAsync();

            if (!roles.Any())
                return Json(new { success = false, message = "No valid roles found" });

            float newStatus = status == "Active" ? 1.0f : 0.0f;

            foreach (var role in roles)
            {
                role.Status = newStatus;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        #endregion

        //====================================== /Administration/UserOrganisation ======================================

        #region Administration/UserOrganisation

        //GET — User Organisation Page (FILTER BY TENANT)
        [Authorize]
        public async Task<IActionResult> UserOrganisation()
        {
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var data = await _context.CoreTenantOrganisations
                .Where(x => x.TenantId == tenantId && x.IsDeleted == 0.0f)
                .ToListAsync();

            return View(data);
        }

        // REQUIRED for: view modal  edit modal  & AJAX population
        [HttpGet]
        public async Task<IActionResult> GetOrganisationDetails(int id)
        {
            var org = await _context.CoreTenantOrganisations
                .FirstOrDefaultAsync(x => x.TenantId == id);

            if (org == null)
                return Json(null);

            return Json(org);
        }

        //UPDATE
        [HttpPost]
        public async Task<IActionResult> UpdateOrganisation(CoreTenantOrganisation model, IFormFile? logoFile)
        {
            if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
            {
                return Json(new { success = false });
            }

            var existing = await _context.CoreTenantOrganisations
                .FirstOrDefaultAsync(x =>
                    x.TenantId == model.TenantId &&
                    x.TenantId == tenantId);

            if (existing == null)
                return Json(new { success = false });

            existing.OrganisationName = model.OrganisationName;
            existing.ShortCode = model.ShortCode;
            existing.Email = model.Email;
            existing.Website = model.Website;
            existing.Phone = model.Phone;
            existing.Fax = model.Fax;
            existing.AttentionTo = model.AttentionTo;
            existing.Address = model.Address;
            existing.City = model.City;
            existing.State = model.State;
            existing.Country = model.Country;
            existing.Zipcode = model.Zipcode;
            existing.Currency = model.Currency;
            existing.ROC = model.ROC;
            existing.TIN = model.TIN;
            existing.SalesTaxNo = model.SalesTaxNo;
            existing.ServiceTaxNo = model.ServiceTaxNo;
            existing.ReportFooter = model.ReportFooter;
            existing.Remark = model.Remark;
            existing.DetentionDays = model.DetentionDays;
            existing.IncentiveStartDate = model.IncentiveStartDate;
            existing.IncentiveEndDate = model.IncentiveEndDate;
            existing.Status = model.Status;

            // 🔥 IMAGE SAVE
            if (logoFile != null && logoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await logoFile.CopyToAsync(ms);
                existing.ORGImage = ms.ToArray();
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET IMAGE (for preview)
        [HttpGet]
        public async Task<IActionResult> GetOrgImage(int id)
        {
            var org = await _context.CoreTenantOrganisations
                .FirstOrDefaultAsync(x => x.TenantId == id);

            if (org?.ORGImage == null)
                return NotFound();

            return File(org.ORGImage, "image/png");
        }

        // [HttpPost]
        //public async Task<IActionResult> DeleteOrganisation(int tenantId)
        //{
        //    var org = await _context.CoreTenantOrganisations
        //        .FirstOrDefaultAsync(x => x.TenantId == tenantId);

        //    if (org == null)
        //        return Json(new { success = false });

        //    org.IsDeleted = 1;

        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true });
        //}

        #endregion

        //====================================== /Administration/Users======================================

        #region Administration/Users

        //GET USERS (WITH ROLE NAME)
        [Authorize]
        public async Task<IActionResult> Users()
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var users = await _context.CoreUsers
                   .Where(u => u.TenantId == tenantId && u.IsDeleted == 0)
                   .Include(u => u.Role)   // 🔥 ADD THIS LINE
                   .ToListAsync();
            //.Select(u => new UserVM
            //{
            //    Id = u.Id,
            //    FirstName = u.FirstName,
            //    LastName = u.LastName,
            //    UserName = u.UserName,
            //    RoleName = u.Role.RoleName,
            //    Status = u.Status
            //})
            //.ToListAsync();

            // For Fetching Roles in Create/Edit Modal Dropdown
            ViewBag.Roles = await _context.CoreRoles
                .Where(r => r.TenantId == tenantId && r.IsDeleted == 0)
                .ToListAsync();

            return View(users);
        }

        // CREATE USER
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CoreUser model)
        {
            try
            {
                var tenantId = GetTenantId();

                if (tenantId == null)
                {
                    await HttpContext.SignOutAsync("MyCookieAuth");
                    return RedirectToAction("Login", "Home");
                }

                // ✅ Validation
                if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
                    return Json(new { success = false, message = "Required fields missing" });

                if (model.RoleId == null || model.RoleId == 0)
                    return Json(new { success = false, message = "Role is required" });

                // ✅ Duplicate check
                if (await _context.CoreUsers.AnyAsync(u =>
                        u.UserName == model.UserName &&
                        u.TenantId == tenantId.Value &&
                        u.IsDeleted == 0))
                {
                    return Json(new { success = false, message = "Username already exists" });
                }

                // ✅ Hash password
                model.Password = HashPasswordShort(model.Password);

                // ✅ REQUIRED DEFAULTS (🔥 IMPORTANT FIX)
                model.TenantId = tenantId.Value;
                model.IsDeleted = 0;
                model.Status = model.Status == 0 ? (byte)0 : (byte)1;

                model.LockedAttempts = 0;
                model.Locked = 0;
                model.FLocked = 0;
                model.OTPLoked = 0;
                model.DeadLocked = 0;

                _context.CoreUsers.Add(model);
                await _context.SaveChangesAsync();

                foreach (var c in User.Claims)
                {
                    Console.WriteLine($"TYPE: {c.Type} | VALUE: {c.Value}");
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        //public async Task<IActionResult> CreateUser(CoreUser model)
        //{
        //    if (!int.TryParse(User.FindFirst("TenantId")?.Value, out int tenantId))
        //        return Json(new { success = false });

        //    // ✅ Validation
        //    if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.Password))
        //        return Json(new { success = false, message = "Required fields missing" });

        //    // ✅ Duplicate check
        //    if (await _context.CoreUsers.AnyAsync(u =>
        //            u.UserName == model.UserName &&
        //            u.TenantId == tenantId &&
        //            u.IsDeleted == 0))
        //    {
        //        return Json(new { success = false, message = "Username already exists" });
        //    }

        //    // ✅ Hash password (IMPORTANT)
        //    model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

        //    model.TenantId = tenantId;
        //    //model.CreatedDateTime = DateTime.Now;
        //    model.IsDeleted = 0;

        //    _context.CoreUsers.Add(model);
        //    await _context.SaveChangesAsync();

        //    return Json(new { success = true });
        //}

        // Update User
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(CoreUser model)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }
            //return Json(new { success = false });

            var user = await _context.CoreUsers
                .FirstOrDefaultAsync(u => u.Id == model.Id && u.TenantId == tenantId && u.IsDeleted == 0);

            if (user == null)
                return Json(new { success = false });

            model.UserName = model.UserName.Trim();

            if (await _context.CoreUsers.AnyAsync(u =>
                    u.UserName == model.UserName &&
                    u.Id != model.Id &&
                    u.TenantId == tenantId &&
                    u.IsDeleted == 0))
            {
                return Json(new { success = false, message = "Username already exists" });
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = HashPasswordShort(model.Password);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            user.RoleId = model.RoleId;
            user.Status = model.Status;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // BULK UPDATE STATUS
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> BulkUpdateUserStatus(List<int> ids, int status)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, message = "No users selected" });

            var tenantId = GetTenantId();

            if (tenantId == null)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var users = await _context.CoreUsers
                .Where(u => ids.Contains(u.Id) && u.TenantId == tenantId && u.IsDeleted == 0)
                .ToListAsync();

            if (!users.Any())
                return Json(new { success = false, message = "No valid users found" });

            foreach (var user in users)
            {
                user.Status = (byte)status;
                // optional:
                // user.ModifiedDateTime = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // DELETE USER (Soft)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.CoreUsers
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);

            if (user == null)
                return Json(new { success = false });

            user.IsDeleted = 1;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        //  Full User Details (for Edit Modal) - WITH ROLE NAME AND ORGANISATION
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var tenantId = GetTenantId();

            if (tenantId == null)
            {
                await HttpContext.SignOutAsync("MyCookieAuth");
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.CoreUsers
                .Include(u => u.Role)
                .Where(u => u.Id == id && u.TenantId == tenantId && u.IsDeleted == 0)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.UserName,
                    u.Email,
                    u.Status,

                    RoleName = u.Role != null ? u.Role.RoleName : "",
                    RoleId = u.RoleId,

                    // 🔥 Organisation from CoreTenantOrganisations
                    Organisation = _context.CoreTenantOrganisations
                        .Where(o => o.TenantId == u.TenantId)
                        .Select(o => o.OrganisationName)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return Json(user);
        }


        #endregion


        
    }
}