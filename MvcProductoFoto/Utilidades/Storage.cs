using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MvcProductoFoto.Utilidades
{
   public class AlmacenamientoAzure
   {
       private StorageCredentials credenciales;
       private CloudStorageAccount cuenta;

       public AlmacenamientoAzure(String account, String key)
       {
            credenciales=new StorageCredentials(account,key);
           cuenta=new CloudStorageAccount(credenciales,true);
       }

       private CloudBlobContainer ComprobarContenedor(String contenedor)
       {
          var cliente = cuenta.CreateCloudBlobClient();
           var container = cliente.GetContainerReference(contenedor);
           container.CreateIfNotExists();
           return container;
       }

       public List<CloudBlockBlob> Listar(String contenedor)
       {

           var container = ComprobarContenedor(contenedor);
           
           var l=new List<CloudBlockBlob>();

           foreach (var blobs in container.ListBlobs(null,false))
           {
               if (blobs is CloudBlockBlob)
               {
                   l.Add(blobs as CloudBlockBlob);
               }

           }
           return l;
       }

       public void SubirObjeto(Stream objeto, String nombre, String contenedor)
       {
           var container = ComprobarContenedor(contenedor);

           var blob = container.GetBlockBlobReference(nombre);
           blob.UploadFromStream(objeto);

           objeto.Close();
       }

       public void BorrarObjeto(String nombre, String contenedor)
       {
           var container = ComprobarContenedor(contenedor);

           var blob = container.GetBlockBlobReference(nombre);
           blob.DeleteIfExists();

       }

   }
}
