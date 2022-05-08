using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSCCSL.AutoBackUp
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseBackUp();
        }

        public static void DatabaseBackUp()
        {
            try
            {
                string path = ConfigurationManager.AppSettings.Get("BackUpPath") + @"BSCCSL_" + DateTime.Now.ToString("dd'-'MM'-'yyyy") + ".bak";
                string zipPath = ConfigurationManager.AppSettings.Get("BackUpPath") + @"\BSCCSL_" + DateTime.Now.ToString("dd'-'MM'-'yyyy") + ".zip";

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BSCCSL"].ConnectionString);
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("BACKUP DATABASE [BSCCSL] TO  DISK = '" + path + "'", conn);
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally { conn.Close(); }


                using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    zip.CreateEntryFromFile(path, "BSCCSL-" + DateTime.Now.ToString("ddMMyyyy") + ".bak", CompressionLevel.Optimal);
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
