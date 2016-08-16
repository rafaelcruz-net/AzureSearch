using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureSearch.Models
{
    [SerializePropertyNamesAsCamelCase]
    public class Product
    {
        public String Id { get; set; } = Guid.NewGuid().ToString();
        public String Name { get; set; }
        public List<String> Categoria { get; set; } = new List<string>(); 
        public Double Preco { get; set; }
        public String Descricao { get; set; }
    }
}