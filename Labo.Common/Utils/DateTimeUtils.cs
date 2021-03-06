﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeUtils.cs" company="Labo">
//   The MIT License (MIT)
//   
//   Copyright (c) 2013 Bora Akgun
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to
//   use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//   the Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary>
//   Defines the DateTimeUtils type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Labo.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Labo.Common.Patterns;

    /// <summary>
    /// Date time utility class.
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Gets the day names.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>Day names list.</returns>
        public static IList<DayName> GetDayNames(CultureInfo culture = null)
        {
            culture = CultureUtils.GetCurrentCultureIfNull(culture);

            return culture.DateTimeFormat.DayNames
                .TakeWhile(m => !m.IsNullOrEmpty())
                .Select((m, i) => new DayName
                {
                    Number = ((i + (7 - (int)culture.DateTimeFormat.FirstDayOfWeek)) % 7) + 1,
                    Name = m
                }).ToList();
        }

        /// <summary>
        /// Gets the month names of the specified culture.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <returns>Month names list.</returns>
        public static IList<MonthName> GetMonthNames(CultureInfo culture = null)
        {
            culture = CultureUtils.GetCurrentCultureIfNull(culture);

            return culture.DateTimeFormat.MonthNames
                .TakeWhile(m => !m.IsNullOrEmpty())
                .Select((m, i) => new MonthName
                    {
                        Number = i + 1,
                        Name = m
                    }).ToList();
        }

        /// <summary>
        /// Gets the first day of month.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>First day of the month of the specified date.</returns>
        public static DateTime GetFirstDayOfMonth(DateTime currentDate)
        {
            return new DateTime(currentDate.Year, currentDate.Month, 1);
        }

        /// <summary>
        /// Gets the last day of month.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Last day of the month of the specified date.</returns>
        public static DateTime GetLastDayOfMonth(DateTime currentDate)
        {
            return GetFirstDayOfMonth(currentDate.AddMonths(1)).AddDays(-1).Date;
        }

        /// <summary>
        /// Gets the days of month.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Total day count of the month.</returns>
        public static int GetDaysOfMonth(DateTime currentDate)
        {
            return GetLastDayOfMonth(currentDate).Day;
        }

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>First day of week.</returns>
        public static DateTime GetFirstDayOfWeek(DateTime currentDate)
        {
            return GetFirstDayOfWeek(currentDate, null);
        }

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>First day of week.</returns>
        public static DateTime GetFirstDayOfWeek(DateTime currentDate, CultureInfo cultureInfo)
        {
            cultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            return GetFirstDayOfWeek(currentDate, cultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// Gets the first day of week.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="firstDay">The first day.</param>
        /// <returns>First day of week.</returns>
        public static DateTime GetFirstDayOfWeek(DateTime currentDate, DayOfWeek firstDay)
        {
            return currentDate.AddDays(firstDay - currentDate.DayOfWeek).Date;
        }

        /// <summary>
        /// Gets the last day of week.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Last day of week.</returns>
        public static DateTime GetLastDayOfWeek(DateTime currentDate)
        {
            return GetLastDayOfWeek(currentDate, null);
        }

        /// <summary>
        /// Gets the last day of week.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="cultureInfo">The culture info.</param>
        /// <returns>Last day of week.</returns>
        public static DateTime GetLastDayOfWeek(DateTime currentDate, CultureInfo cultureInfo)
        {
            cultureInfo = cultureInfo ?? CultureInfo.CurrentCulture;
            return GetLastDayOfWeek(currentDate, cultureInfo.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// Gets the last day of week.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="firstDay">The first day.</param>
        /// <returns>Last day of week.</returns>
        public static DateTime GetLastDayOfWeek(DateTime current, DayOfWeek firstDay)
        {
            return GetFirstDayOfWeek(current, firstDay).AddDays(6);
        }

        /// <summary>
        /// Gets the midnight.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Midnight date.</returns>
        public static DateTime GetMidnight(DateTime currentDate)
        {
            return currentDate.Date;
        }

        /// <summary>
        /// Gets the noon.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Noon date.</returns>
        public static DateTime GetNoon(DateTime currentDate)
        {
            return new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 12, 0, 0);
        }

        /// <summary>
        /// Calculates the age.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="dateTimeProvider">The date time provider.</param>
        /// <returns>Age.</returns>
        public static int CalculateAge(DateTime dateOfBirth, IDateTimeProvider dateTimeProvider)
        {
            if (dateTimeProvider == null) throw new ArgumentNullException("dateTimeProvider");

            return CalculateAge(dateOfBirth, dateTimeProvider.Today);
        }

        /// <summary>
        /// Calculates the age.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns>Age.</returns>
        public static int CalculateAge(DateTime dateOfBirth)
        {
            return CalculateAge(dateOfBirth, DateTimeProvider.Current);
        }

        /// <summary>
        /// Calculates the age.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <param name="currentDate">The current date.</param>
        /// <returns>Age.</returns>
        public static int CalculateAge(DateTime dateOfBirth, DateTime currentDate)
        {
            int birthMonth = dateOfBirth.Month;
            int currentMonth = currentDate.Month;
            if (birthMonth < currentMonth || (birthMonth == currentMonth && dateOfBirth.Day <= currentDate.Day))
            {
                return currentDate.Year - dateOfBirth.Year;
            }

            return currentDate.Year - dateOfBirth.Year - 1;
        }
    }
}
