using System;
using System.Collections.Generic;
using System.Reflection;
using Omi.Education.Enums;
using Omi.Education.Web.Management.Services.Models;

namespace Omi.Education.Web.Management.Services
{
    public class PropertyCompare<T> : IEqualityComparer<T>
    {
        private PropertyInfo _pro1;
        private PropertyInfo _pro2;
        const BindingFlags flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
        public PropertyCompare(string propertyName1, string propertyName2)
        {
            _pro1 = typeof(T).GetProperty(propertyName1, flags);
            _pro2 = typeof(T).GetProperty(propertyName2, flags);
            if (_pro1 == null || _pro2 == null)
                throw new ArgumentException(String.Format("{0} or {1} is not a property of type {2}", propertyName1, propertyName2, typeof(T)));
        }

        public bool Equals(T x, T y)
        {
            object xValue1 = _pro1.GetValue(x, null);
            object xValue2 = _pro2.GetValue(x, null);
            object yValue1 = _pro1.GetValue(y, null);
            object yValue2 = _pro2.GetValue(y, null);

            if (xValue1.Equals(yValue1))
              return xValue2.Equals(yValue2);

            return xValue1.Equals(yValue1);
        }

        public int GetHashCode(T obj)
        {
            object propertyValue = _pro1.GetValue(obj, null);
            if (propertyValue == null)
				return 0;

			else
				return propertyValue.GetHashCode();
        }
    }
}