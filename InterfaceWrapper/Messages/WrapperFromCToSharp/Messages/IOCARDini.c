/* $RIGHTS$ */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/
/*   File             :  IOCARDini.c                                                                                                                     */
/*   Project          :  BLOC_LF5                                                                                                                        */
/*   Project Date     :  29/06/2026 19:12:33                                                                                                             */
/*   System           :  IOCARD                                                                                                                          */
/*   Description      :  Initialize physical variables related to IOCARD CSCI                                                                            */
/*   Software Version :  Files Generation version 2.0.43.0                                                                                               */
/*   Framework Version:  Last                                                                                                                            */
/*   Generation Type  :  C-generation                                                                                                                    */
/*   Generation Name  :  Niron_MBT                                                                                                                       */
/*   Comment          :  Niron Side                                                                                                                      */
/*   Created          :  30/06/2026 17:20:16 (Automatic from ICD File Generation)                                                                        */
/*-------------------------------------------------------------------------------------------------------------------------------------------------------*/

#ifndef IOCARDini_c
#define IOCARDini_c

#include "IOCARDpri.h"
#include "IOCARDvare.h"
#include <string.h>
#include "typelib.h"


/* HDR_ONLY_MESSAGE_IO2LCU */
void HdrOnlyMessageIo2lcu_INIT(PHS_HDRONLYMESSAGEIO2LCU* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
}

/* HDR_ONLY_MESSAGE_LCU2IO */
void HdrOnlyMessageLcu2io_INIT(PHS_HDRONLYMESSAGELCU2IO* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
}

