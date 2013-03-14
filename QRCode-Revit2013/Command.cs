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
            
            Document fdocX = app.NewFamilyDocument("C:\\ProgramData\\Autodesk\\RVT 2013\\Family Templates\\English_I\\Annotations\\Generic Annotation.rft");
            //Document fdocX = app.NewFamilyDocument("C:\\ProgramData\\Autodesk\\RVT 2013\\Family Templates\\English_I\\Generic Model.rft");

            //New Qrencoder using the family document            
            QREncoder qrcode = new QREncoder(fdocX, uidoc, app);

            string contents = Microsoft.VisualBasic.Interaction.InputBox("Enter and text to QR encode.", "QR Encode Prompt", "", -1, -1);

            if (contents != "")
            {

                if (null == fdocX)
                {
                    return Result.Failed;
                }

                // Modify document within a transaction
                using (Transaction tx = new Transaction(fdocX))
                {
                    tx.Start("Generate QR Code");

                    //GenerateQR(fdoc, qrcode, contents);
                    GenerateQR(fdocX, qrcode, contents);                    

                    tx.Commit();
                }

                //save background family doc
                string dir = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

                //Clean invalid filename characters
                string fileNameClean = CleanChars(contents);
                _familyName += fileNameClean;

                string filename = Path.Combine(dir, _familyName + ".rfa");
                SaveAsOptions opt = new SaveAsOptions();
                opt.OverwriteExistingFile = true;
                fdocX.SaveAs(filename, opt);
                fdocX.Close(false);

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

                    //Deprecated in 2013, but too lazy to implement new 
                    doc.Create.NewAnnotationSymbol(p, annoSymbolType, uidoc.ActiveView);


                    //doc.Create.NewFamilyInstance(p, fs, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);                    

                    tx2.Commit();
                }

                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }
        }

        private string CleanChars(string contents)
        {
            
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                contents = contents.Replace(c, '-');
            }

            return contents;
        }

        void GenerateQR(Document doc, QREncoder qrencoder, string contents)
        {
            FilteredElementCollector fec = new FilteredElementCollector(doc)
            .OfClass(typeof(TextNote));

            foreach (Element e in fec)
            {
                doc.Delete(e);
            }
            
            qrencoder.Qrfilled(contents); 


            //qrencoder.QrGeneric(contents);
        }
    }
}
