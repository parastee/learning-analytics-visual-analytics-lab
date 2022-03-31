using ImportData.Console.ICU4;
using ImportData.Console.indiaeducation;
using ImportData.Crawler;
using ImportData.Repository;
using ImportData.Repository.Indiaeducation;
using ImportData.Repository.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImportData.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var dbFile = "resultData.db";
            if (File.Exists(dbFile))
                File.Delete(dbFile);
            using (IUnitOfWork uow = new SQLiteUnitOfwork(dbFile))
            {
                var list = WebCrawler.Crawl<ICU4Crawler>("https://www.4icu.org/de/a-z/", "/html/body/div[2]/div/div[2]/div/table/tbody/tr");
                var rep1 = new ICU4Repository(uow);
                try
                {
                    uow.BeginTrans();
                    foreach (dynamic d in list)
                    {

                        System.Console.WriteLine($@"{d.UniId} - {d.UniName} - {d.City} - {d.Rank} - {d.Acronym} - {d.Founded} 
--------------------------------------------------------------
- {d.Address} - {d.Tel} - {d.Fax} - {d.TuitionLclStdnB} - {d.TuitionIntStdnB} - {d.TuitionLclStdnM} - {d.TuitionIntStdnM}
--------------------------------------------------------------
- {d.Gender} - {d.InterNlStdn} - {d.SelectionType} - {d.StudentEnrollment} - {d.AcademicStaff} - {d.ControlType} 
--------------------------------------------------------------
- {d.EntityType} - {d.AcademicCalendar} - {d.CampusSetting} - {d.ReligiousAffiliation} - {d.Library} - {d.Housing} 
--------------------------------------------------------------
- {d.SportFacilities} - {d.FinancialAids} - {d.StudyAbroad} - {d.DistanceLearning}
--------------------------------------------------------------
- Graduate  - {d.GraduateB} - {d.GraduateM} - {d.GraduateP} - {d.ArtsHumanitiesB} - {d.ArtsHumanitiesM} - {d.ArtsHumanitiesP}
--------------------------------------------------------------
- Graduate  - {d.BusinessSocialB} - {d.BusinessSocialM} - {d.BusinessSocialP} - {d.LanguageCulturalB} - {d.LanguageCulturalM} - {d.LanguageCulturalP}
--------------------------------------------------------------
- Graduate  - {d.MedicineHealthB} - {d.MedicineHealthM} - {d.MedicineHealthP} - {d.EngineeringB} - {d.EngineeringM} - {d.EngineeringP}
--------------------------------------------------------------
- Graduate  - {d.ScienceTechnologyB} - {d.ScienceTechnologyM} - {d.ScienceTechnologyP}
--------------------------------------------------------------
==============================================================");
                        rep1.InsertUni(d);
                    }
                    var s = "let cities=[\""+ string.Join("\",\"" ,((IEnumerable<dynamic>)list).Select(new Func<dynamic,string>( a=>a.City?.Replace(" ...",""))).Distinct());
                    
                    s += "\"];";
                    uow.Commit();
                }
                catch (System.Exception ex)
                {

                    throw;
                }

                list = WebCrawler.Crawl<IndiaeducationCrawler>("https://www.indiaeducation.net/studyabroad/germany/list-of-universities-a-f.aspx",
                   "/div[@id=\"artBody\"]/ul[]/li[]/a");
                var rep2 = new IndiaeducationRepository(uow);
                try
                {
                    uow.BeginTrans();
                    foreach (dynamic d in list)
                    {
                        //rep2.InsertUni(d);
                        System.Console.WriteLine($"{d.UniId} - {d.UniName} - {d.City} - {d.Rank}  - {d.Founded}");
                    }
                    uow.Commit();
                }
                catch (System.Exception ex)
                {

                    throw;
                }
            }
            //var rep = new UniRepository("CollectData.sqlite");
            //using (var uow=rep.CreateUOW())
            //{
            //    uow.BeginTrans();

            //    rep.InsertUni(new { UniName="Test Uni 1", Rank=1, City="Test City" });

            //    uow.Commit();

            //    uow.BeginTrans();

            //    rep.InsertUni(new { UniName="Test Uni 1", Rank=1, City="Test City" });

            //    uow.Commit();
            //}
        }
    }
}
