/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDcvp.c                                                                                                                     */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Convert from interface to physical data                                                                                         */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDcvp_c
#define IOCARDcvp_c

#include "IOCARDprc.h"
#include "IOCARDvare.h"
#include "convlib.h"
#include "typelib.h"


/* Declarations of Temporary convert Vars */
static AdiFloat32 AdiFloat32_tmp;
static AdiFloat64 AdiFloat64_tmp;
static AdiUInt8 double_buffer_tmp[8];
static AdiUInt8 float_buffer_tmp[4];

/* Declarations of Interfaces Error Flags */
AdiUInt8 HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG;
AdiUInt8 HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG;
AdiUInt8 KEEP_ALIVE_MESSAGE_ERR_FLG;
AdiUInt8 LOG_DATA_MESSAGE_ERR_FLG;
AdiUInt8 NONCE_MESSAGE_ERR_FLG;
AdiUInt8 REQUEST_STATUS_MESSAGE_ERR_FLG;
AdiUInt8 SERIAL_IO2LCU_MESSAGE_ERR_FLG;
AdiUInt8 SERIAL_LCU2IO_MESSAGE_ERR_FLG;
AdiUInt8 SET_DISCRETES_MESSAGE_ERR_FLG;
AdiUInt8 SET_SYSTEM_STATE_MESSAGE_ERR_FLG;
AdiUInt8 START_INTERFACE_MESSAGE_ERR_FLG;
AdiUInt8 STATUS_MESSAGE_ERR_FLG;
AdiUInt8 STOP_INTERFACE_MESSAGE_ERR_FLG;
AdiUInt8 TL_ONLY_MESSAGE_IO2LCU_ERR_FLG;
AdiUInt8 TL_ONLY_MESSAGE_LCU2IO_ERR_FLG;

/* HDR_ONLY_MESSAGE_IO2LCU */
AdiInt HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(PHS_HDRONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG = HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG = HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG + 1;
	}
	
	return HDR_ONLY_MESSAGE_IO2LCU_ERR_FLG;
}

/* HDR_ONLY_MESSAGE_LCU2IO */
AdiInt HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(PHS_HDRONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG = HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG = HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG + 1;
	}
	
	return HDR_ONLY_MESSAGE_LCU2IO_ERR_FLG;
}

/* KEEP_ALIVE_MESSAGE */
AdiInt KEEP_ALIVE_MESSAGE_CONVERT_TO_PH(PHS_KEEPALIVEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	KEEP_ALIVE_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			KEEP_ALIVE_MESSAGE_ERR_FLG = KEEP_ALIVE_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		KEEP_ALIVE_MESSAGE_ERR_FLG = KEEP_ALIVE_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 24);
	
	return KEEP_ALIVE_MESSAGE_ERR_FLG;
}

/* LOG_DATA_MESSAGE */
AdiInt LOG_DATA_MESSAGE_CONVERT_TO_PH(PHS_LOGDATAMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	LOG_DATA_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			LOG_DATA_MESSAGE_ERR_FLG = LOG_DATA_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		LOG_DATA_MESSAGE_ERR_FLG = LOG_DATA_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_LOG_STRUCT */
	/* DT_LOG.logLevel */
	phsPtr->DT_LOG.logLevel = (E_LOG_LEVEL)get_ulong_align(intfPtr + 24);
	
	/* DT_LOG.logId */
	phsPtr->DT_LOG.logId = (E_LOG_ID)get_ulong_align(intfPtr + 28);
	
	/* Structure - BUFFER_STRUCT */
	/* DT_LOG.logStr.Size */
	phsPtr->DT_LOG.logStr.Size = (AdiUInt32)get_ulong_align(intfPtr + 32);
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_LOG.logStr.Size); i1++, locationOffset1+= 1)
		{
			/* DT_LOG.logStr.data[i1] */
			phsPtr->DT_LOG.logStr.data[i1] = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 36 + locationOffset1));
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 1060);
	
	return LOG_DATA_MESSAGE_ERR_FLG;
}

