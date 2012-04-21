﻿using System;
using System.Collections.Generic;
using System.Text;
using fyiReporting.RDL;
using System.Drawing;
using System.ComponentModel;
using System.Xml;

namespace fyiReporting.CRI
{
    public class QrCode : ICustomReportItem
    {
        #region ICustomReportItem Members

        bool ICustomReportItem.IsDataRegion()
        {
            return false;
        }

        void ICustomReportItem.DrawImage(ref System.Drawing.Bitmap bm)
        {
            DrawImage(ref bm, _qrCode);
        }

        /// <summary>
        /// Does the actual drawing of the image.
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="qrcode"></param>
        internal void DrawImage(ref System.Drawing.Bitmap bm, string qrcode)
        {
            com.google.zxing.qrcode.QRCodeWriter writer = new com.google.zxing.qrcode.QRCodeWriter();
            com.google.zxing.common.ByteMatrix matrix;

            int size = 180;
            matrix = writer.encode(qrcode, com.google.zxing.BarcodeFormat.QR_CODE, size, size, null);


            bm = new Bitmap(size, size);
            Color Color = Color.FromArgb(0, 0, 0);

            for (int y = 0; y < matrix.Height; ++y)
            {
                for (int x = 0; x < matrix.Width; ++x)
                {
                    Color pixelColor = bm.GetPixel(x, y);

                    //Find the colour of the dot
                    if (matrix.get_Renamed(x, y) == -1)
                    {
                        bm.SetPixel(x, y, Color.White);
                    }
                    else
                    {
                        bm.SetPixel(x, y, Color.Black);
                    }
                }
            }
        }

        /// <summary>
        /// Design time: Draw a hard coded BarCode for design time;  Parameters can't be
        /// relied on since they aren't available.
        /// </summary>
        /// <param name="bm"></param>
        void ICustomReportItem.DrawDesignerImage(ref System.Drawing.Bitmap bm)
        {
            DrawImage(ref bm, "https://github.com/majorsilence/My-FyiReporting");
        }

        private string _qrCode = "";
        void ICustomReportItem.SetProperties(IDictionary<string, object> props)
        {
            try
            {
                _qrCode = props["QrCode"].ToString();
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("QrCode property must be specified");
            }
        }

        object ICustomReportItem.GetPropertiesInstance(System.Xml.XmlNode iNode)
        {
            BarCodePropertiesQR bcp = new BarCodePropertiesQR(this, iNode);
            foreach (XmlNode n in iNode.ChildNodes)
            {
                if (n.Name != "CustomProperty")
                    continue;
                string pname = this.GetNamedElementValue(n, "Name", "");
                switch (pname)
                {
                    case "QrCode":
                        bcp.SetQrCode(GetNamedElementValue(n,  "Value",  ""));
                        break;
                    default:
                        break;
                }
            }

            return bcp;
        }

        /// <summary>
        /// Get the child element with the specified name.  Return the InnerText
        /// value if found otherwise return the passed default.
        /// </summary>
        /// <param name="xNode">Parent node</param>
        /// <param name="name">Name of child node to look for</param>
        /// <param name="def">Default value to use if not found</param>
        /// <returns>Value the named child node</returns>
        string GetNamedElementValue(XmlNode xNode, string name, string def)
        {
            if (xNode == null)
                return def;

            foreach (XmlNode cNode in xNode.ChildNodes)
            {
                if (cNode.NodeType == XmlNodeType.Element &&
                    cNode.Name == name)
                    return cNode.InnerText;
            }
            return def;
        }

        public void SetPropertiesInstance(System.Xml.XmlNode node, object inst)
        {
            node.RemoveAll();       // Get rid of all properties

            BarCodePropertiesQR bcp = inst as BarCodePropertiesQR;
            if (bcp == null)
                return;

            
            CreateChild(node, "QrCode", bcp.QrCode);

        }

        void CreateChild(XmlNode node, string nm, string val)
        {
            XmlDocument xd = node.OwnerDocument;
            XmlNode cp = xd.CreateElement("CustomProperty");
            node.AppendChild(cp);

            XmlNode name = xd.CreateElement("Name");
            name.InnerText = nm;
            cp.AppendChild(name);

            XmlNode v = xd.CreateElement("Value");
            v.InnerText = val;
            cp.AppendChild(v);
        }

        static public readonly float OptimalHeight = 180.0f;          // Optimal height at magnification 1    
        static public readonly float OptimalWidth = 180.0f;            // Optimal width at mag 1

        /// <summary>
        /// Design time call: return string with <CustomReportItem> ... </CustomReportItem> syntax for 
        /// the insert.  The string contains a variable {0} which will be substituted with the
        /// configuration name.  This allows the name to be completely controlled by
        /// the configuration file.
        /// </summary>
        /// <returns></returns>
        string ICustomReportItem.GetCustomReportItemXml()
        {
            return "<CustomReportItem><Type>{0}</Type>" +
                string.Format("<Height>{0}mm</Height><Width>{1}mm</Width>", OptimalHeight, OptimalWidth) +
                "<CustomProperties>" +
                "<CustomProperty>" +
                "<Name>QrCode</Name>" +
                "<Value>Enter Your Value</Value>" +
                "</CustomProperty>" +
                "</CustomProperties>" +
                "</CustomReportItem>";
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            return;
        }

        #endregion

        /// <summary>
        /// BarCodeProperties- All properties are type string to allow for definition of
        /// a runtime expression.
        /// </summary>
        public class BarCodePropertiesQR
        {
            string _QrCode;
            QrCode _bc;
            XmlNode _node;

            internal BarCodePropertiesQR(QrCode bc, XmlNode node)
            {
                _bc = bc;
                _node = node;
            }

            internal void SetQrCode(string ns)
            {
                _QrCode = ns;
            }
            [CategoryAttribute("QrCode"),
               DescriptionAttribute("The text string to be encoded as a QR Code.")]
            public string QrCode
            {
                get { return _QrCode; }
                set { _QrCode = value; _bc.SetPropertiesInstance(_node, this); }
            }


        }
    }

    
}