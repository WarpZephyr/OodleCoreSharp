#include "oodleinterop.h"
#include <stdio.h>

template<int Version>
class Printer {
public:
    static OodleInterop_Print_t s_pPrint;
    static void OodleInterop_Printf(int verbosity, const char* file, const int line, const char* format, ...) {
        if (!s_pPrint)
            return;
        
        va_list args;
        va_start(args, format);
        char buffer[4096];
        buffer[0] = 0;
        int result = vsnprintf(buffer, sizeof(buffer), format, args);
        va_end(args);
        
        s_pPrint(verbosity, file, line, buffer);
    }
};

template<int Version>
OodleInterop_Print_t Printer<Version>::s_pPrint = nullptr;

extern "C" {
    INTEROP_API void OodleInterop_Oodle25_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint) {
        Printer<5>::s_pPrint = pPrint;
        if (pSetPrintf != nullptr) {
            pSetPrintf(Printer<5>::OodleInterop_Printf);
        }
    }
    
    INTEROP_API void OodleInterop_Oodle26_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint) {
        Printer<6>::s_pPrint = pPrint;
        if (pSetPrintf != nullptr) {
            pSetPrintf(Printer<6>::OodleInterop_Printf);
        }
    }
    
    INTEROP_API void OodleInterop_Oodle28_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint) {
        Printer<8>::s_pPrint = pPrint;
        if (pSetPrintf != nullptr) {
            pSetPrintf(Printer<8>::OodleInterop_Printf);
        }
    }
    
    INTEROP_API void OodleInterop_Oodle29_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint) {
        Printer<9>::s_pPrint = pPrint;
        if (pSetPrintf != nullptr) {
            pSetPrintf(Printer<9>::OodleInterop_Printf);
        }
    }
}