/* NONCE_MESSAGE */
AdiInt NONCE_MESSAGE_CONVERT_TO_PH(PHS_NONCEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	NONCE_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			NONCE_MESSAGE_ERR_FLG = NONCE_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		NONCE_MESSAGE_ERR_FLG = NONCE_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_NONCE_STRUCT */
	/* DT_NONCE.ipAddr */
	phsPtr->DT_NONCE.ipAddr = (AdiUInt32)get_ulong_align(intfPtr + 24);
	
	/* DT_NONCE.pbitStatus */
	phsPtr->DT_NONCE.pbitStatus = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 32);
	
	return NONCE_MESSAGE_ERR_FLG;
}

/* REQUEST_STATUS_MESSAGE */
AdiInt REQUEST_STATUS_MESSAGE_CONVERT_TO_PH(PHS_REQUESTSTATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	REQUEST_STATUS_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			REQUEST_STATUS_MESSAGE_ERR_FLG = REQUEST_STATUS_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		REQUEST_STATUS_MESSAGE_ERR_FLG = REQUEST_STATUS_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_REQUEST_STATUS_STRUCT */
	/* DT_REQUEST_STATUS.reqId */
	phsPtr->DT_REQUEST_STATUS.reqId = (E_MESSAGE_TYPE)get_ulong_align(intfPtr + 24);
	
	/* DT_REQUEST_STATUS.errId */
	phsPtr->DT_REQUEST_STATUS.errId = (E_ERROR_ID)get_ulong_align(intfPtr + 28);
	
	/* Structure - BUFFER_STRUCT */
	/* DT_REQUEST_STATUS.errString.Size */
	phsPtr->DT_REQUEST_STATUS.errString.Size = (AdiUInt32)get_ulong_align(intfPtr + 32);
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_REQUEST_STATUS.errString.Size); i1++, locationOffset1+= 1)
		{
			/* DT_REQUEST_STATUS.errString.data[i1] */
			phsPtr->DT_REQUEST_STATUS.errString.data[i1] = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 36 + locationOffset1));
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 1060);
	
	return REQUEST_STATUS_MESSAGE_ERR_FLG;
}

/* SERIAL_IO2LCU_MESSAGE */
AdiInt SERIAL_IO2LCU_MESSAGE_CONVERT_TO_PH(PHS_SERIALIO2LCUMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	SERIAL_IO2LCU_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			SERIAL_IO2LCU_MESSAGE_ERR_FLG = SERIAL_IO2LCU_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		SERIAL_IO2LCU_MESSAGE_ERR_FLG = SERIAL_IO2LCU_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_SERIAL_IO2LCU_STRUCT */
	/* DT_SERIAL_IO2LCU.serialId */
	phsPtr->DT_SERIAL_IO2LCU.serialId = (AdiUInt32)get_ulong_align(intfPtr + 24);
	
	/* Structure - BUFFER_STRUCT */
	/* DT_SERIAL_IO2LCU.data.Size */
	phsPtr->DT_SERIAL_IO2LCU.data.Size = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_SERIAL_IO2LCU.data.Size); i1++, locationOffset1+= 1)
		{
			/* DT_SERIAL_IO2LCU.data.data[i1] */
			phsPtr->DT_SERIAL_IO2LCU.data.data[i1] = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1));
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 1056);
	
	return SERIAL_IO2LCU_MESSAGE_ERR_FLG;
}

/* SERIAL_LCU2IO_MESSAGE */
AdiInt SERIAL_LCU2IO_MESSAGE_CONVERT_TO_PH(PHS_SERIALLCU2IOMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	SERIAL_LCU2IO_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			SERIAL_LCU2IO_MESSAGE_ERR_FLG = SERIAL_LCU2IO_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		SERIAL_LCU2IO_MESSAGE_ERR_FLG = SERIAL_LCU2IO_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_SERIAL_LCU2IO_STRUCT */
	/* DT_SERIAL_LCU2IO.uartId */
	phsPtr->DT_SERIAL_LCU2IO.uartId = (E_UART_ID)get_ulong_align(intfPtr + 24);
	
	/* Structure - BUFFER_STRUCT */
	/* DT_SERIAL_LCU2IO.data.Size */
	phsPtr->DT_SERIAL_LCU2IO.data.Size = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_SERIAL_LCU2IO.data.Size); i1++, locationOffset1+= 1)
		{
			/* DT_SERIAL_LCU2IO.data.data[i1] */
			phsPtr->DT_SERIAL_LCU2IO.data.data[i1] = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1));
			
		}
	}
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 1056);
	
	return SERIAL_LCU2IO_MESSAGE_ERR_FLG;
}

