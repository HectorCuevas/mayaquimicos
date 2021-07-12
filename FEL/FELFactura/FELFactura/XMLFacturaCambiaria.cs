﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

using System.Xml.Linq;
using System.IO;
using System.Data;
using Modelos;
using Firma;
namespace FELFactura
{
    public class XMLFacturaCambiaria : IDocumentRegister
    {
        
        private DataSet dstinvoicexml = new DataSet();
        private DataSet dstdetailinvoicexml = new DataSet();
        private DatosGenerales datosGenerales = new DatosGenerales();
        private Emisor emisor = new Emisor();
        private Receptor receptor = new Receptor();
        private List<Item> items = new List<Item>();
        private Totales totales = new Totales();
        string v_rootxml ="";
        string fac_num = "";
        public String getXML(string XMLInvoice, string XMLDetailInvoce, string frases, string path, string fac_num)
        {
            
            v_rootxml = path;
            this.fac_num = fac_num;
            //convertir a dataset los string para mayor manupulacion
            XmlToDataSet( XMLInvoice,  XMLDetailInvoce);
            //llenar estructuras
            ReaderDataset();
            
            //armar xml
            getXML(frases);
            
            //firmar xml por certificado
            var nombre = fac_num.Trim() + ".xml";
            v_rootxml = v_rootxml + @"\" + nombre;
         //   path = "D:\\xml\\FACT\\";
            string myXML = FirmaDocumento.FirmarDocumento2(Constants.URL_CERTIFICADO, Constants.URL_CERTIFICADO_CONTRASENIA, path, nombre,  path);

            return myXML;

        }


