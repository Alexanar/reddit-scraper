using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//linkitem and linkfinder based on http://www.dotnetperls.com/scraping-html
//link item in the linkfinder list
public struct LinkItem
{
    //link parts
    public string Href;
    public string Text;

    //override to return struct vars
    public override string ToString()
    {
        return Href + Text;
    }
}

public class LinkFinder
{


    //return list of link matches in downloaded string file
    public List<LinkItem> Find(string file) 
    {
        List<LinkItem> list = new List<LinkItem>();

        // 1.
        // Find all matches in file.
        MatchCollection matchCollection = Regex.Matches(file, @"(<a.*?>.*?</a>)",
        RegexOptions.Singleline);

        // 2.
        // Loop matches.
        foreach (Match match in matchCollection)
        {
            string value = match.Groups[1].Value;
            LinkItem i = new LinkItem();

            // 3.
            // Get href attribute.
            Match matchTwo = Regex.Match(value, @"href=\""(.*?)\""",
            RegexOptions.Singleline);
            if (matchTwo.Success)
            {
                i.Href = matchTwo.Groups[1].Value;
            }

            // 4.
            // Remove inner tags from text.
            string text = Regex.Replace(value, @"\s*<.*?>\s*", "",
            RegexOptions.Singleline);
            i.Text = text;

            list.Add(i);

        }
        return list;
    }




}
