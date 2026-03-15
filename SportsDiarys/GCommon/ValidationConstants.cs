namespace SportsDiarys.Common
{
    public static class ValidationConstants
    {
        // UserProfile
        public const int NameMinLength = 2;
        public const int NameMaxLength = 50;

        public const int AgeMin = 10;
        public const int AgeMax = 100;

        // TrainingDiary
        public const int DiaryNotesMaxLength = 500;
        public const int DiaryNameMaxLength = 100;

        // TrainingEntry
        public const int SportNameMinLength = 3;
        public const int SportNameMaxLength = 30;

        public const int DurationMin = 1;
        public const int DurationMax = 600;

        public const int CaloriesMin = 0;
        public const int CaloriesMax = 10000;

        public const double DistanceMin = 0;
        public const double DistanceMax = 500;

    }
}
