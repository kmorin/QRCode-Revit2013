#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.Creation;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Document = Autodesk.Revit.DB.Document;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Util;
#endregion

namespace QRCode_Revit2013
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public Document _doc;
        public string _familyName = "qrcode";

        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            _doc = doc;

            //Prompt for Input contents
            
            Document fdoc = app.NewFamilyDocument("C:\\ProgramData\\Autodesk\\RVT 2013\\Family Templates\\English_I\\Annotations\\Generic Annotation.rft");
            //New Qrencoder using the family document
            QREncoder qrcode = new QREncoder(fdoc, uidoc, app);
            string contents = Microsoft.VisualBasic.Interaction.InputBox("Enter and text to QR encode.", "QR Encode Prompt", "", -1, -1);            

            if (null == fdoc)
            {
                return Result.Failed;
            }

            // Modify document within a transaction
            using (Transaction tx = new Transaction(fdoc))
            {
                tx.Start("Generate QR Code");

                GenerateQR(fdoc, qrcode,contents);
                // Remove and replace in family creation document.
                //qrcode.Qrfilled(contents);

                tx.Commit();
            }

            //save background family doc
            string dir = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            string filename = Path.Combine(dir, _familyName + ".rfa");
            SaveAsOptions opt = new SaveAsOptions();
            opt.OverwriteExistingFile = true;
            fdoc.SaveAs(filename, opt);
            fdoc.Close(false);

            //Insert new family into document
            using (Transaction tx2 = new Transaction(doc))
            {
                tx2.Start("Insert QRCode");

                Family fam = null;

                FilteredElementCollector fec = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

                foreach (Family f in fec)
                {
                    if (f.Name.Equals(_familyName))
                    {
                        fam = f;
                    }
                    else
                    {
                        doc.LoadFamily(filename, out fam);
                    }
                }

                FamilySymbol fs = null;

                foreach (FamilySymbol symbol in fam.Symbols)
                {
                    fs = symbol;
                    break;
                }

                XYZ p = uidoc.Selection.PickPoint("Please pick point to place QR code");

                AnnotationSymbolTypeSet annoset = _doc.AnnotationSymbolTypes;
                AnnotationSymbolType annoSymbolType = null;

                foreach (AnnotationSymbolType type in annoset)
                {
                    if (type.Name.Equals(_familyName))
                    {
                        annoSymbolType = type;
                    }
                }

                doc.Create.NewAnnotationSymbol(p,annoSymbolType,uidoc.ActiveView);
                tx2.Commit();
            }

            return Result.Succeeded;
        }

        void GenerateQR(Document doc, QREncoder qrencoder, string contents)
        {
            Autodesk.Revit.Creation.FamilyItemFactory factory = doc.FamilyCreate;
            Autodesk.Revit.Creation.Application creapp = doc.Application.Create;

            FilteredElementCollector fec = new FilteredElementCollector(doc)
            .OfClass(typeof(TextNote));

            foreach (Element e in fec)
            {
                doc.Delete(e);
            }
            
            qrencoder.Qrfilled(contents);          
        }
    }
}
