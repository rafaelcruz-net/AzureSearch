using AzureSearch.Models;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult SaveDocument(Product model)
        {
            string searchServiceName = "demo-search";
            string apiKey = "api-key";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient("product");

            //Criando o documento a ser indexado
            var document = IndexBatch.Upload(new Product[] { model });

            //Atualizado o indice com o novo documento
            indexClient.Documents.Index(document);

            return View("Index");

        }

        
        public ActionResult Search()
        {
            return View("Search");
        }

        [HttpPost]
        public ActionResult Search(string searchText, string filter = null)
        {

            string searchServiceName = "demo-search";
            string apiKey = "api-key";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient("product");

            var sp = new SearchParameters();

            if (!String.IsNullOrEmpty(filter))
                sp.Filter = filter;

            DocumentSearchResult<Product> response = indexClient.Documents.Search<Product>(searchText, sp);

            return View("Search", response.Results.Select(x => x.Document).ToList());

        }

        public ActionResult Suggest(string term)
        {

            string searchServiceName = "demo-search";
            string apiKey = "api-key";
            string suggestName = "namesuggestion";

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient("product");

            DocumentSuggestResult suggestion = indexClient.Documents.Suggest(term, suggestName);

            return Json(new
            {
                Result = suggestion.Results.Select(x => x.Text).ToArray()
            }, JsonRequestBehavior.AllowGet);

        }





    }
}