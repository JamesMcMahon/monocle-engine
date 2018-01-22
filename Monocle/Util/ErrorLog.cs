using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monocle
{
    public static class ErrorLog
    {
        public const string Filename = "error_log.txt";
        public const string Marker = "==========================================";

        public static void Write(Exception e)
        {
            Write(e.ToString());
        }

        public static void Write(string str)
        {
            StringBuilder s = new StringBuilder();

            //Get the previous contents
            string content = "";
            if (File.Exists(Filename))
            {
                TextReader tr = new StreamReader(Filename);
                content = tr.ReadToEnd();
                tr.Close();

                if (!content.Contains(Marker))
                    content = "";
            }

            //Header
            if (Engine.Instance != null)
                s.Append(Engine.Instance.Title);
            else
                s.Append("Monocle Engine");
            s.AppendLine(" Error Log");
            s.AppendLine(Marker);
            s.AppendLine();

            //Version Number
            if (Engine.Instance.Version != null)
            {
                s.Append("Ver ");
                s.AppendLine(Engine.Instance.Version.ToString());
            }

            //Datetime
            s.AppendLine(DateTime.Now.ToString());

            //String
            s.AppendLine(str);

            //If the file wasn't empty, preserve the old errors
            if (content != "")
            {
                int at = content.IndexOf(Marker) + Marker.Length;
                string after = content.Substring(at);
                s.AppendLine(after);
            }

            TextWriter tw = new StreamWriter(Filename, false);
            tw.Write(s.ToString());
            tw.Close();
        }

        public static void Open()
        {
            if (File.Exists(Filename))
                System.Diagnostics.Process.Start(Filename);
        }
    }
}