/* SET_DISCRETES_MESSAGE */
AdiInt SET_DISCRETES_MESSAGE_CONVERT_TO_PH(PHS_SETDISCRETESMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	SET_DISCRETES_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			SET_DISCRETES_MESSAGE_ERR_FLG = SET_DISCRETES_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		SET_DISCRETES_MESSAGE_ERR_FLG = SET_DISCRETES_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_SET_DISC_STRUCT */
	/* DT_SET_DISC.discNum */
	phsPtr->DT_SET_DISC.discNum = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 24));
	
	/* Array - DISCRETE_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 256 && (i1 < phsPtr->DT_SET_DISC.discNum); i1++, locationOffset1+= 2)
		{
			/* Structure - DISCRETE_STRUCT */
			/* DT_SET_DISC.discretes[i1].discreteId */
			phsPtr->DT_SET_DISC.discretes[i1].discreteId = (E_DISCRETE_ID)(*(AdiUInt8 *)(intfPtr + 25 + locationOffset1));
			
			/* DT_SET_DISC.discretes[i1].discreteValue */
			phsPtr->DT_SET_DISC.discretes[i1].discreteValue = (E_BOOLEAN)(*(AdiUInt8 *)(intfPtr + 26 + locationOffset1));
			
		}
	}
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 537);
	
	return SET_DISCRETES_MESSAGE_ERR_FLG;
}

/* SET_SYSTEM_STATE_MESSAGE */
AdiInt SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_PH(PHS_SETSYSTEMSTATEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	SET_SYSTEM_STATE_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			SET_SYSTEM_STATE_MESSAGE_ERR_FLG = SET_SYSTEM_STATE_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		SET_SYSTEM_STATE_MESSAGE_ERR_FLG = SET_SYSTEM_STATE_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_SET_SYS_STATE_STRUCT */
	/* DT_SET_SYS_STATE.sysState */
	phsPtr->DT_SET_SYS_STATE.sysState = (E_UNIT_STATE)get_ulong_align(intfPtr + 24);
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	return SET_SYSTEM_STATE_MESSAGE_ERR_FLG;
}

/* START_INTERFACE_MESSAGE */
AdiInt START_INTERFACE_MESSAGE_CONVERT_TO_PH(PHS_STARTINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	START_INTERFACE_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			START_INTERFACE_MESSAGE_ERR_FLG = START_INTERFACE_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		START_INTERFACE_MESSAGE_ERR_FLG = START_INTERFACE_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_START_INTERFACE_STRUCT */
	/* DT_START_INTERFACE.uartId */
	phsPtr->DT_START_INTERFACE.uartId = (E_UART_ID)get_ulong_align(intfPtr + 24);
	
	/* DT_START_INTERFACE.baudrate */
	phsPtr->DT_START_INTERFACE.baudrate = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	/* DT_START_INTERFACE.txParity */
	phsPtr->DT_START_INTERFACE.txParity = (E_PARITY)get_ulong_align(intfPtr + 32);
	
	/* DT_START_INTERFACE.rxParity */
	phsPtr->DT_START_INTERFACE.rxParity = (E_PARITY)get_ulong_align(intfPtr + 36);
	
	/* DT_START_INTERFACE.stopBits */
	phsPtr->DT_START_INTERFACE.stopBits = (AdiUInt32)get_ulong_align(intfPtr + 40);
	
	/* DT_START_INTERFACE.dataBits */
	phsPtr->DT_START_INTERFACE.dataBits = (AdiUInt32)get_ulong_align(intfPtr + 44);
	
	/* DT_START_INTERFACE.loopback */
	phsPtr->DT_START_INTERFACE.loopback = (E_BOOLEAN)get_ulong_align(intfPtr + 48);
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 52);
	
	return START_INTERFACE_MESSAGE_ERR_FLG;
}