/* KEEP_ALIVE_MESSAGE */
void KeepAliveMessage_INIT(PHS_KEEPALIVEMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* LOG_DATA_MESSAGE */
void LogDataMessage_INIT(PHS_LOGDATAMESSAGE* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
	phsPtr->DT_LOG.logLevel=E_LOG_LEVEL_VERBOSE;
	phsPtr->DT_LOG.logId=E_LOG_ID_UNDEFINED_1;
	phsPtr->DT_LOG.logStr.Size=0UL;
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 1024; i1++)
		{
			phsPtr->DT_LOG.logStr.data[i1]=0UL;
		}
	}
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* NONCE_MESSAGE */
void NonceMessage_INIT(PHS_NONCEMESSAGE* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
	phsPtr->DT_NONCE.ipAddr=0UL;
	phsPtr->DT_NONCE.pbitStatus=0UL;
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* REQUEST_STATUS_MESSAGE */
void RequestStatusMessage_INIT(PHS_REQUESTSTATUSMESSAGE* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
	phsPtr->DT_REQUEST_STATUS.reqId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->DT_REQUEST_STATUS.errId=E_ERROR_ID_SUCCESS;
	phsPtr->DT_REQUEST_STATUS.errString.Size=0UL;
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 1024; i1++)
		{
			phsPtr->DT_REQUEST_STATUS.errString.data[i1]=0UL;
		}
	}
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* SERIAL_IO2LCU_MESSAGE */
void SerialIo2lcuMessage_INIT(PHS_SERIALIO2LCUMESSAGE* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
	phsPtr->DT_SERIAL_IO2LCU.serialId=0UL;
	phsPtr->DT_SERIAL_IO2LCU.data.Size=0UL;
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 1024; i1++)
		{
			phsPtr->DT_SERIAL_IO2LCU.data.data[i1]=0UL;
		}
	}
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* SERIAL_LCU2IO_MESSAGE */
void SerialLcu2ioMessage_INIT(PHS_SERIALLCU2IOMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->DT_SERIAL_LCU2IO.uartId=E_UART_ID_NAV_10HZ;
	phsPtr->DT_SERIAL_LCU2IO.data.Size=0UL;
	/* Array - SERIAL_DATA_BUFFER_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 1024; i1++)
		{
			phsPtr->DT_SERIAL_LCU2IO.data.data[i1]=0UL;
		}
	}
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* SET_DISCRETES_MESSAGE */
void SetDiscretesMessage_INIT(PHS_SETDISCRETESMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->DT_SET_DISC.discNum=0UL;
	/* Array - DISCRETE_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 256; i1++)
		{
			phsPtr->DT_SET_DISC.discretes[i1].discreteId=E_DISCRETE_ID_MU_RTF_CMD;
			phsPtr->DT_SET_DISC.discretes[i1].discreteValue=E_BOOLEAN_FALSE;
		}
	}
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* SET_SYSTEM_STATE_MESSAGE */
void SetSystemStateMessage_INIT(PHS_SETSYSTEMSTATEMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->DT_SET_SYS_STATE.sysState=E_UNIT_STATE_IDLE;
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* START_INTERFACE_MESSAGE */
void StartInterfaceMessage_INIT(PHS_STARTINTERFACEMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->DT_START_INTERFACE.uartId=E_UART_ID_NAV_10HZ;
	phsPtr->DT_START_INTERFACE.baudrate=0UL;
	phsPtr->DT_START_INTERFACE.txParity=E_PARITY_OFF;
	phsPtr->DT_START_INTERFACE.rxParity=E_PARITY_OFF;
	phsPtr->DT_START_INTERFACE.stopBits=0UL;
	phsPtr->DT_START_INTERFACE.dataBits=0UL;
	phsPtr->DT_START_INTERFACE.loopback=E_BOOLEAN_FALSE;
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* STATUS_MESSAGE */
void StatusMessage_INIT(PHS_STATUSMESSAGE* phsPtr)
{
	phsPtr->HDR_IO2LCU.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_IO2LCU.totalLength=0UL;
	phsPtr->HDR_IO2LCU.statusSeqNum=0UL;
	phsPtr->HDR_IO2LCU.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->HDR_IO2LCU.nonce=0UL;
	phsPtr->HDR_IO2LCU.timestamp=-99999999999999999.9F;
	phsPtr->DT_MONITORED_DATA.registeredUnits.length=0UL;
	/* Array - REGISTERED_UNITS_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 10; i1++)
		{
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitMainVer=0UL;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitSubVer=0UL;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitRevision=0UL;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].unitId=E_UNIT_ID_IOUNIT;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].cbitStatus=E_CBIT_STATUS_CBIT_PASS;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].registerStatus=E_REGISTRATION_STATUS_PBIT_PASS;
			phsPtr->DT_MONITORED_DATA.registeredUnits.registeredUnits[i1].sysState=E_UNIT_STATE_IDLE;
		}
	}
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitMainVer=0UL;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitSubVer=0UL;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitRevision=0UL;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.unitId=E_UNIT_ID_IOUNIT;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.cbitStatus=E_CBIT_STATUS_CBIT_PASS;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.registerStatus=E_REGISTRATION_STATUS_PBIT_PASS;
	phsPtr->DT_MONITORED_DATA.internalUnitInfo.sysState=E_UNIT_STATE_IDLE;
	phsPtr->DT_MONITORED_DATA.periodicDiscInfo.length=0UL;
	/* Array - PERIODIC_DISC_INFO_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 256; i1++)
		{
			phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteId=E_DISCRETE_ID_MU_RTF_CMD;
			phsPtr->DT_MONITORED_DATA.periodicDiscInfo.periodicDiscInfo[i1].discreteValue=E_BOOLEAN_FALSE;
		}
	}
	phsPtr->DT_MONITORED_DATA.analogInputsInfo.length=0UL;
	/* Array - ANALOG_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 100; i1++)
		{
			phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogId=0UL;
			phsPtr->DT_MONITORED_DATA.analogInputsInfo.analogInputsInfo[i1].analogVal=-99999999999999999.9F;
		}
	}
	phsPtr->DT_MONITORED_DATA.serialInputsInfo.length=0UL;
	/* Array - SERIAL_INPUTS_INFO_ARR */
	{
		AdiInt i1;
		for (i1 = 0; i1 < 10; i1++)
		{
			phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].serialId=0UL;
			phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.Size=0UL;
			/* Array - SERIAL_DATA_BUFFER_ARR */
			{
				AdiInt i2;
				for (i2 = 0; i2 < 1024; i2++)
				{
					phsPtr->DT_MONITORED_DATA.serialInputsInfo.serialInputsInfo[i1].data.data[i2]=0UL;
				}
			}
		}
	}
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* STOP_INTERFACE_MESSAGE */
void StopInterfaceMessage_INIT(PHS_STOPINTERFACEMESSAGE* phsPtr)
{
	phsPtr->HDR_LCU2IO.msgId=E_MESSAGE_TYPE_SET_DISCRETES;
	phsPtr->HDR_LCU2IO.totalLength=0UL;
	phsPtr->HDR_LCU2IO.requestSeqNum=0UL;
	phsPtr->HDR_LCU2IO.enableResponse=E_BOOLEAN_FALSE;
	phsPtr->HDR_LCU2IO.nonce=0UL;
	phsPtr->HDR_LCU2IO.timestamp=-99999999999999999.9F;
	phsPtr->DT_STOP_INTERFACE.uartId=E_UART_ID_NAV_10HZ;
	phsPtr->TL_LCU2IO.crc=0UL;
}

/* TL_ONLY_MESSAGE_IO2LCU */
void TlOnlyMessageIo2lcu_INIT(PHS_TLONLYMESSAGEIO2LCU* phsPtr)
{
	phsPtr->TL_IO2LCU.crc=0UL;
}

/* TL_ONLY_MESSAGE_LCU2IO */
void TlOnlyMessageLcu2io_INIT(PHS_TLONLYMESSAGELCU2IO* phsPtr)
{
	phsPtr->TL_LCU2IO.crc=0UL;
}



/* Init message in bus IOCARD_LCU_PR(using the static physical structure). */
void IOCARD_LCU_PR_INIT_Message(AdiInt MsgID)
{
	switch(MsgID)
	{
	case 1002: /* HDR_ONLY_MESSAGE_LCU2IO */
		HdrOnlyMessageLcu2io_INIT(&Phs_hdronlymessagelcu2io); break;
	case 7: /* KEEP_ALIVE_MESSAGE */
		KeepAliveMessage_INIT(&Phs_keepalivemessage); break;
	case 2: /* SERIAL_LCU2IO_MESSAGE */
		SerialLcu2ioMessage_INIT(&Phs_seriallcu2iomessage); break;
	case 1: /* SET_DISCRETES_MESSAGE */
		SetDiscretesMessage_INIT(&Phs_setdiscretesmessage); break;
	case 3: /* SET_SYSTEM_STATE_MESSAGE */
		SetSystemStateMessage_INIT(&Phs_setsystemstatemessage); break;
	case 5: /* START_INTERFACE_MESSAGE */
		StartInterfaceMessage_INIT(&Phs_startinterfacemessage); break;
	case 6: /* STOP_INTERFACE_MESSAGE */
		StopInterfaceMessage_INIT(&Phs_stopinterfacemessage); break;
	case 1004: /* TL_ONLY_MESSAGE_LCU2IO */
		TlOnlyMessageLcu2io_INIT(&Phs_tlonlymessagelcu2io); break;
	case 1001: /* HDR_ONLY_MESSAGE_IO2LCU */
		HdrOnlyMessageIo2lcu_INIT(&Phs_hdronlymessageio2lcu); break;
	case 104: /* LOG_DATA_MESSAGE */
		LogDataMessage_INIT(&Phs_logdatamessage); break;
	case 200: /* NONCE_MESSAGE */
		NonceMessage_INIT(&Phs_noncemessage); break;
	case 101: /* REQUEST_STATUS_MESSAGE */
		RequestStatusMessage_INIT(&Phs_requeststatusmessage); break;
	case 103: /* SERIAL_IO2LCU_MESSAGE */
		SerialIo2lcuMessage_INIT(&Phs_serialio2lcumessage); break;
	case 102: /* STATUS_MESSAGE */
		StatusMessage_INIT(&Phs_statusmessage); break;
	case 1003: /* TL_ONLY_MESSAGE_IO2LCU */
		TlOnlyMessageIo2lcu_INIT(&Phs_tlonlymessageio2lcu); break;
	default: break;
	}
}




#endif 
