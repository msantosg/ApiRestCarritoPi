using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ApiRestCarritoPi.App_Code;
using ApiRestCarritoPi.Models;
using Newtonsoft.Json;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using System.IO;
using iText.Barcodes;
using iText.Layout.Element;
using iText.Kernel.Pdf.Xobject;

namespace ApiRestCarritoPi.Controllers
{
    [RoutePrefix("api/TransaccionesCarrito")]
    public class TransacCarritoController : ApiController
    {
        [HttpPost]
        [Route("AgregarEncabezadoPreventa")]
        public dynamic GrabarEncabezadoPreventa([FromBody] dynamic DatosEncabezado)
        {
            try
            {

                string guidlog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(DatosEncabezado), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.IN);
                using (var datab = new db_preventa_superEntities())
                {
                    string GuidPreventa = Guid.NewGuid().ToString();
                    preventa prev = new preventa();
                    prev.pre_cod_preventa = GuidPreventa;
                    prev.cli_nit_cliente = DatosEncabezado.nit_cliente.ToString();
                    prev.pre_estado_venta = 0;
                    prev.pre_fecha_venta = DateTime.Now;
                    prev.car_cod_carro = DatosEncabezado.ip_carrito.ToString();
                    prev.pre_precio_total = 0;
                    
                    datab.preventa.Add(prev);
                    datab.SaveChanges();
                    /* response.CodRes = 0;
                     response.msgRes = "OK";
                     response.GuidVenta = GuidPreventa;*/
                    clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(new { CodRes = 0, msgRes = "OK", GuidVenta = GuidPreventa }), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.OUT, guidlog);
                    return new { CodRes = 0, msgRes = "OK", GuidVenta = GuidPreventa };
                }

