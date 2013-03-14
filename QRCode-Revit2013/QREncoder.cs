using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autodesk.Revit.ApplicationServices;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.Collections;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;
using Autodesk.Revit.UI;
using Autodesk.Revit.Utility;
using Autodesk.Revit.DB.ExtensibleStorage;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
using Autodesk.Revit.Creation;

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

        /*
        public virtual bool QrGeneric(string content)
        {
            return (QRCodeUtility.IsUniCode(content) ?
                QrGeneric(content, Encoding.Unicode) :
                QrGeneric(content, Encoding.ASCII)
                );
        }
        
        public bool QrGeneric(string content, Encoding encoding)
        {

            double ScaleModifier = 0.0833; //0.0026; // 1/32" size boxes
            SketchPlane _sp = FindElement(_doc, typeof(SketchPlane), "Ref. Level") as SketchPlane;

            bool[][] matrix = calQrcode(encoding.GetBytes(content));

            try
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix.Length; j++)
                    {
                        if (matrix[j][i])
                        {
                            XYZ start = new XYZ(j * (QRCodeScale * ScaleModifier), i * (QRCodeScale * ScaleModifier), 0);
                            XYZ end = new XYZ((j + 1) * (QRCodeScale * ScaleModifier), i * (QRCodeScale * ScaleModifier), 0);
                            XYZ end2 = new XYZ((j + 1) * (QRCodeScale * ScaleModifier), (i + 1) * (QRCodeScale * ScaleModifier), 0);
                            XYZ end3 = new XYZ(j * (QRCodeScale * ScaleModifier), (i + 1) * (QRCodeScale * ScaleModifier), 0);                           

                            Line l = _app.Create.NewLineBound(start, end);
                            Line l1 = _app.Create.NewLineBound(end, end2);
                            Line l2 = _app.Create.NewLineBound(end2, end3);
                            Line l3 = _app.Create.NewLineBound(end3, start);

                            CurveArrArray crrProf = new CurveArrArray();
                            CurveArray cArr = new CurveArray();
                            cArr.Append(l); cArr.Append(l1); cArr.Append(l2); cArr.Append(l3);

                            crrProf.Append(cArr);

                            Random random = new Random();
                            double dblRand = random.Next(1,5);

                            FamilyItemFactory factory = _doc.FamilyCreate;
                            factory.NewExtrusion(true, crrProf, _sp, dblRand);

                            crrProf.Clear();
                            cArr.Clear();
                            
                        }

                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        */
        Element FindElement(Document doc, Type targetType, string targetName)
        {
            return new FilteredElementCollector(doc)
            .OfClass(targetType)
            .First<Element>(e => e.Name.Equals(targetName));
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
            ElementId viewId = null;
            //ElementId viewId = _uiDoc.ActiveView.Id;
            FilteredElementCollector fecDoc = new FilteredElementCollector(_doc)
            .OfClass(typeof(View));

            foreach (View v in fecDoc)
            {
                viewId = v.Id;
                break;
            }
            
            List<CurveLoop> boundaries = new List<CurveLoop>();
            List<ElementId> frGroup = new List<ElementId>();
            double ScaleModifier = 0.0026; // 1/32" size boxes

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

                        FilledRegion fr = FilledRegion.Create(_doc, typeId, viewId, boundaries);
                        frGroup.Add(fr.Id);
                        boundaries.Clear();
                    }

                }
            }
            return true;
        }
    }
}
