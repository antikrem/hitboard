// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the VIGEWRAPPER_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// VIGEWRAPPER_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef VIGEMWRAPPER_EXPORTS
#define VIGEMWRAPPER_API __declspec(dllexport)
#else
#define VIGEMWRAPPER_API __declspec(dllimport)
#endif

enum ViGEmWrapper_ErrorCode {
	SUCCESS = 0,
	MEMALLOC = 1,
	BUSFAIL = 2,
	PLUGIN = 3,
	INPUT_DROPPED = 4
};

/**
 * Initialises the library
 * And sets up a connection to VIGE
*/
extern "C"
VIGEMWRAPPER_API int ViGEmWrapper_Initialise(void);

/**
 * Set controller to be in given input state
 */
extern "C"
VIGEMWRAPPER_API int ViGEmWrapper_Input(XINPUT_STATE* input);

/**
 * Free ViGEm resources
*/
extern "C"
VIGEMWRAPPER_API int ViGEmWrapper_Free(void);