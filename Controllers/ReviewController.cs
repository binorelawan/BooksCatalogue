using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BooksCatalogue.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksCatalogue.Controllers
{
    public class ReviewController : Controller
    {
        private string apiEndpoint = "https://bookscatalogueapi-dicoding.azurewebsites.net/api/";

        public ReviewController()
        {
        }

        // GET: Review/AddReview/2
        public async Task<IActionResult> AddReview(int? bookId)
        {
            if (bookId == null)
            {
                return NotFound();
            }

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, apiEndpoint + "books/" + bookId);

            HttpResponseMessage response = await client.SendAsync(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    string responseString = await response.Content.ReadAsStringAsync();
                    var book = JsonSerializer.Deserialize<Book>(responseString);

                    ViewData["BookId"] = bookId;
                    return View("Add");
                case HttpStatusCode.NotFound:
                    return NotFound();
                default:
                    return ErrorAction("Error. Status code = " + response.StatusCode + ": " + response.ReasonPhrase);
            }
        }

        // POST: Review/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([Bind("Id,BookId,ReviewerName,Rating,Comment")] Review review)
        {

            if (ModelState.IsValid)
            {
                var httpContent = new[] {
                    new KeyValuePair<string, string>("bookId", review.BookId.ToString()),
                    new KeyValuePair<string, string>("reviewerName", review.ReviewerName),
                    new KeyValuePair<string, string>("rating", review.Rating.ToString()),
                    new KeyValuePair<string, string>("comment", review.Comment)
                };

                HttpClient client = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(httpContent);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiEndpoint + "review/");
                request.Content = content;

                HttpResponseMessage response = await client.SendAsync(request);

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Created:
                        return Redirect("/Books/Details/" + review.BookId);
                    default:
                        return ErrorAction("Error. Status code = " + response.StatusCode);
                }
            }

            return View(review);
        }

        private ActionResult ErrorAction(string message)
        {
            return new RedirectResult("/Home/Error?message=" + message);
        }
    }
}