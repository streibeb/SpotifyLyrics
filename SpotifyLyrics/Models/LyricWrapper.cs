using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SpotifyAPI.SpotifyLocalAPI;

namespace SpotifyLyrics.Models
{
    public class LyricWrapper
    {
        private Track _currentTrack;

        internal string GetArtist()
        {
            var sb = new StringBuilder();
            foreach (char c in _currentTrack.GetArtistName())
            {
                if (!char.IsPunctuation(c) && !char.IsSeparator(c))
                {
                    sb.Append(char.ToLower(c));
                }
            }
            string artistName = sb.ToString();
            return artistName;
        }

        internal string GetTrackTitle(bool includeParentheses)
        {
            var sb = new StringBuilder();
            foreach (char c in _currentTrack.GetTrackName())
            {
                if (c == '(' || c == ')' || (!char.IsPunctuation(c) && !char.IsSeparator(c)))
                {
                    sb.Append(char.ToLower(c));
                }
            }
            var trackName = sb.ToString().Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            return !includeParentheses 
                ? trackName[0] 
                : trackName.Where(t => !t.StartsWith("feat")).Aggregate(string.Empty, (current, t) => current + t);
        }

        public string GetLyrics(Track currentTrack)
        {
            _currentTrack = currentTrack;
            string[] sURL =
            {
                "http://www.azlyrics.com/lyrics/" + GetArtist() + "/" + GetTrackTitle(false) + ".html",
                "http://www.azlyrics.com/lyrics/" + GetArtist() + "/" + GetTrackTitle(true) + ".html"
            };

            for (int i = 0; i < sURL.Length; i++)
            {
                WebRequest checkURL = WebRequest.Create(sURL[i]);
                checkURL.Proxy = null;

                try
                {
                    return ScrapeLyrics(checkURL.GetResponse().GetResponseStream());
                }
                catch (WebException wex)
                {
                    // Catch 404 Error
                    if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.NotFound)
                    {
                        continue;
                    }
                    return "An unexpected error occurred";
                }
            }
            return "No lyrics found";
        }

        private string ScrapeLyrics(Stream objStream)
        {
            using (objStream)
            {
                var objReader = new StreamReader(objStream);

                var sb = new StringBuilder();
                bool lyricRead = false;
                while (!objReader.EndOfStream)
                {
                    string sLine = objReader.ReadLine();
                    if (sLine != null && sLine.Trim().StartsWith("<!-- start of lyrics -->"))
                    {
                        lyricRead = true;
                        continue;
                    }
                    if (sLine != null && sLine.Trim().StartsWith("<!-- end of lyrics -->"))
                    {
                        break;
                    }
                    if (sLine != null && lyricRead)
                    {
                        var l = sLine.Trim();
                        l = l.Replace("<br />", "\r\n");
                        l = l.Replace("<i>", "");
                        l = l.Replace("</i>", "");
                        sb.Append(l);
                    }
                }
                return sb.ToString();
            }
        }
    }
}
