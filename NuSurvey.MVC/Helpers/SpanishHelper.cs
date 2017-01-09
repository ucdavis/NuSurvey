using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NuSurvey.MVC.Helpers
{
    public static class SpanishHelper
    {
        public static bool IsSpanish(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            if (value.Trim().EndsWith("S", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public static string TranslateErrorMessage(this string value)
        {
            if (value.Equals("Answer is required", StringComparison.OrdinalIgnoreCase))
            {
                return "La respuesta es requerida";
            }
            if (value.Equals("Matching Value to score not found", StringComparison.OrdinalIgnoreCase))
            {
                return "Valor a juego con la puntuación no se encontró";
            }
            if (value.Equals("Answer must be a whole number", StringComparison.OrdinalIgnoreCase))
            {
                return "Respuesta debe ser un número entero";
            }
            if (value.Equals("Matching Value to score not found", StringComparison.OrdinalIgnoreCase))
            {
                return "Valor a juego con la puntuación no se encontró";
            }
            if (value.Equals("Answer must be a number (decimal ok)", StringComparison.OrdinalIgnoreCase))
            {
                return "Respuesta debe ser un número (ok decimal)";
            }
            if (value.Equals("Answer must be a Time (hh:mm)", StringComparison.OrdinalIgnoreCase))
            {
                return "Respuesta debe ser un tiempo (hh: mm)";
            }
            if (value.Equals("Answer must be a Time (hh:mm AM/PM)", StringComparison.OrdinalIgnoreCase))
            {
                return "Respuesta debe ser un tiempo (hh: mm AM / PM)";
            }

            return value;
        }
    }
}