                //clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(response), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.OUT, guidlog);
            }
            catch (Exception ex)
            {
                /*response.CodRes = -1;
                response.msgRes = "Error al agregar una nueva compra.";
                response.GuidVenta = "";
                response.lstProductos = null;*/
                clsLog.EscribeLogErr(ex, "TransacCarrito.GrabarEncabezadoCarrito");

                return new { CodRes = -1, msgRes = "Error al agregar una nueva compra.", GuidVenta = "" };
            }
        }


        [HttpPost]
        [Route("AgregarProductos")]
        public dynamic GrabarProducto([FromBody] dynamic DatosProd)
        {
            try
            {
                var guidLog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(DatosProd), "TransacCarrito.GrabarProducto", clsLog.tInOut.IN);

                Producto prod = new Producto();
                string barra = DatosProd.Producto.ToString();
                int cant = Convert.ToInt32(DatosProd.Cantidad.ToString());

                using(var datab = new db_preventa_superEntities())
                {
                    var producto = datab.producto.FirstOrDefault(x => x.pro_correlativo.Equals(barra) && x.pro_estado == 0);

                    string ipCarrito = DatosProd.ip_carrito.ToString();
                    
                    var preventa = datab.preventa.Where(x => x.car_cod_carro.Equals(ipCarrito) && x.pre_estado_venta == 0).FirstOrDefault();

                    var dt_prev = datab.detalle_preventa.FirstOrDefault(x => x.pre_cod_preventa.Equals(preventa.pre_cod_preventa) && x.pro_correlativo.Equals(barra)); 
                    if(dt_prev != null)
                    {
                        dt_prev.dep_cantidad_venta = dt_prev.dep_cantidad_venta + cant;
                        datab.SaveChanges();
                    }
                    else
                    {
                        var detalle_prev = new detalle_preventa();
                        detalle_prev.dep_cod_dte_preventa = Guid.NewGuid().ToString();
                        detalle_prev.pre_cod_preventa = preventa.pre_cod_preventa;
                        detalle_prev.pro_correlativo = producto.pro_correlativo;
                        detalle_prev.dep_cantidad_venta = cant;
                        detalle_prev.dep_precio_venta = producto.pro_precio_venta;
                        detalle_prev.dep_descuento_prod = producto.pro_descuento_prod;
                        datab.detalle_preventa.Add(detalle_prev);
                        datab.SaveChanges();
                    }
                    /*response.CodRes = 0;
                    response.msgRes = "OK";
                    response.GuidVenta = preventa.pre_cod_preventa;
                    response.lstProductos = null;*/
                    clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(new { CodRes = 0, msgRes = "OK", GuidVenta = preventa.pre_cod_preventa }), "TransacCarrito.GrabarProducto", clsLog.tInOut.OUT, guidLog);
                    return new { CodRes = 0, msgRes = "OK", GuidVenta = preventa.pre_cod_preventa };
                }

                
            }
            catch (Exception ex)
            {
                /*response.CodRes = -1;
                response.msgRes = "Error al agregar un nuevo producto.";
                response.GuidVenta = "";
                response.lstProductos = null;*/
                clsLog.EscribeLogErr(ex, "TransacCarrito.GrabarProducto");
                return new { CodRes = -1, msgRes = "Error al agregar una nueva compra.", GuidVenta = "" };
            }
            
        }

        [HttpPost, Route("FinalizarPreventa")]
        public dynamic finalizarPreventa([FromBody] dynamic DatosEncabezado)
        {
            try
            {

                string guidlog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(DatosEncabezado), "TransacCarrito.FinalizarPreventa", clsLog.tInOut.IN);
                using (var datab = new db_preventa_superEntities())
                {
                    string ip = DatosEncabezado.ip_carrito.ToString();
                   // string GuidPreventa = DatosEncabezado.GuidPreventa.ToString();

                    var prev = datab.preventa.FirstOrDefault(x => x.car_cod_carro.Equals(ip) && x.pre_estado_venta == 0);
                    if(prev != null)
                    {
                        prev.pre_estado_venta = 1;
                        prev.pre_precio_total = Convert.ToDecimal(DatosEncabezado.precioTotal.ToString());
                    }

                    datab.SaveChanges();
                    /* response.CodRes = 0;
                     response.msgRes = "OK";
                     response.GuidVenta = GuidPreventa;*/
                    clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(new { CodRes = 0, msgRes = "OK", GuidVenta = prev.pre_cod_preventa }), "TransacCarrito.FinalizarPreventa", clsLog.tInOut.OUT, guidlog);
                    CrearPDF(prev.pre_cod_preventa);
                    return new { CodRes = 0, msgRes = "OK", GuidVenta = prev.pre_cod_preventa };
                }

                //clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(response), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.OUT, guidlog);
            }
            catch (Exception ex)
            {
                /*response.CodRes = -1;
                response.msgRes = "Error al agregar una nueva compra.";
                response.GuidVenta = "";
                response.lstProductos = null;*/
                clsLog.EscribeLogErr(ex, "TransacCarrito.FinalizarPreventa");

                return new { CodRes = -1, msgRes = "Error al finaliza la compra.", GuidVenta = "" };
            }
        }

        [HttpPost, Route("BuscarGuidVenta")]
        public dynamic BuscaVenta([FromBody] dynamic Carrito)
        {
            try
            {

                string guidlog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(Carrito), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.IN);
                using (var datab = new db_preventa_superEntities())
                {
                    string ip_carrito = Carrito.ip_carrito.ToString();

                    var prev = datab.preventa.FirstOrDefault(x => x.car_cod_carro.Equals(ip_carrito) && x.pre_estado_venta == 0);
                    
                    /* response.CodRes = 0;
                     response.msgRes = "OK";
                     response.GuidVenta = GuidPreventa;*/
                    clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(new { CodRes = 0, msgRes = "OK", GuidVenta = prev.pre_cod_preventa }), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.OUT, guidlog);
                    return new { CodRes = 0, msgRes = "OK", GuidVenta = prev.pre_cod_preventa };
                }

                //clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(response), "TransacCarrito.GrabarEncabezadoPreventa", clsLog.tInOut.OUT, guidlog);
            }
            catch (Exception ex)
            {
                /*response.CodRes = -1;
                response.msgRes = "Error al agregar una nueva compra.";
                response.GuidVenta = "";
                response.lstProductos = null;*/
                clsLog.EscribeLogErr(ex, "TransacCarrito.GrabarEncabezadoCarrito");

                return new { CodRes = -1, msgRes = "Error al finaliza la compra.", GuidVenta = "" };
            }
        }

        [HttpPost]
        [Route("Nps")]
        public dynamic CrearNps([FromBody] dynamic datanps)
        {
            try 
            {
                var guidLog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(datanps), "TransacCarrito.CrearNps", clsLog.tInOut.IN);
                using(var dbdata = new db_preventa_superEntities())
                {
                    dbdata.nps.Add(new nps()
                    {
                        idNps = Guid.NewGuid().ToString(),
                        estadoNps = 0,
                        ScorePregunta = Convert.ToDecimal(datanps.Score.ToString()),
                        Comentario = datanps.Comentario.ToString(),
                        fechaNps = DateTime.Now
                    });

                    dbdata.SaveChanges();
                }
                clsLog.EscribeLogInOut("Data de salida: " + JsonConvert.SerializeObject(new { codRes = 0, msgRes = "Ok" }), "TransacCarrito.CrearNps", clsLog.tInOut.OUT, guidLog);
                return new { codRes = 0, msgRes = "Ok" };
            }
            catch(Exception ex)
            {
                clsLog.EscribeLogErr(ex, "TransacCarrito.CrearNps");
                return new { codRes = -1, msgRes = "Ocurrio un error al momento de registrar la información del NPS" };
            }
        }
        
        [HttpPost]
        [Route("BuscaProductos")]
        public ResponseProductos BuscaProducto([FromBody] dynamic datosBusca)
        {
            ResponseProductos res = new ResponseProductos();
            try
            {
                var guidLog = clsLog.EscribeLogInOut("Data de entrada: " + JsonConvert.SerializeObject(datosBusca), "TransacCarrito.BuscaProducto", clsLog.tInOut.IN);
                string ip_carrito = datosBusca.ip_carrito.ToString();
                string codbarrprod = datosBusca.codProducto.ToString();
                int cantidad = Convert.ToInt32(datosBusca.cantidadVenta.ToString());
                bool eliminar = Convert.ToBoolean(datosBusca.eliminarprod.ToString());
                using (var dbdata = new db_preventa_superEntities())
                {
                    var prev = dbdata.preventa.FirstOrDefault(x => x.car_cod_carro.Equals(ip_carrito) && x.pre_estado_venta == 0);
                    if(prev != null)
                    {
                        if (!codbarrprod.Equals(""))
                        {
                            var producto = dbdata.detalle_preventa.FirstOrDefault(x => x.pre_cod_preventa.Equals(prev.pre_cod_preventa) && x.pro_correlativo.Equals(codbarrprod));

                            if(producto != null)
                            {
                                if(eliminar == false)
                                {
                                    producto.dep_cantidad_venta = cantidad;
                                    dbdata.SaveChanges();
                                }
                                else
                                {
                                    dbdata.detalle_preventa.Remove(producto); 
                                    dbdata.SaveChanges();
                                }
                            }
                        }

                        var prod = dbdata.detalle_preventa.Where(x => x.pre_cod_preventa.Equals(prev.pre_cod_preventa)).ToList();
                        if(prod != null)
                        {
                            res.CodRes = 0;
                            res.msgRes = "OK";
                            res.GuidVenta = prev.pre_cod_preventa.ToString();

                            res.lstProductos = new List<Producto>();
                            foreach (detalle_preventa det in prod)
                            {
                                res.lstProductos.Add(new Producto()
                                {
                                    IdProducto = det.pro_correlativo.ToString(),
                                    CantidadCompra = Convert.ToInt32(det.dep_cantidad_venta.ToString()),
                                    PrecioCompra = Convert.ToDecimal(det.dep_precio_venta.ToString()),
                                    NombreProducto = dbdata.producto.FirstOrDefault(x => x.pro_correlativo.Equals(det.pro_correlativo)).pro_nombre
                                    
                                }) ;
                            }
                        }
                    }
                    clsLog.EscribeLogInOut("Trama de salida: " + JsonConvert.SerializeObject(res), "TransacCarrito.BuscaProducto", clsLog.tInOut.OUT, guidLog);
                    
                }
            }
            catch (Exception ex)
            {
                clsLog.EscribeLogErr(ex, "TransacCarrito.BuscaProducto");
                res.CodRes = -1;
                res.msgRes = "Error al momento de consultar la lista de productos";
                res.lstProductos = null;
                res.GuidVenta = "";
            }
            return res;
        }

        private void CrearPDF(String GuidVenta)
        {
            if (!Directory.Exists(@"C:\PDF")) 
                Directory.CreateDirectory(@"C:\PDF");

            PdfWriter pdf = new PdfWriter(@"C:\PDF\bar_" + GuidVenta + ".pdf");
            PdfDocument pdfDoc = new PdfDocument(pdf);
            Document doc = new Document(pdfDoc);
            BarcodeQRCode barQr = new BarcodeQRCode(GuidVenta);
            PdfFormXObject codeFormObject = barQr.CreateFormXObject(pdfDoc);
            Image codeImage = new Image(codeFormObject);
            codeImage.SetWidth(300);
            doc.Add(new Paragraph("Código de preventa para pago en caja: " + GuidVenta));
            
            doc.Add(codeImage);
            doc.Close();
        }
        
    }
}
