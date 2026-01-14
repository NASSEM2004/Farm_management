namespace Farm_management.Services
{
    public static class TextHelper
    {
        // هذه الدالة تحسب مدى التشابه بين كلمتين (تعطي رقم يمثل عدد التغييرات المطلوبة)
        public static int CalculateDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return target?.Length ?? 0;
            if (string.IsNullOrEmpty(target)) return source.Length;

            int[,] distance = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; distance[i, 0] = i++) ;
            for (int j = 0; j <= target.Length; distance[0, j] = j++) ;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }
            return distance[source.Length, target.Length];
        }

        // دالة لفحص هل الكلمة المدخلة قريبة من كلمة معينة (بنسبة خطأ مقبولة)
        public static bool IsSimilar(string input, string compareTo, int maxErrors = 1)
        {
            if (string.IsNullOrEmpty(input)) return false;

            // تحويل للنصوص الصغيرة وإزالة المسافات الزائدة لتوحيد المقارنة
            input = input.Trim().ToLower();
            compareTo = compareTo.Trim().ToLower();

            int distance = CalculateDistance(input, compareTo);
            return distance <= maxErrors;
        }
    }
}
