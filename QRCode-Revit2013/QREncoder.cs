using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Utility;
using Autodesk.Revit.DB.ExtensibleStorage;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;

namespace QRCode_Revit2013
{
    class QREncoder : QRCodeEncoder
    {
        public UIDocument _uiDoc;
        public Document _doc;
        public Application _app;

        //Constructor
        public QREncoder(Document doc, UIDocument uidoc, Application app)
        {
            base.QRCodeEncodeMode = ENCODE_MODE.BYTE;
            base.QRCodeVersion = 7;
            base.QRCodeErrorCorrect = ERROR_CORRECTION.M;
            base.QRCodeScale = 1;

            _doc = doc;
            _uiDoc = uidoc;
            _app = app;
        }

        public virtual bool Qrfilled(string content)
        {
            return (QRCodeUtility.IsUniCode(content) ?
                Qrfilled(content, Encoding.Unicode) :
                Qrfilled(content, Encoding.ASCII)
                );
        }

        public bool Qrfilled(string content, Encoding encoding)
        {
            ElementId typeId = null;
            ElementId viewId = _uiDoc.ActiveView.Id;
            List<CurveLoop> boundaries = new List<CurveLoop>();
            List<ElementId> frGroup = new List<ElementId>();
            double ScaleModifier = 0.0125;

            //string stringbuild = "";

            FilteredElementCollector fec = new FilteredElementCollector(_doc)
            .OfClass(typeof(FilledRegionType));

            foreach (Element e in fec)
            {
                if (e.Name == "Solid Black")
                    typeId = e.Id;
            }


            bool[][] matrix = calQrcode(encoding.GetBytes(content));
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (matrix[j][i])
                    {
                        XYZ start = new XYZ(j * (QRCodeScale*ScaleModifier), i * (QRCodeScale*ScaleModifier), 0);
                        XYZ end = new XYZ((j + 1) * (QRCodeScale*ScaleModifier), i * (QRCodeScale*ScaleModifier), 0);
                        XYZ end2 = new XYZ((j + 1) * (QRCodeScale*ScaleModifier), (i + 1) * (QRCodeScale*ScaleModifier), 0);
                        XYZ end3 = new XYZ(j * (QRCodeScale*ScaleModifier), (i + 1) * (QRCodeScale*ScaleModifier), 0);

                        Line l = _app.Create.NewLineBound(start, end);
                        Line l1 = _app.Create.NewLineBound(end, end2);
                        Line l2 = _app.Create.NewLineBound(end2, end3);
                        Line l3 = _app.Create.NewLineBound(end3, start);
                        Curve c = l as Curve;
                        Curve c1 = l1 as Curve;
                        Curve c2 = l2 as Curve;
                        Curve c3 = l3 as Curve;
                        CurveLoop cloop = new CurveLoop();
                        cloop.Append(c);
                        cloop.Append(c1);
                        cloop.Append(c2);
                        cloop.Append(c3);
                        boundaries.Add(cloop);
                        //stringbuild += "\nLine 1: " + start.ToString() + "," + end.ToString() +
                        //    "\nLine 2: " + end2.ToString() +
                        //    "\nLine 3: " + end3.ToString() +
                        //    "\nLine end: " + end.ToString();
                        FilledRegion fr = FilledRegion.Create(_doc, typeId, viewId, boundaries);
                        frGroup.Add(fr.Id);
                        boundaries.Clear();
                    }

                }
                //TaskDialog.Show("s", stringbuild);

            }

            //Group elements
            //Used for project documents
                //Group group = _doc.Create.NewGroup(frGroup);
            //string groupname = new Guid().ToString();
            //group.Name = groupname.Substring(groupname.Length - 4);
            return true;
        }
    }
}
