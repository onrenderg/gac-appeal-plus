using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DigitalNagrik.Areas.Public.Models
{
    public class MimeTypes
    {
        private static readonly byte[] BMP = { 66, 77 };
        private static readonly byte[] DOC = { 208, 207, 17, 224, 161, 177, 26, 225 };
        private static readonly byte[] EXE_DLL = { 77, 90 };
        private static readonly byte[] GIF = { 71, 73, 70, 56 };
        private static readonly byte[] ICO = { 0, 0, 1, 0 };
        private static readonly byte[] JPG = { 255, 216, 255 };
        private static readonly byte[] MP3 = { 255, 251, 48 };
        private static readonly byte[] OGG = { 79, 103, 103, 83, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] PDF = { 37, 80, 68, 70, 45, 49, 46 };
        private static readonly byte[] PNG = { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82 };
        private static readonly byte[] RAR = { 82, 97, 114, 33, 26, 7, 0 };
        private static readonly byte[] SWF = { 70, 87, 83 };
        private static readonly byte[] TIFF = { 73, 73, 42, 0 };
        private static readonly byte[] TORRENT = { 100, 56, 58, 97, 110, 110, 111, 117, 110, 99, 101 };
        private static readonly byte[] TTF = { 0, 1, 0, 0, 0 };
        private static readonly byte[] WAV_AVI = { 82, 73, 70, 70 };
        private static readonly byte[] WMV_WMA = { 48, 38, 178, 117, 142, 102, 207, 17, 166, 217, 0, 170, 0, 98, 206, 108 };
        private static readonly byte[] ZIP_DOCX = { 80, 75, 3, 4 };

        public static string GetMimeType(byte[] file, string fileName)
        {

            string mime = "application/octet-stream"; //DEFAULT UNKNOWN MIME TYPE

            //Ensure that the filename isn't empty or null
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return mime;
            }

            //Get the file extension
            string extension = Path.GetExtension(fileName) == null
                                   ? string.Empty
                                   : Path.GetExtension(fileName).ToUpper();

            //Get the MIME Type
            if (file.Take(2).SequenceEqual(BMP))
            {
                mime = "image/bmp";
            }
            else if (file.Take(8).SequenceEqual(DOC))
            {
                mime = "application/msword";
            }
            else if (file.Take(2).SequenceEqual(EXE_DLL))
            {
                mime = "application/x-msdownload"; //both use same mime type
            }
            else if (file.Take(4).SequenceEqual(GIF))
            {
                mime = "image/gif";
            }
            else if (file.Take(4).SequenceEqual(ICO))
            {
                mime = "image/x-icon";
            }
            else if (file.Take(3).SequenceEqual(JPG))
            {
                mime = "image/jpeg";
            }
            else if (file.Take(3).SequenceEqual(MP3))
            {
                mime = "audio/mpeg";
            }
            else if (file.Take(14).SequenceEqual(OGG))
            {
                if (extension == ".OGX")
                {
                    mime = "application/ogg";
                }
                else if (extension == ".OGA")
                {
                    mime = "audio/ogg";
                }
                else
                {
                    mime = "video/ogg";
                }
            }
            else if (file.Take(7).SequenceEqual(PDF))
            {
                mime = "application/pdf";
            }
            else if (file.Take(16).SequenceEqual(PNG))
            {
                mime = "image/png";
            }
            else if (file.Take(7).SequenceEqual(RAR))
            {
                mime = "application/x-rar-compressed";
            }
            else if (file.Take(3).SequenceEqual(SWF))
            {
                mime = "application/x-shockwave-flash";
            }
            else if (file.Take(4).SequenceEqual(TIFF))
            {
                mime = "image/tiff";
            }
            else if (file.Take(11).SequenceEqual(TORRENT))
            {
                mime = "application/x-bittorrent";
            }
            else if (file.Take(5).SequenceEqual(TTF))
            {
                mime = "application/x-font-ttf";
            }
            else if (file.Take(4).SequenceEqual(WAV_AVI))
            {
                mime = extension == ".AVI" ? "video/x-msvideo" : "audio/x-wav";
            }
            else if (file.Take(16).SequenceEqual(WMV_WMA))
            {
                mime = extension == ".WMA" ? "audio/x-ms-wma" : "video/x-ms-wmv";
            }
            else if (file.Take(4).SequenceEqual(ZIP_DOCX))
            {
                mime = extension == ".DOCX" ? "application/vnd.openxmlformats-officedocument.wordprocessingml.document" : "application/x-zip-compressed";
            }

            return mime;
        }
        public static bool isInvalidTagsExistsorNot(string str)
        {
            string[] badchars = new string[] { "script", "video", "alert", "prompt", "svg", "img", "iframe", "body", "confirm", "xss", "XML", "img src", "src", "href", "document" };
            //  static string[] badchars2 = new string[] { "onload", "onmouseover", "onerror", "onmousemove", "href", "action" };

            string[] badchars2 = new string[] { "FSCommand", "onAbort", "onActivate", "onAfterPrint", "onAfterUpdate", "onBeforeActivate", "onBeforeCopy", "onBeforeCut", "onBeforeDeactivate", "onBeforeEditFocus",
"onBeforePaste", "onBeforePrint", "onBeforeUnload", "onBeforeUpdate", "onBegin", "onBlur", "onBounce", "onCellChange", "onChange", "onClick", "onContextMenu", "onControlSelect",
"onCopy", "onCut", "onDataAvailable", "onDataSetChanged", "onDataSetComplete", "onDblClick", "onDeactivate", "onDrag", "onDragEnd", "onDragLeave", "onDragEnter", "onDragOver",
"onDragDrop", "onDragStart", "onDrop", "onEnd", "onError", "onErrorUpdate", "onFilterChange", "onFinish", "onFocus", "onFocusIn", "onFocusOut", "onHashChange", "onHelp", "onInput",
"onKeyDown", "onKeyPress", "onKeyUp", "onLayoutComplete", "onLoad", "onLoseCapture", "onMediaComplete", "onMediaError", "onMessage", "onMouseDown", "onMouseEnter", "onMouseLeave",
"onMouseMove", "onMouseOut", "onMouseOver", "onMouseUp", "onMouseWheel", "onMove", "onMoveEnd", "onMoveStart", "onOffline", "onOnline", "onOutOfSync", "onPaste", "onPause", "onPopState",
"onProgress", "onPropertyChange", "onReadyStateChange", "onRedo", "onRepeat", "onReset", "onResize", "onResizeEnd", "onResizeStart", "onResume", "onReverse", "onRowsEnter", "onRowExit",
"onRowDelete", "onRowInserted", "onScroll", "onSeek", "onSelect", "onSelectionChange", "onSelectStart", "onStart", "onStop", "onStorage", "onSyncRestored", "onSubmit", "onTimeError",
"onTrackChange", "onUndo", "onUnload", "onURLFlip", "seekSegmentTime", "href", "action" };

            Regex reg;
            for (int i = 0; i < badchars.Length; i++)
            {
                //reg = new Regex("([<]|[\\][x][2][8]|(&lt;)|(%3C)|(&#x3C;)|[\\][x][3][C])[ ]*" + badchars[i], RegexOptions.IgnoreCase);

                reg = new Regex("([<]|[\\][x][2][8]|(&lt;)|(&lt)|(%3C)|(&#x3C;)|[\\][x][3][C])[ ]*" + badchars[i], RegexOptions.IgnoreCase);


                if (reg.Matches(str).Count > 0)
                    return false;
            }

            reg = new Regex("javascript[ ]*[:]", RegexOptions.IgnoreCase);
            if (reg.Matches(str).Count > 0)
                return false;

            for (int i = 0; i < badchars2.Length; i++)
            {

                reg = new Regex(badchars2[i] + "[ ]*[=]", RegexOptions.IgnoreCase);
                if (reg.Matches(str).Count > 0)
                    return false;
            }

            return true;

        }
    }
}