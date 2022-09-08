using System.Globalization;

namespace Appointment.Domain.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string text)
        {
            var textInfo = new CultureInfo("es-ES", false).TextInfo;
            return textInfo.ToTitleCase(text);
        } 
    }
}
