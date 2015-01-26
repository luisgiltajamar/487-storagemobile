using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.MobileServices;
using MvcProductoFoto.Models;
using MvcProductoFoto.Utilidades;

namespace MvcProductoFoto.Controllers
{
    public class ProductosController : Controller
    {
        public MobileServiceClient MobileService = new MobileServiceClient(
    "https://servicioproductosfoto.azure-mobile.net/",
    "YsHbUtxPOKVXMeRshodAnpcAkgWhsg67"
);

        private String urlBase = "https://pruebasstorageluis.blob.core.windows.net/fotosproductos/";
        private string cont = "fotosproductos";
        private string key = "/wJoTVT6tZARGVu2sGdokAllgVkxflB11GyTpkWm+bWKE/csoQRSBW8QUlHIBWrGbKd6NY+rw2OjXha+lyB+Ng==";
        private string cuenta = "pruebasstorageluis";
        // GET: Productos
        public async Task<ActionResult> Index()
        {
            var tabla = MobileService.GetTable<Producto>();
            var tFoto = MobileService.GetTable<Foto>();
            
            var data =await tabla.CreateQuery().ToListAsync();

            foreach (var producto in data)
            {
                var fotos = await tFoto.CreateQuery().
                    Where(o => o.idProducto == producto.id).ToListAsync();
                producto.Fotos = fotos;
            }

            return View(data);
        }

        public ActionResult Alta()
        {
            return View(new Producto());
        }

        public async Task<ActionResult> Fotos(String id)
        {
            var t = MobileService.GetTable<Foto>();
            var data =await t.CreateQuery().
                Where(o => o.idProducto == id).ToListAsync();

                
            return View(data);
        }

        [HttpPost]
        public async Task<ActionResult> Alta(Producto pr,
            HttpPostedFileBase[] ficheros)
        {
            var tabla = MobileService.GetTable<Producto>();
            var tFoto = MobileService.GetTable<Foto>();
            await tabla.InsertAsync(pr);
            var sto = new AlmacenamientoAzure(cuenta, key);
            int n = 1;
            foreach (var httpPostedFileBase in ficheros)
            {
                if (httpPostedFileBase != null
                    && httpPostedFileBase.ContentLength > 0)
                {
                    var nom = pr.id + n +
                              httpPostedFileBase.
                              FileName.Substring(
                              httpPostedFileBase.FileName.LastIndexOf("."));
                    sto.SubirObjeto(httpPostedFileBase.InputStream,nom,cont);

                    var foto = new Foto()
                    {
                        idProducto = pr.id,
                        url = urlBase + nom
                    };
                  await  tFoto.InsertAsync(foto);
                  n++;
                }
            }



            return RedirectToAction("Index");
        }

    }
}