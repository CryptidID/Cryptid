#include <stdio.h>
#include "stdafx.h"
#include "OEM.h"
#include "CommBase.h"
#include "sb_protocol_oem.h"
#include <iostream>
#include <fstream>

/************************************************************************/
/*                                                                      */
/************************************************************************/
#pragma pack(1)
struct FP_BITMAP
{
	BITMAPFILEHEADER bmfHdr;
	BITMAPINFO bmInfo;
	RGBQUAD bmiColors[255];

	FP_BITMAP(int cx, int cy)
	{
		bmfHdr.bfType = ((WORD)('M' << 8) | 'B');  // "BM"
		bmfHdr.bfSize = sizeof(FP_BITMAP) + cx*cy;
		bmfHdr.bfReserved1 = 0;
		bmfHdr.bfReserved2 = 0;
		bmfHdr.bfOffBits = sizeof(FP_BITMAP);

		bmInfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
		bmInfo.bmiHeader.biWidth = cx;
		bmInfo.bmiHeader.biHeight = -cy;
		bmInfo.bmiHeader.biPlanes = 1;
		bmInfo.bmiHeader.biBitCount = 8;
		bmInfo.bmiHeader.biCompression = 0;
		bmInfo.bmiHeader.biSizeImage = cx*cy;
		bmInfo.bmiHeader.biXPelsPerMeter = 0;
		bmInfo.bmiHeader.biYPelsPerMeter = 0;
		bmInfo.bmiHeader.biClrUsed = 0;
		bmInfo.bmiHeader.biClrImportant = 0;

		RGBQUAD *pals = bmInfo.bmiColors;
		for (int i = 0; i < 256; i++) {
			pals[i].rgbBlue = i;
			pals[i].rgbGreen = i;
			pals[i].rgbRed = i;
			pals[i].rgbReserved = 0;
		}
	}
};
#pragma pack()

FP_BITMAP fp_bmp256(256, 256);
FP_BITMAP fp_bmpraw(320, 240);

void fp_bmp_draw(HDC hdc, const FP_BITMAP* fp_bmp, void* p, int x, int y, int cx, int cy)
{
	SetStretchBltMode(hdc, COLORONCOLOR);

	StretchDIBits(hdc, x, y, cx, cy,
		0, 0, fp_bmp->bmInfo.bmiHeader.biWidth, abs(fp_bmp->bmInfo.bmiHeader.biHeight),
		p, &fp_bmp->bmInfo,
		DIB_RGB_COLORS, SRCCOPY);
}

BOOL fp_bmp_save(LPCTSTR szFilePath, FP_BITMAP* fp_bmp, void* p)
{
	CFile f;
	if (!f.Open(szFilePath, CFile::typeBinary | CFile::modeCreate | CFile::modeWrite))
		return FALSE;

	int w = fp_bmp->bmInfo.bmiHeader.biWidth;
	int h_org = fp_bmp->bmInfo.bmiHeader.biHeight;
	int h = abs(h_org);

	fp_bmp->bmInfo.bmiHeader.biHeight = h;
	f.Write(fp_bmp, sizeof(FP_BITMAP));
	fp_bmp->bmInfo.bmiHeader.biHeight = h_org;

	int i;
	for (i = h - 1; i >= 0; i--)
		f.Write((BYTE*)p + i*w, w);

	f.Close();

	return TRUE;
}

char* readFileBytes(const char *name) {
	FILE *fl = fopen(name, "r");
	fseek(fl, 0, SEEK_END);
	long len = ftell(fl);
	char *ret = (char*) malloc(len);
	fseek(fl, 0, SEEK_SET);
	fread(ret, 1, len, fl);
	fclose(fl);
	return ret;
}

extern "C"
{
	__declspec(dllexport) int close(void)
	{
		return oem_close();
	}

	__declspec(dllexport) int usb_internal_check(void) {
		return oem_usb_internal_check();
	}

	__declspec(dllexport) int change_baudrate(int nBaudrate)
	{
		return oem_change_baudrate(nBaudrate);
	}

	__declspec(dllexport) int cmos_led(BOOL bOn)
	{
		return oem_cmos_led(bOn);
	}

	__declspec(dllexport) int enroll_count(void)
	{
		return oem_enroll_count();
	}

	__declspec(dllexport) int check_enrolled(int nPos)
	{
		return oem_check_enrolled(nPos);
	}

	__declspec(dllexport) int enroll_start(int nPos)
	{
		return oem_enroll_start(nPos);
	}

	__declspec(dllexport) int enroll_nth(int nPos, int nTurn)
	{
		return oem_enroll_nth(nPos, nTurn);
	}

	__declspec(dllexport) int is_press_finger(void)
	{
		oem_is_press_finger();
		return gwLastAckParam;
	}

	__declspec(dllexport) int _delete(int nPos)
	{
		return oem_delete(nPos);
	}

	__declspec(dllexport) int delete_all(void)
	{
		return oem_delete_all();
	}

	__declspec(dllexport) int verify(int nPos)
	{
		return oem_verify(nPos);
	}

	__declspec(dllexport) int identify(void)
	{
		return oem_identify();
	}

	__declspec(dllexport) int verify_template(int nPos)
	{
		return oem_verify_template(nPos);
	}

	__declspec(dllexport) int identify_template(void)
	{
		return oem_identify_template();
	}

	__declspec(dllexport) int capture(BOOL bBest)
	{
		return oem_capture(bBest);
	}

	__declspec(dllexport) BYTE* get_image(void)
	{
		oem_get_image();
		return gbyImg8bit;
	}

	__declspec(dllexport) BYTE* get_raw_image(void)
	{
		oem_get_rawimage();
		return gbyImgRaw;
	}

	__declspec(dllexport) int add_template(int nPos)
	{
		return oem_add_template(nPos);
	}

	__declspec(dllexport) int get_database_end(void) {
		return oem_get_database_end();
	}

	__declspec(dllexport) int get_database_start(void) {
		return oem_get_database_start();
	}

	__declspec(dllexport) int open(int port, int baud)
	{
		if (port == 0) {
			if (!comm_open_usb()) {
				return -1;
			}
		} else {
			if (!comm_open_serial(port, 9600)) {
				return -10;
			}
			if (oem_change_baudrate(baud) < 0) {
				return -11;
			}
			if (!comm_open_serial(port, baud)) {
				return -12;
			}
		}

		if (oem_open() < 0) {
			return -3;
		} if (gwLastAck == NACK_INFO) {
			return -4;
		}

		return 1;
	}

}
