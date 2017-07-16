using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WIndowsAuthCommon.Utilities
{
    /// <summary>
    /// Bunch of shared extensions just to make life a bit easier
    /// </summary>
    public static class SharedExtensions
    {
        #region "Double"
        //https://stackoverflow.com/questions/485175/is-it-safe-to-check-floating-point-values-for-equality-to-0-in-c-net
        private const double staticprecision = 0.0000001;
        public static bool AlmostEquals(this double double1, double double2, double precision)
        {
            return (Math.Abs(double1 - double2) <= precision);
        }

        public static bool AlmostEquals(this double double1, double double2)
        {
            return AlmostEquals(double1, double2, staticprecision);
        }

        public static double? ConvertToDouble(object value)
        {
            return ConvertToDouble(value, System.Globalization.CultureInfo.CurrentCulture);
        }
        public static double? ConvertToDouble(object value, System.Globalization.CultureInfo culture)
        {
            Nullable<double> theReturn = null;
            if ((value != null) && (IsNumeric(value)))
            {
                //theReturn = CDbl(doubleValue.ToString)
                double tmp = 0;
                string doubleStr = null;
                if (((value) is double))
                {
                    doubleStr = ((double)value).ToString(culture);
                }
                else if (((value) is decimal))
                {
                    doubleStr = ((decimal)value).ToString(culture);
                    //ElseIf (TypeOf (doubleValue) Is String) Then
                    //doubleStr = DirectCast(doubleValue, String).ToString(culture)
                }
                else
                {
                    doubleStr = value.ToString();
                }
                if (double.TryParse(doubleStr, System.Globalization.NumberStyles.Any, culture, out tmp))
                {
                    theReturn = tmp;
                }
            }
            return theReturn;
        }

        #endregion

        public static void Empty(this System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }



        public static string ScrubHtml(string value)
        {
            var step1 = Regex.Replace(value, @"<[^>]+>|&nbsp;", "").Trim();
            var step2 = Regex.Replace(step1, @"\s{2,}", " ");
            return step2;
        }

        public static string ToFormattedString(object stringValue, bool treatZeroAsEmpty)
        {
            if ((stringValue == null) | (object.ReferenceEquals(stringValue, DBNull.Value)))
            {
                return string.Empty;
            }
            else
            {
                if (stringValue.ToString().Trim().Length > 0)
                {
                    //treating zeros as nulls
                    if ((object.ReferenceEquals(stringValue.GetType(), typeof(int))) && stringValue.ToString().Trim() == "0" && treatZeroAsEmpty)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return stringValue.ToString().Trim();
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public static string ToFormattedString(object stringValue)
        {
            return ToFormattedString(stringValue, true);
        }


        public static bool IsNumeric(this object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }


        #region "boolean"

        public static bool? ToNullableBoolean(this object value)
        {
            return ConvertToBoolean(value);
        }

        private static Nullable<bool> ConvertToBoolean(object boolValue)
        {
            if ((boolValue != null) && !object.ReferenceEquals(boolValue, DBNull.Value) && boolValue.ToString().Length > 0)
            {
                bool ret = false;
                bool convHappened = bool.TryParse(ToFormattedString(boolValue), out ret);
                if (convHappened)
                {
                    return ret;
                }
            }
            return null;
        }

        #endregion

        #region "integer"
        public static int ToInteger(this object value)
        {
            return ToInteger(value, 0);
        }

        public static int ToInteger(this object value, int defaultValue)
        {
            int? ret = FormatIntegerObject(value);
            if (ret.HasValue)
            {
                return ret.Value;
            }
            return defaultValue;
        }

        private static int? ToNullableInteger(this object value)
        {
            return FormatIntegerObject(value);
        }

        private static Nullable<int> FormatIntegerObject(object intValue, System.Globalization.NumberFormatInfo nmFrmInfo)
        {
            Nullable<int> theReturn = null;
            if (intValue != null)
            {
                //if (IsNumeric(intValue) | intValue.ToString() == "0")
                //{
                //theReturn = CDbl(doubleValue.ToString)

                //first see if it is a float
                double dbl = 0;
                if (double.TryParse(intValue.ToString(), System.Globalization.NumberStyles.Any, nmFrmInfo, out dbl))
                {
                    theReturn = Convert.ToInt32(Math.Floor(dbl));
                    return theReturn;
                }

                int tmp = 0;
                if (int.TryParse(intValue.ToString(), System.Globalization.NumberStyles.Any, nmFrmInfo, out tmp))
                {
                    theReturn = tmp;
                }
                // }
            }
            return theReturn;
        }

        public static Nullable<int> FormatIntegerObject(object intValue)
        {
            return FormatIntegerObject(intValue, System.Globalization.NumberFormatInfo.CurrentInfo);
        }
        #endregion

        #region "Date"

        public static DateTime? ToNullableDateTime(this object value)
        {
            return FormatDateObject(value);
        }

        private static DateTime? FormatDateObject(object datevalue)
        {
            return FormatDateObject(datevalue, System.Globalization.CultureInfo.CurrentCulture);
        }

        private static DateTime? FormatDateObject(object datevalue, System.Globalization.CultureInfo cultureType)
        {
            return FormatDateObject(datevalue, cultureType, DateTimeKind.Utc);
        }

        private static DateTime? FormatDateObject(object datevalue, System.Globalization.CultureInfo cultureType, DateTimeKind dateKind)
        {
            Nullable<DateTime> theDate = default(Nullable<DateTime>);
            if ((datevalue != null) && (!object.ReferenceEquals(datevalue, DBNull.Value)) && (datevalue.ToString().Length > 0))
            {
                DateTime dDate = default(DateTime);
                //i want to use assumeuniversal, but that doesn't seem to do anything
                if (DateTime.TryParse(datevalue.ToString(), cultureType, System.Globalization.DateTimeStyles.AdjustToUniversal, out dDate))
                {
                    if (dDate <= System.DateTime.MinValue | dDate > System.DateTime.MaxValue)
                    {
                        theDate = null;
                    }
                    else if (dDate == Convert.ToDateTime("1/1/0001"))
                    {
                        theDate = null;
                    }
                    else
                    {
                        theDate = DateTime.SpecifyKind(dDate, dateKind);
                    }
                }
                else
                {
                    //try with a format
                    string[] format = { "yyyyMMdd" };
                    if ((DateTime.TryParseExact(datevalue.ToString(), format, cultureType, System.Globalization.DateTimeStyles.None, out dDate)))
                    {
                        theDate = DateTime.SpecifyKind(dDate, dateKind);
                    }
                    else
                    {
                        theDate = null;
                    }

                }
            }
            else
            {
                theDate = null;
            }
            return theDate;
        }

        #endregion
    }
}
