using AzureSearch.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureSearch.Controllers
{

    public class HomeController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SaveDocument(Answers model)
        {
            string searchServiceName = "demo-icatu-azure-search";
            string apiKey = "F8096514640A1CE105729B47582E93BA";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("icatu-demo-search");


            var tags = model.Tags.FirstOrDefault().ToString().Split(',').ToList();

            model.Tags.Clear();
            model.Tags.AddRange(tags);

            var index = new IndexAction<Answers>[]
            {
                IndexAction.Upload(model)
            };

            //Criando o documento a ser indexado
            var batch = IndexBatch.New(index);

            //Atualizado o indice com o novo documento
            try
            {
                await indexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
            catch (System.Exception ex)
            {
                throw ex;

            }

            return View("Index");

        }

        
        public ActionResult Search()
        {
            return View("Search");
        }

        [HttpPost]
        public async Task<ActionResult> Search(string searchText, string filter = null)
        {

            string searchServiceName = "demo-icatu-azure-search";
            string apiKey = "F8096514640A1CE105729B47582E93BA";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("icatu-demo-search");

            var sp = new SearchParameters();

            if (!String.IsNullOrEmpty(filter))
                sp.Filter = filter;

            var parameters = new SearchParameters
            {
                // Enter Hotel property names into this list so only these values will be returned.
                // If Select is empty, all values will be returned, which can be inefficient.
                IncludeTotalResultCount = true,
                ScoringProfile = "ScoringTitle",
                Top = 3,
                HighlightFields = new List<string> { "ExplainText", "Title" },
                HighlightPreTag = "<font style=\"color:blue; background-color:yellow;\">",
                HighlightPostTag = "</font>",
                QueryType = QueryType.Full,
                SearchMode = SearchMode.Any

            };

            DocumentSearchResult<Answers> response = await indexClient.Documents.SearchAsync<Answers>(searchText, parameters);

            var answer = new List<Answers>();

            foreach (var item in response.Results)
            {
                answer.Add(new Answers()
                {
                    Title = item.Document.Title,
                    ExplainText = item.Document.ExplainText,
                    Tags = item.Document.Tags
                });
            }

            return View("Search", answer);

        }

        public async Task<ActionResult> Suggest(string term)
        {

            string searchServiceName = "demo-icatu-azure-search";
            string apiKey = "F8096514640A1CE105729B47582E93BA";
            string suggestName = "namesuggestion";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("icatu-demo-search");

            var suggestion = await indexClient.Documents.SuggestAsync(term, suggestName);

            return Json(new
            {
                Result = suggestion.Results.Select(x => x.Text).ToArray()
            }, JsonRequestBehavior.AllowGet);

        }

        





    }
}