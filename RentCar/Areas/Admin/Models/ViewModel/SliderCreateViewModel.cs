using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentCar.Areas.Admin.Models.ViewModel
{
    public class SliderCreateViewModel
    {
        public IFormFile[] Files { get; set; }
    }
}
