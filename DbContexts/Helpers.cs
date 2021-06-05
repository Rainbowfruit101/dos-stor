using Models.Enums;
using ViewModels;
using System;
using System.Collections.Generic;

namespace DbContexts
{
    public static class Helpers
    {
        private static readonly Dictionary<ComparisonMode, Func<DateTime, DateTime, bool>> ComparisonModeToFunc = new Dictionary<ComparisonMode, Func<DateTime, DateTime, bool>>()
        {
            {ComparisonMode.Equal,(date1,date2) => date1 == date2},
            {ComparisonMode.Less, (date1,date2) => date1 < date2},
            {ComparisonMode.More, (date1,date2) => date1 > date2}
        };
        public static bool CheckDate(DateTime docDate, DateSearchViewModel model) 
        {
            return ComparisonModeToFunc[model.Mode].Invoke(docDate, model.Date);
        }
    }
}
