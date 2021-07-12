using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml;
using System.Text;
using System.IO;
using System.Data;
namespace FELFactura
{
    public class RegisterDocument
    {


        public XmlDocument registerDte(String token, String dataXml, String url)
        {
            try{   //ENVIANDO DOCUMENTO
            var request = (HttpWebRequest)WebRequest.Create(url + Constants.URL_REGISTRAR_DOCUMENTO);
            var postData = getPostData(dataXml);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(postData.ToString());
            var data = Encoding.UTF8.GetBytes(xmlDoc.InnerXml);
            request.Headers.Add("authorization", "Bearer " + token.ToString().Trim());
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            var response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml(responseString);
            return xmlDoc2;
        }
            catch(Exception ex) {
                
                String path = "C:\\Nueva\\errores.txt";
                File.WriteAllText(path, ex.Message);
                return null;
            }


        }

        private String getPostData(String GTDocumento)
        {
    

            string uuid = Guid.NewGuid().ToString().ToUpper();

            String request = "<?xml version='1.0' encoding='UTF-8'?>\n" +
              "<RegistraDocumentoXMLRequest id=\"" + uuid + "\">\n" +
                        "<xml_dte>" +
                            " <![CDATA[" + GTDocumento + "]]>\n" +
                            "</xml_dte>\n"+
                            "</RegistraDocumentoXMLRequest>";

            return request;
        }
    }
}