using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.SqlServer.ReportingServices2010;
using ReportMetaDataGrabber.Properties;

namespace ReportMetaDataGrabber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Working...");

            var reportingService2010 = new ReportingService2010();

            reportingService2010.Credentials = CredentialCache.DefaultCredentials;

            var results = reportingService2010.ListChildren("/", true);
            
            var reportMetas = new List<ReportMeta>();

            reportMetas.Add(new ReportMeta { CreatedBy = "CreatedBy", Folder = "Folder", ModifiedBy = "ModifiedBy", Name = "Name", ReportItemType = ReportItemType.Type, LastModifiedDate = "LastModifiedDate" });

            foreach (var result in results)
            {
                var reportMeta = new ReportMeta();
                reportMeta.Name = result.Name;
                reportMeta.CreatedBy = result.CreatedBy;
                reportMeta.ModifiedBy = result.ModifiedBy;
                reportMeta.LastModifiedDate = result.ModifiedDate.ToString("yyyy-MM-dd hh:mm");
                reportMeta.Folder = result.Path;

                switch (result.TypeName)
                {
                    case "Folder":
                        reportMeta.ReportItemType = ReportItemType.Folder;
                        
                        break;

                    case "Report":
                        reportMeta.ReportItemType = ReportItemType.Report;
                        break;

                    case "DataSource":
                        reportMeta.ReportItemType = ReportItemType.DataSource;
                        break;

                    case "Component":
                        reportMeta.ReportItemType = ReportItemType.Component;
                        break;

                    case "Resource":
                        reportMeta.ReportItemType = ReportItemType.Resource;
                        break;
                }

                reportMetas.Add(reportMeta);
            }

            if (File.Exists(Settings.Default.TargetFile))
                File.Delete(Settings.Default.TargetFile);

            File.WriteAllLines(Settings.Default.TargetFile, reportMetas.Select(obj => obj.ToString()));

            Console.WriteLine("Done!");
        }
    }

    public enum ReportItemType
    {
        Type = -1,
        Folder = 0,
        Report = 1,
        DataSource = 2,
        Component = 3,
        Resource = 4
    }

    public class ReportMeta
    {
        public ReportItemType ReportItemType { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string LastModifiedDate { get; set; }

        public override string ToString()
        {
            return ReportItemType + "," + Name + "," + Folder + "," + CreatedBy + "," + ModifiedBy + "," + LastModifiedDate;
        }
    }
}
