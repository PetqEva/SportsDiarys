using Microsoft.AspNetCore.Mvc;
using SportsDiarys.ViewModels.Calculators;

namespace SportsDiarys.Controllers
{
    public class CalculatorsController : Controller
    {
        [HttpGet]
        public IActionResult Tdee()
        {
            var model = new TdeeInputVm
            {
                Gender = Gender.Male,
                ActivityMultiplier = 1.2
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Tdee(TdeeInputVm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            double bmr;
            double? leanBodyMass = null;
            string method;

            if (model.BodyFatPercent.HasValue)
            {
                leanBodyMass = model.WeightKg * (1 - model.BodyFatPercent.Value / 100.0);
                bmr = 370 + (21.6 * leanBodyMass.Value);
                method = "Katch-McArdle";
            }
            else
            {
                if (model.Gender == Gender.Male)
                {
                    bmr = 10 * model.WeightKg + 6.25 * model.HeightCm - 5 * model.Age + 5;
                }
                else
                {
                    bmr = 10 * model.WeightKg + 6.25 * model.HeightCm - 5 * model.Age - 161;
                }

                method = "Mifflin-St Jeor";
            }

            double tdee = bmr * model.ActivityMultiplier;
            double bmi = model.WeightKg / Math.Pow(model.HeightCm / 100.0, 2);

            ViewBag.Result = new TdeeResultVm
            {
                Bmr = Math.Round(bmr, 2),
                Tdee = Math.Round(tdee, 2),
                Cut15 = Math.Round(tdee * 0.85, 2),
                Cut20 = Math.Round(tdee * 0.80, 2),
                Bulk10 = Math.Round(tdee * 1.10, 2),
                Method = method,
                Bmi = Math.Round(bmi, 2),
                LeanBodyMassKg = leanBodyMass.HasValue ? Math.Round(leanBodyMass.Value, 2) : null,
                ProteinTargetG = Math.Round(model.WeightKg * 2.0, 2)
            };

            return View(model);
        }
    }
}