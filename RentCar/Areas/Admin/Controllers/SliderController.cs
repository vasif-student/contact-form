using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentCar.Areas.Admin.Constants;
using RentCar.Areas.Admin.Models.ViewModel;
using RentCar.Areas.Admin.Utilis;
using RentCar.DAL;
using RentCar.Data;
using RentCar.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConstants.SuperAdmin)]
    public class SliderController : Controller
    {
        private readonly AppDbContext _dbContext;

        public SliderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _dbContext.Sliders.Where(s => !s.IsDeleted).ToListAsync();
            return View(sliders);
        }

        //***** Add Slider Photos*****//

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSliderPhotos(SliderCreateViewModel model)
        {
            List<Slider> sliders = new List<Slider>();

            foreach (var image in model.Files)
            {
                if (image == null)
                {
                    ModelState.AddModelError("Files", "Select an image");
                    return View();
                }
                if (!image.IsSupported())
                {
                    ModelState.AddModelError("Files", "File is unsupported");
                    return View();
                }
                if (image.IsGreaterThanGivenSize(1024))
                {
                    ModelState.AddModelError(nameof(PhotoCreateViewModel.Files),
                        "File size cannot be greater than 1 mb");
                    return View();
                }

                var imgName = FileUtil.CreatedFile(Path.Combine(FileConstants.ImagePath, "carousel"), image);
                await _dbContext.Sliders.AddAsync(new Slider { Image = imgName });

            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Slider");
        }

        //***** Delete Slider Photo *****//
        public async Task<IActionResult> DeleteSliderPhoto(int id)
        {
            Slider slider = await _dbContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }

            return View(slider);
        }

        [HttpPost]
        [ActionName("DeleteSliderPhoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSliderPhotoPost(int id)
        {
            Slider slider = await _dbContext.Sliders.FindAsync(id);
            if (slider == null)
            {
                return NotFound();
            }

            slider.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index", "Slider", new { id = id });
        }
    }
}
