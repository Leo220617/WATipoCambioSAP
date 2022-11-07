using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WATickets.Models;

namespace WATickets.Controllers
{
    public class TipoCambioController: ApiController
    {
            G G = new G();
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetAsync()
        {

            try
            {
                HttpClient clienteProd = new HttpClient();
                clienteProd.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "https://tipodecambio.paginasweb.cr/api//" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                try
                {
                    HttpResponseMessage response3 = await clienteProd.GetAsync(url);
                    if (response3.IsSuccessStatusCode)
                    {
                        response3.Content.Headers.ContentType.MediaType = "application/json";
                        var res = await response3.Content.ReadAsStringAsync();


                        try
                        {
                            var respZoho = await response3.Content.ReadAsAsync<RespuestaTipoCambio>();

                            var tp = (SAPbobsCOM.SBObob)Conexion.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);

                            tp.SetCurrencyRate(G.ObtenerConfig("Moneda"), DateTime.Now, Convert.ToDouble(respZoho.venta));

                        }
                        catch (Exception)
                        {


                        }

                    }
                }
                catch (Exception)
                {


                }




                return Request.CreateResponse(HttpStatusCode.OK, "procesado con exito");
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }


        }
    }
}