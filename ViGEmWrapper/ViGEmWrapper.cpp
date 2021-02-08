// ViGEWrapper.cpp : Defines the exported functions for the DLL.
//

#include "framework.hpp"
#include "ViGEmWrapper.hpp"

// Client used for G State
PVIGEM_CLIENT client = nullptr;
PVIGEM_TARGET pad = nullptr;

VIGEMWRAPPER_API int ViGEmWrapper_Initialise(void)
{
	// Create client and pad
	client = vigem_alloc();
	pad = vigem_target_x360_alloc();

	// Allocate client
	if (!client || !pad) {
		return ViGEmWrapper_ErrorCode::MEMALLOC;
	}

	// Connect to underlying driver
	const auto conerror = vigem_connect(client);
	if (!VIGEM_SUCCESS(conerror))
	{
		return ViGEmWrapper_ErrorCode::BUSFAIL;
	}

	// Plug in pad
	const auto error = vigem_target_add(client, pad);
	if (!VIGEM_SUCCESS(error))
	{
		return ViGEmWrapper_ErrorCode::PLUGIN;
	}


	return ViGEmWrapper_ErrorCode::SUCCESS;
}

VIGEMWRAPPER_API int ViGEmWrapper_Input(XINPUT_STATE* input)
{
	return 
			VIGEM_SUCCESS(
					vigem_target_x360_update(client, pad, *reinterpret_cast<XUSB_REPORT*>(&input->Gamepad))
				) 
			? ViGEmWrapper_ErrorCode::SUCCESS 
			: ViGEmWrapper_ErrorCode::INPUT_DROPPED;
}

VIGEMWRAPPER_API int ViGEmWrapper_Free(void) 
{
	vigem_disconnect(client);
	vigem_free(client);

	return 0;
}