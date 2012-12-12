using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Facebook;
using System.Text.RegularExpressions;
using Flannel;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Project Flannel!";

            return View();
        }

        public ActionResult Main()
        {
            ViewBag.Message = "Please log in first";

            if (Session["AccessToken"] != null)
            {
                var accessToken = Session["AccessToken"].ToString();
                var client = new FacebookClient(accessToken);

                try
                {
                    dynamic result = client.Get("me", new { fields = "name,id,gender,likes" });
                    //dynamic result = client.Get("me");
                    string name = result.name;
                    string id = result.id;
                    string gender = result.gender;
                    JsonObject likes = result.likes;
                    if (likes == null)
                        ViewBag.Message = "We didn't get anything back";
                    else
                        ViewBag.Message = "Test: ";

                    JsonArray list_likes = (JsonArray)likes.ElementAt(0).Value;
                    int element_id = 0;
                    foreach (dynamic element in list_likes)
                    {
                        if (element.category == "Musician/band" || element.category == "Entertainer" || element.category == "Artist")
                        {
                            if (Metadata.IsInDB(element.name))
                            {
                                element_id++;
                                ViewBag.Likes += "<div class=\"music_box\">";
                                ViewBag.Likes += "<input type=\"checkbox\" id=\"" + element_id + "\" value=\"" + element.name + "\" style=\"margin-top:10px;\">" + element.name + "<br>";
                                ViewBag.Likes += "</div>";
                            }
                        }
                    }
                    ViewBag.highest_id = element_id;
                }
                catch (FacebookOAuthException x)
                {

                }
            }

            return View();
        }

        public ActionResult Results(FormCollection formCollection)
        {
            ViewBag.Message = "Input:<br \\>";

            String url = Request.Url.ToString();

            List<string> Artists = new List<string>();

            //ViewBag.Message = formCollection["bands"];
            Regex find_bands = new Regex("\\|[^\\|]{1,}\\|");
            MatchCollection matches = find_bands.Matches(url);
            foreach (Match match in matches)
            {
                String found_match = match.ToString().Substring(1).TrimEnd('|') + "<br \\>";
                Artists.Add(match.ToString().Substring(1).TrimEnd('|'));
                ViewBag.Message += found_match;
            }
            ViewBag.Message += "<br \\><br \\>Output:<br \\>";




            List<Song> test = Rec.GeneratePlaylist(Artists);
            foreach (Song sng in test)
            {
                ViewBag.Message += sng.Title + "<br \\>";
                ViewBag.Message += sng.Artist + "<br \\>";
            }
            return View();
        }

        public void FacebookLogin(string uid, string accessToken)
        {
            var context = this.HttpContext;
            context.Session["AccessToken"] = accessToken;
        }

        public ActionResult FacebookLoginNoJs()
        {
            return Redirect("https://www.facebook.com/dialog/oauth?client_id=435662283157258&redirect_uri=http://localhost:23232/Home/ConnectResponse&state=secret&scope=user_likes");
        }

        public ActionResult ConnectResponse(string state, string code, string error, string error_reason, string error_description, string access_token, string expires)
        {
            if (string.IsNullOrEmpty(error))
            {
                try
                {
                    var client = new FacebookClient();
                    dynamic result = client.Post("oauth/access_token",
                                              new
                                              {
                                                  client_id = "435662283157258",
                                                  client_secret = "66b1e2c2ba9eed935bce0c8fb4553f28",
                                                  redirect_uri = "http://localhost:23232/Home/ConnectResponse",
                                                  code = code
                                              });

                    Session["AccessToken"] = result.access_token;

                    if (result.ContainsKey("expires"))
                        Session["ExpiresIn"] = DateTime.Now.AddSeconds(result.expires);

                }
                catch
                {
                    // handle errors
                }
            }
            else
            {
                // Declined, check error
            }

            return RedirectToAction("Main");
        }

        public JsonResult isInDB(string id)
        {
            bool isValid = Metadata.IsInDB(id); //.. check
            var obj = new
            {
                valid = isValid
            };
            return Json(obj);
        }
    }
}
