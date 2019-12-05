using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSearch.Models
{
    public class Answers
    {
        public String id { get; set; } = Guid.NewGuid().ToString();
        public String Title { get; set; }
        public List<String> Tags { get; set; } = new List<string>(); 
        public String ExplainText { get; set; }
    }
}