        //Convertir XML a DataSet
        private bool XmlToDataSet( string XMLInvoice, string XMLDetailInvoce)
        {
            try
            {
                      
                //Convieriendo XMl a DataSet Factura
                System.IO.StringReader rdinvoice = new System.IO.StringReader(XMLInvoice);
                dstinvoicexml.ReadXml(rdinvoice);

                //Convieritiendo XML a DataSet Detalle Factura
                System.IO.StringReader rddetailinvoice = new System.IO.StringReader(XMLDetailInvoce);
                dstdetailinvoicexml.ReadXml(rddetailinvoice);
                return true;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }


        //Lectura de Documentos
        private void ReaderDataset()
        {

            LlenarEstructuras.DatosGenerales(dstinvoicexml, datosGenerales, Constants.TIPO_FACTURA_CAMBIARIA);
            LlenarEstructuras.DatosEmisor(dstinvoicexml,  emisor);
            LlenarEstructuras.DatosReceptor( dstinvoicexml,  receptor, datosGenerales);
            LlenarEstructuras.DatosItems(dstdetailinvoicexml, items);
            LlenarEstructuras.Totales(dstinvoicexml, totales,items);
        }


        private string[] setFrases(string xfrases)
        {

            return xfrases.Split(';');
        }

        private string[] setNumerosFrases(string xfrases)
        {

            return xfrases.Split(',');
        }


        private String getXML(string f)
        {

            Boolean exenta = true;
            XNamespace cfc = XNamespace.Get("http://www.sat.gob.gt/dte/fel/CompCambiaria/0.1.0");
            XNamespace dte = XNamespace.Get("http://www.sat.gob.gt/dte/fel/0.2.0");
            XNamespace xd = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
            //Encabezado del Documento
            XDeclaration declaracion = new XDeclaration("1.0", "utf-8", null);

            //GTDocumento
            XElement parameters = new XElement(dte + "GTDocumento",
                            new XAttribute(XNamespace.Xmlns + "dte", dte.NamespaceName),
                           new XAttribute(XNamespace.Xmlns + "xd", xd.NamespaceName),
                           new XAttribute("Version", "0.1"));
            //SAT
            XElement SAT = new XElement(dte + "SAT", new XAttribute("ClaseDocumento", "dte"));
            parameters.Add(SAT);

            // formando dte
            XElement DTE = new XElement(dte + "DTE", new XAttribute("ID", "DatosCertificados"));
            SAT.Add(DTE);

            //datos de emision
            XElement DatosEmision = new XElement(dte + "DatosEmision", new XAttribute("ID", "DatosEmision"));
            DTE.Add(DatosEmision);

            //datos generales
            XElement DatosGenerales = new XElement(dte + "DatosGenerales", new XAttribute("CodigoMoneda", this.datosGenerales.CodigoMoneda),
                 new XAttribute("FechaHoraEmision", this.datosGenerales.FechaHoraEmision),new XAttribute("Tipo", this.datosGenerales.Tipo));
            DatosEmision.Add(DatosGenerales);

            //datos emisor
            XElement Emisor = new XElement(dte + "Emisor", new XAttribute("AfiliacionIVA", this.emisor.AfiliacionIVA),
                new XAttribute("CodigoEstablecimiento", this.emisor.CodigoEstablecimiento), 
                new XAttribute("CorreoEmisor", this.emisor.CorreoEmisor), new XAttribute("NITEmisor", this.emisor.NITEmisor), 
                new XAttribute("NombreComercial", this.emisor.NombreComercial), new XAttribute("NombreEmisor", this.emisor.NombreEmisor));
            DatosEmision.Add(Emisor);
            //direccion del emisor
            XElement DireccionEmisor = new XElement(dte + "DireccionEmisor");
            Emisor.Add(DireccionEmisor);
            //elementos dentro de direccion de emisor, dirección, codigopostal, municipio, departamento, pais
            XElement Direccion = new XElement(dte + "Direccion", this.emisor.Direccion);
            XElement CodigoPostal = new XElement(dte + "CodigoPostal", this.emisor.CodigoPostal);
            XElement Municipio = new XElement(dte + "Municipio", this.emisor.Municipio);
            XElement Departamento = new XElement(dte + "Departamento", this.emisor.Departamento);
            XElement Pais = new XElement(dte + "Pais", this.emisor.Pais);
            DireccionEmisor.Add(Direccion);
            DireccionEmisor.Add(CodigoPostal);
            DireccionEmisor.Add(Municipio);
            DireccionEmisor.Add(Departamento);
            DireccionEmisor.Add(Pais);

            //datos Receptor
            XElement Receptor = new XElement(dte + "Receptor", new XAttribute("CorreoReceptor", this.receptor.CorreoReceptor), 
                new XAttribute("IDReceptor", this.receptor.IDReceptor),
                new XAttribute("NombreReceptor", this.receptor.NombreReceptor));
            DatosEmision.Add(Receptor);
            //direccion del receptor
            XElement DireccionReceptor = new XElement(dte + "DireccionReceptor");
            Receptor.Add(DireccionReceptor);
            //elementos dentro de direccion de emisor, dirección, codigopostal, municipio, departamento, pais
            XElement DireccionRecp = new XElement(dte + "Direccion", this.receptor.Direccion);
            XElement CodigoPostalReceptor = new XElement(dte + "CodigoPostal", this.receptor.CodigoPostal);
            XElement MunicipioReceptor = new XElement(dte + "Municipio", this.receptor.Municipio);
            XElement DepartamentoReceptor = new XElement(dte + "Departamento", this.receptor.Departamento);
            XElement PaisReceptor = new XElement(dte + "Pais", this.receptor.Pais);
            DireccionReceptor.Add(DireccionRecp);
            DireccionReceptor.Add(CodigoPostalReceptor);
            DireccionReceptor.Add(MunicipioReceptor);
            DireccionReceptor.Add(DepartamentoReceptor);
            DireccionReceptor.Add(PaisReceptor);


            //frases
            XElement Frases = new XElement(dte + "Frases");
            DatosEmision.Add(Frases);


            int ss = setFrases(f).Length;
            for (int i = 0; i < ss; i++)
            {
                string[] arr = setFrases(f);
                string cod = setNumerosFrases(arr[i])[0];
                string tipo = setNumerosFrases(arr[i])[1];

                XElement frase = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", cod), new XAttribute("TipoFrase", tipo));
                Frases.Add(frase);
            }


            //XElement Frase1 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "1"));
            //Frases.Add(Frase1);
            //XElement Frase2 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "2"));
            //Frases.Add(Frase2);


            // detalle de factura 

            XElement Items = new XElement(dte + "Items");
            DatosEmision.Add(Items);
            if (this.items!=null) {
                foreach (Item item in this.items) {
                    //Items


                    //item
                    XElement Item = new XElement(dte + "Item", new XAttribute("BienOServicio", item.BienOServicio), new XAttribute("NumeroLinea", item.NumeroLinea));
                    XElement Cantidad = new XElement(dte + "Cantidad", item.Cantidad);
                    XElement UnidadMedida = new XElement(dte + "UnidadMedida", item.UnidadMedida);
                    XElement Descripcion = new XElement(dte + "Descripcion", item.Descripcion);
                    XElement PrecioUnitario = new XElement(dte + "PrecioUnitario", item.PrecioUnitario);
                    XElement Precio = new XElement(dte + "Precio", item.Precio);
                    XElement Descuento = new XElement(dte + "Descuento", item.Descuento);
                    XElement TotalItem = new XElement(dte + "Total", item.Total);
                    //impuestos
                    XElement Impuestos = new XElement(dte + "Impuestos");

                    Item.Add(Cantidad);
                    Item.Add(UnidadMedida);
                    Item.Add(Descripcion);
                    Item.Add(PrecioUnitario);
                    Item.Add(Precio);
                    Item.Add(Descuento);
                    Item.Add(Impuestos);
                    Item.Add(TotalItem);
                    Items.Add(Item);



                    //impuesto por item
                 if (item.impuestos != null) {
                        foreach (Impuesto im in item.impuestos) {
                            XElement Impuesto = new XElement(dte + "Impuesto");
                            XElement NombreCorto = new XElement(dte + "NombreCorto", im.NombreCorto);
                            XElement CodigoUnidadGravable = new XElement(dte + "CodigoUnidadGravable", im.CodigoUnidadGravable);
                            XElement MontoGravable = new XElement(dte + "MontoGravable", im.MontoGravable);
                          //  XElement CantidadUnidadesGravables = new XElement(dte + "CantidadUnidadesGravables", im.CantidadUnidadesGravables);
                            XElement MontoImpuesto = new XElement(dte + "MontoImpuesto", im.MontoImpuesto);
                            Impuesto.Add(NombreCorto);
                            Impuesto.Add(CodigoUnidadGravable);
                            Impuesto.Add(MontoGravable);
                            //Impuesto.Add(CantidadUnidadesGravables);
                            Impuesto.Add(MontoImpuesto);
                            Impuestos.Add(Impuesto);

                            if ("2".Equals(im.CodigoUnidadGravable))
                            {
                                exenta = true;

                            }
                        }                    
                 }
               }
            }

            //frase cuando es una factura excenta
           /* if (exenta)
            {
                XElement Frase3 = new XElement(dte + "Frase", new XAttribute("CodigoEscenario", "1"), new XAttribute("TipoFrase", "4"));
                Frases.Add(Frase3);

            }*/

            //Totales
            XElement Totales = new XElement(dte + "Totales");
            DatosEmision.Add(Totales);

            //total impuestos
            XElement TotalImpuestos = new XElement(dte + "TotalImpuestos");
            XElement TotalImpuesto = new XElement(dte + "TotalImpuesto", new XAttribute("NombreCorto", totales.NombreCorto), new XAttribute("TotalMontoImpuesto", totales.TotalMontoImpuesto));
            TotalImpuestos.Add(TotalImpuesto);
            Totales.Add(TotalImpuestos);

            //total general
            XElement GranTotal = new XElement(dte + "GranTotal", totales.GranTotal);
            Totales.Add(GranTotal);


            XElement Complementos = new XElement(dte + "Complementos");
            DatosEmision.Add(Complementos);
            XElement Complemento = new XElement(dte + "Complemento", new XAttribute("NombreComplemento", "ncomp"), new XAttribute("URIComplemento", "http://www.sat.gob.gt/face2/ComplementoFacturaCambiaria/0.1.0"));
            Complementos.Add(Complemento);

            XElement AbonosFacturaCambiaria = new XElement(cfc + "AbonosFacturaCambiaria",
                new XAttribute(XNamespace.Xmlns + "cfc", cfc.NamespaceName),
                           
                                              new XAttribute("Version", "1")
                   );

            XElement Abono = new XElement(cfc + "Abono");
            XElement NumeroAbono = new XElement(cfc + "NumeroAbono", "1");
            XElement FechaVencimiento = new XElement(cfc + "FechaVencimiento", DateTime.Now.ToString("yyyy-MM-dd"));
            XElement MontoAbono = new XElement(cfc + "MontoAbono", "0");
            Abono.Add(NumeroAbono);
            Abono.Add(FechaVencimiento);
            Abono.Add(MontoAbono);
            AbonosFacturaCambiaria.Add(Abono);

            Complemento.Add(AbonosFacturaCambiaria);

         
            XDocument myXML = new XDocument(declaracion, parameters);
            String res = myXML.ToString();
          
            try
            {
                v_rootxml = string.Format(@"{0}\{1}.xml", v_rootxml, fac_num.Trim());
                if (!File.Exists(v_rootxml))
                {
                    
                    myXML.Save(v_rootxml);
                }
                else
                {
                    System.IO.File.Delete(v_rootxml);
                    myXML.Save(v_rootxml);
                }
            }
            catch (Exception ex)
            {
                String path = "C:\\Nueva\\errores.txt";
                File.WriteAllText(path, ex.Message);
            }
            return res;
        }
        }
}