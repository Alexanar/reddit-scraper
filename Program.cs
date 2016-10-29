using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Net.Http;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using RedditScrapper;
using System.ComponentModel;

//****************************************************************************
//  BY ARA ALEXANIAN-Farr
//  with scrapping method based on http://www.dotnetperls.com/scraping-html
//****************************************************************************
public class Program
{
    private string address = "https://www.reddit.com/r/";
    public int maxPages { get; set; } = 1;
    public string fileDir { get; set; }

    //xaml references
    MainWindow xamlInterface;

    public Program(MainWindow mainWindow)
    {
        xamlInterface = mainWindow;
    }

    public void RunScrapper()
    {
        int pageNumber = 1;    //starter page number
        //create client
        WebClient client = new WebClient();
        client.Headers.Add(HttpRequestHeader.Cookie, "over18=1"); //add over18 cookie to client
        string website = client.DownloadString(address);         //download strings from website

        OutputWriteLine("Scanning subreddit...");

        //page scanner
        LinkFinder mainPage = new LinkFinder();
        //converted strings
        string trimmedLink = null;
        string savedLinkName = null;

        //loop through pages until max is reached, then end program
        while (pageNumber <= maxPages)
        {
            OutputWriteLine("Page: " + pageNumber);
            //loop through links with imgur and download them
            foreach (LinkItem i in mainPage.Find(website))
            {
                //check if link is an imgur link
                if (i.ToString().Contains("http://i.imgur.com/") || i.ToString().Contains("https://i.imgur.com/"))
                {
                    //set link var to file link
                    trimmedLink = i.ToString();
                    //check if file is a jpg or gif and then save if true
                    if (SupportedExtension(trimmedLink))
                    {
                        //save gifv and gif as webm for smaller files
                        trimmedLink = trimmedLink.Replace("gifv", "webm");
                        trimmedLink = trimmedLink.Replace("gif", "webm");
                        //trim link for save format
                        savedLinkName = SaveFormat(trimmedLink);
                        //try to download or write error
                        DownloadFile(client, trimmedLink, savedLinkName, fileDir);
                    }
                }

                //check if link is a gfycat link and filter out links with text
                if (i.ToString().Contains("http://gfycat.com/") && !i.ToString().Contains(" ")
                    || i.ToString().Contains("https://gfycat.com/") && !i.ToString().Contains(" "))
                {
                    //trim link
                    trimmedLink = i.ToString();
                    //try different gfycat webm servers
                    trimmedLink += ".webm";  //make gfycat a webm
                    //try different gfycat hosting servers servers
                    trimmedLink = trimmedLink.Replace("https://", "https://zippy.");
                    trimmedLink = trimmedLink.Replace("http://", "http://zippy.");
                    if (!FileExists(trimmedLink))
                    {
                        trimmedLink = trimmedLink.Replace("https://zippy.", "https://fat.");
                        trimmedLink = trimmedLink.Replace("http://zippy.", "http://fat.");
                    }
                    if (!FileExists(trimmedLink))
                    {
                        trimmedLink = trimmedLink.Replace("https://fat.", "https://giant.");
                        trimmedLink = trimmedLink.Replace("http://fat.", "http://giant.");
                    }
                    if (!FileExists(trimmedLink))       //if file still isnt found
                    {
                        OutputWriteLine("ERROR: Could not get hosting server for " + trimmedLink);
                    }
                    else
                    {
                        savedLinkName = SaveFormat(trimmedLink);      //trim link for save format
                        DownloadFile(client, trimmedLink, savedLinkName, fileDir);      //try to download or write error
                    }
                }

                //get next page
                if (i.ToString().Contains("count=") && i.ToString().Contains("after="))
                {
                    OutputWriteLine("Getting next page...");
                    pageNumber++;
                    address = i.Href; //set new address to new page's href
                }

            }

            mainPage = new LinkFinder();    //new linkfinder object for link list

            //try to download the next page strings, if execption, exit loop and end
            try
            {
                website = client.DownloadString(address);
            }
            catch
            {
                OutputWriteLine("Couldn't Get Next Page");
                break;
            }

        }

        //end program after desired pages has been met
        OutputWriteLine("Finished Scrapping");
        ResetData();

        DialogResult message = System.Windows.Forms.MessageBox.Show("Do you wish to see the output folder?", "Scrapper Completed", MessageBoxButtons.YesNo);
        if (message == DialogResult.Yes)
        {
            try
            {
                Process.Start(fileDir);
                ChangeView(Visibility.Visible, Visibility.Hidden);
            }
            catch
            {
                DialogResult errorMessage = System.Windows.Forms.MessageBox.Show("Failed to open directory!", "Error!", MessageBoxButtons.OK);
                ChangeView(Visibility.Visible, Visibility.Hidden);
            }
        }
        else if (message == DialogResult.No
            || message == DialogResult.Abort)
        {
            ChangeView(Visibility.Visible, Visibility.Hidden);
        }
    }

