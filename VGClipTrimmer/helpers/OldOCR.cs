using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VGClipTrimmer.helpers
{
    public class OldOCR
    {
        /*
        private string OCR3(Bitmap img)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "eng", EngineMode.Default))
            {
                // Whitelist of chars to recognize
                engine.SetVariable("tessedit_char_whitelist", "ACDEIKLMNOTW");
                //engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                // Dont bother with word plausibility
                engine.SetVariable("tessedit_unrej_any_wd", true);

                // Look for one line
                Page page = engine.Process(img, PageSegMode.Auto);
                res = page.GetText();
            }
            return res;
        }
        */
    }
}
