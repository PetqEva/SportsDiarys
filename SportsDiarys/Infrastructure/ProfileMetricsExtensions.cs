using SportsDiarys.Data.Models;

namespace SportsDiarys.Infrastructure
{
    public static class ProfileMetricsExtensions
    {
        public static double? HeightMeters(this UserProfile p)
        {
            if (p.HeightCm <= 0) return null;
            return p.HeightCm / 100.0;
        }

        public static double? BMI(this UserProfile p)
        {
            var h = p.HeightMeters();
            if (h == null || p.StartWeightKg <= 0) return null;

            var bmi = p.StartWeightKg / (h.Value * h.Value);
            return Math.Round(bmi, 1);
        }

        // BMR (Mifflin–St Jeor)
        // Male: 10W + 6.25H - 5A + 5
        // Female: 10W + 6.25H - 5A - 161
        public static int? BMR(this UserProfile p)
        {
            if (p.StartWeightKg <= 0 || p.HeightCm <= 0 || p.Age <= 0) return null;

            var baseVal = 10 * p.StartWeightKg + 6.25 * p.HeightCm - 5 * p.Age;

            if (p.Gender == "Male") return (int)Math.Round(baseVal + 5);
            if (p.Gender == "Female") return (int)Math.Round(baseVal - 161);

            return (int)Math.Round(baseVal); // ако полът е празен/друг
        }

        public static double ActivityFactor(this UserProfile p)
        {
            return p.ActivityLevel switch
            {
                "Low" => 1.2,      // ниска активност
                "Medium" => 1.55,  // средна активност
                "High" => 1.725,   // висока активност
                _ => 1.2
            };
        }

        // TDEE = BMR * activity factor (поддържащи калории)
        public static int? DailyCalories(this UserProfile p)
        {
            var bmr = p.BMR();
            if (bmr == null) return null;

            return (int)Math.Round(bmr.Value * p.ActivityFactor());
        }

        public static (string text, string cssClass) ActivityBadge(this UserProfile p)
        {
            return p.ActivityLevel switch
            {
                "Low" => ("Ниска активност", "bg-secondary"),
                "Medium" => ("Средна активност", "bg-success"),
                "High" => ("Висока активност", "bg-danger"),
                _ => ("-", "bg-secondary")
            };
        }

        // Прогрес (ще стане реален, когато имаш CurrentWeightKg)
        public static string ProgressNote(this UserProfile p)
        {
            return "Прогресът ще е реален, когато добавим поле „Текущи килограми“ (CurrentWeightKg) или тегло по записи.";
        }
    }
}
