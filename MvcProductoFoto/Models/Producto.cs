using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Newtonsoft.Json;

namespace MvcProductoFoto.Models
{
   public class Producto
    {
       public String id { get; set; }
       public String nombre { get; set; }
       
       public double precio { get; set; }

       [JsonIgnore]
       public List<Foto> Fotos { get; set; }
    }
}
