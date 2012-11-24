using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebIT.Temp.Models;
using System.Web.Mvc;
using System.IO;

namespace WebIT.Lib
{
    public static partial class Utils
    {
        /* FRONT END METHODS */
        public static IEnumerable<Header> GetPageHeaders(string tab, string section, string page)
        {
            DBDataContext db = Utils.DB.GetContext();
            int id = GetWebPageID(tab, section, page);
            if(id > 0)
            {
                WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
                if (p != null)
                {
                    return p.Headers;
                }
            }

            return null;
        }

        public static WebPage RetrieveWebPage(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
            if(p != null)
            {
                return p;
            }

            return null;
        }

        public static string GetWebPagePath(int id)
        {
            string path = "";

            DBDataContext db = Utils.DB.GetContext();
            WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
            if (p != null)
            {
                if (p.IsHomePage)
                {
                    path = "";
                }
                else
                {
                    string s = ((p.NavigationLinks.SingleOrDefault(x => x.WebPageID == p.ID) != null)
                            ? p.NavigationLinks.Single(x => x.WebPageID == p.ID).Section.URLTitle
                            : ((p.Template.Code == "qa" && p.Tabs.SingleOrDefault(y => y.FAQPageID == p.ID) != null) ? "faq" : ""));

                    if (!string.IsNullOrEmpty(s))
                    {
                        string t = "";
                        if (s != "faq")
                        {
                            t = ((p.NavigationLinks.SingleOrDefault(x => x.WebPageID == p.ID) != null
                              || (p.Template.Code == "qa" && p.Tabs.SingleOrDefault(y => y.FAQPageID == p.ID) != null))
                              ? p.NavigationLinks.Single(x => x.WebPageID == p.ID).Section.Tab.URLTitle
                              : "");
                        }
                        else
                        {
                            t = p.Tabs.Single(x => x.FAQPageID == p.ID).URLTitle;
                        }
                        if (!string.IsNullOrEmpty(t))
                        {
                            path = t + "/" + s + "/" + p.URLTitle;
                        }
                    }

                    if (path == "")
                    {
                        path = "error:This page has not been assigned to a navigation link";
                    }
                }
            }

            return path;
        }

        public static int GetWebPageID(string tab, string section, string page)
        {
            int webPageId = 0;

            DBDataContext db = Utils.DB.GetContext();
            if (!string.IsNullOrEmpty(tab) && !string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(page))
            {
                Tab t = db.Tabs.SingleOrDefault(x => x.URLTitle.Equals(tab));
                if (t != null)
                {
                    if (section.Equals("faq"))
                    {
                        webPageId = Convert.ToInt32(t.FAQPageID);
                    }
                    else
                    {
                        Section s = t.Sections.SingleOrDefault(x => x.URLTitle.Equals(section));
                        if (s != null)
                        {
                            NavigationLink l = s.NavigationLinks.SingleOrDefault(x => x.WebPage.URLTitle.Equals(page));
                            if (l != null)
                            {
                                webPageId = l.WebPageID;
                            }
                        }
                    }
                }
            }
            else
            {
                //home page
                WebPage p = db.WebPages.SingleOrDefault(x => x.IsHomePage == true);
                if (p != null)
                {
                    webPageId = p.ID;
                }
            }

            return webPageId;
        }



        public static IEnumerable<Category> GetCategories(int pageId)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Category> data = db.Categories.Where(x => x.Documents.All(y => y.WebPageID == pageId)).OrderBy(x=>x.ID);
            return data;
        }

        /* BACK END METHODS */
        public static string GetTab(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
            if (t != null)
            {
                return t.Title;
            }

            return "";
        }

        public static string GetSection(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Section s = db.Sections.SingleOrDefault(x => x.ID == id);
            if (s != null)
            {
                return s.Title;
            }
            return "";
        }

        public static string GetWebPage(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == id);
            if (pg != null)
            {
                return pg.Title;
            }
            return "";
        }

        public static string GetTemplate(string code)
        {
            DBDataContext db = Utils.DB.GetContext();
            Template t = db.Templates.SingleOrDefault(x => x.Code.Equals(code));
            if(t != null)
            {
                return t.Name;
            }
            return "";
        }

        public static int GetTemplateId(string code)
        {
            DBDataContext db = Utils.DB.GetContext();
            Template t = db.Templates.SingleOrDefault(x => x.Code.Equals(code));
            if (t != null)
            {
                return t.ID;
            }

            return 0;
        }

        public static IEnumerable<Template> GetTemplates()
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Template> data = db.Templates;
            return data;
        }


        /* front end utilities */
        public static class Navigation
        {
            public static IEnumerable<Tab> Tabs()
            {
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<Tab> tabs = db.Tabs.OrderBy(x => x.Position);
                return tabs;
            }

            public static IEnumerable<Section> Sections(int tab)
            {
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<Section> sections = db.Sections.Where(x => x.TabID == tab).OrderBy(x => x.Position);
                return sections;
            }

            public static IEnumerable<NavigationLink> Links(int section)
            {
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<NavigationLink> links = db.NavigationLinks.Where(x => x.SectionID == section).OrderBy(x => x.Position);
                return links;
            }
        }

    }
}