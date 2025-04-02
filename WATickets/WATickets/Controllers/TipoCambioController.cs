using System.Text.Json;
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
                var valorCambioVenta = 0.0d;
                HttpClient clienteProd = new HttpClient();
                clienteProd.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = "https://apis.gometa.org/tdc/tdc.json?fbclid=IwAR1Wfr6SFwV8_0-x9n5JrjMmNTkOcUIWdekp1Sc6sFpTnIuP29ok-aVuQWI";//"https://tipodecambio.paginasweb.cr/api//" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                try
                {
                    HttpResponseMessage response3 = await clienteProd.GetAsync(url);
                    if (response3.IsSuccessStatusCode)
                    {
                        //        response3.Content.Headers.ContentType.MediaType = "application/json";
                        var res = await response3.Content.ReadAsStringAsync();


                        try
                        {
                            //var respZoho = await response3.Content.ReadAsAsync<RespuestaTipoCambio>();
                            var respZoho = JsonSerializer.Deserialize<RespuestaTipoCambio>(res);

                            if (Convert.ToDouble(respZoho.venta) > 10000)
                            {
                                respZoho.venta = respZoho.venta.Replace(".", ",");
                            }
                            valorCambioVenta = Convert.ToDouble(respZoho.venta);


                        }
                        catch (Exception ex)
                        {
                            var texto = DateTime.Now.ToLongDateString() + " -> Primer intento: " + ex.Message + "\n";
                            texto += "===================================================";
                            G.GuardarTxt("BitacoraTP_" + DateTime.Now.ToShortDateString() + ".txt", texto);

                        }

                    }
                }
                catch (Exception ex)
                {
                    var texto = DateTime.Now.ToLongDateString() + " -> Primer intento: " + ex.Message + "\n";
                    texto += "===================================================";
                    G.GuardarTxt("BitacoraTP_" + DateTime.Now.ToShortDateString() + ".txt", texto);


                }
                 
                if(valorCambioVenta == 0)
                {
                    clienteProd = new HttpClient();
                    clienteProd.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    url = "https://tipodecambio.paginasweb.cr/api//" + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
                    try
                    {
                        HttpResponseMessage response3 = await clienteProd.GetAsync(url);
                        if (response3.IsSuccessStatusCode)
                        {
                            //        response3.Content.Headers.ContentType.MediaType = "application/json";
                            var res = await response3.Content.ReadAsStringAsync();


                            try
                            {
                                //var respZoho = await response3.Content.ReadAsAsync<RespuestaTipoCambio>();
                                var respZoho = JsonSerializer.Deserialize<RespuestaTP>(res);

                                valorCambioVenta = Convert.ToDouble(respZoho.venta);

                            }
                            catch (Exception ex)
                            {
                                var texto = DateTime.Now.ToLongDateString() + " -> Segundo intento: " + ex.Message + "\n";
                                texto += "===================================================";
                                G.GuardarTxt("BitacoraTP_" + DateTime.Now.ToShortDateString() + ".txt", texto);

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        var texto = DateTime.Now.ToLongDateString() + " -> Segundo intento: " + ex.Message + "\n"; 
                        texto += "===================================================";
                        G.GuardarTxt("BitacoraTP_" + DateTime.Now.ToShortDateString() + ".txt", texto);


                    }
                }


               



                if(valorCambioVenta > 0)
                {
                    var tp = (SAPbobsCOM.SBObob)Conexion.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
                    tp.SetCurrencyRate(G.ObtenerConfig("Moneda"), DateTime.Now, valorCambioVenta);
                    Conexion.Desconectar();
                }
                else
                {
                    throw new Exception("No se ha podido ingresar el Tipo de Cambio ninguno de los dos nos genero un Tipo de cambio valido");
                }

              

                return Request.CreateResponse(HttpStatusCode.OK, "procesado con exito");
            }
            catch (Exception ex)
            {
                Conexion.Desconectar();
                var texto = DateTime.Now.ToLongDateString() + " ->  " + ex.Message + "\n";
                texto += "===================================================";
                G.GuardarTxt("BitacoraTP_" + DateTime.Now.ToShortDateString() + ".txt", texto);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }


        }
    }
}