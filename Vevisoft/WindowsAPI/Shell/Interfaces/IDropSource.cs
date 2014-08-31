using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;


namespace Vevisoft.WindowsAPI.Shell
{
    [ComImport]
    [GuidAttribute("00000121-0000-0000-C000-000000000046")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDropSource
    {
        // Determines whether a drag-and-drop operation should continue
        [PreserveSig]
        Int32 QueryContinueDrag(
            bool fEscapePressed,
            ShellAPI.MK grfKeyState);

        // Gives visual feedback to an end user during a drag-and-drop operation
        [PreserveSig]
        Int32 GiveFeedback(
            DragDropEffects dwEffect);
    }
}