/* STATUS_MESSAGE */
AdiInt STATUS_MESSAGE_CONVERT_TO_PH(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	STATUS_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	phsPtr->HDR_IO2LCU.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_IO2LCU.totalLength */
	phsPtr->HDR_IO2LCU.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_IO2LCU.statusSeqNum */
	phsPtr->HDR_IO2LCU.statusSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_IO2LCU.unitId */
	phsPtr->HDR_IO2LCU.unitId = (E_UNIT_ID)get_ulong_align(intfPtr + 8);
	
	/* HDR_IO2LCU.nonce */
	phsPtr->HDR_IO2LCU.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_IO2LCU.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			STATUS_MESSAGE_ERR_FLG = STATUS_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_IO2LCU.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		STATUS_MESSAGE_ERR_FLG = STATUS_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_MONITORED_DATA_STRUCT */
	/* Structure - REGISTERED_UNITS_STRUCT */
	/* DT_MONITORED_DATA.registeredUnits.length */
	phsPtr->DT_MONITORED_DATA.registeredUnits.length = (AdiUInt32)get_ulong_align(intfPtr + 24);
	
	/* Array - REGISTERED_UNITS_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 10 && (i1 < phsPtr->DT_MONITORED_DATA.registeredUnits.length); i1++, locationOffset1+= 8)
		{
			/* Structure - sRegInfo_STRUCT */
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 28 + locationOffset1));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 29 + locationOffset1));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision = (AdiUInt16)get_ushort_align(intfPtr + 30 + locationOffset1);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId = (E_UNIT_ID)(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus = (E_CBIT_STATUS)(*(AdiUInt8 *)(intfPtr + 33 + locationOffset1));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus = (E_REGISTRATION_STATUS)(*(AdiUInt8 *)(intfPtr + 34 + locationOffset1));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState */
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState = (E_UNIT_STATE)(*(AdiUInt8 *)(intfPtr + 35 + locationOffset1));
			
		}
	}
	/* Structure - sRegInfo_STRUCT */
	/* DT_MONITORED_DATA.internalUnitInfo.unitMainVer */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitMainVer = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 108));
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitSubVer */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitSubVer = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 109));
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitRevision */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitRevision = (AdiUInt16)get_ushort_align(intfPtr + 110);
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitId */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitId = (E_UNIT_ID)(*(AdiUInt8 *)(intfPtr + 112));
	
	/* DT_MONITORED_DATA.internalUnitInfo.cbitStatus */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.cbitStatus = (E_CBIT_STATUS)(*(AdiUInt8 *)(intfPtr + 113));
	
	/* DT_MONITORED_DATA.internalUnitInfo.registerStatus */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.registerStatus = (E_REGISTRATION_STATUS)(*(AdiUInt8 *)(intfPtr + 114));
	
	/* DT_MONITORED_DATA.internalUnitInfo.sysState */
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.sysState = (E_UNIT_STATE)(*(AdiUInt8 *)(intfPtr + 115));
	
	/* Structure - MONITORED_DATA_DISC_STRUCT */
	/* DT_MONITORED_DATA.periodicDiscInfo.length */
	phsPtr->DT_MONITORED_DATA.periodicDiscInfo.length = (AdiUInt32)get_ulong_align(intfPtr + 116);
	
	/* Array - PERIODIC_DISC_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 256 && (i1 < phsPtr->DT_MONITORED_DATA.periodicDiscInfo.length); i1++, locationOffset1+= 2)
		{
			/* Structure - sDiscInfo_STRUCT */
			/* DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId */
			phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId = (E_DISCRETE_ID)(*(AdiUInt8 *)(intfPtr + 120 + locationOffset1));
			
			/* DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue */
			phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue = (E_BOOLEAN)(*(AdiUInt8 *)(intfPtr + 121 + locationOffset1));
			
		}
	}
	/* Structure - ANALOG_INPUTS_STRUCT */
	/* DT_MONITORED_DATA.analogInputsInfo.length */
	phsPtr->DT_MONITORED_DATA.analogInputsInfo.length = (AdiUInt32)get_ulong_align(intfPtr + 632);
	
	/* Array - ANALOG_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 100 && (i1 < phsPtr->DT_MONITORED_DATA.analogInputsInfo.length); i1++, locationOffset1+= 8)
		{
			/* Structure - sAnalogInfo_STRUCT */
			/* DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId */
			phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId = (AdiUInt32)get_ulong_align(intfPtr + 636 + locationOffset1);
			
			/* DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal */
			align_buffer(float_buffer_tmp, 4, (intfPtr + 640 + locationOffset1), 4);
			if (check_if_float(float_buffer_tmp, 4))
			{
				AdiFloat32_tmp = (AdiFloat32)get_float_align(intfPtr + 640 + locationOffset1);
				if (( AdiFloat32_tmp <-99999999999999999.9) || ( AdiFloat32_tmp >999999999999999999.9))
				{
					STATUS_MESSAGE_ERR_FLG = STATUS_MESSAGE_ERR_FLG + 1;
				}
				else
				{
					phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal = (AdiFloat32)(AdiFloat32_tmp);
				}
			}
			else
			{
				STATUS_MESSAGE_ERR_FLG = STATUS_MESSAGE_ERR_FLG + 1;
			}
			
		}
	}
	/* Structure - SERIAL_INPUTS_STRUCT */
	/* DT_MONITORED_DATA.serialInputsInfo.length */
	phsPtr->DT_MONITORED_DATA.serialInputsInfo.length = (AdiUInt32)get_ulong_align(intfPtr + 1436);
	
	/* Array - SERIAL_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 10 && (i1 < phsPtr->DT_MONITORED_DATA.serialInputsInfo.length); i1++, locationOffset1+= 1032)
		{
			/* Structure - sSerialInfo_STRUCT */
			/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId */
			phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId = (AdiUInt32)get_ulong_align(intfPtr + 1440 + locationOffset1);
			
			/* Structure - BUFFER_STRUCT */
			/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size */
			phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size = (AdiUInt32)get_ulong_align(intfPtr + 1444 + locationOffset1);
			
			/* Array - SERIAL_DATA_BUFFER_ARR */
			{
				AdiInt i2;
				AdiInt locationOffset2= 0;
				for (i2 = 0; i2 < 1024 && (i2 < phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size); i2++, locationOffset2+= 1)
				{
					/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.data[i2] */
					phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.data[i2] = (AdiUInt8)(*(AdiUInt8 *)(intfPtr + 1448 + locationOffset1 + locationOffset2));
					
				}
			}
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 11760);
	
	return STATUS_MESSAGE_ERR_FLG;
}

