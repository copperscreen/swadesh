using System.Web;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace Core
{
    public static class Scraper
    {
        static string Trim(string before, string item, string after)
        {
            return item.Substring(before.Length, item.Length - after.Length - before.Length);
        }
        const string SITE = "https://en.wiktionary.org";
        const string HREF_HEAD = "/wiki/Appendix:";
        const string HREF_TAIL = "_Swadesh_list";
        const string TITLE_HEAD = "Appendix:";
        const string TITLE_TAIL = " Swadesh list";
        static void ParseLinks(string url, Dictionary<string, string> result)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var links = htmlDoc.GetElementbyId("mw-pages").QuerySelectorAll("a");
            if (links[0].InnerText == "next page")
            {
                ParseLinks((new Uri(new Uri(url), System.Web.HttpUtility.HtmlDecode(links[0].GetAttributeValue("href", string.Empty)))).AbsoluteUri, result);
            }
            foreach (var link in links)
            {
                if (link.GetAttributeValue("title", string.Empty) == "Appendix:Swadesh lists") continue;
                if (!link.InnerHtml.StartsWith(TITLE_HEAD)) continue;
                if (!link.InnerHtml.EndsWith(TITLE_TAIL)) continue;
                string href = link.GetAttributeValue("href", string.Empty);
                result[Trim(HREF_HEAD, href, HREF_TAIL)] = Trim(TITLE_HEAD, link.InnerText, TITLE_TAIL);
            }
        }
        public static string[][] Get()
        {
            var result = new Dictionary<string, string>();
            ParseLinks("https://en.wiktionary.org/wiki/Category:Swadesh_lists_by_language", result);

            return result.Select(_ => new string[] { _.Key, _.Value }).OrderBy(_ => _[1]).ToArray();
        }
        static string[] cssClasses = new string[] { "odd", "even" };
        private static void ProcessRow(Dictionary<string, List<string>> result, int tableCount, List<string>? row, string key, int width)
        {
            List<string> list;
            if (!result.TryGetValue(key, out list))
            {
                result[row[0]] = list = new List<string>() { key };
                for (int i = 1; i < width; i++)
                {
                    list.Add(string.Empty);
                }
            }
            foreach (var column in row.Skip(1))
            {
                list.Add(column);
            }
        }

        static List<string> Numbers = new List<string> { "\u2116", "No.", "no." };
        static List<List<string>> ParseTable(string param)
        {
            string url = SITE + HREF_HEAD + param + HREF_TAIL;
            System.Diagnostics.Debug.WriteLine("Start " + url);
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            var result = new List<List<string>>();
            var rows = htmlDoc.QuerySelectorAll("table.wikitable > tbody > tr");
            var firstRow = rows[0].QuerySelectorAll("th,td");
            int engIndex = firstRow.TakeWhile(_ => !_.InnerText.StartsWith("English")).Count();
            if (engIndex >= firstRow.Count)
            {
                return result;
            }
            int start = Numbers.Contains(firstRow[0].InnerText.Trim()) ? 1 : 0;
            foreach (var row in rows)
            {
                var cells = row.QuerySelectorAll("th,td");
                var line = new List<string> { cells[engIndex].InnerText.Trim() };
                for (int i = start; i < engIndex; i++)
                {
                    line.Add(HttpUtility.HtmlDecode(cells[i].InnerText.Trim()));
                }
                for (int i = engIndex + 1; i < cells.Count; i++)
                {
                    line.Add(HttpUtility.HtmlDecode(cells[i].InnerText.Trim()));
                }
                result.Add(line);
            }
            System.Diagnostics.Debug.WriteLine("End " + url);
            return result;
        }
        public static CompiledTable Compiled(string[] urls)
        {
            var tables = urls.AsParallel().AsOrdered().Select(_ => ParseTable(_)).ToList();

            var dict = new Dictionary<string, List<string>>();
            int width = 1;
            int tableCount = 0;

            foreach (var table in tables)
            {
                ProcessRow(dict, tableCount, table[0], "English", width);
                foreach (var row in table.Skip(1))
                {
                    ProcessRow(dict, tableCount, row, row[0], width);
                }
                width += table[0].Count - 1;
                foreach (var val in dict.Values)
                {
                    if (val.Count < width)
                    {
                        for (int i = val.Count; i < width; i++)
                        {
                            val.Add(string.Empty);
                        }
                    }
                }
                tableCount++;
            }

            var result = dict.Values.Select(_ => _.ToArray()).ToList();
            var first = result.Take(1).ToArray();
            result.RemoveRange(0, 1);
            result.Sort((a, b) => string.Compare(a[0], b[0]));
            result.InsertRange(0, first);
            return new CompiledTable { table = result.ToArray(), widths = tables.Select(_ => _[0].Count - 1).ToArray() };
        }
    }
}