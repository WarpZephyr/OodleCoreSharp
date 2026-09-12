#pragma once
#include <stdarg.h>

#if defined(_WIN32)
    #define INTEROP_API __declspec(dllexport)
#else
    #define INTEROP_API __attribute__((visibility("default")))
#endif

typedef void (*OodlePrintf_t)(int verbosity, const char* file, const int line, const char* format, ...);
typedef void (*OodlePlugins_SetPrintf_t)(OodlePrintf_t cb);
typedef void (*OodleInterop_Print_t)(int verbosity, const char* file, const int line, const char* message);

extern "C"
{
    INTEROP_API void OodleInterop_Oodle25_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint);
    INTEROP_API void OodleInterop_Oodle26_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint);
    INTEROP_API void OodleInterop_Oodle28_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint);
    INTEROP_API void OodleInterop_Oodle29_SetPrint(OodlePlugins_SetPrintf_t pSetPrintf, OodleInterop_Print_t pPrint);
}