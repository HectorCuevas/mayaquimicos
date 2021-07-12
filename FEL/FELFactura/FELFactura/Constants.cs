﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FELFactura
{
    public class Constants
    {

       
        public  static  String URL_SOLICITAR_TOKEN= "api/solicitarToken";
        public  static  String URL_REGISTRAR_DOCUMENTO = "api/registrarDocumentoXML";
        public static String URL_VAIDAR_DOCUMENTO = "api/validarDocumento";
        public static String URL_ANULAR_DOCUMENTO = "api/anularDocumentoXML";
        public static String URL_CERTIFICADO = "C:\\certificado\\3225607-5b2591455ab7a100.pfx";
        public static String URL_CERTIFICADO_CONTRASENIA = "Mayaquimicos80#";
        public static String TIPO_FACTURA = "FACT";
        public static String TIPO_FACTURA_CAMBIARIA = "FCAM";
        public static String TIPO_FACTURA_PEQUENIO_CONTRIBUYENTE = "FPEQ";
        public static String TIPO_FACTURA_ESPECIAL = "FESP";
        public static String TIPO_NOTA_ABONO = "NABN";
        public static String TIPO_RECIBO_DONACION = "RDON";
        public static String TIPO_RECIBO = "RECI";
        public static String TIPO_NOTA_DEBITO = "NDEB";
        public static String TIPO_NOTA_CREDITO = "NCRE";
    }
}