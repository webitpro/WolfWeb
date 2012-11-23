using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using WebIT.Lib;
using WebIT.Temp.Models;
using System.IO;

namespace WebIT.Temp
{

    public static class Form
    {

        public static class MultiSelectElement
        {
            public static class Navigation
            {
                public static MultiSelectList Tabs(int? selectedID)
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    List<int> selectedValues = new List<int>();
                    DBDataContext db = Utils.DB.GetContext();
                    IEnumerable<WebIT.Temp.Models.Tab> tabs = db.Tabs.OrderBy(x => x.Position).Select(x => x);
                    if (tabs.Count() > 0)
                    {
                        foreach (Tab t in tabs)
                        {
                            list.Add(new SelectListItem
                            {
                                Text = t.Title,
                                Value = t.ID.ToString(),
                                Selected = ((selectedID != null) ? ((t.ID.Equals((int)selectedID)) ? true : false) : ((t.ID.Equals(tabs.First().ID)) ? true : false))
                            });
                        }

                        selectedValues.Add(((selectedID != null) ? (int)selectedID : tabs.First().ID));
                    }
                    return new MultiSelectList(list, "Value", "Text", selectedValues);
                }

                public static MultiSelectList Sections(int tabID, int? selectedID)
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    List<int> selectedValues = new List<int>();
                    DBDataContext db = Utils.DB.GetContext();
                    IEnumerable<Section> sections = db.Sections.Where(x=>x.TabID == tabID).OrderBy(x => x.Position).Select(x => x);
                    if (sections.Count() > 0)
                    {
                        foreach (Section s in sections)
                        {
                            list.Add(new SelectListItem
                            {
                                Text = s.Title,
                                Value = s.ID.ToString(),
                                Selected = ((selectedID != null) ? ((s.ID.Equals((int)selectedID)) ? true : false) : ((s.ID.Equals(sections.First().ID)) ? true : false))
                            });
                        }
                        
                        selectedValues.Add(((selectedID != null) ? (int)selectedID : sections.First().ID));
                    }

                    return new MultiSelectList(list, "Value", "Text", selectedValues);
                }                

                public static MultiSelectList Links(int sectionID, int? selectedID)
                {
                    List<SelectListItem> list = new List<SelectListItem>();
                    List<int> selectedValues = new List<int>();
                    DBDataContext db = Utils.DB.GetContext();
                    IEnumerable<NavigationLink> links = db.NavigationLinks.Where(x=>x.SectionID == sectionID).OrderBy(x => x.Position).Select(x => x);
                    if (links.Count() > 0)
                    {
                        foreach (NavigationLink lnk in links)
                        {
                            list.Add(new SelectListItem
                            {
                                Text = lnk.Title,
                                Value = lnk.ID.ToString(),
                                Selected = ((selectedID != null) ? ((lnk.ID.Equals((int)selectedID)) ? true : false) : ((lnk.ID.Equals(links.First().ID)) ? true : false))
                            });
                        }

                        selectedValues.Add(((selectedID != null) ? (int)selectedID : links.First().ID));
                    }
                    return new MultiSelectList(list, "Value", "Text", selectedValues);
                }

            }