/* STOP_INTERFACE_MESSAGE */
AdiInt STOP_INTERFACE_MESSAGE_CONVERT_TO_PH(PHS_STOPINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	STOP_INTERFACE_MESSAGE_ERR_FLG = 0;
	
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	phsPtr->HDR_LCU2IO.msgId = (E_MESSAGE_TYPE)get_ushort_align(intfPtr + 0);
	
	/* HDR_LCU2IO.totalLength */
	phsPtr->HDR_LCU2IO.totalLength = (AdiUInt16)get_ushort_align(intfPtr + 2);
	
	/* HDR_LCU2IO.requestSeqNum */
	phsPtr->HDR_LCU2IO.requestSeqNum = (AdiUInt32)get_ulong_align(intfPtr + 4);
	
	/* HDR_LCU2IO.enableResponse */
	phsPtr->HDR_LCU2IO.enableResponse = (E_BOOLEAN)get_ulong_align(intfPtr + 8);
	
	/* HDR_LCU2IO.nonce */
	phsPtr->HDR_LCU2IO.nonce = (AdiUInt32)get_ulong_align(intfPtr + 12);
	
	/* HDR_LCU2IO.timestamp */
	align_buffer(double_buffer_tmp, 8, (intfPtr + 16), 8);
	if (check_if_double(double_buffer_tmp, 8))
	{
		AdiFloat64_tmp = (AdiFloat64)get_double_align(intfPtr + 16);
		if (( AdiFloat64_tmp <-99999999999999999.9) || ( AdiFloat64_tmp >999999999999999999.9))
		{
			STOP_INTERFACE_MESSAGE_ERR_FLG = STOP_INTERFACE_MESSAGE_ERR_FLG + 1;
		}
		else
		{
			phsPtr->HDR_LCU2IO.timestamp = (AdiFloat32)(AdiFloat64_tmp);
		}
	}
	else
	{
		STOP_INTERFACE_MESSAGE_ERR_FLG = STOP_INTERFACE_MESSAGE_ERR_FLG + 1;
	}
	
	/* Structure - DT_STOP_INTERFACE_STRUCT */
	/* DT_STOP_INTERFACE.uartId */
	phsPtr->DT_STOP_INTERFACE.uartId = (E_UART_ID)get_ulong_align(intfPtr + 24);
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 28);
	
	return STOP_INTERFACE_MESSAGE_ERR_FLG;
}

