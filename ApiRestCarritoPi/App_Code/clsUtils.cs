using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiRestCarritoPi.App_Code
{
    public class clsUtils
    {
        public static string TraeKeyConfig(string key)
        {
            string conf = string.Empty;
            try
            {
                string mappingpath = System.Web.HttpContext.Current.Server.MapPath("~/Config/VariablesConfig.json");
                JObject config = JObject.Parse(File.ReadAllText(mappingpath));
                JToken confs = config.SelectToken(key);
                conf = confs.Value<string>();
            }
            catch(Exception ex)
            {
                clsLog.EscribeLogErr(ex, "clsUtils.TraeKeyConfig");
                conf = string.Empty;
            }
            return conf;
        }
    }
}