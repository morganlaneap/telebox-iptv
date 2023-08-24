namespace Telebox.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UnixTimeStampToDateTime(this string text)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return string.IsNullOrEmpty(text) ? dtDateTime : dtDateTime.AddSeconds(Convert.ToInt32(text));
        }
    }
}
