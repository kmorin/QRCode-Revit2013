#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
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
            QREncoder qrcode = new QREncoder(_doc, uidoc, app);
            string contents = Microsoft.VisualBasic.Interaction.InputBox("Enter and text to QR encode.", "QR Encode Prompt", "", -1, -1);

            // Modify document within a transaction
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Generate QR Code");

                qrcode.Qrfilled(contents);

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}
