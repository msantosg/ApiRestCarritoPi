using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ApiRestCarritoPi.App_Code
{
    public class clsLog
    {
        public const string NewLine = "\r\n";
        public enum tInOut
        {
            IN = 1,
            OUT = 2
        }

        public static void EscribeLogErr(Exception ex, string metodom, bool alt = false)
        {
            try
            {
                string path = clsUtils.TraeKeyConfig(alt ? "Log.DirAlt" : "Log.DirLog");
                string LogFile = path + clsUtils.TraeKeyConfig("Log.LogErr") + DateTime.Now.ToString("yyyyMMdd") + ".log";
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (!File.Exists(LogFile))
                {
                    var file = File.Create(LogFile);
                    file.Close();
                }

                using(var sw = new StreamWriter(LogFile, true))
                {
                    sw.Write(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|" + metodom + "|" + ex.Message + NewLine + ex.StackTrace + NewLine);
                }
            }
            catch(Exception e)
            {
                if (alt == false)
                    EscribeLogErr(e, "EscribeLogErr", true);
            }
        }

        public static string EscribeLogInOut(string trama,  string metodo, tInOut tLog, string guidLog = "")
        {
            string guidlog = string.Empty;
            try
            {
                string vArchivo = clsUtils.TraeKeyConfig("Log.LogInOut") + DateTime.Now.ToString("yyyyMMdd") + ".log";
                string vDirectorio = clsUtils.TraeKeyConfig("Log.DirLog") + DateTime.Now.ToString("yyyyMMdd") + "/";
                guidlog = (tLog == tInOut.IN ? Guid.NewGuid().ToString() : guidLog);
                string vPath = vDirectorio + vArchivo;
                if (!Directory.Exists(vDirectorio)) Directory.CreateDirectory(vDirectorio);

                using(StreamWriter sw = new StreamWriter(vPath, true))
                {
                    string tipoTrama = tLog == tInOut.IN ? "ENTRADA" : "SALIDA";
                    sw.WriteLine(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|" + tipoTrama + "|" + guidlog + "|" + metodo + "|" + trama);
                }
            }
            catch(Exception ex)
            {
                EscribeLogErr(ex, "clsLog.EscribeLogInOut");
                guidLog = string.Empty;
            }

            return guidlog;
        }
    }
}