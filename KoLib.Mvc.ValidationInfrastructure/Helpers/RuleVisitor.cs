using System;
using System.Collections.Generic;
using System.Web.Mvc;
using KoLib.Mvc.ValidationInfrastructure.Contracts;

namespace KoLib.Mvc.ValidationInfrastructure.Helpers
{
    public static class RuleMethods
    {
        public static IDateTimeProvider DateTimeProvider { get; set; }
        static RuleMethods()
        {
            DateTimeProvider = DependencyResolver.Current.GetService<IDateTimeProvider>();
        }
        public static DateTime Today()
        {
            return DateTimeProvider.Today;
        }

        public static DateTime Yesterday()
        {
            return DateTime.Today.AddDays(-1);
        }

        public static DateTime Now()
        {
            return DateTime.Now;
        }

        public static DateTime Tomorrow()
        {
            return DateTime.Today.AddDays(1);
        }
        
        public static decimal SumIf<TSource>(this IEnumerable<TSource> list, Func<TSource, decimal> itemMember, Func<TSource, bool> selector)
        {
            decimal retVal = 0;
            foreach (var item in list)
            {
                //TODO: Need to ignore null item here
                if (selector(item)) retVal += itemMember(item);
            }
            return retVal;
        }

        public static T Val<T>(this Nullable<T> value) where T : struct
        {
            return value.GetValueOrDefault();
        }

        public static string Str<T>(this Nullable<T> value) where T : struct
        {
            var result = null as string;
            if (value.HasValue)
            {
                result = value.Value.ToString();
            }


            return result;
        }

        public static string FormState()
        {
            return "test";
        }
    }
}