            public static MultiSelectList SecurityRoleList(IEnumerable<SecurityRole> selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<int> selectedRoles = new List<int>();
                if (selected != null)
                {
                    foreach (SecurityRole r in selected)
                    {
                        foreach (SecurityRole item in Enum.GetValues(typeof(SecurityRole)))
                        {
                            if (r == item)
                            {
                                selectedRoles.Add((int)item);
                            }
                        }
                    }
                }

                foreach (SecurityRole item in Enum.GetValues(typeof(SecurityRole)))
                {
                    list.Add(new SelectListItem
                    {
                        Text = Enum.GetName(typeof(SecurityRole), item),
                        Value = ((int)item).ToString()

                    });
                }

                return new MultiSelectList(list, "Value", "Text", selectedRoles);
            }
        }

        public static class DropDownElement
        {
            public static SelectList MediaTypes(string selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<string> arr = new List<string>();
                arr.Add("Image");
                arr.Add("Flash");
                foreach (string s in arr)
                {
                    list.Add(new SelectListItem
                    {
                        Text = s, 
                        Value = s.Substring(0, 1).ToUpper(),
                        Selected = ((!string.IsNullOrEmpty(selected)) ? ((s.Equals(selected)) ? true : false) : ((s.Equals("I") ? true : false)))
                    });
                }

                return new SelectList(list, "Value", "Text", selected);
            }

            public static SelectList Types(string selectedValue)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<Models.Type> data = db.Types.Select(x => x);
                foreach (Models.Type t in data)
                {
                    list.Add(new SelectListItem
                    {
                        Text = t.Name,
                        Value = t.ID.ToString(),
                        Selected = ((!string.IsNullOrEmpty(selectedValue)) ? ((t.Name.Equals(selectedValue)) ? true : false) : ((t.Name.Equals("Image")) ? true : false))
                    });
                }

                return new SelectList(list, "Value", "Text", selectedValue);
            }

            public static SelectList Categories (string selectedValue)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<Models.Category> cat = db.Categories.Select(x => x);
                list.Add(new SelectListItem
                {
                    Text = "Select Category",
                    Value = "0"
                });
                foreach (Models.Category c in cat)
                {
                    list.Add(new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.ID.ToString(),
                        Selected = ((!string.IsNullOrEmpty(selectedValue)) ? ((c.Name.Equals(selectedValue)) ? true : false) : ((c.ID == cat.First().ID) ? true : false))
                    });
                }

                return new SelectList(list, "Value", "Text", selectedValue);
            }

            public static SelectList WebPages(int tempId, string selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<WebPage> data = db.WebPages.Where(x=>x.TemplateID == tempId).OrderBy(x => x.Title).Select(x => x);
                if (data.Count() > 0)
                {
                    foreach(WebPage p in data)
                    {
                        list.Add(new SelectListItem
                        {
                            Text = p.Title,
                            Value = p.ID.ToString(),
                            Selected = ((!string.IsNullOrEmpty(selected)) ? ((p.ID == Convert.ToInt32(selected)) ? true : false) : ((p.ID == data.First().ID) ? true : false))
                        });  
                    }
                }

                return new SelectList(list, "Value", "Text", selected);

            }

            public static SelectList PredefinedForms()
            {
                List<SelectListItem> list = new List<SelectListItem>();
                DirectoryInfo info = new DirectoryInfo(HttpContext.Current.Server.MapPath("/Content/forms"));
                FileInfo [] files = info.GetFiles();
                foreach (FileInfo f in files)
                {
                    string name = f.Name.Split('.')[0];
                    name = name.Replace("_", " ");
                    list.Add(new SelectListItem
                    {
                        Text = name,
                        Value = "/Content/forms/" + f.Name
                    });
                }

                return new SelectList(list, "Value", "Text", null);
            }

            public static SelectList Templates(int? selectedTemplate)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                DBDataContext db = Utils.DB.GetContext();
                IEnumerable<Template> data = db.Templates.OrderBy(x => x.Name).Select(x => x);
                if (data.Count() > 0)
                {
                    foreach (Template t in data)
                    {
                        list.Add(new SelectListItem
                        {
                            Text= t.Name,
                            Value = t.ID.ToString(),
                            Selected = ((selectedTemplate == null) ? ((data.First().ID == t.ID) ? true : false) : ((selectedTemplate == t.ID) ? true : false))
                        });
                    }
                }

                return new SelectList(list, "Value", "Text", ((selectedTemplate == null) ? data.First().ID : (int)selectedTemplate));
            }

            public static SelectList FileCategories(string selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Utils.FileType.Category> cats = Utils.FileType.Categories;
                foreach (Utils.FileType.Category c in cats)
                {
                    string ext = "";
                    foreach (string s in c.Extensions)
                    {
                        ext += " | " + s;
                    }
                    ext = ext.Substring(3);

                    list.Add(new SelectListItem
                    {
                        Text = c.Name + " (" + ext + ")",
                        Value = c.Name,
                        Selected = ((c.Name.Equals(selected)) ? true : false)
                    });
                }

                return new SelectList(list, "Value", "Text", selected);
            }

            public static SelectList USStates(Lib.Type.GeographyNameType geoType, string selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                string[] states = Utils.Data.USStates(HttpContext.Current.Server.MapPath("~/Content/xml/"), geoType);

                foreach (string s in states)
                {
                    list.Add(new SelectListItem
                    {
                        Text = s,
                        Value = s,
                        Selected = ((s.Equals(selected)) ? true : false)
                    });
                }

                return new SelectList(list, "Value", "Text", selected);
            }

            public static SelectList WorldCountries(Lib.Type.GeographyNameType geoType, string selected)
            {
                List<SelectListItem> list = new List<SelectListItem>();
                string[] countries = Utils.Data.WorldCountries(HttpContext.Current.Server.MapPath("~/Content/xml/"), geoType);
                foreach (string c in countries)
                {
                    list.Add(new SelectListItem
                    {
                        Text = c,
                        Value = c,
                        Selected = ((c.Equals(selected)) ? true : false)
                    });
                }

                return new SelectList(list, "Value", "Text", selected);

            }
        }
    }

}