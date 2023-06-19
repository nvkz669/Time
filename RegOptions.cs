using Microsoft.Win32;

namespace TiMonSys
{
    public class RegOptions
    {
        private const string BaseKey = "HKEY_CURRENT_USER\\Software\\TMS";

       public static object GetKeyValue(string componentName, string key, object defaultValue)
        {
            var value = Registry.GetValue(BaseKey + "\\Components\\" + componentName, key, defaultValue);
            return value ?? defaultValue;
        }
      

        public static void SetKeyValue(string componentName, string key, object value)
        {
            Registry.SetValue(BaseKey + "\\Components\\" + componentName, key, value);
        }
    }
}
