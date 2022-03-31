using ImportData.Crawler;
using HtmlAgilityPack;

namespace ImportData.Console.ICU4
{
    public class ICU4Crawler : ListWebCrawler<UniCrawler>
    {

        public ICU4Crawler( string source, string path) : base( source, path)
        {
        }
        protected override UniCrawler ParseModel(HtmlNode node)
        {
            return new UniCrawler( node.OuterHtml, "//tr");
            //return new UniCrawler("https://www.4icu.org" + node.GetAttributeValue("href", ""), "/html/body/div[3]");
        }
    }
    public class UniCrawler : ObjWebCrawler
    {
        public UniCrawler(string source, string path)
            : base(source, path
                  , (config) => config
                  .AddPath("UniId", node => new IUC4UniIdWebCrawler(node.InnerHtml, "//td[2]/a"))
                  .AddPath("UniName", node => new StringWebCrawler(node.InnerHtml, "//td[2]/a"))
                  .AddPath("City", node => new StringWebCrawler(node.InnerHtml, "//td[3]"))
                  .AddPath("Rank", node => new StringWebCrawler(node.InnerHtml, "//td[1]/b"))
                  .AddPath("Acronym", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[3]/div[1]/div/div[2]/table/tbody/tr[3]/td/abbr"))
                  .AddPath("Founded", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""), 
                      "//html/body/div[3]/div[3]/div[1]/div/div[2]/table/tbody/tr[4]/td/span"))
                  .AddPath("Address", node => new TextMultiLineWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[3]/div[2]/div[2]/div[2]/table/tr[1]/td"))
                  .AddPath("Tel", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[3]/div[2]/div[2]/div[2]/table/tr[2]/td/span"))
                  .AddPath("Fax", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[3]/div[2]/div[2]/div[2]/table/tr[3]/td/span"))
                  .AddPath("TuitionLclStdnB", node => new TextMultiLineWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[5]/div[2]/div/table/tbody/tr[1]/td[2]"))
                  .AddPath("TuitionIntStdnB", node => new TextMultiLineWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[5]/div[2]/div/table/tbody/tr[2]/td[2]"))
                  .AddPath("TuitionLclStdnM", node => new TextMultiLineWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[5]/div[2]/div/table/tbody/tr[1]/td[3]"))
                  .AddPath("TuitionIntStdnM", node => new TextMultiLineWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[5]/div[2]/div/table/tbody/tr[2]/td[3]"))
                  .AddPath("Gender", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[6]/div[2]/div/div[2]/table//tr[1]/td"))
                  .AddPath("InterNlStdn", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[6]/div[2]/div/div[2]/table//tr[2]/td"))
                  .AddPath("SelectionType", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[6]/div[2]/div/div[2]/table//tr[3]/td"))
                  .AddPath("AdmissionRate", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[6]/div[2]/div/div[2]/table//tr[4]/td"))
                  .AddPath("StudentEnrollment", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[1]/table//tr[1]/td"))
                  .AddPath("AcademicStaff", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[1]/table//tr[2]/td"))
                  .AddPath("ControlType", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[1]/table//tr[3]/td"))
                  .AddPath("EntityType", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[1]/table//tr[4]/td"))
                  .AddPath("AcademicCalendar", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[2]/table//tr[1]/td"))
                  .AddPath("CampusSetting", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[2]/table//tr[2]/td"))                  
                  .AddPath("ReligiousAffiliation", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[7]/div[2]/div/div[2]/table//tr[3]/td"))
                  .AddPath("Library", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[1]/table//tr[1]/td"))
                  .AddPath("Housing", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[1]/table//tr[2]/td"))
                  .AddPath("SportFacilities", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[1]/table//tr[3]/td"))
                  .AddPath("FinancialAids", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[2]/table//tr[1]/td"))
                  .AddPath("StudyAbroad", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[2]/table//tr[2]/td"))
                  .AddPath("DistanceLearning", node => new StringWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[8]/div[2]/div/div[2]/table//tr[3]/td"))
                  .AddPath("GraduateB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/thead/tr[3]/td[2]"))
                  .AddPath("GraduateM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/thead/tr[3]/td[3]"))
                  .AddPath("GraduateP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/thead/tr[3]/td[4]"))
                  .AddPath("ArtsHumanitiesB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[1]/td[2]"))
                  .AddPath("ArtsHumanitiesM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[1]/td[3]"))
                  .AddPath("ArtsHumanitiesP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[1]/td[4]"))
                  .AddPath("BusinessSocialB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[2]/td[2]"))
                  .AddPath("BusinessSocialM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[2]/td[3]"))
                  .AddPath("BusinessSocialP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[2]/td[4]"))
                  .AddPath("LanguageCulturalB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[3]/td[2]"))
                  .AddPath("LanguageCulturalM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[3]/td[3]"))
                  .AddPath("LanguageCulturalP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[3]/td[4]"))
                  .AddPath("MedicineHealthB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[4]/td[2]"))
                  .AddPath("MedicineHealthM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[4]/td[3]"))
                  .AddPath("MedicineHealthP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[4]/td[4]"))
                  .AddPath("EngineeringB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[5]/td[2]"))
                  .AddPath("EngineeringM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[5]/td[3]"))
                  .AddPath("EngineeringP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[5]/td[4]"))
                  .AddPath("ScienceTechnologyB", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[6]/td[2]"))
                  .AddPath("ScienceTechnologyM", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[6]/td[3]"))
                  .AddPath("ScienceTechnologyP", node => new YesNoFromCssClassWebCrawler("https://www.4icu.org" + node.SelectSingleNode("//td[2]/a").GetAttributeValue("href", ""),
                      "//html/body/div[3]/div[4]/div[2]/div[1]/table/tbody/tr[6]/td[4]"))
                  )
        {

        }
    }

    public class IUC4UniIdWebCrawler : StringWebCrawler
    {
        public IUC4UniIdWebCrawler(string source, string path) : base(source, path)
        {
        }
        protected override string ConvertValue(HtmlNode document)
        {
            return document.GetAttributeValue("href", "").Split("/")[2].Split(".")[0];
        }
    }

    public class TextMultiLineWebCrawler : StringWebCrawler
    {
        public TextMultiLineWebCrawler(string source, string path) : base(source, path)
        {
        }
        protected override string ConvertValue(HtmlNode document)
        {
             document.InnerHtml=document.InnerHtml
                .Replace("<br>", "\r\n")
                .Replace("<br />", "\r\n")
                .Replace("</p>", "\r\n")
                .Replace("<p>", "");
            return document.InnerText;
        }
    }
    public class YesNoFromCssClassWebCrawler : StringWebCrawler
    {
        public YesNoFromCssClassWebCrawler(string source, string path) : base(source, path)
        {
        }
        protected override string ConvertValue(HtmlNode document)
        {
            document.InnerHtml = document.InnerHtml
               .Replace("<br>", "")
               .Replace("<br />", "")
               .Replace("&nbsp;", "")
               .Replace("<i class=\"sp sp-1\"></i>", "Yes")
               .Replace("<i class=\"sp sp-0\"></i>", "No");
            return document.InnerText;
        }
    }
}
