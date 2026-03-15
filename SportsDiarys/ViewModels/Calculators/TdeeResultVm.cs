namespace SportsDiarys.ViewModels.Calculators
{
    public class TdeeResultVm
    {
        public double Bmr { get; set; }
        public double Tdee { get; set; }

        public double Cut15 { get; set; }
        public double Cut20 { get; set; }
        public double Bulk10 { get; set; }

        public string Method { get; set; } = "";

        public double Bmi { get; set; }
        public double? LeanBodyMassKg { get; set; }     // null ако няма BodyFatPercent
        public double ProteinTargetG { get; set; }
    }
}


