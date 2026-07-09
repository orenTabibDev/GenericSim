/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDcvi.c                                                                                                                     */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Convert from physical to interface data                                                                                         */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDcvi_c
#define IOCARDcvi_c

#include "IOCARDprc.h"
#include "IOCARDvare.h"
#include "convlib.h"
#include "typelib.h"


/* Declarations of Temporary convert Vars */

/* HDR_ONLY_MESSAGE_IO2LCU */
void HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(PHS_HDRONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
}

/* HDR_ONLY_MESSAGE_LCU2IO */
void HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(PHS_HDRONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
}

/* KEEP_ALIVE_MESSAGE */
void KEEP_ALIVE_MESSAGE_CONVERT_TO_IN(PHS_KEEPALIVEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* LOG_DATA_MESSAGE */
void LOG_DATA_MESSAGE_CONVERT_TO_IN(PHS_LOGDATAMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
	/* Structure - DT_LOG_STRUCT */
	/* DT_LOG.logLevel */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_LOG.logLevel));
	
	/* DT_LOG.logId */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_LOG.logId));
	
	/* Structure - BUFFER_STRUCT */
	/* DT_LOG.logStr.Size */
	set_ulong_align((intfPtr + 32), (AdiUInt32)(phsPtr->DT_LOG.logStr.Size));
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_LOG.logStr.Size); i1++, locationOffset1+=1)
		{
			/* DT_LOG.logStr.data[i1] */
			(*(AdiUInt8 *)(intfPtr + 36 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_LOG.logStr.data[i1]);
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 1060), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* NONCE_MESSAGE */
void NONCE_MESSAGE_CONVERT_TO_IN(PHS_NONCEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
	/* Structure - DT_NONCE_STRUCT */
	/* DT_NONCE.ipAddr */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_NONCE.ipAddr));
	
	/* DT_NONCE.pbitStatus */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_NONCE.pbitStatus));
	
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 32), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* REQUEST_STATUS_MESSAGE */
void REQUEST_STATUS_MESSAGE_CONVERT_TO_IN(PHS_REQUESTSTATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
	/* Structure - DT_REQUEST_STATUS_STRUCT */
	/* DT_REQUEST_STATUS.reqId */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_REQUEST_STATUS.reqId));
	
	/* DT_REQUEST_STATUS.errId */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_REQUEST_STATUS.errId));
	
	/* Structure - BUFFER_STRUCT */
	/* DT_REQUEST_STATUS.errString.Size */
	set_ulong_align((intfPtr + 32), (AdiUInt32)(phsPtr->DT_REQUEST_STATUS.errString.Size));
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_REQUEST_STATUS.errString.Size); i1++, locationOffset1+=1)
		{
			/* DT_REQUEST_STATUS.errString.data[i1] */
			(*(AdiUInt8 *)(intfPtr + 36 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_REQUEST_STATUS.errString.data[i1]);
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 1060), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* SERIAL_IO2LCU_MESSAGE */
void SERIAL_IO2LCU_MESSAGE_CONVERT_TO_IN(PHS_SERIALIO2LCUMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
	/* Structure - DT_SERIAL_IO2LCU_STRUCT */
	/* DT_SERIAL_IO2LCU.serialId */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_SERIAL_IO2LCU.serialId));
	
	/* Structure - BUFFER_STRUCT */
	/* DT_SERIAL_IO2LCU.data.Size */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_SERIAL_IO2LCU.data.Size));
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_SERIAL_IO2LCU.data.Size); i1++, locationOffset1+=1)
		{
			/* DT_SERIAL_IO2LCU.data.data[i1] */
			(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_SERIAL_IO2LCU.data.data[i1]);
			
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 1056), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* SERIAL_LCU2IO_MESSAGE */
void SERIAL_LCU2IO_MESSAGE_CONVERT_TO_IN(PHS_SERIALLCU2IOMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - DT_SERIAL_LCU2IO_STRUCT */
	/* DT_SERIAL_LCU2IO.uartId */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_SERIAL_LCU2IO.uartId));
	
	/* Structure - BUFFER_STRUCT */
	/* DT_SERIAL_LCU2IO.data.Size */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_SERIAL_LCU2IO.data.Size));
	
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 1024 && (i1 < phsPtr->DT_SERIAL_LCU2IO.data.Size); i1++, locationOffset1+=1)
		{
			/* DT_SERIAL_LCU2IO.data.data[i1] */
			(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_SERIAL_LCU2IO.data.data[i1]);
			
		}
	}
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 1056), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* SET_DISCRETES_MESSAGE */
void SET_DISCRETES_MESSAGE_CONVERT_TO_IN(PHS_SETDISCRETESMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - DT_SET_DISC_STRUCT */
	/* DT_SET_DISC.discNum */
	(*(AdiUInt8 *)(intfPtr + 24)) = (AdiUInt8)(phsPtr->DT_SET_DISC.discNum);
	
	/* Array - DISCRETE_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 256 && (i1 < phsPtr->DT_SET_DISC.discNum); i1++, locationOffset1+=2)
		{
			/* Structure - DISCRETE_STRUCT */
			/* DT_SET_DISC.discretes[i1].discreteId */
			(*(AdiUInt8 *)(intfPtr + 25 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_SET_DISC.discretes[i1].discreteId);
			
			/* DT_SET_DISC.discretes[i1].discreteValue */
			(*(AdiUInt8 *)(intfPtr + 26 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_SET_DISC.discretes[i1].discreteValue);
			
		}
	}
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 537), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* SET_SYSTEM_STATE_MESSAGE */
void SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_IN(PHS_SETSYSTEMSTATEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - DT_SET_SYS_STATE_STRUCT */
	/* DT_SET_SYS_STATE.sysState */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_SET_SYS_STATE.sysState));
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* START_INTERFACE_MESSAGE */
void START_INTERFACE_MESSAGE_CONVERT_TO_IN(PHS_STARTINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - DT_START_INTERFACE_STRUCT */
	/* DT_START_INTERFACE.uartId */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_START_INTERFACE.uartId));
	
	/* DT_START_INTERFACE.baudrate */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->DT_START_INTERFACE.baudrate));
	
	/* DT_START_INTERFACE.txParity */
	set_ulong_align((intfPtr + 32), (AdiUInt32)(phsPtr->DT_START_INTERFACE.txParity));
	
	/* DT_START_INTERFACE.rxParity */
	set_ulong_align((intfPtr + 36), (AdiUInt32)(phsPtr->DT_START_INTERFACE.rxParity));
	
	/* DT_START_INTERFACE.stopBits */
	set_ulong_align((intfPtr + 40), (AdiUInt32)(phsPtr->DT_START_INTERFACE.stopBits));
	
	/* DT_START_INTERFACE.dataBits */
	set_ulong_align((intfPtr + 44), (AdiUInt32)(phsPtr->DT_START_INTERFACE.dataBits));
	
	/* DT_START_INTERFACE.loopback */
	set_ulong_align((intfPtr + 48), (AdiUInt32)(phsPtr->DT_START_INTERFACE.loopback));
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 52), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* STATUS_MESSAGE */
void STATUS_MESSAGE_CONVERT_TO_IN(PHS_STATUSMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_IO2LCU_STRUCT */
	/* HDR_IO2LCU.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_IO2LCU.msgId));
	
	/* HDR_IO2LCU.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_IO2LCU.totalLength));
	
	/* HDR_IO2LCU.statusSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_IO2LCU.statusSeqNum));
	
	/* HDR_IO2LCU.unitId */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_IO2LCU.unitId));
	
	/* HDR_IO2LCU.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_IO2LCU.nonce));
	
	/* HDR_IO2LCU.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_IO2LCU.timestamp));
	
	/* Structure - DT_MONITORED_DATA_STRUCT */
	/* Structure - REGISTERED_UNITS_STRUCT */
	/* DT_MONITORED_DATA.registeredUnits.length */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.registeredUnits.length));
	
	/* Array - REGISTERED_UNITS_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 10 && (i1 < phsPtr->DT_MONITORED_DATA.registeredUnits.length); i1++, locationOffset1+=8)
		{
			/* Structure - sRegInfo_STRUCT */
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer */
			(*(AdiUInt8 *)(intfPtr + 28 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer */
			(*(AdiUInt8 *)(intfPtr + 29 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision */
			set_ushort_align((intfPtr + 30 + locationOffset1), (AdiUInt16)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision));
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId */
			(*(AdiUInt8 *)(intfPtr + 32 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus */
			(*(AdiUInt8 *)(intfPtr + 33 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus */
			(*(AdiUInt8 *)(intfPtr + 34 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus);
			
			/* DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState */
			(*(AdiUInt8 *)(intfPtr + 35 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState);
			
		}
	}
	/* Structure - sRegInfo_STRUCT */
	/* DT_MONITORED_DATA.internalUnitInfo.unitMainVer */
	(*(AdiUInt8 *)(intfPtr + 108)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitMainVer);
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitSubVer */
	(*(AdiUInt8 *)(intfPtr + 109)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitSubVer);
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitRevision */
	set_ushort_align((intfPtr + 110), (AdiUInt16)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitRevision));
	
	/* DT_MONITORED_DATA.internalUnitInfo.unitId */
	(*(AdiUInt8 *)(intfPtr + 112)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitId);
	
	/* DT_MONITORED_DATA.internalUnitInfo.cbitStatus */
	(*(AdiUInt8 *)(intfPtr + 113)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.cbitStatus);
	
	/* DT_MONITORED_DATA.internalUnitInfo.registerStatus */
	(*(AdiUInt8 *)(intfPtr + 114)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.registerStatus);
	
	/* DT_MONITORED_DATA.internalUnitInfo.sysState */
	(*(AdiUInt8 *)(intfPtr + 115)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.internalUnitInfo.sysState);
	
	/* Structure - MONITORED_DATA_DISC_STRUCT */
	/* DT_MONITORED_DATA.periodicDiscInfo.length */
	set_ulong_align((intfPtr + 116), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.periodicDiscInfo.length));
	
	/* Array - PERIODIC_DISC_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 256 && (i1 < phsPtr->DT_MONITORED_DATA.periodicDiscInfo.length); i1++, locationOffset1+=2)
		{
			/* Structure - sDiscInfo_STRUCT */
			/* DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId */
			(*(AdiUInt8 *)(intfPtr + 120 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId);
			
			/* DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue */
			(*(AdiUInt8 *)(intfPtr + 121 + locationOffset1)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue);
			
		}
	}
	/* Structure - ANALOG_INPUTS_STRUCT */
	/* DT_MONITORED_DATA.analogInputsInfo.length */
	set_ulong_align((intfPtr + 632), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.analogInputsInfo.length));
	
	/* Array - ANALOG_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 100 && (i1 < phsPtr->DT_MONITORED_DATA.analogInputsInfo.length); i1++, locationOffset1+=8)
		{
			/* Structure - sAnalogInfo_STRUCT */
			/* DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId */
			set_ulong_align((intfPtr + 636 + locationOffset1), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId));
			
			/* DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal */
			set_float_align((intfPtr + 640 + locationOffset1), (AdiFloat32)(phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal));
			
		}
	}
	/* Structure - SERIAL_INPUTS_STRUCT */
	/* DT_MONITORED_DATA.serialInputsInfo.length */
	set_ulong_align((intfPtr + 1436), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.serialInputsInfo.length));
	
	/* Array - SERIAL_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		AdiInt locationOffset1= 0;
		for (i1 = 0; i1 < 10 && (i1 < phsPtr->DT_MONITORED_DATA.serialInputsInfo.length); i1++, locationOffset1+=1032)
		{
			/* Structure - sSerialInfo_STRUCT */
			/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId */
			set_ulong_align((intfPtr + 1440 + locationOffset1), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId));
			
			/* Structure - BUFFER_STRUCT */
			/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size */
			set_ulong_align((intfPtr + 1444 + locationOffset1), (AdiUInt32)(phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size));
			
			/* Array - SERIAL_DATA_BUFFER_ARR */
			{
				AdiInt i2;
				AdiInt locationOffset2= 0;
				for (i2 = 0; i2 < 1024 && (i2 < phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size); i2++, locationOffset2+=1)
				{
					/* DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.data[i2] */
					(*(AdiUInt8 *)(intfPtr + 1448 + locationOffset1 + locationOffset2)) = (AdiUInt8)(phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.data[i2]);
					
				}
			}
		}
	}
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 11760), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* STOP_INTERFACE_MESSAGE */
void STOP_INTERFACE_MESSAGE_CONVERT_TO_IN(PHS_STOPINTERFACEMESSAGE* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - HDR_LCU2IO_STRUCT */
	/* HDR_LCU2IO.msgId */
	set_ushort_align((intfPtr + 0), (AdiUInt16)(phsPtr->HDR_LCU2IO.msgId));
	
	/* HDR_LCU2IO.totalLength */
	set_ushort_align((intfPtr + 2), (AdiUInt16)(phsPtr->HDR_LCU2IO.totalLength));
	
	/* HDR_LCU2IO.requestSeqNum */
	set_ulong_align((intfPtr + 4), (AdiUInt32)(phsPtr->HDR_LCU2IO.requestSeqNum));
	
	/* HDR_LCU2IO.enableResponse */
	set_ulong_align((intfPtr + 8), (AdiUInt32)(phsPtr->HDR_LCU2IO.enableResponse));
	
	/* HDR_LCU2IO.nonce */
	set_ulong_align((intfPtr + 12), (AdiUInt32)(phsPtr->HDR_LCU2IO.nonce));
	
	/* HDR_LCU2IO.timestamp */
	set_double_align((intfPtr + 16), (AdiFloat64)(phsPtr->HDR_LCU2IO.timestamp));
	
	/* Structure - DT_STOP_INTERFACE_STRUCT */
	/* DT_STOP_INTERFACE.uartId */
	set_ulong_align((intfPtr + 24), (AdiUInt32)(phsPtr->DT_STOP_INTERFACE.uartId));
	
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 28), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}