/* TL_ONLY_MESSAGE_IO2LCU */
AdiInt TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(PHS_TLONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	TL_ONLY_MESSAGE_IO2LCU_ERR_FLG = 0;
	
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	phsPtr->TL_IO2LCU.crc = (AdiUInt32)get_ulong_align(intfPtr + 0);
	
	return TL_ONLY_MESSAGE_IO2LCU_ERR_FLG;
}

/* TL_ONLY_MESSAGE_LCU2IO */
AdiInt TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(PHS_TLONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr)
{
	/* init interface error flag */
	TL_ONLY_MESSAGE_LCU2IO_ERR_FLG = 0;
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	phsPtr->TL_LCU2IO.crc = (AdiUInt32)get_ulong_align(intfPtr + 0);
	
	return TL_ONLY_MESSAGE_LCU2IO_ERR_FLG;
}


/* Declarations of Compound Interfaces Error Flags */


/* Convert a message in the bus to Physical (using the static physical structure). */
AdiInt IOCARD_LCU_PR_ConvertToPhysical(AdiInt MsgID, AdiUInt8* MessageBuffer)
{
	switch(MsgID)
	{
	case 1002: /* HDR_ONLY_MESSAGE_LCU2IO */
		return HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(&Phs_hdronlymessagelcu2io, MessageBuffer);
	case 7: /* KEEP_ALIVE_MESSAGE */
		return KEEP_ALIVE_MESSAGE_CONVERT_TO_PH(&Phs_keepalivemessage, MessageBuffer);
	case 2: /* SERIAL_LCU2IO_MESSAGE */
		return SERIAL_LCU2IO_MESSAGE_CONVERT_TO_PH(&Phs_seriallcu2iomessage, MessageBuffer);
	case 1: /* SET_DISCRETES_MESSAGE */
		return SET_DISCRETES_MESSAGE_CONVERT_TO_PH(&Phs_setdiscretesmessage, MessageBuffer);
	case 3: /* SET_SYSTEM_STATE_MESSAGE */
		return SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_PH(&Phs_setsystemstatemessage, MessageBuffer);
	case 5: /* START_INTERFACE_MESSAGE */
		return START_INTERFACE_MESSAGE_CONVERT_TO_PH(&Phs_startinterfacemessage, MessageBuffer);
	case 6: /* STOP_INTERFACE_MESSAGE */
		return STOP_INTERFACE_MESSAGE_CONVERT_TO_PH(&Phs_stopinterfacemessage, MessageBuffer);
	case 1004: /* TL_ONLY_MESSAGE_LCU2IO */
		return TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_PH(&Phs_tlonlymessagelcu2io, MessageBuffer);
	case 1001: /* HDR_ONLY_MESSAGE_IO2LCU */
		return HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(&Phs_hdronlymessageio2lcu, MessageBuffer);
	case 104: /* LOG_DATA_MESSAGE */
		return LOG_DATA_MESSAGE_CONVERT_TO_PH(&Phs_logdatamessage, MessageBuffer);
	case 200: /* NONCE_MESSAGE */
		return NONCE_MESSAGE_CONVERT_TO_PH(&Phs_noncemessage, MessageBuffer);
	case 101: /* REQUEST_STATUS_MESSAGE */
		return REQUEST_STATUS_MESSAGE_CONVERT_TO_PH(&Phs_requeststatusmessage, MessageBuffer);
	case 103: /* SERIAL_IO2LCU_MESSAGE */
		return SERIAL_IO2LCU_MESSAGE_CONVERT_TO_PH(&Phs_serialio2lcumessage, MessageBuffer);
	case 102: /* STATUS_MESSAGE */
		return STATUS_MESSAGE_CONVERT_TO_PH(&Phs_statusmessage, MessageBuffer);
	case 1003: /* TL_ONLY_MESSAGE_IO2LCU */
		return TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_PH(&Phs_tlonlymessageio2lcu, MessageBuffer);
	default:  return -1;
	}
}




#endif 
