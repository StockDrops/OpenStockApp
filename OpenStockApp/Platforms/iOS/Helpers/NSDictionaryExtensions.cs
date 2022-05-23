using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Platforms.iOS.Helpers
{
    public static class NSDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NSDictionary nsDictionary) =>
          nsDictionary.ToDictionary<KeyValuePair<NSObject, NSObject>, string, string>
              (item => item.Key as NSString, item => item.Value.ToString());
    }
}