/* TL_ONLY_MESSAGE_IO2LCU */
void TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(PHS_TLONLYMESSAGEIO2LCU* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - TL_IO2LCU_STRUCT */
	/* TL_IO2LCU.crc */
	set_ulong_align((intfPtr + 0), (AdiUInt32)(phsPtr->TL_IO2LCU.crc));
	
}

/* TL_ONLY_MESSAGE_LCU2IO */
void TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(PHS_TLONLYMESSAGELCU2IO* phsPtr, AdiUInt8* intfPtr)
{
	/* Structure - TL_LCU2IO_STRUCT */
	/* TL_LCU2IO.crc */
	set_ulong_align((intfPtr + 0), (AdiUInt32)(phsPtr->TL_LCU2IO.crc));
	
}



/* Convert a message in the bus to Interface (using the static physical structure). */
void IOCARD_LCU_PR_ConvertToInterface(AdiInt MsgID, AdiUInt8* MessageBuffer)
{
	switch(MsgID)
	{
	case 1002: /* HDR_ONLY_MESSAGE_LCU2IO */
		HDR_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(&Phs_hdronlymessagelcu2io, MessageBuffer); break;
	case 7: /* KEEP_ALIVE_MESSAGE */
		KEEP_ALIVE_MESSAGE_CONVERT_TO_IN(&Phs_keepalivemessage, MessageBuffer); break;
	case 2: /* SERIAL_LCU2IO_MESSAGE */
		SERIAL_LCU2IO_MESSAGE_CONVERT_TO_IN(&Phs_seriallcu2iomessage, MessageBuffer); break;
	case 1: /* SET_DISCRETES_MESSAGE */
		SET_DISCRETES_MESSAGE_CONVERT_TO_IN(&Phs_setdiscretesmessage, MessageBuffer); break;
	case 3: /* SET_SYSTEM_STATE_MESSAGE */
		SET_SYSTEM_STATE_MESSAGE_CONVERT_TO_IN(&Phs_setsystemstatemessage, MessageBuffer); break;
	case 5: /* START_INTERFACE_MESSAGE */
		START_INTERFACE_MESSAGE_CONVERT_TO_IN(&Phs_startinterfacemessage, MessageBuffer); break;
	case 6: /* STOP_INTERFACE_MESSAGE */
		STOP_INTERFACE_MESSAGE_CONVERT_TO_IN(&Phs_stopinterfacemessage, MessageBuffer); break;
	case 1004: /* TL_ONLY_MESSAGE_LCU2IO */
		TL_ONLY_MESSAGE_LCU2IO_CONVERT_TO_IN(&Phs_tlonlymessagelcu2io, MessageBuffer); break;
	case 1001: /* HDR_ONLY_MESSAGE_IO2LCU */
		HDR_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(&Phs_hdronlymessageio2lcu, MessageBuffer); break;
	case 104: /* LOG_DATA_MESSAGE */
		LOG_DATA_MESSAGE_CONVERT_TO_IN(&Phs_logdatamessage, MessageBuffer); break;
	case 200: /* NONCE_MESSAGE */
		NONCE_MESSAGE_CONVERT_TO_IN(&Phs_noncemessage, MessageBuffer); break;
	case 101: /* REQUEST_STATUS_MESSAGE */
		REQUEST_STATUS_MESSAGE_CONVERT_TO_IN(&Phs_requeststatusmessage, MessageBuffer); break;
	case 103: /* SERIAL_IO2LCU_MESSAGE */
		SERIAL_IO2LCU_MESSAGE_CONVERT_TO_IN(&Phs_serialio2lcumessage, MessageBuffer); break;
	case 102: /* STATUS_MESSAGE */
		STATUS_MESSAGE_CONVERT_TO_IN(&Phs_statusmessage, MessageBuffer); break;
	case 1003: /* TL_ONLY_MESSAGE_IO2LCU */
		TL_ONLY_MESSAGE_IO2LCU_CONVERT_TO_IN(&Phs_tlonlymessageio2lcu, MessageBuffer); break;
	default: break;
	}
}




#endif 
