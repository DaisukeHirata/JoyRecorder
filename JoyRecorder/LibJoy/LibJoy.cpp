// joy4d2u.cspp
#include	<windows.h>
#include	<stdio.h>
#include	<math.h>
#include	<iostream>

extern "C" {
#include	"PPJIoctl.h"
};

using namespace std;

#define	VIRTUAL_JOYSTICK_DEVICE	"\\\\.\\PPJoyIOCTL1"

#define NUM_ANALOG  8
#define NUM_DIGITAL 16

#pragma pack( push, 1 )
typedef struct {
	unsigned long	Signature;
	char	NumAnalog;
	long	Analog[ NUM_ANALOG ];
	char	NumDigital;
	char	Digital[ NUM_DIGITAL ];
} JOYSTICK_STATE;
#pragma pack( pop )

#define XAXIS 1
#define YAXIS 2
#define ZROTATION 3
#define UP 4
#define DOWN 5
#define LEFT 6
#define RIGHT 7
#define ENTERBUTTON 4
#define ZROTATION 3

static HANDLE	h;

void initJoystick()
{
	// open virtual joystick device
	h = CreateFile( VIRTUAL_JOYSTICK_DEVICE,
					GENERIC_WRITE,
					FILE_SHARE_WRITE,
					NULL,
					OPEN_EXISTING,
					0,
					NULL );
	if ( h == INVALID_HANDLE_VALUE ) {
		cerr << "cannot open " << VIRTUAL_JOYSTICK_DEVICE << endl;
		exit(-1);
	}
}

void closeJoystick()
{
	// open virtual joystick device
	if ( !CloseHandle( h )) {
		DWORD	rc = GetLastError();
		cerr << "CloseHandle error " << rc << endl;
	}
}

void moveJoystick(int xaxis, int yaxis, int zrotate, int button1)
{
	// prepare joystick state struct
	JOYSTICK_STATE	js;

	js.Signature = JOYSTICK_STATE_V1;
	js.NumAnalog = NUM_ANALOG;
	js.NumDigital = NUM_DIGITAL;

	js.Analog[ 0 ] = xaxis;
	js.Analog[ 1 ] = yaxis;
	js.Analog[ 3 ] = zrotate;

	js.Digital[ 0 ] = button1;		 // pressed 1, not pressed 0

	// send joystick state to virtual joystick
	DWORD	RetSize;
	if ( !DeviceIoControl( h, IOCTL_PPORTJOY_SET_STATE,
						   &js, sizeof( js ), NULL, 0, &RetSize, NULL )) {
		// check error	
		DWORD	rc = GetLastError();
		if ( rc == 2 ) {
			cerr << "Joystick device deleted. Quit" << endl;
		} else {
			cerr << "DeviceIoControl error " << rc << endl;
		}
	}
}