    /*****************************
    *********|  METHODS | **********
    *****************************/

    //download method
    public void DownloadFile(WebClient clientO, string fileLink, string saveName, string fileDirectory)
    {
        try
        {
            //check if file exists, downloads if doesnt exist
            if (File.Exists(Path.Combine(fileDirectory, saveName)))
            {
                OutputWriteLine(fileDirectory + saveName + " already exists... skipping");
            }
            else
            {
                //download
                OutputWriteLine("Found " + saveName);
                clientO.DownloadFile(fileLink, Path.Combine(fileDirectory, saveName));
                OutputWriteLine("Downloaded " + saveName);
            }
        }
        catch
        {
            OutputWriteLine("Failed to Download " + saveName);
        }
    }

    //convert link into savable format
    public static string SaveFormat(string link)
    {
        string saveName = link.Replace(":", "")
                              .Replace("/", "");
        return saveName;
    }

    //bool method to check if file extentsions
    public static bool SupportedExtension(string compared)
    {
        if (compared.EndsWith(".jpg")
            || compared.EndsWith(".png")
            || compared.EndsWith(".gif")
            || compared.EndsWith(".gifv"))
        {
            return true;

        }
        return false;
    }

    //check if url exists
    static public bool FileExists(string url)
    {
        WebRequest webRequest = WebRequest.Create(url);
        WebResponse webResponse = null;
        try
        {
            webResponse = webRequest.GetResponse();
            webResponse.Close();
        }
        catch //get exception
        {
            return false;
        }
        return true;
    }

    public void SubredditGet()
    {
        address = "https://www.reddit.com/r/" + xamlInterface.subredditTextbox.Text;       //user enter subreddit extention
        address.Trim();
    }
    //get page from slider
    public void MaxPagesGet()
    {
        maxPages = (int)xamlInterface.pageSlider.Value;
    }
    public void FolderDirectoryGet()
    {
        DialogResult result = DialogResult.Cancel;
        //make user find folder until result is ok, or if cancelled exit program
            try
            {
                FolderBrowserDialog fileDialog = new FolderBrowserDialog();
                result = fileDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    fileDir = @fileDialog.SelectedPath;
                    xamlInterface.directoryTextbox.Text = fileDir.Replace(@"\", "/");
                }
            }
            catch
            {
                DialogResult message = System.Windows.Forms.MessageBox.Show("Folder Browser failed!", "Error!", MessageBoxButtons.OK);
            }
    }

    public void FolderTextDirectoryGet()
    {
        fileDir = xamlInterface.directoryTextbox.Text;
    }

    private void OutputWriteLine(string text)
    {
        xamlInterface.Dispatcher.Invoke(() => xamlInterface.outputText.Text += "\n" + text);
        xamlInterface.Dispatcher.Invoke(() => xamlInterface.textScroller.ScrollToEnd());
    }
    private void ChangeView(Visibility main, Visibility outPut)
    {
        xamlInterface.Dispatcher.Invoke(() => xamlInterface.ShowWindow(main, outPut));
    }
    private void ResetData()
    {
        xamlInterface.Dispatcher.Invoke(() => address = "https://www.reddit.com/r/" + xamlInterface.subredditTextbox.Text);
    }






}
